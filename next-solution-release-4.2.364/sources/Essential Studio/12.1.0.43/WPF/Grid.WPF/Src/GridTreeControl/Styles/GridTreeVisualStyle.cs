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
using System.Windows.Shapes;

    public interface IGridTreeVisualStyle
    {
        /// <summary>
        /// Get GridTreeControl Border Brush
        /// </summary>
        Brush GridTreeBorderBrush { get; }

        /// <summary>
        /// Get GridTreeControl Border Brush
        /// </summary>
        Thickness GridTreeBorderThickness { get; }

        #region HeaderRelated

        /// <summary>
        /// Get GridTreeControl Header Background Brush 
        /// </summary>
        Brush GridTreeHeaderBackgroundBrush { get; }

        /// <summary>
        /// Get GridTreeControl Header Foreground Brush 
        /// </summary>
        Brush GridTreeHeaderForegroundBrush { get; }

        /// <summary>
        /// Get GridTreeControl HeaderHover Background Brush
        /// </summary>
        Brush GridTreeHeaderHoverBackgroundBrush { get; }

        /// <summary>
        /// Get GridTreeControl HeaderHover Foreground Brush
        /// </summary>
        Brush GridTreeHeaderHoverForegroundBrush { get; }

        /// <summary>
        /// Get GridTreeControl Header Font
        /// </summary>
        GridFontInfo GridTreeHeaderFont { get; }

        /// <summary>
        /// Get Margin for Header Text
        /// </summary>
        CellMarginsInfo GridTreeHeaderTextMargins { get; }

        /// <summary>
        /// Get GridTreeControl Header InnerBorder Brush
        /// </summary>
        //Brush GridTreeHeaderInnerBorderBrush { get; }   

        /// <summary>
        /// Get GridTreeControl HeaderInnerBorder Thickness
        /// </summary>

        // Thickness GridTreeHeaderInnerBorderThickness { get; }        
        /// <summary>
        /// Get GridTreeControl SortWidget Brush
        /// </summary>
        Brush GridTreeSortWidgetBrush { get; }

        /// <summary>
        /// Get GridTreeControl SortWidget Border Brush
        /// </summary>
        Brush GridTreeSortWidgetBorderBrush { get; }

        /// <summary>
        /// Get GridTreeSortWidget BorderHover Background Brush
        /// </summary>
        Brush GridTreeSortWidgetBorderHoverBackgroundBrush { get; }

        #endregion

        #region Row Related

        /// <summary>
        /// Get Highlightlight selection Background brush for GridTreeControl
        /// </summary>
        Brush GridTreeHighlightSelectionBackground { get; }

        /// <summary>
        /// Get GridTreeControl HighlightSelection Foreground Brush
        /// </summary>
        Brush GridTreeHighlightSelectionForeground { get; }

        /// <summary>
        /// Get GridTreeControl CurrentCellSelection Background Brush
        /// </summary>
        Brush GridTreeCurrentCellSelectionBackground { get; }

        /// <summary>
        /// Get GridTreControl CurrentCellSelection Foreground Brush
        /// </summary>
        Brush GridTreeCurrentCellSelectionForeground { get; }

        /// <summary>
        /// Get GridTreeControl CurrentCell Border Brush
        /// </summary>
        Brush GridTreeCurrentCellBorderBrush { get; }

        /// <summary>
        /// Get GridTreeControl CurrentCell Border Width
        /// </summary>
        double GridTreeCurrentCellBorderWidth { get; }

        /// <summary>
        /// Get GridTreeControl RowHoverBackground Brush
        /// </summary>
        Brush GridTreeRowHoverBackgroundBrush { get; }

        /// <summary>
        /// Get GridTreeControl RowHover Foreground Brush
        /// </summary>
        Brush GridTreeRowHoverForegroundBrush { get; }

        //Brush GridTreeNodeHighlightBrush { get; }
        /// <summary>
        /// Get GridTreeControl RowHeaderBackgroundBrush
        /// </summary>
        Brush GridTreeRowHeaderBackgroundBrush { get; }

        /// <summary>
        /// Get GridTreeControl RowHeaderForegroundBrush
        /// </summary>
        Brush GridTreeRowHeaderForegroundBrush { get; }
        #endregion

        #region Expander Related
#if !SILVERLIGHT

        /// <summary>
        /// Gets the Expander Plus Path Data
        /// </summary>
        Geometry GridTreeExpanderPlusPath { get; }

        /// <summary>
        /// Gets the Expander Minus Path Data
        /// </summary>
        Geometry GridTreeExpanderMinusPath { get; }
#else
        Path GridTreeExpanderPlusPath { get; }

        Path GridTreeExpanderMinusPath { get; }
#endif

        /// <summary>
        /// Gets the ExpanderBackground Brush.
        /// </summary>
        Brush GridTreeExpanderBackground { get; }

        /// <summary>
        /// Gets the ExpanderBorderBrush
        /// </summary>

        Brush GridTreeExpanderBorderBrush { get; }

        //Brush GridTreeExpanderForeground { get; }

        /// <summary>
        /// Gets the ExpanderBackgroundBrush while Expander is Expanded State
        /// </summary>

        Brush GridTreeExpanderExpandedBackground { get; }

        /// <summary>
        /// Gets the ExpanderExpanded BorderBrush
        /// </summary>

        Brush GridTreeExpanderExpandedBorderBrush { get; }

        /// <summary>
        /// Gets Expander Hover Background Brush
        /// </summary>

        Brush GridTreeExpanderHoverBackground { get; }

        /// <summary>
        /// Gets Expander Hover BorderBrush
        /// </summary>

        Brush GridTreeExpanderHoverBorderBrush { get; }



        #endregion

        #region  Cell Related


        /// <summary>
        /// Gets the Cell Borders for GridTreeControl Cells
        /// </summary>

        CellBordersInfo GridTreeCellBorders { get; }

        /// <summary>
        /// Gets the Cell Font for the GridTreeControl
        /// </summary>

        GridFontInfo GridTreeCellFont { get; }


        /// <summary>
        /// Gets  Text Margins for the GridTreeControl
        /// </summary>
        CellMarginsInfo GridTreeCellTextMargins { get; }


        /// <summary>
        /// Gets the CellBackground Brush
        /// </summary>

        Brush GridTreeCellBackgroundBrush { get; }


        /// <summary>
        /// Gets Cell Foreground Brush for the GridTreeControl.
        /// </summary>

        Brush GridTreeCellForegroundBrush { get; }

        //double GridTreeCellBorderWidth{ get; }

        //Brush GridTreeCellBorderBrush { get; }
        #endregion
    }

    public class GridTreeMetroVisualStyle : IGridTreeVisualStyle
    {

        public Brush GridTreeRowHoverForegroundBrush
        {
            get { return this.GridTreeCellForegroundBrush; }
        }

        public Brush GridTreeBorderBrush
        {
            get
            {
               
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#D1D1D1"));
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD1D1D1"));
#endif
            }
        }

        public Thickness GridTreeBorderThickness
        {
            get
            {
                return new Thickness(1d);
            }
        }

        public Brush GridTreeHeaderBackgroundBrush
        {
            get
            {

#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF119EDA"));
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF119EDA"));
#endif
            }
        }

        public Brush GridTreeHeaderForegroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Brush GridTreeHeaderHoverBackgroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF5EC8F5"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF5EC8F5"));

#endif
            }
        }

        public Brush GridTreeHeaderHoverForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        public GridFontInfo GridTreeHeaderFont
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

        public CellMarginsInfo GridTreeHeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d,
                };
            }
        }

        public Brush GridTreeHeaderInnerBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFC5C5C5"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Thickness GridTreeHeaderInnerBorderThickness
        {
            get
            {
#if SILVERLIGHT

                return new Thickness(0.2d);
#else
                return new Thickness(1, 0, 1, 0);
#endif
            }
        }

        public Brush GridTreeSortWidgetBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF67D9FF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF67D9FF"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderHoverBackgroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF67D9FF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF67D9FF"));

#endif
            }
        }

        public Brush GridTreeHighlightSelectionBackground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF2ABFF1"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2ABFF1"));

#endif
            }
        }

        public Brush GridTreeHighlightSelectionForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Brush GridTreeCurrentCellSelectionBackground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF2AABCF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2AABCF"));

#endif
            }
        }

        public Brush GridTreeCurrentCellSelectionForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Brush GridTreeCurrentCellBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF2ABFF1"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2ABFF1"));

#endif
            }
        }

        public double GridTreeCurrentCellBorderWidth
        {

            get
            {
                return 0.5d;
            }
        }

        public Brush GridTreeRowHoverBackgroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFDADADA"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFDADADA"));

#endif
            }
        }

        public Brush GridTreeNodeHighlightBrush
        {
            get
            {
                return this.GridTreeCellBackgroundBrush;
            }
        }
#if !SILVERLIGHT
        public Geometry GridTreeExpanderPlusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 288.334,183.208L 288.334,174.125L 294.958,178.542L 288.334,183.208 Z ");
            }
        }

        public Geometry GridTreeExpanderMinusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 274.487,165.737L 282.487,165.737L 282.487,157.681L 274.487,165.737 Z ");

            }
        }
#else
        public Path GridTreeExpanderPlusPath
        {
            get
            {
                string data = "M-91.702,-146.448L68.0101,-31.5536L-91.702,99.2988z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;

            }
        }

        public Path GridTreeExpanderMinusPath
        {
            get
            {
                string data = "M105.264,-136.319L106.268,65.2559L-91.702,65.2559z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;
            }
        }
#endif

      

        public Brush GridTreeExpanderBackground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF7F7F7F"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7F7F7F"));

#endif
            }
        }

        public Brush 
            
            GridTreeExpanderBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF7F7F7F"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7F7F7F"));

#endif
            }
        }

        public Brush GridTreeExpanderForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF7F7F7F"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7F7F7F"));

#endif
            }
        }

        public Brush GridTreeExpanderExpandedBackground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF7F7F7F"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7F7F7F"));

#endif
            }
        }

        public Brush GridTreeExpanderExpandedBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF7F7F7F"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7F7F7F"));

#endif
            }
        }

        public Brush GridTreeExpanderExpandedForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF7F7F7F"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7F7F7F"));

#endif
            }
        }

        public Brush GridTreeExpanderHoverBackground
        {
            get
            {
               
                #if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFE3ECEE"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE3ECEE"));

#endif
                //#if !SILVERLIGHT
//                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF41B1E1"));

//#else
//                return new SolidColorBrush(ColorExtensions.StringToColor("#FF41B1E1"));

//#endif
            }
        }

        public Brush GridTreeExpanderHoverBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                //return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFB8BBBC"));
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF7F7F7F"));

#else
               // return new SolidColorBrush(ColorExtensions.StringToColor("#FFB8BBBC"));
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7F7F7F"));

#endif
            }
        }

        public Brush GridTreeExpanderHoverForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF7F7F7F"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7F7F7F"));

#endif
            }
        }

        public CellBordersInfo GridTreeCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    
#if !SILVERLIGHT
                    All = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFC5C5C5")), 0.25d),
#else
                     Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC5C5C5")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC5C5C5")), 0.25d), // newly added left and right
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC5C5C5")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC5C5C5")), 0.25d)
#endif
                };
            }
        }

        public GridFontInfo GridTreeCellFont
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

        public CellMarginsInfo GridTreeCellTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d
                };
            }
        }

        public Brush GridTreeCellBackgroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Brush GridTreeCellForegroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF333333"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF333333"));

#endif
            }

        }

        public double GridTreeCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush GridTreeCellBorderBrush
        {
            get
            {
#if !SILVERLIGHT

                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFC1DDCF"));
#else 
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFC1DDCF"));
#endif
            }
        }


        public Brush GridTreeRowHeaderBackgroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF119EDA"));
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF119EDA"));
#endif
            }
        }

        public Brush GridTreeRowHeaderForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }
    }

    public class GridTreeBureauBlueVisualStyle : IGridTreeVisualStyle
    {
        public Brush GridTreeRowHoverForegroundBrush
        {
            get { return this.GridTreeCellForegroundBrush; }
        }

        public Brush InBuiltBlackBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        public Brush GridTreeBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return GridUtil.GetXamlConvertedValue<Brush>("#FFC7C8CB");
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFC7C8CB"));
#endif

            }
        }

        public Thickness GridTreeBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush GridTreeHeaderBackgroundBrush
        {           

            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
#if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF0F0F0"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFD9D9D9"), 1d), 
                    });
#else
                   
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFF0F0F0"), Offset= 0d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFD9D9D9"),Offset= 1d},
                    },0.0d);
#endif

                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHeaderForegroundBrush
        {
            get
            {
                return Brushes.MidnightBlue;
            }
        }

        public Brush GridTreeHeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF8F8F8"), 0.202),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFECECEC"), 0.986),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFB8B8B8"), 0.993),
                    });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFF8F8F8"), Offset= 0.202},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFECECEC"), Offset= 0.986},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFB8B8B8"), Offset= 0.993},
                    },0.0d);
#endif

                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHeaderHoverForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        public GridFontInfo GridTreeHeaderFont
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

        public CellMarginsInfo GridTreeHeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 12d
                };
            }
        }

        public Brush GridTreeHeaderInnerBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFC7C8CB"));
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFC7C8CB"));
#endif
            }
        }

        public Thickness GridTreeHeaderInnerBorderThickness
        {
            get
            {
#if SILVERLIGHT
                return new Thickness(0.2d);
#else
                 return new Thickness(0.75d);
#endif

            }
        }

        public Brush GridTreeSortWidgetBrush
        {
            get
            {
#if !SILVERLIGHT

                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF323232"));
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF323232"));
#endif
            }
        }

        public Brush GridTreeSortWidgetBorderBrush
        {
            get
            {
#if !SILVERLIGHT

                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFC7C8CB"));
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFC7C8CB"));
#endif
            }
        }

        public Brush GridTreeSortWidgetBorderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF7F7F7"), 0.23d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE8E8E8"), 0.991d),         
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFBCBCBC"), 1d),
                    });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFF7F7F7"),Offset= 0.23d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFE8E8E8"),Offset= 0.991d},         
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFBCBCBC"),Offset= 1d},
                },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF96DDF7"), 0),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF0BB2F0"), 1)
                     });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FF96DDF7"),Offset= 0},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF0BB2F0"),Offset= 1}
                     },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHighlightSelectionForeground
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush GridTreeCurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFD9605"), 0),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFD6D01"), 0.918),
                    });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFFD9605"), Offset= 0},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFD6D01"), Offset= 0.918},
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeCurrentCellSelectionForeground
        {

            get
            {
#if !SILVERLIGHT
            return GridUtil.GetXamlConvertedValue<Brush>("#FFFFFFFF"); 
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }


        

        public Brush GridTreeCurrentCellBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFD7401"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFD7401"));

#endif
            }
        }

        public double GridTreeCurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public Brush GridTreeRowHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
#if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFEFEFE"), 0),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF7F7F7"), 0.207),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF4F4F4"), 1)
                      });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFFEFEFE"),Offset= 0},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFF7F7F7"),Offset= 0.207},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFF4F4F4"),Offset= 1}
                      },0.0d);
#endif
              
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;

            }
        }

        public Brush GridTreeNodeHighlightBrush
        {
            get
            {
                return this.GridTreeCellBackgroundBrush;
            }
        }
#if !SILVERLIGHT
        public Geometry GridTreeExpanderPlusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 288.334,183.208L 288.334,174.125L 294.958,178.542L 288.334,183.208 Z ");
            }
        }

        public Geometry GridTreeExpanderMinusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 274.487,165.737L 282.487,165.737L 282.487,157.681L 274.487,165.737 Z ");

            }
        }
        
#else
        public Path GridTreeExpanderPlusPath
        {
            get
            {
                string data = "F1 M 288.334,183.208L 288.334,174.125L 294.958,178.542L 288.334,183.208 Z ";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;

            }
        }

        public Path GridTreeExpanderMinusPath
        {
            get
            {
                string data = "M105.264,-136.319L106.268,65.2559L-91.702,65.2559z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;
            }
        }
#endif

       
        public Brush GridTreeExpanderBackground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF5B5B5B"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF5B5B5B"));

#endif
            }
        }

        public Brush GridTreeExpanderBorderBrush
        {
            get
            {
                return GridTreeExpanderBackground;
            }
        }

        public Brush GridTreeExpanderForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF323232"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF323232"));

#endif
            }
        }

        public Brush GridTreeExpanderExpandedBackground
        {
            get
            {
                return this.GridTreeExpanderBackground;
            }
        }

        public Brush GridTreeExpanderExpandedBorderBrush
        {
            get
            {
                return this.GridTreeExpanderBackground;
            }
        }

        public Brush GridTreeExpanderExpandedForeground
        {
            get
            {
                return this.GridTreeExpanderForeground;
            }
        }

        public Brush GridTreeExpanderHoverBackground
        {
            get
            {
                //LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                //{
                   
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF5B5B5B"));
                //     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFEBA50"), 0.015d),
                //    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFE9A05"), 0.029), 
                //    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFE6800"), 0.985),
                //    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFD85800"), 1), 
                //     });
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF5B5B5B"));
                    // new GradientStop(){Color= ColorExtensions.StringToColor("#FFFEBA50"),Offset= 0.015d},
                    //new GradientStop(){Color= ColorExtensions.StringToColor("#FFFE9A05"),Offset= 0.029}, 
                    //new GradientStop(){Color= ColorExtensions.StringToColor("#FFFE6800"),Offset= 0.985},
                    //new GradientStop(){Color= ColorExtensions.StringToColor("#FFD85800"),Offset= 1}, 
                    // },0.0d);
#endif

                //brush.StartPoint = new Point(0.5, 0);
                //brush.EndPoint = new Point(0.5, 1);
                //return brush;
            }
        }

        public Brush GridTreeExpanderHoverBorderBrush
        {
            get
            {
                return GridTreeExpanderHoverBackground;
            }
        }

        public Brush GridTreeExpanderHoverForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public CellBordersInfo GridTreeCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
#if !SILVERLIGHT
                    Top = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFC7C8CB")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFC7C8CB")), 0.3d),
                    Left = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFC7C8CB")), 0.3d),
                    Right = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFC7C8CB")), 0.3d)
#else
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC7C8CB")), 0.3d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC7C8CB")), 0.3d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC7C8CB")), 0.3d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC7C8CB")), 0.3d)
#endif


                };
            }
        }

        public GridFontInfo GridTreeCellFont
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

        public CellMarginsInfo GridTreeCellTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d
                };
            }
        }

        public Brush GridTreeCellBackgroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
#endif

            }
        }

        public Brush GridTreeCellForegroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF48525B"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF48525B"));

#endif
            }
        }

        public double GridTreeCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public Brush GridTreeCellBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFD7401"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFD7401"));

#endif
            }
        }


        public Brush GridTreeRowHeaderBackgroundBrush
        {
            get
            {
                return this.GridTreeHeaderBackgroundBrush;
            }
        }

        public Brush GridTreeRowHeaderForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }
    }
    
    public class GridTreeBlendVisualStyle : IGridTreeVisualStyle
    {
        public Brush GridTreeRowHoverForegroundBrush
        {
            get { return this.GridTreeCellForegroundBrush; }
        }
        public Brush InBuiltBlackBrush
        {
            get
            {

                return new SolidColorBrush(Colors.Black);
            }
        }

        public Brush GridTreeBorderBrush
        {
#if !SILVERLIGHT
            get { return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF333333")); }

#else
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF333333")); }

#endif
        }

        public Thickness GridTreeBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush GridTreeHeaderBackgroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF3A3A3A"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A"));

#endif
            }
        }

        public Brush GridTreeHeaderForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush GridTreeHeaderHoverBackgroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF525252"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF525252"));

#endif
            }
        }

        public Brush GridTreeHeaderHoverForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        public GridFontInfo GridTreeHeaderFont
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

        public CellMarginsInfo GridTreeHeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo() { Left = 3d, Right = 3d };
            }
        }

        public Brush GridTreeHeaderInnerBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF2B2B2B"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2B2B2B"));

#endif
            }
        }

        public Thickness GridTreeHeaderInnerBorderThickness
        {
            get
            {
#if SILVERLIGHT
                return new Thickness(0.2d);
#else
                 return new Thickness(0.75d);
#endif
               
            }
        }

        public Brush GridTreeSortWidgetBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF2B2B2B"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2B2B2B"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderHoverBackgroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF525252"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF525252"));

#endif
            }
        }

        public Brush GridTreeHighlightSelectionBackground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF535352"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF535352"));

#endif
            }
        }

        public Brush GridTreeHighlightSelectionForeground
        {
            get
            {
                return this.GridTreeCellForegroundBrush;
            }
        }

        public Brush GridTreeCurrentCellSelectionBackground
        {
            get { return GridTreeHighlightSelectionBackground; }
        }

        public Brush GridTreeCurrentCellSelectionForeground
        {
            get { return Brushes.White; }
        }

        public Brush GridTreeCurrentCellBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF3D3D3D"));
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3D3D3D"));
#endif
            }
        }

        public double GridTreeCurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public Brush GridTreeRowHoverBackgroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF535352"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF535352"));

#endif
            }
        }

        public Brush GridTreeNodeHighlightBrush
        {
            get
            {
                return this.GridTreeCellBackgroundBrush;
            }
        }

#if !SILVERLIGHT
        public Geometry GridTreeExpanderPlusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 288.334,183.208L 288.334,174.125L 294.958,178.542L 288.334,183.208 Z ");
            }
        }

        public Geometry GridTreeExpanderMinusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 274.487,165.737L 282.487,165.737L 282.487,157.681L 274.487,165.737 Z ");

            }
        }
#else
        public Path GridTreeExpanderPlusPath
        {
            get
            {
                string data = "M-91.702,-146.448L68.0101,-31.5536L-91.702,99.2988z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;

            }
        }

        public Path GridTreeExpanderMinusPath
        {
            get
            {
                string data = "M105.264,-136.319L106.268,65.2559L-91.702,65.2559z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;
            }
        }
#endif

       

        public Brush GridTreeExpanderBackground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFA2A2A2"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA2A2A2"));

#endif
            }
        }

        public Brush GridTreeExpanderBorderBrush
        {
            get
            {
                return this.GridTreeExpanderBackground;
            }
        }

        public Brush GridTreeExpanderForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Brush GridTreeExpanderExpandedBackground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Brush GridTreeExpanderExpandedBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Brush GridTreeExpanderExpandedForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Brush GridTreeExpanderHoverBackground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Brush GridTreeExpanderHoverBorderBrush
        {
            get
            {
                return GridTreeExpanderHoverBackground;
            }
        }

        public Brush GridTreeExpanderHoverForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public CellBordersInfo GridTreeCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
#if !SILVERLIGHT
                    All = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF2B2B2B")), 0.3d)

#else
                    All = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF2B2B2B")), 0.3d)

#endif
                };
            }
        }

        public GridFontInfo GridTreeCellFont
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

        public CellMarginsInfo GridTreeCellTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d
                };
            }
        }

        public Brush GridTreeCellBackgroundBrush
        {
            get
            {
#if !SILVERLIGHT 
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF414141"));
#else
                return new SolidColorBrush( ColorExtensions.StringToColor("#FF414141"));
#endif

            }
        }

        public Brush GridTreeCellForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public double GridTreeCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public Brush GridTreeCellBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF333333"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF333333"));

#endif
            }
        }


        public Brush GridTreeRowHeaderBackgroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF3A3A3A"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A"));

#endif
            }
        }

        public Brush GridTreeRowHeaderForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        
    }

    public class GridTreeOffice14BlackVisualStyle : IGridTreeVisualStyle
    {

        public Brush GridTreeRowHoverForegroundBrush
        {
            get { return Brushes.White; }
        }
        public Brush InBuiltBlackBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        public Brush GridTreeBorderBrush
        {
            get
            { 
#if !SILVERLIGHT
                return GridUtil.GetXamlConvertedValue<Brush>("#FF3B3B3B");

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B"));

#endif
            }

        }

        public Thickness GridTreeBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush GridTreeHeaderBackgroundBrush
        {
            get
            {
               
                    
                    #if !SILVERLIGHT
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEAEAEA"), 0.08d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFDFDFDF"), 0.936d), 
                 });
                brush.StartPoint = new Point(0.032, 0.066);
                brush.EndPoint = new Point(0.032, 0.957);
                return brush;
#else
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     new GradientStop(){  Color=ColorExtensions.StringToColor("#FFEAEAEA"),Offset= 0.08d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFDFDFDF"),Offset= 0.936d}, 
                 },0.0d);
                brush.StartPoint = new Point(0.032, 0.066);
                brush.EndPoint = new Point(0.032, 0.957);
                return brush;
               //return new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A"));
                    // new GradientStop(){Color= ColorExtensions.StringToColor("#FFEAEAEA"), 0.08d),
                    //new GradientStop(){Color= ColorExtensions.StringToColor("#FFDFDFDF"), 0.936d), 
#endif
               
            }
        }

        public Brush GridTreeHeaderForegroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF363636"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF363636"));

#endif
            }
        }

        public Brush GridTreeHeaderHoverBackgroundBrush
        {
            get
            {
               
                  
                    #if !SILVERLIGHT
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFDFDFDF"), 0.069d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEAEAEA"), 0.931d),
                });

                brush.StartPoint = new Point(0.789, 0.014);
                brush.EndPoint = new Point(0.779, 0.987);
                return brush;
#else
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFDFDFDF"),Offset= 0.069d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFEAEAEA"), Offset= 0.931d},
                },0.0d);

                brush.StartPoint = new Point(0.789, 0.014);
                brush.EndPoint = new Point(0.779, 0.987);
                return brush;
                    //return new SolidColorBrush(ColorExtensions.StringToColor("#FF525252"));                    
#endif
                
            }
        }

        public Brush GridTreeHeaderHoverForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        public GridFontInfo GridTreeHeaderFont
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

        public CellMarginsInfo GridTreeHeaderTextMargins
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

        public Brush GridTreeHeaderInnerBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF3B3B3B"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B"));

#endif
            }
        }

        public Thickness GridTreeHeaderInnerBorderThickness
        {
            get
            {
#if SILVERLIGHT
                return new Thickness(0.2d);
#else
                 return new Thickness(0.5d);
#endif
               
            }
        }

        public Brush GridTreeSortWidgetBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF3B3B3B"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF3B3B3B"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderHoverBackgroundBrush
        {
            get
            {
                
                     
                    #if !SILVERLIGHT
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE9E9E9"),0.134d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE0E0E0"),0.768d),
                 });
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
#else
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE9E9E9"),Offset=0.134d},
                    new GradientStop{Color=ColorExtensions.StringToColor("#FFE0E0E0"),Offset=0.768d},                                                                                   
                }, 0.0d);
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
#endif
               
            }
        }

        public Brush GridTreeHighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                      
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF5B5B5B"), 0.043d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF484848"), 0.399d),  
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF505050"), 0.745d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF6B6B6B"), 0.973d),
                    });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF5B5B5B"),Offset= 0.043d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF484848"),Offset= 0.399d},  
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF505050"),Offset= 0.745d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF6B6B6B"),Offset= 0.973d},
                    }, 0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0.052);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHighlightSelectionForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Brush GridTreeCurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                     #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF3D3D3D"), 0),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF434343"), 0.026), 
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF424242"), 1)
                     });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF3D3D3D"),Offset= 0},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF434343"),Offset= 0.026}, 
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FF424242"),Offset= 1}
                      },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeCurrentCellSelectionForeground
        {
            get { return Brushes.White; }
        }

        public Brush GridTreeCurrentCellBorderBrush
        {
            get
            {
#if ! SILVERLIGHT

                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF313131"));
#else
                return InBuiltBlackBrush;
#endif
            }
        }

        public double GridTreeCurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public Brush GridTreeRowHeaderBackgroundBrush
        {
            get
            {
                return this.GridTreeHeaderBackgroundBrush;
            }
        }

        public Brush GridTreeRowHeaderForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        public Brush GridTreeRowHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                      new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF7D7D7D"), 0.04d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF4F4F4F"), 0.598d),  
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF555555"), 0.879d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF6B6B6B"), 0.984d),
                    });
#else
                      new GradientStop(){Color= ColorExtensions.StringToColor("#FF7D7D7D"),Offset= 0.04d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF4F4F4F"), Offset= 0.598d},  
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF555555"),Offset= 0.879d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF6B6B6B"),Offset= 0.984d},
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeNodeHighlightBrush
        {
            get
            {
                return this.GridTreeCellBackgroundBrush;
            }
        }

#if !SILVERLIGHT

        public Geometry GridTreeExpanderPlusPath
        {
            get
            {                
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 288.334,183.208L 288.334,174.125L 294.958,178.542L 288.334,183.208 Z ");
            }
        }

        public Geometry GridTreeExpanderMinusPath
        {
            get
            {               
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 274.487,165.737L 282.487,165.737L 282.487,157.681L 274.487,165.737 Z ");

            }
        }

       
#else
        public Path GridTreeExpanderPlusPath
        {
            get
            {
                string data = "M-91.702,-146.448L68.0101,-31.5536L-91.702,99.2988z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;

            }
        }

        public Path GridTreeExpanderMinusPath
        {
            get
            {
                string data = "M105.264,-136.319L106.268,65.2559L-91.702,65.2559z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;
            }
        }

#endif
       

        public Brush GridTreeExpanderBackground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959")); //Brushes.Black; //new SolidColorBrush(Brushes.Black);

#endif
            }
        }

        public Brush GridTreeExpanderBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFA8A8A8"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA8A8A8"));

#endif
            }
        }

        public Brush GridTreeExpanderForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Brush GridTreeExpanderExpandedBackground
        {
            get
            {
                return this.GridTreeExpanderBackground;
            }
        
        }

        public Brush GridTreeExpanderExpandedBorderBrush
        {
            get
            {
                return this.GridTreeExpanderBorderBrush;
            }
        }

        public Brush GridTreeExpanderExpandedForeground
        {
            get
            {
                return this.GridTreeExpanderForeground;
            }
        }

        public Brush GridTreeExpanderHoverBackground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFB11A"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFB11A"));

#endif
            }
        }

        public Brush GridTreeExpanderHoverBorderBrush
        {
            get
            {
                return GridTreeExpanderHoverBackground;
            }
        }

        public Brush GridTreeExpanderHoverForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFB11A"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFB11A"));

#endif
            }
        }

        public CellBordersInfo GridTreeCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                   
#if !SILVERLIGHT
                     Top = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF3B3B3B")), 0.30d),
                    Bottom = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF3B3B3B")), 0.30d),
                    Right = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF3B3B3B")), 0.30d),
                    Left = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF3B3B3B")), 0.30d)
#else
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 0.30d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 0.30d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 0.30d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF3B3B3B")), 0.30d)
#endif
                };
            }
        }

        public GridFontInfo GridTreeCellFont
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

        public CellMarginsInfo GridTreeCellTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d
                };
            }
        }

        public Brush GridTreeCellBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush GridTreeCellForegroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF6C6D6F"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6C6D6F"));

#endif
            }
        }

        public double GridTreeCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush GridTreeCellBorderBrush
        {
            get
            {
                return InBuiltBlackBrush;
            }
        }
    }

    public class GridTreeOffice14BlueVisualStyle : IGridTreeVisualStyle
    {

        public Brush GridTreeRowHoverForegroundBrush
        {
            get { return this.GridTreeCellForegroundBrush; }
        }
        public Brush InBuiltBlackBrush
        {

            get
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        private Brush _gridTreeBorderBrush = null;
        public Brush GridTreeBorderBrush
        {
            get
            {
                if (_gridTreeBorderBrush == null)
#if !SILVERLIGHT
                    _gridTreeBorderBrush = new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF849DBD"));
#else
                _gridTreeBorderBrush =new SolidColorBrush( ColorExtensions.StringToColor("#FF849DBD"));
#endif

                return _gridTreeBorderBrush;
            }
        }

        public Thickness GridTreeBorderThickness
        {
            get { return new Thickness(1); }
        }

        private Brush _gridTreeHeaderBackgroundBrush = null;
        public Brush GridTreeHeaderBackgroundBrush
        {
            get
            {
                if (_gridTreeHeaderBackgroundBrush == null)
                {
                    LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                    {
                        
                        #if !SILVERLIGHT
                        new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEFF5FB"), 0d),
                        new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE1ECFA"), 1d)
                         });
#else
                        new GradientStop(){Color= ColorExtensions.StringToColor("#FFEFF5FB"), Offset= 0d},
                        new GradientStop(){Color= ColorExtensions.StringToColor("#FFE1ECFA"),Offset= 1d}
                         },0.0d);
#endif
                   
                    brush.StartPoint = new Point(0.994, 0.101);
                    brush.EndPoint = new Point(0.994, 0.922);
                    _gridTreeHeaderBackgroundBrush = brush;
                }
                return _gridTreeHeaderBackgroundBrush;
            }
        }

        public Brush GridTreeHeaderForegroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF1E395B"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));

#endif
            }
        }

        public Brush GridTreeHeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFD9E7F8"), 0.031d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEFF5FB"), 0.963d), 
                     });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFD9E7F8"),Offset= 0.031d},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FFEFF5FB"),Offset= 0.963d}, 
                     },0.0d);
#endif
               
                brush.StartPoint = new Point(0.781, 0.014);
                brush.EndPoint = new Point(0.779, 0.987);
                return brush;
            }
        }

        public Brush GridTreeHeaderHoverForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        public GridFontInfo GridTreeHeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI Semibold"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    //FontWeight = FontWeights.SemiBold,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public CellMarginsInfo GridTreeHeaderTextMargins
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

        public Brush GridTreeHeaderInnerBorderBrush
        {
            get
            {
#if !SILVERLIGHT
               
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF849DBD"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD"));

#endif
            }
        }

        public Thickness GridTreeHeaderInnerBorderThickness
        {
            get
            {
#if SILVERLIGHT
                return new Thickness(0.2d);
#else
                 return new Thickness(0.5d);
#endif
                
            }
        }

        public Brush GridTreeSortWidgetBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF849DBD"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF849DBD"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                         
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEEF4FA"),0),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE1EBF9"), 1d),
                    });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFEEF4FA"),Offset= 0},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFE1EBF9"),Offset= 1d},
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }


        private Brush _gridTreeHighlightSelectionBackground = null;
        public Brush GridTreeHighlightSelectionBackground
        {
            get
            {
                if (_gridTreeHighlightSelectionBackground == null)
                {
                    LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFC8DBEF"), 0.058d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFB3CBE5"), 0.598d),   
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFCFE2F2"), 0.984d),
                    });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFC8DBEF"),Offset= 0.058d},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FFB3CBE5"),Offset= 0.598d},   
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFCFE2F2"), Offset= 0.984d},
                    },0.0d);
#endif
                
                    brush.StartPoint = new Point(0.5, 0);
                    brush.EndPoint = new Point(0.5, 1);
                    _gridTreeHighlightSelectionBackground = brush;
                }
                return _gridTreeHighlightSelectionBackground;
            }
        }

        private Brush _gridTreeHighlightSelectionForeground = null;
        public Brush GridTreeHighlightSelectionForeground
        {
            get
            {
                if (_gridTreeHighlightSelectionForeground == null)
#if !SILVERLIGHT
                    _gridTreeHighlightSelectionForeground = new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF6C6D6F"));
#else
                  _gridTreeHighlightSelectionForeground = new SolidColorBrush( ColorExtensions.StringToColor("#FF6C6D6F"));
#endif


                return _gridTreeHighlightSelectionForeground;
            }
        }


        private Brush _gridTreeCurrentCellSelectionBackground = null;
        public Brush GridTreeCurrentCellSelectionBackground
        {
            get
            {
                if (_gridTreeCurrentCellSelectionBackground == null)
                {
                    LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF758EAB"), 0.014),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF8AA3C2"), 0.019), 
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF8AA3C2"), 0.042),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF95AECD"), 0.047),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF9FB7D6"), 0.948),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF96AECD"), 0.977), 
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF87A0BC"), 0.981), 
                     });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FF758EAB"),Offset= 0.014},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF8AA3C2"),Offset= 0.019}, 
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF8AA3C2"),Offset= 0.042},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF95AECD"),Offset= 0.047},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF9FB7D6"),Offset= 0.948},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF96AECD"),Offset= 0.977}, 
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF87A0BC"),Offset= 0.981}, 
                     },0.0d);
#endif
               
                    brush.StartPoint = new Point(0.5, 0);
                    brush.EndPoint = new Point(0.5, 1);
                    _gridTreeCurrentCellSelectionBackground = brush;
                }
                return _gridTreeCurrentCellSelectionBackground;
            }
        }

        public Brush GridTreeCurrentCellSelectionForeground
        {
            get { return Brushes.White; }
        }

        public Brush GridTreeCurrentCellBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF768CA7"));
#else
                return InBuiltBlackBrush;
#endif
            }
        }

        public double GridTreeCurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush GridTreeRowHeaderBackgroundBrush
        {
            get
            {
                return this.GridTreeHeaderBackgroundBrush;
            }
        }

        public Brush GridTreeRowHeaderForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        private Brush _gridTreeRowHoverBackgroundBrush = null;
        public Brush GridTreeRowHoverBackgroundBrush
        {
            get
            {
                if (_gridTreeRowHoverBackgroundBrush == null)
                {
                    LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                      
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF9F9F9"), 0.054d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFC6DDF1"), 0.406d),   
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFD5E5F4"), 0.975d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF5FDFE"), 0.987d),
                     });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFF9F9F9"),Offset= 0.054d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFC6DDF1"),Offset= 0.406d},   
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFD5E5F4"),Offset= 0.975d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFF5FDFE"),Offset= 0.987d},
                     },0.0d);
#endif
               
                    brush.StartPoint = new Point(0.5, 0);
                    brush.EndPoint = new Point(0.5, 1);
                    _gridTreeRowHoverBackgroundBrush = brush;
                }
                return _gridTreeRowHoverBackgroundBrush;
            }
        }

        public Brush GridTreeNodeHighlightBrush
        {
            get
            {
                return this.GridTreeCellBackgroundBrush;
            }
        }
#if !SILVERLIGHT
        public Geometry GridTreeExpanderPlusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 288.334,183.208L 288.334,174.125L 294.958,178.542L 288.334,183.208 Z ");
            }
        }

        public Geometry GridTreeExpanderMinusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 274.487,165.737L 282.487,165.737L 282.487,157.681L 274.487,165.737 Z ");

            }
        }
#else
        public Path GridTreeExpanderPlusPath
        {
            get
            {
                string data = "M-91.702,-146.448L68.0101,-31.5536L-91.702,99.2988z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;

            }
        }

        public Path GridTreeExpanderMinusPath
        {
            get
            {
                string data = "M105.264,-136.319L106.268,65.2559L-91.702,65.2559z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;
            }
        }
#endif

     

        private Brush _gridTreeExpanderBackground = null;
        public Brush GridTreeExpanderBackground
        {
            get
            {
                if (_gridTreeExpanderBackground == null)
                {

#if !SILVERLIGHT
                    _gridTreeExpanderBackground = new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF595959"));
#else
                    _gridTreeExpanderBackground = new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));
#endif

                    //#if !SILVERLIGHT
//_gridTreeExpanderBackground = new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));
//#else
//                    _gridTreeExpanderBackground = new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
//#endif
                }
                return _gridTreeExpanderBackground;
            }
        }

        private Brush _gridTreeExpanderBorderBrush = null;
        public Brush GridTreeExpanderBorderBrush
        {
            get
            {
                if (_gridTreeExpanderBorderBrush == null)
#if !SILVERLIGHT
                    //_gridTreeExpanderBorderBrush = new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFA8A8A8"));
                    _gridTreeExpanderBorderBrush= new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF252525"));

#else
                    //_gridTreeExpanderBorderBrush = new SolidColorBrush(ColorExtensions.StringToColor("#FFA8A8A8"));
                _gridTreeExpanderBorderBrush= new SolidColorBrush(ColorExtensions.StringToColor("#FF252525"));

#endif
                return _gridTreeExpanderBorderBrush;
            }
        }

        private Brush _gridTreeExpanderForeground = null;
        public Brush GridTreeExpanderForeground
        {
            get
            {
                if (_gridTreeExpanderForeground == null)
#if !SILVERLIGHT
                    _gridTreeExpanderForeground = new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                    _gridTreeExpanderForeground = new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
                    return _gridTreeExpanderForeground;
            }
        }

        public Brush GridTreeExpanderExpandedBackground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF595959"));
#else
                    return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));
#endif

//#if !SILVERLIGHT
//                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF595959"));

//#else
//                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

//#endif
            }
        }

        public Brush GridTreeExpanderExpandedBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF252525"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF252525"));

#endif
            }
        }

        public Brush GridTreeExpanderExpandedForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF252525"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF252525"));

#endif
            }
        }

        public Brush GridTreeExpanderHoverBackground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFB11A"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFB11A"));

#endif
            }
        }

        public Brush GridTreeExpanderHoverBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFF861A"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFF861A"));

#endif
            }
        }

        public Brush GridTreeExpanderHoverForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFB11A"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFB11A"));

#endif
            }
        }

        private CellBordersInfo _gridTreeCellBorders = null;
        public CellBordersInfo GridTreeCellBorders
        {
            get
            {
                if (_gridTreeCellBorders == null)
                {
                    _gridTreeCellBorders = new CellBordersInfo()
                    {
                       
#if !SILVERLIGHT
                         Top = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF849DBD")), 0.25d),
                        Bottom = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF849DBD")), 0.25d),
                        Right = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF849DBD")), 0.25d),
                        Left = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF849DBD")), 0.25d)
#else
                        Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")), 0.25d),
                        Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")), 0.25d),
                        Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")), 0.25d),
                        Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD")), 0.25d)
#endif
                    };
                }
                return _gridTreeCellBorders;
            }
        }

        private GridFontInfo _gridTreeCellFont = null;
        public GridFontInfo GridTreeCellFont
        {
            get
            {
                if (_gridTreeCellFont == null)
                {
                    _gridTreeCellFont = new GridFontInfo()
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
                return _gridTreeCellFont;
            }
        }


        private CellMarginsInfo _gridTreeCellTextMargins = null;
        public CellMarginsInfo GridTreeCellTextMargins
        {
            get
            {
                if (_gridTreeCellTextMargins == null)
                {
                    _gridTreeCellTextMargins = new CellMarginsInfo()
                    {
                        Left = 4d
                    };
                }
                return _gridTreeCellTextMargins;
            }
        }

        public Brush GridTreeCellBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        private Brush _gridTreeCellForegroundBrush = null;
        public Brush GridTreeCellForegroundBrush
        {
            get
            {
                if (_gridTreeCellForegroundBrush == null)
#if !SILVERLIGHT
                    _gridTreeCellForegroundBrush = new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF6C6D6F"));
#else
                _gridTreeCellForegroundBrush = new SolidColorBrush(ColorExtensions.StringToColor("#FF6C6D6F"));
#endif

                return _gridTreeCellForegroundBrush;
            }
        }

        public double GridTreeCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush GridTreeCellBorderBrush
        {
            get
            {
                return InBuiltBlackBrush;
            }
        }
    }

     public class GridTreeOffice14SilverVisualStyle : IGridTreeVisualStyle
     {

         public Brush GridTreeRowHoverForegroundBrush
         {
             get { return this.GridTreeCellForegroundBrush; }
         }
         public Brush InBuiltBlackBrush
         {
             get
             {
                 return new SolidColorBrush(Colors.Black);
             }
         }

         public Brush GridTreeBorderBrush
         {
             get 
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFA5ACB5")); 
#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")); 
#endif

             }
         }

         public Thickness GridTreeBorderThickness
         {
             get { return new Thickness(1); }
         }

         public Brush GridTreeHeaderBackgroundBrush
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF5F7F9"), 0d), 
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEFF2F6"), 1d)
                    });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFF5F7F9"), Offset= 0d}, 
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFEFF2F6"), Offset= 1d}
                    },0.0d);
#endif
                
                 brush.StartPoint = new Point(0.0320000015199184, 0.111000001430511);
                 brush.EndPoint = new Point(0.0320000015199184, 0.912000000476837);
                 return brush;
             }
         }

         public Brush GridTreeHeaderForegroundBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF1E395B"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF1E395B"));

#endif
             }
         }

         public Brush GridTreeHeaderHoverBackgroundBrush
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEFF2F6"), 0),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF5F7F9"), 0.881d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF5F7F9"), 1d)
                    });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFEFF2F6"),Offset= 0},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFF5F7F9"),Offset= 0.881d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFF5F7F9"),Offset= 1d}
                    },0.0d);
#endif
                
                 brush.StartPoint = new Point(0.779, 0.987);
                 brush.EndPoint = new Point(0.781, 0.014);
                 return brush;
             }
         }

         public Brush GridTreeHeaderHoverForegroundBrush
         {
             get
             {
                 return this.GridTreeHeaderForegroundBrush;
             }
         }

         public GridFontInfo GridTreeHeaderFont
         {
             get
             {
                 return new GridFontInfo()
                 {
                     FontFamily = new FontFamily("Segoe UI Semibold"),
                     FontSize = 12d,
                     FontStretch = new FontStretch(),
                     FontStyle = new FontStyle(),
                     //FontWeight = FontWeights.Bold,
                     TextDecorations = null,
                     Orientation = 0
                 };
             }
         }

         public CellMarginsInfo GridTreeHeaderTextMargins
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

         public Brush GridTreeHeaderInnerBorderBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFA5ACB5"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5"));

#endif
             }
         }

         public Thickness GridTreeHeaderInnerBorderThickness
         {
             get
             {
#if SILVERLIGHT
                 return new Thickness(0.2d);
#else
                 return new Thickness(0.5d);
#endif
                 
             }
         }

         public Brush GridTreeSortWidgetBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF595959"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

#endif
             }
         }

         public Brush GridTreeSortWidgetBorderBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFA5ACB5"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5"));

#endif
             }
         }

         public Brush GridTreeSortWidgetBorderHoverBackgroundBrush
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                        
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEFF2F6"),0.002d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE1EBF9"),1d),
                     });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFEFF2F6"),Offset= 0.002d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFE1EBF9"),Offset= 1d},
                     },0.0d);
#endif
               
                 brush.StartPoint = new Point(0.5, 0.101);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush;
             }
         }

         public Brush GridTreeHighlightSelectionBackground
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                     #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFD7DCE2"), 0.213d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFD4DAE1"), 0.622d),   
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE6ECF1"), 0.975d),
                    });
#else
                     new GradientStop(){Color=ColorExtensions.StringToColor("#FFD7DCE2"),Offset= 0.213d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFD4DAE1"),Offset= 0.622d},   
                     new GradientStop(){Color=ColorExtensions.StringToColor("#FFE6ECF1"),Offset= 0.975d},
                     },0.0d);
#endif
                
                 brush.StartPoint = new Point(0.5, 0);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush;
             }
         }

         public Brush GridTreeHighlightSelectionForeground
         {
             get
             {
#if !SILVERLIGHT
                 return GridUtil.GetXamlConvertedValue<Brush>("#FF6C6D6F");
#else
                 return new SolidColorBrush( ColorExtensions.StringToColor("#FF6C6D6F"));
#endif

             }
         }

         public Brush GridTreeCurrentCellSelectionBackground
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA6ACB4"), 0.02),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFB0B8C0"), 0.025), 
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFBCC3CB"), 0.204),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFBCC3CB"), 0.975),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFB4BAC2"), 0.98)
                    });
#else
                     new GradientStop(){ Color= ColorExtensions.StringToColor("#FFA6ACB4"),Offset= 0.02},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFB0B8C0"),Offset= 0.025}, 
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFBCC3CB"),Offset= 0.204},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFBCC3CB"),Offset= 0.975},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFB4BAC2"),Offset= 0.98}
                    },0.0d);
#endif
                
                 brush.StartPoint = new Point(0.5, 0);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush;
             }
         }

         public Brush GridTreeCurrentCellSelectionForeground
         {
             get { return Brushes.White; }
         }

         public Brush GridTreeCurrentCellBorderBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF9D9FA1"));
#else
                return InBuiltBlackBrush;
#endif
             }
         }

         public double GridTreeCurrentCellBorderWidth
         {
             get
             {
                 return 0.5d;
             }
         }

         public Brush GridTreeRowHeaderBackgroundBrush
         {
             get
             {
                 return this.GridTreeHeaderBackgroundBrush;
             }
         }

         public Brush GridTreeRowHeaderForegroundBrush
         {
             get
             {
                 return this.GridTreeHeaderForegroundBrush;
             }
         }

         public Brush GridTreeRowHoverBackgroundBrush
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     
                     #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFD7DCE2"), 0.213d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFD4DAE1"), 0.622d),   
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE6ECF1"), 0.975d),
                    });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFD7DCE2"),Offset= 0.213d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFD4DAE1"),Offset= 0.622d},   
                     new GradientStop(){Color=ColorExtensions.StringToColor("#FFE6ECF1"),Offset= 0.975d},
                     },0.0d);
#endif
                
                 brush.StartPoint = new Point(0.5, 0);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush;
             }
         }

         public Brush GridTreeNodeHighlightBrush
         {
             get
             {
                 return this.GridTreeCellBackgroundBrush;
             }
         }
#if !SILVERLIGHT
         public Geometry GridTreeExpanderPlusPath
         {
             get
             {
                 return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 288.334,183.208L 288.334,174.125L 294.958,178.542L 288.334,183.208 Z ");
             }
         }

         public Geometry GridTreeExpanderMinusPath
         {
             get
             {
                 return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 274.487,165.737L 282.487,165.737L 282.487,157.681L 274.487,165.737 Z ");

             }
         }
#else
         public Path GridTreeExpanderPlusPath
         {
             get
             {
                 string data = "M-91.702,-146.448L68.0101,-31.5536L-91.702,99.2988z";
                 string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                 return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;

             }
         }

         public Path GridTreeExpanderMinusPath
         {
             get
             {
                 string data = "M105.264,-136.319L106.268,65.2559L-91.702,65.2559z";
                 string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                 return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;
             }
         }
#endif



         public Brush GridTreeExpanderBackground
         {
             get
             {

#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF595959"));
#else
                    return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));
#endif
//#if !SILVERLIGHT
//                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

//#else
//                 return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

//#endif
             }
         }

         public Brush GridTreeExpanderBorderBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFA8A8A8"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FFA8A8A8"));

#endif
             }
         }

         public Brush GridTreeExpanderForeground
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
             }
         }

         public Brush GridTreeExpanderExpandedBackground
         {
             get
             {
                 return this.GridTreeExpanderBackground;
             }
         }

         public Brush GridTreeExpanderExpandedBorderBrush
         {
             get { return this.GridTreeExpanderBorderBrush; }
         }

         public Brush GridTreeExpanderExpandedForeground
         {
             get { return this.GridTreeExpanderForeground; }
         }

         public Brush GridTreeExpanderHoverBackground
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFB11A"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFB11A"));

#endif
             }
         }

         public Brush GridTreeExpanderHoverBorderBrush
         {
             get
             {
                 return GridTreeExpanderHoverBackground;
             }
         }

         public Brush GridTreeExpanderHoverForeground
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFB11A"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFB11A"));

#endif
             }
         }

         public CellBordersInfo GridTreeCellBorders
         {
             get
             {
                 return new CellBordersInfo()
                 {
                     
#if !SILVERLIGHT
                     Top = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFA5ACB5")), 0.25d),
                     Bottom = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFA5ACB5")), 0.25d),
                     Right = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFA5ACB5")), 0.25d),
                     Left = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFA5ACB5")), 0.25d)
#else
                     Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 0.25d),
                     Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 0.25d),
                     Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 0.25d),
                     Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFA5ACB5")), 0.25d)
#endif
                 };
             }
         }

         public GridFontInfo GridTreeCellFont
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

         public CellMarginsInfo GridTreeCellTextMargins
         {
             get
             {
                 return new CellMarginsInfo()
                 {
                     Left = 4d
                 };
             }
         }

         public Brush GridTreeCellBackgroundBrush
         {
             get
             {
                 return Brushes.White;
             }
         }

         public Brush GridTreeCellForegroundBrush
         {
             get
             {
#if !SILVERLIGHT
                  return  GridUtil.GetXamlConvertedValue<Brush>("#FF6C6D6F");
#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF6C6D6F"));
#endif

             }
         }

         public double GridTreeCellBorderWidth
         {
             get
             {
                 return 0.2d;
             }
         }

         public Brush GridTreeCellBorderBrush
         {
             get
             {
                 return InBuiltBlackBrush;
             }
         }
     }

    public class GridTreeOffice2007BlackVisualStyle : IGridTreeVisualStyle
    {

        public Brush GridTreeRowHoverForegroundBrush
        {
            get { return this.GridTreeCellForegroundBrush; }
        }
        public Brush InBuiltBlackBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Black);
            }
        }



        public Brush GridTreeBorderBrush
        {
            get 
            {
#if !SILVERLIGHT
                return  GridUtil.GetXamlConvertedValue<Brush>("#FF4C5051"); 
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF4C5051")); 
#endif

            }
        }

        public Thickness GridTreeBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush GridTreeHeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFCFDFE"), 0.061d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFD7D7E1"), 0.896),
                    });
#else
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FFFCFDFE"),Offset= 0.061d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFD7D7E1"),Offset= 0.896},
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHeaderForegroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF00080E"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF00080E"));

#endif
            }
        }

        public Brush GridTreeHeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFBFCFD"), 0.046d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFD6D6E0"), 0.904d),
                     });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFFBFCFD"),Offset= 0.046d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFD6D6E0"),Offset= 0.904d},
                     },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHeaderHoverForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        public GridFontInfo GridTreeHeaderFont
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

        public CellMarginsInfo GridTreeHeaderTextMargins
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

        public Brush GridTreeHeaderInnerBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#004C535C"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#004C535C"));

#endif
            }
        }

        public Thickness GridTreeHeaderInnerBorderThickness
        {
            get
            {
#if SILVERLIGHT
                return new Thickness(0.2d);
#else
                 return new Thickness(0.5d);
#endif
            }
        }

        public Brush GridTreeSortWidgetBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF00080E"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF00080E"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF4C5051"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF4C5051"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                      
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFBFCFD"), 0.046d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFD6D6E0"), 0.904d),
                    });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFBFCFD"),Offset= 0.046d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFD6D6E0"),Offset= 0.904d},
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFD4A1"), 0.012d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFBA6B"), 0.304),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFEAE42"), 0.31d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFEE89A"), 1d)
                    });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFD4A1"),Offset= 0.012d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFBA6B"),Offset= 0.304},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFEAE42"),Offset= 0.31d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFEE89A"),Offset= 1d}
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHighlightSelectionForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF485565"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));

#endif
            }
        }

        public Brush GridTreeCurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE8A567"), 0.006),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFA83D"), 0.298),  
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFF8D00"), 0.304),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFC551"), 0.963),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFBC35"), 1),
                     });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFE8A567"),Offset= 0.006},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFA83D"),Offset= 0.298},  
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFF8D00"),Offset= 0.304},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFC551"),Offset= 0.963},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFBC35"),Offset= 1},
                 },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeCurrentCellSelectionForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF485565"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));

#endif
            }

        }

        public Brush GridTreeCurrentCellBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFBB83D"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFBB83D"));

#endif
            }
        }

        public double GridTreeCurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public Brush GridTreeRowHeaderBackgroundBrush
        {
            get
            {
                return this.GridTreeHeaderBackgroundBrush;
            }
        }

        public Brush GridTreeRowHeaderForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        public Brush GridTreeRowHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFFBDA"), 0.022d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFE8A8"), 0.499),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFD767"), 0.507d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFE59C"), 1d)
                    });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFFBDA"),Offset= 0.022d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFE8A8"),Offset= 0.499},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFD767"),Offset= 0.507d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFE59C"),Offset= 1d}
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeNodeHighlightBrush
        {
            get
            {
                return this.GridTreeCellBackgroundBrush;
            }
        }
#if !SILVERLIGHT
        public Geometry GridTreeExpanderPlusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 288.334,183.208L 288.334,174.125L 294.958,178.542L 288.334,183.208 Z ");
            }
        }

        public Geometry GridTreeExpanderMinusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 274.487,165.737L 282.487,165.737L 282.487,157.681L 274.487,165.737 Z ");

            }
        }
#else
        public Path GridTreeExpanderPlusPath
        {
            get
            {
                string data = "M-91.702,-146.448L68.0101,-31.5536L-91.702,99.2988z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;

            }
        }

        public Path GridTreeExpanderMinusPath
        {
            get
            {
                string data = "M105.264,-136.319L106.268,65.2559L-91.702,65.2559z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;
            }
        }
#endif
       

        public Brush GridTreeExpanderBackground
        {
            get
            {
             
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF485565"));
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
#endif
                //                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
//                {
                   
//                    #if !SILVERLIGHT
//                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"), 0.229d),
//                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFB0C8F0"), 0.974d)
//                     });
//#else
//                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFFFFF"), Offset= 0.229d},
//                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFB0C8F0"),Offset= 0.974d}
//                     },0.0d);
//#endif
               
//                brush.StartPoint = new Point(0.101, 0.088);
//                brush.EndPoint = new Point(0.792, 0.86);
                //return brush;
            }
        }

        public Brush GridTreeExpanderBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF3F576F"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));

#endif
            }
        }

        public Brush GridTreeExpanderForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF304860"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF304860"));

#endif
            }
        }

        public Brush GridTreeExpanderExpandedBackground
        {
            get
            {
                return this.GridTreeExpanderBackground;
            }
        }

        public Brush GridTreeExpanderExpandedBorderBrush
        {
            get { return GridTreeExpanderBorderBrush; }
        }

        public Brush GridTreeExpanderExpandedForeground
        {
            get { return GridTreeExpanderForeground; }
        }

        public Brush GridTreeExpanderHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFFBDA"), 0.022d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFE8A8"), 0.499), 
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFD767"), 0.507),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFE59C"), 1), 
                    });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFFBDA"),Offset= 0.022d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFE8A8"),Offset= 0.499}, 
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFD767"),Offset= 0.507},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFE59C"),Offset= 1}, 
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeExpanderHoverBorderBrush
        {
            get { return this.GridTreeExpanderBorderBrush; }
        }

        public Brush GridTreeExpanderHoverForeground
        {
            get 
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF3F576F"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));

#endif
            }

        }

        public CellBordersInfo GridTreeCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    
#if !SILVERLIGHT
Top = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFBDBDBD")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFBDBDBD")), 0.25d),
                    Right = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFBDBDBD")), 0.25d),
                    Left = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFBDBDBD")), 0.25d)
#else
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFBDBDBD")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFBDBDBD")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFBDBDBD")), 0.25d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFBDBDBD")), 0.25d)
#endif
                };
            }
        }

        public GridFontInfo GridTreeCellFont
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

        public CellMarginsInfo GridTreeCellTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d
                };
            }
        }

        public Brush GridTreeCellBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush GridTreeCellForegroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF485565"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));

#endif
            }
        }

        public double GridTreeCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush GridTreeCellBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF4C5051"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF4C5051"));

#endif
            }
        }
    }

     public class GridTreeOffice2007BlueVisualStyle : IGridTreeVisualStyle
     {

         public Brush GridTreeRowHoverForegroundBrush
         {
             get { return this.GridTreeCellForegroundBrush; }
         }
         public Brush InBuiltBlackBrush
         {
             get
             {
                 return new SolidColorBrush(Colors.Black);
             }
         }

         public Brush GridTreeBorderBrush
         {
             get
             {
#if !SILVERLIGHT
                 return  GridUtil.GetXamlConvertedValue<Brush>("#FF6C98D1");
#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1"));
#endif

             }
         }

         public Thickness GridTreeBorderThickness
         {
             get { return new Thickness(1); }
         }

         public Brush GridTreeHeaderBackgroundBrush
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFC4D9FF"), 0.97),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFBFFFF"), 0.03), 
                    });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFC4D9FF"),Offset= 0.97},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFBFFFF"),Offset= 0.03}, 
                    },0.0d);
#endif
                
                 brush.StartPoint = new Point(0.5, 0);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush;
             }
         }

         public Brush GridTreeHeaderForegroundBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF15428B"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF15428B"));

#endif
             }
         }

         public Brush GridTreeHeaderHoverBackgroundBrush
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE4FBFD"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFC3D8FE"), 1), 
                     });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFE4FBFD"),Offset= 0d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFC3D8FE"),Offset= 1},  
                     },0.0d);
#endif
               
                 brush.StartPoint = new Point(0.5, 0.101);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush;
             }
         }

         public Brush GridTreeHeaderHoverForegroundBrush
         {
             get
             {
                 return this.GridTreeHeaderForegroundBrush;
             }
         }

         public GridFontInfo GridTreeHeaderFont
         {
             get
             {
                 return new GridFontInfo()
                 {
                     FontFamily = new FontFamily("Segoe UI Semibold"),
                     FontSize = 12d,
                     FontStretch = new FontStretch(),
                     FontStyle = new FontStyle(),
                     //FontWeight = FontWeights.Bold,
                     TextDecorations = null,
                     Orientation = 0
                 };
             }
         }

         public CellMarginsInfo GridTreeHeaderTextMargins
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

         public Brush GridTreeHeaderInnerBorderBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF6C98D1"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1"));

#endif
             }
         }

         public Thickness GridTreeHeaderInnerBorderThickness
         {
             get
             {
#if SILVERLIGHT
                 return new Thickness(0.2d);
#else
                 return new Thickness(0.5d);
#endif
             }
         }

         public Brush GridTreeSortWidgetBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF15428B"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF15428B"));

#endif
             }
         }

         public Brush GridTreeSortWidgetBorderBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF6C98D1"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1"));

#endif
             }
         }

         public Brush GridTreeSortWidgetBorderHoverBackgroundBrush
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                         
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF6FBFE"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFC3D8FE"), 1),
                    });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFF6FBFE"),Offset= 0d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFC3D8FE"),Offset= 1},
                    },0.0d);
#endif
                
                 brush.StartPoint = new Point(0.5, 0.101);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush;
             }
         }

         public Brush GridTreeHighlightSelectionBackground
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFD4A1"), 0.012),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFBA6B"), 0.304),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFEAE42"), 0.31),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFEE89A"), 1d)
                     });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFD4A1"),Offset= 0.012},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset= 0.304},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFEAE42"),Offset= 0.31},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFEE89A"),Offset= 1d}
                     },0.0d);
#endif
               
                 brush.StartPoint = new Point(0.5, 0);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush;
             }
         }

         public Brush GridTreeHighlightSelectionForeground
         {
             get
             {
#if !SILVERLIGHT
                 return GridUtil.GetXamlConvertedValue<Brush>("#FF485565");
#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
#endif

             }
         }

         public Brush GridTreeCurrentCellSelectionBackground
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE8A567"), 0.006),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFA83D"), 0.298),  
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFF8D00"), 0.304),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFC551"), 0.963),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFBC35"), 1),
                    });
#else
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FFE8A567"),Offset= 0.006},
                    new GradientStop(){ Color=ColorExtensions.StringToColor("#FFFFA83D"),Offset= 0.298},  
                    new GradientStop(){ Color=ColorExtensions.StringToColor("#FFFF8D00"),Offset= 0.304},
                    new GradientStop(){ Color=ColorExtensions.StringToColor("#FFFFC551"),Offset= 0.963},
                    new GradientStop(){ Color=ColorExtensions.StringToColor("#FFFFBC35"),Offset= 1},
                    },0.0d);
#endif
                
                 brush.StartPoint = new Point(0.5, 0);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush; ;
             }
         }

         public Brush GridTreeCurrentCellSelectionForeground
         {
             get 
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF485565"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));

#endif
             }

         }

         public Brush GridTreeCurrentCellBorderBrush
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                       
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF7B6541"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA48D62"), 0.996),
                    });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FF7B6541"),Offset= 0d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFA48D62"),Offset= 0.996},
                    },0.0d);
#endif
                
                 brush.StartPoint = new Point(0.5, 0);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush;
             }
         }

         public double GridTreeCurrentCellBorderWidth
         {
             get
             {
                 return 0.5d;
             }
         }

         public Brush GridTreeRowHoverBackgroundBrush
         {

             get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFFBDA"), 0.022d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFE8A8"), 0.499),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFD767"), 0.507d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFE59C"), 1d)
                    });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFFBDA"),Offset= 0.022d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFE8A8"),Offset= 0.499},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFD767"),Offset= 0.507d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFE59C"),Offset= 1d}
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
             //get
             //{
             //    LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
             //   {
             //       new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEFF7FD"), 0.056),
             //       new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFDEEEF9"), 0.944),
             //       new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFD767"), 0.507),
             //       new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFE59C"), 1d)
             //   });
             //    brush.StartPoint = new Point(0.5, 0);
             //    brush.EndPoint = new Point(0.5, 1);
             //    return brush;

             //}
         }

         public Brush GridTreeNodeHighlightBrush
         {
             get
             {
                 return this.GridTreeCellBackgroundBrush;
             }
         }
#if !SILVERLIGHT
         public Geometry GridTreeExpanderPlusPath
         {
             get
             {
                 return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 288.334,183.208L 288.334,174.125L 294.958,178.542L 288.334,183.208 Z ");
             }
         }

         public Geometry GridTreeExpanderMinusPath
         {
             get
             {
                 return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 274.487,165.737L 282.487,165.737L 282.487,157.681L 274.487,165.737 Z ");

             }
         }
#else
         public Path GridTreeExpanderPlusPath
         {
             get
             {
                 string data = "M-91.702,-146.448L68.0101,-31.5536L-91.702,99.2988z";
                 string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                 return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;

             }
         }

         public Path GridTreeExpanderMinusPath
         {
             get
             {
                 string data = "M105.264,-136.319L106.268,65.2559L-91.702,65.2559z";
                 string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                 return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;
             }
         }
#endif


         public Brush GridTreeExpanderBackground
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF485565"));
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
#endif
//                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
//                {
                    
//                    #if !SILVERLIGHT
//                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("White"), 0.229),
//                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFB0C8F0"), 0.974)
//                    });
//#else
//                    new GradientStop(){Color= ColorExtensions.StringToColor("White"), Offset= 0.229},
//                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFB0C8F0"),Offset= 0.974}
//                    },0.0d);
//#endif
                
//                 brush.StartPoint = new Point(0.101, 0.088);
//                 brush.EndPoint = new Point(0.792, 0.86);
//                 return brush;
             }
         }

         public Brush GridTreeExpanderBorderBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF3F576F"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));

#endif
             }
         }

         public Brush GridTreeExpanderForeground
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF304860"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF304860"));

#endif
             }
         }

         public Brush GridTreeExpanderExpandedBackground
         {
             get { return this.GridTreeExpanderBackground; }
         }

         public Brush GridTreeExpanderExpandedBorderBrush
         {
             get { return this.GridTreeExpanderBorderBrush; }
         }

         public Brush GridTreeExpanderExpandedForeground
         {
             get { return this.GridTreeExpanderForeground; }
         }

         public Brush GridTreeExpanderHoverBackground
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFFBDA"), 0.022d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFE8A8"), 0.499), 
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFD767"), 0.507),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFE59C"), 1),
                    });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFFBDA"),Offset= 0.022d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFE8A8"),Offset= 0.499}, 
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFD767"),Offset= 0.507},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFE59C"),Offset= 1},
                    },0.0d);
#endif
                
                 brush.StartPoint = new Point(0.5, 0.101);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush;
             }
         }

         public Brush GridTreeExpanderHoverBorderBrush
         {
             get { return this.GridTreeExpanderBorderBrush; }
         }

         public Brush GridTreeExpanderHoverForeground
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF485565"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));

#endif
             }
         }

         private CellBordersInfo _gridTreeCellBorders = null;
         public CellBordersInfo GridTreeCellBorders
         {
             get
             {
                 if (_gridTreeCellBorders == null)
                     _gridTreeCellBorders = new CellBordersInfo()
                     {
                         
#if !SILVERLIGHT
                         Top = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF6C98D1")), 0.25d),
                         Bottom = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF6C98D1")), 0.25d),
                         Right = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF6C98D1")), 0.25d),
                         Left = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF6C98D1")), 0.25d)
#else
                         Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1")), 0.25d),
                         Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1")), 0.25d),
                         Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1")), 0.25d),
                         Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF6C98D1")), 0.25d)
#endif
                     };
                 return _gridTreeCellBorders;
             }
         }

         public GridFontInfo GridTreeCellFont
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

         public CellMarginsInfo GridTreeCellTextMargins
         {
             get
             {
                 return new CellMarginsInfo()
                 {
                     Left = 4d
                 };
             }
         }

         public Brush GridTreeCellBackgroundBrush
         {
             get
             {
                 return Brushes.White;
             }
         }

         public Brush GridTreeCellForegroundBrush
         {
             get
             {
#if !SILVERLIGHT
                 return GridUtil.GetXamlConvertedValue<Brush>("#FF485565");
#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
#endif

             }
         }

         public double GridTreeCellBorderWidth
         {
             get
             {
                 return 0.2d;
             }
         }

         public Brush GridTreeCellBorderBrush
         {
             get
              {
                  LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFBB83D"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFBB83D"), 0.996),
                     });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFBB83D"),Offset= 0d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFBB83D"),Offset= 0.996}, 
                     }, 0.0d);
#endif

                  brush.StartPoint = new Point(0.5, 0);
                  brush.EndPoint = new Point(0.5, 1);
                  return brush;
            }
         }


         public Brush GridTreeRowHeaderBackgroundBrush
         {
             get
             {
#if !SILVERLIGHT
                 return GridUtil.GetXamlConvertedValue<Brush>("#FFE3EFFF");
#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FFE3EFFF"));
#endif

             }
         }

         public Brush GridTreeRowHeaderForegroundBrush
         {
             get
             {
                 return this.GridTreeHeaderForegroundBrush;
             }
         }
     }

    public class GridTreeOffice2007SilverVisualStyle : IGridTreeVisualStyle
    {

        public Brush GridTreeRowHoverForegroundBrush
        {
            get { return this.GridTreeCellForegroundBrush; }
        }
        public Brush InBuiltBlackBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Black);
            }
        }


        public Brush GridTreeBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                 return  GridUtil.GetXamlConvertedValue<Brush>("#FF707173"); 
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF707173")); 
#endif

            }
        }

        public Thickness GridTreeBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush GridTreeHeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFDDDEE2"), 0.874d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFAFBFD"), 0.082),
                    });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFDDDEE2"),Offset= 0.874d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFAFBFD"),Offset= 0.082},
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHeaderForegroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF303E7B"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF303E7B"));

#endif
            }
        }

        public Brush GridTreeHeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFDCDDE1"), 0.888d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF9FAFC"), 0.056), 
                    });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFDCDDE1"),Offset= 0.888d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFF9FAFC"),Offset= 0.056}, 
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHeaderHoverForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        public GridFontInfo GridTreeHeaderFont
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

        public CellMarginsInfo GridTreeHeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo() { Left = 4d, Right = 4d };
            }
        }

        public Brush GridTreeHeaderInnerBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#006F7074"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#006F7074"));

#endif
            }
        }

        public Thickness GridTreeHeaderInnerBorderThickness
        {
            get
            {
#if SILVERLIGHT
                return new Thickness(0.2d);
#else
                 return new Thickness(0.5d);
#endif
            }
        }

        public Brush GridTreeSortWidgetBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF485565"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF707173"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF707173"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFDCDDE1"), 0.888d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF9FAFC"), 0.056d),
                     });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFDCDDE1"),Offset= 0.888d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFF9FAFC"),Offset= 0.056d},
                     },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFD4A1"), 0.012d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFBA6B"), 0.304),                  
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFEAE42"), 0.31d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFEE89A"), 1), 
                     });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFD4A1"),Offset= 0.012d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFBA6B"),Offset= 0.304},                  
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFEAE42"),Offset= 0.31d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFEE89A"),Offset= 1}, 
                     },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHighlightSelectionForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF485565"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));

#endif
            }
        }

        public Brush GridTreeCurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE8A567"), 0.006),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFA83D"), 0.298), 
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFF8D00"), 0.304),   
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFC551"), 0.963),   
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFBC35"), 1), 
                    });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFE8A567"),Offset= 0.006},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFA83D"),Offset= 0.298}, 
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFF8D00"),Offset= 0.304},   
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFC551"),Offset= 0.963},   
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFBC35"),Offset= 1}, 
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush; ;
            }
        }

        public Brush GridTreeCurrentCellSelectionForeground
        {
            get
            { 
               
#if !SILVERLIGHT
 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF485565"));
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
#endif
            }

        }

        public Brush GridTreeCurrentCellBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFBB83D"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFBB83D"), 0.996),
                    });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFBB83D"),Offset= 0d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFBB83D"),Offset= 0.996},
                    },0.0d);
#endif

                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public double GridTreeCurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public Brush GridTreeRowHeaderBackgroundBrush
        {
            get
            {
                return this.GridTreeHeaderBackgroundBrush;
            }
        }

        public Brush GridTreeRowHeaderForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        public Brush GridTreeRowHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                       
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFFBDA"), 0.022d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFE8A8"), 0.499),                  
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFD767"), 0.507d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFE59C"), 1),
                    });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFFBDA"),Offset= 0.022d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFE8A8"),Offset= 0.499},                  
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFD767"),Offset= 0.507d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFE59C"),Offset= 1}, 
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeNodeHighlightBrush
        {
            get
            {
                return this.GridTreeCellBackgroundBrush;
            }
        }
#if !SILVERLIGHT
        public Geometry GridTreeExpanderPlusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 288.334,183.208L 288.334,174.125L 294.958,178.542L 288.334,183.208 Z ");
            }
        }

        public Geometry GridTreeExpanderMinusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 274.487,165.737L 282.487,165.737L 282.487,157.681L 274.487,165.737 Z ");

            }
        }
#else
        public Path GridTreeExpanderPlusPath
        {
            get
            {
                string data = "M-91.702,-146.448L68.0101,-31.5536L-91.702,99.2988z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;

            }
        }

        public Path GridTreeExpanderMinusPath
        {
            get
            {
                string data = "M105.264,-136.319L106.268,65.2559L-91.702,65.2559z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;
            }
        }
#endif

      

        public Brush GridTreeExpanderBackground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF485565"));
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));
#endif
//                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
//                {
                   
//                    #if !SILVERLIGHT
//                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"), 0.229d),
//                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFB0C8F0"), 0.974d)
//                     });
//#else
//                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFFFFF"),Offset= 0.229d},
//                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFB0C8F0"),Offset= 0.974d}
//                     },0.0d);
//#endif
               
//                brush.StartPoint = new Point(0.101, 0.088);
//                brush.EndPoint = new Point(0.792, 0.86);
//                return brush;
            }
        }

        public Brush GridTreeExpanderBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF3F576F"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));

#endif
            }
        }

        public Brush GridTreeExpanderForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF304860"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF304860"));

#endif
            }
        }

        public Brush GridTreeExpanderExpandedBackground
        {
            get { return this.GridTreeExpanderBackground; }
        }

        public Brush GridTreeExpanderExpandedBorderBrush
        {
            get { return this.GridTreeExpanderBorderBrush; }
        }

        public Brush GridTreeExpanderExpandedForeground
        {
            get { return this.GridTreeExpanderForeground; }
        }

        public Brush GridTreeExpanderHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFFBDA"), 0.022d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFE8A8"), 0.499), 
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFD767"), 0.507),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFE59C"), 1), 
                    });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFFBDA"),Offset= 0.022d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFE8A8"),Offset= 0.499}, 
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFD767"),Offset= 0.507},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FFFFE59C"),Offset= 1}, 
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeExpanderHoverBorderBrush
        {
            get { return this.GridTreeExpanderBorderBrush; }
        }

        public Brush GridTreeExpanderHoverForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF3F576F"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));

#endif
            }
        }

        public CellBordersInfo GridTreeCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    
#if !SILVERLIGHT
                    Top = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFC6C6C6")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFC6C6C6")), 0.25d),
                    Right = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFC6C6C6")), 0.25d),
                    Left = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFC6C6C6")), 0.25d)
#else
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC6C6C6")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC6C6C6")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC6C6C6")), 0.25d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC6C6C6")), 0.25d)
#endif
                };
            }
        }

        public GridFontInfo GridTreeCellFont
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

        public CellMarginsInfo GridTreeCellTextMargins
        {
            get
            {
                return new CellMarginsInfo() { Left = 4d };
            }
        }

        public Brush GridTreeCellBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush GridTreeCellForegroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF485565"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF485565"));

#endif
            }
        }

        public double GridTreeCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush GridTreeCellBorderBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF7B6541"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA48D62"), 0.996), 
                      });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FF7B6541"),Offset= 0d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFA48D62"),Offset= 0.996}, 
                      },0.0d);
#endif
              
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }
    }

    public class GridTreeShinyBlueVisualStyle : IGridTreeVisualStyle
    {

        public Brush GridTreeRowHoverForegroundBrush
        {
            get 
            {
                return Brushes.White;
                //return this.GridTreeCellForegroundBrush; 
            }
        }
        public Brush InBuiltBlackBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        public Brush GridTreeBorderBrush
        {
            get 
            {
#if !SILVERLIGHT
                return  GridUtil.GetXamlConvertedValue<Brush>("#FF3A3A3A"); 
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A")); 
#endif

            }
        }

        public Thickness GridTreeBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush GridTreeHeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA4D5F9"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF4193E0"), 0.384),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF1276D4"), 0.412),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF0168C8"), 0.506),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF08477D"), 0.973), 
                     });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFA4D5F9"),Offset= 0d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF4193E0"),Offset= 0.384},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF1276D4"),Offset= 0.412},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF0168C8"),Offset= 0.506},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF08477D"),Offset= 0.973}, 
                     },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHeaderForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush GridTreeHeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                      
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA4D5F9"), 0),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF4193E0"), 0.343),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF1276D4"), 0.392),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF0168C8"), 0.441),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF035DB0"), 0.608),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF559ACB"), 0.804),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF93D5FA"), 0.951),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFC5E9FE"), 1d), 
                     });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFA4D5F9"),Offset= 0},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF4193E0"),Offset= 0.343},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF1276D4"),Offset= 0.392},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF0168C8"),Offset= 0.441},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF035DB0"),Offset= 0.608},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF559ACB"),Offset= 0.804},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF93D5FA"),Offset= 0.951},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFC5E9FE"),Offset= 1d}, 
                     },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHeaderHoverForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        public GridFontInfo GridTreeHeaderFont
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

        public CellMarginsInfo GridTreeHeaderTextMargins
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

        public Brush GridTreeHeaderInnerBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF5AA4E6"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF5AA4E6"));

#endif
            }
        }

        public Thickness GridTreeHeaderInnerBorderThickness
        {
            get
            {
#if SILVERLIGHT
                return new Thickness(0.2d);
#else
                 return new Thickness(0.5d);
#endif
            }
        }

        public Brush GridTreeSortWidgetBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF3A3A3A"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA4D5F9"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF4193E0"), 0.343d),         
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF1276D4"), 0.392d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF0168C8"), 0.401d),   
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF035DB0"), 0.608d),   
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF559ACB"), 0.804d),         
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF93D5FA"), 0.951d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFC5E9FE"), 1d), 
                     });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFA4D5F9"),Offset= 0d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF4193E0"),Offset= 0.343d},         
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF1276D4"),Offset= 0.392d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF0168C8"),Offset= 0.401d},   
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF035DB0"),Offset= 0.608d},   
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF559ACB"),Offset= 0.804d},         
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF93D5FA"),Offset= 0.951d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFC5E9FE"),Offset= 1d}, 
                     },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFB5B5B7"), 0.021),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF767877"), 0.37d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF6B6B6B"), 0.377d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF9C979E"), 0.951d)
                     });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFB5B5B7"),Offset= 0.021},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF767877"),Offset= 0.37d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF6B6B6B"),Offset= 0.377d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF9C979E"),Offset= 0.951d}
                     },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHighlightSelectionForeground
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush GridTreeCurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEAECEB"), 0.023),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFBCBDBF"), 0.402),   
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA3A7AA"), 0.41),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA3A7AA"), 0.448),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF979CA0"), 0.456),   
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF74787B"), 0.938),
                    });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFEAECEB"),Offset= 0.023},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFBCBDBF"),Offset= 0.402},   
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFA3A7AA"),Offset= 0.41},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFA3A7AA"),Offset= 0.448},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF979CA0"),Offset= 0.456},   
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF74787B"),Offset= 0.938},
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeCurrentCellSelectionForeground
        {
            get { return Brushes.White; }
        }

        public Brush GridTreeCurrentCellBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF353332"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF353332"));

#endif
            }
        }

        public double GridTreeCurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush GridTreeRowHeaderBackgroundBrush
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF79B8EE"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF0E73D1"), 0.384d),
                   
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF064F8E"), 1d)
                     });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FF79B8EE"),Offset= 0d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF0E73D1"),Offset= 0.384d},
                    
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF064F8E"),Offset= 1d}
                     },0.0d);
#endif

                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                //brush.Opacity = 0.415;
                return brush;
                //return this.GridTreeHeaderBackgroundBrush;
            }
        }

        public Brush GridTreeRowHeaderForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        public Brush GridTreeRowHoverBackgroundBrush
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    //new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEAECEB"), 0.023),
                    //new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFBCBDBF"), 0.402),   
                    //new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA3A7AA"), 0.41),
                    //new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA3A7AA"), 0.448),
                    //new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF979CA0"), 0.456),   
                    //new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF74787B"), 0.938),
                    //});
                   
                    #if !SILVERLIGHT
                //new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEAECEB"), 0.023),
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF9A9A9A"), 0.038),
                    //new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFBCBDBF"), 0.402),   
                     //new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA3A7AA"), 0.41),
                    //new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA3A7AA"), 0.448),
                    //new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF979CA0"), 0.456),   
                   new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF6A6A6A"), 0.614d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF777777"), 0.633d),
                    //new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF74787B"), 0.938),
                   new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFB6B6B6"), 1d)
                    });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FF9A9A9A"),Offset= 0.038},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FF6A6A6A"),Offset= 0.614d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF777777"),Offset= 0.633d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFB6B6B6"),Offset= 1d}
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
               // brush.Opacity = 0.415;
               // brush.Opacity = 0.5;
               // return GridUtil.GetXamlConvertedValue<Brush>("#FFD1D1D1");
               // return this.GridTreeCurrentCellSelectionBackground;
                return brush;

            }
        }

        public Brush GridTreeNodeHighlightBrush
        {
            get
            {
                return this.GridTreeCellBackgroundBrush;
            }
        }

#if !SILVERLIGHT
        public Geometry GridTreeExpanderPlusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 288.334,183.208L 288.334,174.125L 294.958,178.542L 288.334,183.208 Z ");
            }
        }

        public Geometry GridTreeExpanderMinusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 274.487,165.737L 282.487,165.737L 282.487,157.681L 274.487,165.737 Z ");

            }
        }
#else
        public Path GridTreeExpanderPlusPath
        {
            get
            {
                string data = "M-91.702,-146.448L68.0101,-31.5536L-91.702,99.2988z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;

            }
        }

        public Path GridTreeExpanderMinusPath
        {
            get
            {
                string data = "M105.264,-136.319L106.268,65.2559L-91.702,65.2559z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;
            }
        }
#endif
        


        public Brush GridTreeExpanderBackground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF5F1514"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF5F1514"));

#endif
            }
        }

        public Brush GridTreeExpanderBorderBrush
        {
            get { return this.GridTreeExpanderBackground; }
        }

        public Brush GridTreeExpanderForeground
        {
             get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            } 
        }

        public Brush GridTreeExpanderExpandedBackground
        {
            get { return this.GridTreeExpanderBackground; }
        }

        public Brush GridTreeExpanderExpandedBorderBrush
        {
            get { return this.GridTreeExpanderBorderBrush; }
        }

        public Brush GridTreeExpanderExpandedForeground
        {
            get { return this.GridTreeExpanderForeground; }
        }

        public Brush GridTreeExpanderHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF9BCEF6"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF4092DE"), 0.366), 
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF1175D2"), 0.406),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF0168C6"), 0.503), 
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF08477D"), 0.987), 
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF203F5A"), 1), 
                      });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FF9BCEF6"),Offset= 0d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF4092DE"),Offset= 0.366}, 
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF1175D2"),Offset= 0.406},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF0168C6"),Offset= 0.503}, 
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF08477D"),Offset= 0.987}, 
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF203F5A"),Offset= 1}, 
                      },0.0d);
#endif
              
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeExpanderHoverBorderBrush
        {
            get { return this.GridTreeExpanderHoverBackground; }
        }

        public Brush GridTreeExpanderHoverForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public CellBordersInfo GridTreeCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    
#if !SILVERLIGHT
                    Top = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF353332")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF353332")), 0.25d),
                    Left = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF353332")), 0.25d),
                    Right = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF353332")), 0.25d)
#else
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")), 0.25d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")), 0.25d)
#endif
                };
            }
        }

        public GridFontInfo GridTreeCellFont
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

        public CellMarginsInfo GridTreeCellTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d
                };
            }
        }

        public Brush GridTreeCellBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush GridTreeCellForegroundBrush
        {
            get
            {
                return InBuiltBlackBrush;
            }
        }

        public double GridTreeCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush GridTreeCellBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF6FB2C2"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF6FB2C2"));

#endif
            }
        }
    }

    public class GridTreeShinyRedVisualStyle : IGridTreeVisualStyle
    {

        public Brush GridTreeRowHoverForegroundBrush
        {
            get
            {
                return Brushes.White;
                //return this.GridTreeCellForegroundBrush; 
            }
        }
        public Brush InBuiltBlackBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Black);
            }
        }


        public Brush GridTreeBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return GridUtil.GetXamlConvertedValue<Brush>("#FF3A3A3A"); 
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A"));
#endif

            }
        }

        public Thickness GridTreeBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush GridTreeHeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE6A6A7"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFB83731"), 0.5),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA3221C"), 0.5),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF6D1919"), 1d),
                    });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFE6A6A7"),Offset= 0d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFB83731"),Offset= 0.5},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFA3221C"),Offset= 0.5},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF6D1919"),Offset= 1d},
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHeaderForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush GridTreeHeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE3A0A0"), 0.003),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFB73832"), 0.496),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA2221C"), 0.501),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFE9DB"), 1),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFBD5C54"), 0.769d),
                     });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFE3A0A0"),Offset= 0.003},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFB73832"),Offset= 0.496},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFA2221C"),Offset= 0.501},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFE9DB"),Offset= 1},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFBD5C54"),Offset= 0.769d},
                     },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHeaderHoverForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        public GridFontInfo GridTreeHeaderFont
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

        public CellMarginsInfo GridTreeHeaderTextMargins
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

        public Brush GridTreeHeaderInnerBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF24201F"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF24201F"));

#endif
            }
        }

        public Thickness GridTreeHeaderInnerBorderThickness
        {
            get
            {
#if SILVERLIGHT
                return new Thickness(0.2d);
#else
                 return new Thickness(0.5d);
#endif
            }
        }

        public Brush GridTreeSortWidgetBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF3A3A3A"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3A3A3A"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                      
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE3A0A0"), 0.03d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFB73832"), 0.496d),         
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA2221C"), 0.501d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFE9DB"), 1d),   
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFBD5C54"), 0.769d), 
                    });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFE3A0A0"),Offset= 0.03d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFB73832"),Offset= 0.496d},         
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFA2221C"),Offset= 0.501d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFFFE9DB"),Offset= 1d},   
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFBD5C54"),Offset= 0.769d}, 
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFB5B5B7"), 0.021),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF767877"), 0.37),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF6B6B6B"), 0.377),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF9C979E"), 0.953)
                     });
#else
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FFB5B5B7"),Offset= 0.021},
                    new GradientStop(){ Color=ColorExtensions.StringToColor("#FF767877"),Offset= 0.37},
                    new GradientStop(){ Color=ColorExtensions.StringToColor("#FF6B6B6B"),Offset= 0.377},
                    new GradientStop(){ Color=ColorExtensions.StringToColor("#FF9C979E"),Offset= 0.953}
                     },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHighlightSelectionForeground
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush GridTreeCurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                        
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEAECEB"), 0.023),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFBCBDBF"), 0.402),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA3A7AA"), 0.41),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA3A7AA"), 0.448),  
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF979CA0"), 0.456),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF74787B"), 0.938),
                    });
#else
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FFEAECEB"),Offset= 0.023},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FFBCBDBF"),Offset= 0.402},
                    new GradientStop(){ Color=ColorExtensions.StringToColor("#FFA3A7AA"),Offset= 0.41},
                    new GradientStop(){ Color=ColorExtensions.StringToColor("#FFA3A7AA"),Offset= 0.448},  
                    new GradientStop(){ Color=ColorExtensions.StringToColor("#FF979CA0"),Offset= 0.456},
                    new GradientStop(){ Color=ColorExtensions.StringToColor("#FF74787B"),Offset= 0.938},
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeCurrentCellSelectionForeground
        {
            get { return Brushes.White; }
        }

        public Brush GridTreeCurrentCellBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF353332"));
#else
                return Brushes.Transparent;
#endif
            }
        }

        public double GridTreeCurrentCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush GridTreeRowHeaderBackgroundBrush
        {
            get
            {
    //             <LinearGradientBrush EndPoint="0.5,1" StartPoint="0.5,0">
    // <GradientStop Color="#FFDD9190" Offset="0"/>
    // <GradientStop Color="#FFA3241E" Offset="0.47"/>
    // <GradientStop Color="#FF7E2020" Offset="1"/>
    //</LinearGradientBrush>
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFDD9190"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA3241E"), 0.47d),
                   
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF7E2020"), 1d)
                     });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFDD9190"),Offset= 0d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFA3241E"),Offset= 0.47d},
                    
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF7E2020"),Offset= 1d}
                     },0.0d);
#endif

                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                //brush.Opacity = 0.415;
                return brush;
            }

            //return this.GridTreeHeaderBackgroundBrush;

        }

        public Brush GridTreeRowHeaderForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        public Brush GridTreeRowHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF9A9A9A"), 0.038d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF6A6A6A"), 0.614d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF777777"), 0.633d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFB6B6B6"), 1d)
                     });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FF9A9A9A"),Offset= 0.038d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF6A6A6A"),Offset= 0.614d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF777777"),Offset= 0.633d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFB6B6B6"),Offset= 1d}
                     },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                //brush.Opacity = 0.415;
                return brush;
            }
        }

        public Brush GridTreeNodeHighlightBrush
        {
            get
            {
                return this.GridTreeCellBackgroundBrush;
            }
        }

#if !SILVERLIGHT
        public Geometry GridTreeExpanderPlusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 288.334,183.208L 288.334,174.125L 294.958,178.542L 288.334,183.208 Z ");
            }
        }

        public Geometry GridTreeExpanderMinusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 274.487,165.737L 282.487,165.737L 282.487,157.681L 274.487,165.737 Z ");

            }
        }
#else
        public Path GridTreeExpanderPlusPath
        {
            get
            {
                string data = "M-91.702,-146.448L68.0101,-31.5536L-91.702,99.2988z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;

            }
        }

        public Path GridTreeExpanderMinusPath
        {
            get
            {
                string data = "M105.264,-136.319L106.268,65.2559L-91.702,65.2559z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;
            }
        }
#endif
       

        public Brush GridTreeExpanderBackground
        {
            get
            {
#if !SILVERLIGHT
                return GridUtil.GetXamlConvertedValue<Brush>("#FF5F1514");
#else
                return new SolidColorBrush( ColorExtensions.StringToColor ("#FF5F1514"));
#endif

            }
        }

        public Brush GridTreeExpanderBorderBrush
        {
            get
            {
                return this.GridTreeExpanderBackground;
            }
        }

        public Brush GridTreeExpanderForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFDFA"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFDFA"));

#endif
            }
        }

        public Brush GridTreeExpanderExpandedBackground
        {
            get { return this.GridTreeExpanderBackground; }
        }

        public Brush GridTreeExpanderExpandedBorderBrush
        {
            get { return this.GridTreeExpanderBorderBrush; }
        }

        public Brush GridTreeExpanderExpandedForeground
        {
            get { return this.GridTreeExpanderForeground; }
        }

        public Brush GridTreeExpanderHoverBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFDD9493"), 0.018d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFB83C36"), 0.469), 
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA2241E"), 0.513),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFBC5B53"), 0.783), 
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF4D4C7"), 1),
                    });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFDD9493"),Offset= 0.018d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFB83C36"),Offset= 0.469}, 
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFA2241E"),Offset= 0.513},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFBC5B53"),Offset= 0.783}, 
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFF4D4C7"),Offset= 1},
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeExpanderHoverBorderBrush
        {
            get { return this.GridTreeExpanderHoverBackground; }
        }

        public Brush GridTreeExpanderHoverForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public CellBordersInfo GridTreeCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                   
#if !SILVERLIGHT
                     Top = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF353332")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF353332")), 0.25d),
                    Left = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF353332")), 0.25d),
                    Right = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF353332")), 0.25d)
#else
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")), 0.25d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")), 0.25d)
#endif
                };
            }
        }

        public GridFontInfo GridTreeCellFont
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

        public CellMarginsInfo GridTreeCellTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d
                };
            }
        }

        public Brush GridTreeCellBackgroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Brush GridTreeCellForegroundBrush
        {
            get
            {
                return InBuiltBlackBrush;
            }
        }

        public double GridTreeCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush GridTreeCellBorderBrush
        {
            get
            {
                return Brushes.Transparent;
            }
        }
    }

     public class GridTreeSyncfusionVisualStyle : IGridTreeVisualStyle
     {
         public Brush GridTreeRowHoverForegroundBrush
         {
             get { return this.GridTreeCellForegroundBrush; }
         }
         public Brush InBuiltBlackBrush
         {
             get
             {
                 return new SolidColorBrush(Colors.Black);
             }
         }



         public Brush GridTreeBorderBrush
         {

             get 
             {
#if !SILVERLIGHT
                 return GridUtil.GetXamlConvertedValue<Brush>("#FFC1DFF6"); 
#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FFC1DFF6")); 
#endif

             }
         }

         public Thickness GridTreeBorderThickness
         {
             get { return new Thickness(1); }
         }

         public Brush GridTreeHeaderBackgroundBrush
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                       
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF2066AB"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF144591"), 1d),
                     });
#else
                     new GradientStop(){ Color= ColorExtensions.StringToColor("#FF2066AB"),Offset= 0d},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FF144591"),Offset= 1d},
                     },0.0d);
#endif
               
                 brush.StartPoint = new Point(0.5, 0);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush;
             }
         }

         public Brush GridTreeHeaderForegroundBrush
         {
             get
             {
                 return Brushes.White;
             }
         }

         public Brush GridTreeHeaderHoverBackgroundBrush
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF1F65A9"), 0.009d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF144691"), 0.991d),
                    });
#else
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FF1F65A9"),Offset= 0.009d},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FF144691"),Offset= 0.991d}, 
                    },0.0d);
#endif
                
                 brush.StartPoint = new Point(0.5, 0);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush;
             }
         }

         public Brush GridTreeHeaderHoverForegroundBrush
         {
             get
             {
                 return this.GridTreeHeaderForegroundBrush;
             }
         }

         public GridFontInfo GridTreeHeaderFont
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

         public CellMarginsInfo GridTreeHeaderTextMargins
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

         public Brush GridTreeHeaderInnerBorderBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
             }
         }

         public Thickness GridTreeHeaderInnerBorderThickness
         {
             get
             {
#if SILVERLIGHT
                 return new Thickness(0.2d);
#else
                 return new Thickness(0.5d);
#endif
             }
         }

         public Brush GridTreeSortWidgetBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
             }
         }

         public Brush GridTreeSortWidgetBorderBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF628CB6"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF628CB6"));

#endif
             }
         }

         public Brush GridTreeSortWidgetBorderHoverBackgroundBrush
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                      
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF1F65A9"),0.009d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF144691"),0.991d), 
                    });
#else
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FF1F65A9"), Offset= 0.009d},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FF144691"), Offset= 0.991d}, 
                    },0.0d);
#endif
                
                 brush.StartPoint = new Point(0.5, 0);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush;
             }
         }

         public Brush GridTreeHighlightSelectionBackground
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF3870D"), 0.018d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFD05311"), 0.996d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFF865D"), 1d)
                    });
#else
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FFF3870D"),Offset= 0.018d},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FFD05311"),Offset= 0.996d},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FFFF865D"),Offset= 1d}
                    },0.0d);
#endif
                
                 brush.StartPoint = new Point(0.5, 0);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush;
             }
         }

         public Brush GridTreeHighlightSelectionForeground
         {
             get
             {
                 return Brushes.White;
             }
         }

         public Brush GridTreeCurrentCellSelectionBackground
         {
#if !SILVERLIGHT
             get { return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF1F62A6")); }

#else
             get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF1F62A6")); }

#endif
         }

         public Brush GridTreeCurrentCellSelectionForeground
         {
#if !SILVERLIGHT
             get { return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF")); }

#else
             get { return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF")); }

#endif
         }

         public Brush GridTreeCurrentCellBorderBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFF3870D"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FFF3870D"));

#endif
             }
         }

         public double GridTreeCurrentCellBorderWidth
         {
             get
             {
                 return 0.5d;
             }
         }

         public Brush GridTreeRowHeaderBackgroundBrush
         {
             get
             {
                 return this.GridTreeHeaderBackgroundBrush;
             }
         }

         public Brush GridTreeRowHeaderForegroundBrush
         {
             get
             {
                 return this.GridTreeHeaderForegroundBrush;
             }
         }

         public Brush GridTreeRowHoverBackgroundBrush
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEFF7FD"), 0.056d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFDEEEF9"), 0.944d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"), 1d)
                     });
#else
                     new GradientStop(){ Color= ColorExtensions.StringToColor("#FFEFF7FD"),Offset= 0.056d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFDEEEF9"),Offset= 0.944d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFFFFF"),Offset= 1d}
                     },0.0d);
#endif
               
                 brush.StartPoint = new Point(0.5, 0);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush;
             }
         }

         public Brush GridTreeNodeHighlightBrush
         {
             get
             {
                 return this.GridTreeCellBackgroundBrush;
             }
         }
#if !SILVERLIGHT
         public Geometry GridTreeExpanderPlusPath
         {
             get
             {
                 return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 288.334,183.208L 288.334,174.125L 294.958,178.542L 288.334,183.208 Z ");
             }
         }

         public Geometry GridTreeExpanderMinusPath
         {
             get
             {
                 return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 274.487,165.737L 282.487,165.737L 282.487,157.681L 274.487,165.737 Z ");

             }
         }
#else
         public Path GridTreeExpanderPlusPath
         {
             get
             {
                 string data = "M-91.702,-146.448L68.0101,-31.5536L-91.702,99.2988z";
                 string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                 return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;

             }
         }

         public Path GridTreeExpanderMinusPath
         {
             get
             {
                 string data = "M105.264,-136.319L106.268,65.2559L-91.702,65.2559z";
                 string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                 return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;
             }
         }
#endif

       

         public Brush GridTreeExpanderBackground
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF5B5B5B"));
                 //return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF2C367E"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF5B5B5B"));
                 //return new SolidColorBrush(ColorExtensions.StringToColor("#FF2C367E"));

#endif
             }
         }

         public Brush GridTreeExpanderBorderBrush
         {
             get
             {
                 return Brushes.Black;
               // return this.GridTreeHighlightSelectionBackground;
//#if !SILVERLIGHT
//                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF305CB7"));

//#else
//                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF305CB7"));

//#endif
             }
         }

         public Brush GridTreeExpanderForeground
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF000000"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));

#endif
             }
         }

         public Brush GridTreeExpanderExpandedBackground
         {
             get { return this.GridTreeExpanderBackground; }
         }

         public Brush GridTreeExpanderExpandedBorderBrush
         {
             get { return this.GridTreeExpanderBorderBrush; }
         }

         public Brush GridTreeExpanderExpandedForeground
         {
             get { return this.GridTreeExpanderForeground; }
         }

         public Brush GridTreeExpanderHoverBackground
         {
             get
             {
                 //return this.GridTreeExpanderBackground;
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                   
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF2860D"),0.08d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFD9600F"),0.97d), 
                     });
#else
                     new GradientStop(){ Color= ColorExtensions.StringToColor("#FFF2860D"),Offset= 0.08d},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FFD9600F"),Offset= 0.97d}, 
                     },0.0d);
#endif

                 brush.StartPoint = new Point(0.5, 0);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush;
             }
         }

         public Brush GridTreeExpanderHoverBorderBrush
         {
             get { return this.GridTreeExpanderBorderBrush; }
         }

         public Brush GridTreeExpanderHoverForeground
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF2860D"),0.08d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFD9600F"),0.97d),
                      });
#else
                     new GradientStop(){ Color= ColorExtensions.StringToColor("#FFF2860D"), Offset= 0.08d},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FFD9600F"), Offset= 0.97d},
                      },0.0d);
#endif
              
                 brush.StartPoint = new Point(0.5, 0);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush;
             }
         }

         public CellBordersInfo GridTreeCellBorders
         {
             get
             {
                 return new CellBordersInfo()
                 {
                    
#if !SILVERLIGHT
 Top = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFC1DFF6")), 0.25d),
                     Bottom = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFC1DFF6")), 0.25d), // newly added left and right
                     Left = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFC1DFF6")), 0.25d),
                     Right = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFC1DFF6")), 0.25d)
#else
                     Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC1DFF6")), 0.25d),
                     Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC1DFF6")), 0.25d), // newly added left and right
                     Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC1DFF6")), 0.25d),
                     Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFC1DFF6")), 0.25d)
#endif
                 };
             }
         }

         public GridFontInfo GridTreeCellFont
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

         public CellMarginsInfo GridTreeCellTextMargins
         {
             get
             {
                 return new CellMarginsInfo()
                 {
                     Left = 4d
                 };
             }
         }

         public Brush GridTreeCellBackgroundBrush
         {
             get
             {
                 return Brushes.White;
             }
         }

         public Brush GridTreeCellForegroundBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF2C367E"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF2C367E"));

#endif
             }
         }

         public double GridTreeCellBorderWidth
         {
             get
             {
                 return 0.5d;
             }
         }

         public Brush GridTreeCellBorderBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFF3870D"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FFF3870D"));

#endif
             }
         }
     }

     public class GridTreeTwilightBlueVisualStyle : IGridTreeVisualStyle
     {

         public Brush GridTreeRowHoverForegroundBrush
         {
             get { return this.GridTreeCellForegroundBrush; }
         }
         public Brush InBuiltBlackBrush
         {
             get
             {
                 return new SolidColorBrush(Colors.Black);
             }
         }

         public Brush GridTreeBorderBrush
         {
             get
             {
#if !SILVERLIGHT
                 return GridUtil.GetXamlConvertedValue<Brush>("#FFAFE4F7"); 
#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FFAFE4F7")); 
#endif

             }
         }

         public Thickness GridTreeBorderThickness
         {
             get { return new Thickness(1); }
         }

         public Brush GridTreeHeaderBackgroundBrush
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA2E2F9"), 0),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF00AFF0"), 0.999),
                     });
#else
                     new GradientStop(){ Color= ColorExtensions.StringToColor("#FFA2E2F9"),Offset= 0},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FF00AFF0"),Offset= 0.999},
                     },0.0d);
#endif
               
                 brush.StartPoint = new Point(0.5, 0);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush;
             }
         }

         public Brush GridTreeHeaderForegroundBrush
         {
             get
             {
                 return Brushes.White;
             }
         }

         public Brush GridTreeHeaderHoverBackgroundBrush
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFC6F0FF"), 0),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF07B0EF"), 0.999), 
                    });
#else
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FFC6F0FF"),Offset= 0},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FF07B0EF"),Offset= 0.999}, 
                    },0.0d);
#endif
                
                 brush.StartPoint = new Point(0.5, 0);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush;
             }
         }

         public Brush GridTreeHeaderHoverForegroundBrush
         {
             get
             {
                 return this.GridTreeHeaderForegroundBrush;
             }
         }

         public GridFontInfo GridTreeHeaderFont
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

         public CellMarginsInfo GridTreeHeaderTextMargins
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

         public Brush GridTreeHeaderInnerBorderBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFE4EBF0"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FFE4EBF0"));

#endif
             }
         }

         public Thickness GridTreeHeaderInnerBorderThickness
         {
             get
             {
#if SILVERLIGHT
                 return new Thickness(0.2d);
#else
                 return new Thickness(0.5d);
#endif
             }
         }

         public Brush GridTreeSortWidgetBrush
         {
             get
             {
                 return Brushes.White;
             }
         }

         public Brush GridTreeSortWidgetBorderBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFAFE4F7"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FFAFE4F7"));

#endif
             }
         }

         public Brush GridTreeSortWidgetBorderHoverBackgroundBrush
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF9CE0F8"), 0.992d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF05B0E2"), 0d),
                    });
#else
                     new GradientStop(){ Color= ColorExtensions.StringToColor("#FF9CE0F8"),Offset= 0.992d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF05B0E2"),Offset= 0d},
                 },0.0d);
#endif
                
                 brush.StartPoint = new Point(0.5, 0);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush;
             }
         }

         public Brush GridTreeHighlightSelectionBackground
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF44AEEC"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF44AEEC"));

#endif
             }
         }

         public Brush GridTreeHighlightSelectionForeground
         {
             get
             {
                 return Brushes.White;
             }
         }

         public Brush GridTreeCurrentCellSelectionBackground
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFBBD878"), 0.01),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF8ACB22"), 0.874), 
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF92CA11"), 0.906),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF8DC61E"), 0.979),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF83CA0E"), 1)
                     });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFBBD878"),Offset= 0.01},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF8ACB22"),Offset= 0.874}, 
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FF92CA11"),Offset= 0.906},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF8DC61E"),Offset= 0.979},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FF83CA0E"),Offset= 1}
                     },0.0d);
#endif
               
                 brush.StartPoint = new Point(0.5, 0);
                 brush.EndPoint = new Point(0.5, 1);
                 return brush;
             }
         }

         public Brush GridTreeCurrentCellSelectionForeground
         {
             get { return Brushes.White; }
         }

         public Brush GridTreeCurrentCellBorderBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFE9C256"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FFE9C256"));

#endif
             }
         }

         public double GridTreeCurrentCellBorderWidth
         {
             get
             {
                 return 0.5d;
             }
         }

         public Brush GridTreeRowHeaderBackgroundBrush
         {
             get
             {
                 return this.GridTreeHeaderBackgroundBrush;
             }
         }

         public Brush GridTreeRowHeaderForegroundBrush
         {
             get
             {
                 return this.GridTreeHeaderForegroundBrush;
             }
         }

         public Brush GridTreeRowHoverBackgroundBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF7FD7F7"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF7FD7F7"));

#endif
             }
         }

         public Brush GridTreeNodeHighlightBrush
         {
             get
             {
                 return this.GridTreeCellBackgroundBrush;
             }
         }
#if !SILVERLIGHT
         public Geometry GridTreeExpanderPlusPath
         {
             get
             {
                 return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 288.334,183.208L 288.334,174.125L 294.958,178.542L 288.334,183.208 Z ");
             }
         }

         public Geometry GridTreeExpanderMinusPath
         {
             get
             {
                 return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 274.487,165.737L 282.487,165.737L 282.487,157.681L 274.487,165.737 Z ");

             }
         }
#else
         public Path GridTreeExpanderPlusPath
         {
             get
             {
                 string data = "M-91.702,-146.448L68.0101,-31.5536L-91.702,99.2988z";
                 string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                 return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;

             }
         }

         public Path GridTreeExpanderMinusPath
         {
             get
             {
                 string data = "M105.264,-136.319L106.268,65.2559L-91.702,65.2559z";
                 string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                 return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;
             }
         }
#endif

        

         public Brush GridTreeExpanderBackground
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF5B5B5B"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF5B5B5B"));

#endif
             }
         }

         public Brush GridTreeExpanderBorderBrush
         {
             get { return this.GridTreeExpanderBackground; }
         }

         public Brush GridTreeExpanderForeground
         {
             get { return this.GridTreeExpanderBackground; }
         }

         public Brush GridTreeExpanderExpandedBackground
         {
             get { return this.GridTreeExpanderBackground; }
         }

         public Brush GridTreeExpanderExpandedBorderBrush
         {
             get { return this.GridTreeExpanderBorderBrush; }
         }

         public Brush GridTreeExpanderExpandedForeground
         {
             get { return this.GridTreeExpanderForeground; }
         }

         public Brush GridTreeExpanderHoverBackground
         {
             get
             {
                 LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                      
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA2E2F9"), 0),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF00AFF0"), 0.998d),
                     });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFA2E2F9"),Offset= 0},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF00AFF0"),Offset= 0.998d},
                     },0.0d);
#endif
               
                 brush.StartPoint = new Point(0.832, 0.472);
                 brush.EndPoint = new Point(0.168, 0.528);
                 return brush;
             }
         }

         public Brush GridTreeExpanderHoverBorderBrush
         {
             get { return this.GridTreeExpanderBackground; }
         }

         public Brush GridTreeExpanderHoverForeground
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
             }
         }

         public CellBordersInfo GridTreeCellBorders
         {
             get
             {
                 return new CellBordersInfo()
                 {
                     
#if !SILVERLIGHT
                     Top = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFAFE4F7")), 0.25d),
                     Bottom = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFAFE4F7")), 0.25d),
                     Left = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFAFE4F7")), 0.25d),
                     Right = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFAFE4F7")), 0.25d)
#else
                     Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFAFE4F7")), 0.25d),
                     Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFAFE4F7")), 0.25d),
                     Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFAFE4F7")), 0.25d),
                     Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFAFE4F7")), 0.25d)
#endif
                 };
             }
         }

         public GridFontInfo GridTreeCellFont
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

         public CellMarginsInfo GridTreeCellTextMargins
         {
             get
             {
                 return new CellMarginsInfo()
                 {
                     Left = 4d
                 };
             }
         }

         public Brush GridTreeCellBackgroundBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFEFEFE"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FFFEFEFE"));

#endif
             }
         }

         public Brush GridTreeCellForegroundBrush
         {
             get
             {
#if !SILVERLIGHT
                 return GridUtil.GetXamlConvertedValue<Brush>("#FF49535C");
#else
                 return new SolidColorBrush(ColorExtensions.StringToColor ("#FF49535C"));
#endif
             }
         }

         public double GridTreeCellBorderWidth
         {
             get
             {
                 return 0.2d;
             }
         }

         public Brush GridTreeCellBorderBrush
         {
             get
             {
#if !SILVERLIGHT
                 return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF6FB2C2"));

#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FF6FB2C2"));

#endif
             }
         }
     }

    public class GridTreeVS2010VisualStyle : IGridTreeVisualStyle
    {

        public Brush GridTreeRowHoverForegroundBrush
        {
            get { return this.GridTreeCellForegroundBrush; }
        }
        public Brush InBuiltBlackBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        public Brush GridTreeBorderBrush
        {
            get 
            {
#if !SILVERLIGHT
                return GridUtil.GetXamlConvertedValue<Brush>("#FFC1DFF6"); 
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFC1DFF6")); 
#endif

            }

        }

        public Thickness GridTreeBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush GridTreeHeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                      
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF4A5D80"), 0.225d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF3E5378"), 0.885d),
                     });
#else
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FF4A5D80"),Offset= 0.225d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF3E5378"),Offset= 0.885d},
                     },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);

                return brush;
            }
        }

        public Brush GridTreeHeaderForegroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Brush GridTreeHeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                          
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFB3B8BF"),0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE5E5E1"),0.01d),                   
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFFBF2"),0.02d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFF4D0"),0.459d),  
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFE8A6"),0.551d),
                     });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFB3B8BF"), Offset= 0d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFE5E5E1"), Offset= 0.01d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFFBF2"), Offset= 0.02d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFF4D0"), Offset= 0.459d},  
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFE8A6"), Offset= 0.551d},
                     },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHeaderHoverForegroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF000000"));

#else
                return Brushes.Black;
                 //return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));

#endif
            }
        }

        public GridFontInfo GridTreeHeaderFont
        {
            get
            {
                return new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe UI Semibold"),
                    FontSize = 12d,
                    FontStretch = new FontStretch(),
                    FontStyle = new FontStyle(),
                    //FontWeight = FontWeights.Bold,
                    TextDecorations = null,
                    Orientation = 0
                };
            }
        }

        public CellMarginsInfo GridTreeHeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 12d

                };
            }
        }

        public Brush GridTreeHeaderInnerBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF4A5E80"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF4A5E80"));

#endif
            }
        }

        public Thickness GridTreeHeaderInnerBorderThickness
        {
            get
            {
#if SILVERLIGHT
                return new Thickness(0.25d);
#else
                 return new Thickness(0.25d);
#endif
            }
        }

        public Brush GridTreeSortWidgetBrush
        {
            get
            {
#if !SILVERLIGHT
                 return GridUtil.GetXamlConvertedValue<Brush>("#FFFFFFFF");
#else
                 return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));
#endif
               
            }
        }

        public Brush GridTreeSortWidgetBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFE4C365"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE4C365"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFB3B8BF"),0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE5E5E1"),0.01d),                   
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFFBF2"),0.02d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFF4D0"),0.459d),  
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFE8A6"),0.551d), 
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFB3B8BF"),Offset= 0d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFE5E5E1"),Offset=0.01d},                   
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFFBF2"),Offset=0.02d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFF4D0"),Offset=0.459d},  
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFE8A6"),Offset=0.551d}, 
#endif
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHighlightSelectionBackground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFE9ECEE"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE9ECEE"));

#endif
            }
        }

        public Brush GridTreeHighlightSelectionForeground
        {
            get
            {
                return InBuiltBlackBrush;
            }
        }

        public Brush GridTreeCurrentCellSelectionBackground
        {
            get { return Brushes.White; }
        }

        public Brush GridTreeCurrentCellSelectionForeground
        {
            get { return InBuiltBlackBrush; }
        }

        public Brush GridTreeCurrentCellBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFE9C256"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE9C256"));

#endif
            }
        }

        public double GridTreeCurrentCellBorderWidth
        {
            get
            {
                return 1d;
            }
        }

        public Brush GridTreeRowHeaderBackgroundBrush
        {
            get
            {
                return this.GridTreeHeaderBackgroundBrush;
            }
        }

        public Brush GridTreeRowHeaderForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        public Brush GridTreeRowHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFFBEF"), 0.064d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFF3CF"), 0.485d),      
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFECB5"), 0.491d),
                    });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFFBEF"),Offset= 0.064d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFF3CF"),Offset= 0.485d},      
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFECB5"),Offset= 0.491d},
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeNodeHighlightBrush
        {
            get
            {
                return this.GridTreeCellBackgroundBrush;
            }
        }

#if !SILVERLIGHT
        public Geometry GridTreeExpanderPlusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 288.334,183.208L 288.334,174.125L 294.958,178.542L 288.334,183.208 Z ");
            }
        }

        public Geometry GridTreeExpanderMinusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 274.487,165.737L 282.487,165.737L 282.487,157.681L 274.487,165.737 Z ");

            }
        }
#else
        public Path GridTreeExpanderPlusPath
        {
            get
            {
                string data = "M-91.702,-146.448L68.0101,-31.5536L-91.702,99.2988z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;

            }
        }

        public Path GridTreeExpanderMinusPath
        {
            get
            {
                string data = "M105.264,-136.319L106.268,65.2559L-91.702,65.2559z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;
            }
        }
#endif
       

        public Brush GridTreeExpanderBackground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF1A283D"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1A283D"));

#endif
            }
        }

        public Brush GridTreeExpanderBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF141F17"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF141F17"));

#endif
            }
        }

        public Brush GridTreeExpanderForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF000000"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF000000"));

#endif
            }
        }

        public Brush GridTreeExpanderExpandedBackground
        {
            get
            {
                return this.GridTreeExpanderBackground;
            }
        }

        public Brush GridTreeExpanderExpandedBorderBrush
        {
            get
            {
                return this.GridTreeExpanderBorderBrush;
            }
        }

        public Brush GridTreeExpanderExpandedForeground
        {
            get
            {
                return this.GridTreeExpanderForeground;
            }
        }

        public Brush GridTreeExpanderHoverBackground
        {
            get
            {

                //<LinearGradientBrush EndPoint="0.5,1" StartPoint="0.5,0">
                //    <GradientStop Color="#FFE4E8F0" Offset="0"/>
                //    <GradientStop Color="#FFA3A7AF" Offset="1"/>
                //</LinearGradientBrush>


                   LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     
#if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE4E8F0"),0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA3A7AF"),1d),           
                   
                    });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFE4E8F0"), Offset= 0d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFA3A7AF"),Offset=1d},           
                   
                    },0.0d);
#endif

                   brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }


//                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
//                {
                     
//                    #if !SILVERLIGHT
//                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFAEB3B8"),0d),
//                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF0EEE7"),0.01d),           
//                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFEFAEF"),0.042d),
//                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFF4D0"),0.422d),        
//                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFE8A6"),0.484d),
//                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFE8A6"),0.958d),        
//                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF293955"),0.99d),
//                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF47566E"),1d),
//                    });
//#else
//                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFAEB3B8"), Offset= 0d},
//                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFF0EEE7"),Offset=0.01d},           
//                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFEFAEF"),Offset=0.042d},
//                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFF4D0"),Offset=0.422d},        
//                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFE8A6"),Offset=0.484d},
//                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFE8A6"),Offset=0.958d},        
//                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF293955"),Offset=0.99d},
//                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF47566E"),Offset=1d},
//                    },0.0d);
//#endif
                
//                brush.StartPoint = new Point(0.5, 0);
//                brush.EndPoint = new Point(0.5, 1);
//                return brush;
//            }
        }

        public Brush GridTreeExpanderHoverBorderBrush
        {
            get
            {
                return GridTreeExpanderBackground;
            }
        }

        public Brush GridTreeExpanderHoverForeground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                      
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFAEB3B8"),0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF0EEE7"),0.01d),           
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFEFAEF"),0.042d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFF4D0"),0.422d),        
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFE8A6"),0.484d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFFE8A6"),0.958d),        
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF293955"),0.99d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF47566E"),1d),
                    });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFAEB3B8"), Offset= 0d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFF0EEE7"),Offset=0.01d},           
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFEFAEF"),Offset=0.042d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFF4D0"),Offset=0.422d},        
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFE8A6"),Offset=0.484d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFFFE8A6"),Offset=0.958d},        
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF293955"),Offset=0.99d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF47566E"),Offset=1d},
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public CellBordersInfo GridTreeCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                   
#if !SILVERLIGHT
                     Top = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF8591A2")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF8591A2")), 0.25d),
                    Left = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF8591A2")), 0.25d),
                    Right = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF8591A2")), 0.25d)
#else
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8591A2")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8591A2")), 0.25d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8591A2")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF8591A2")), 0.25d)
#endif
                };
            }
        }

        public GridFontInfo GridTreeCellFont
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

        public CellMarginsInfo GridTreeCellTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d
                };
            }
        }

        public Brush GridTreeCellBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush GridTreeCellForegroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return GridUtil.GetXamlConvertedValue<Brush>("#FF1B293E");
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1B293E"));
#endif
                
            }
        }

        public double GridTreeCellBorderWidth
        {
            get
            {
                return 1d;
            }
        }

        public Brush GridTreeCellBorderBrush
        {
            get
            {
                //return Brushes.Red;
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFBCC7D8"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFBCC7D8"));

#endif
            }
        }
    }

    public class GridTreeWindows7VisualStyle : IGridTreeVisualStyle
    {

        public Brush GridTreeRowHoverForegroundBrush
        {
            get { return this.GridTreeCellForegroundBrush; }
        }
        public Brush InBuiltBlackBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        public Brush GridTreeBorderBrush
        {
            get 
            {
#if !SILVERLIGHT
                return GridUtil.GetXamlConvertedValue<Brush>("#FFE2E2E2"); 
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFE2E2E2"));
#endif
                
            }
        }

        public Thickness GridTreeBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush GridTreeHeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF5FAFF"), 0.099),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE6F0FA"), 0.515), 
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFDCE6F4"), 0.52)
                    });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFF5FAFF"),Offset= 0.099},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFE6F0FA"),Offset= 0.515}, 
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFDCE6F4"),Offset= 0.52}
                },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHeaderForegroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF2A447F"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2A447F"));

#endif
            }
        }

        public Brush GridTreeHeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF5FAFF"), 0.099d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE6F0FA"), 0.515d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFDCE6F4"), 0.52d)
                    });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFF5FAFF"),Offset= 0.099d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFE6F0FA"),Offset= 0.515d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFDCE6F4"),Offset= 0.52d}
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.994, 0.101);
                brush.EndPoint = new Point(0.994, 0.922);
                return brush;
            }
        }

        public Brush GridTreeHeaderHoverForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        public GridFontInfo GridTreeHeaderFont
        {
            get
            {
                return GridFontInfo.Default;
            }
        }

        public CellMarginsInfo GridTreeHeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo() { Left = 3d, Right = 3d };
            }
        }

        public Brush GridTreeHeaderInnerBorderBrush
        {
            get { return Brushes.Transparent; }
        }

        public Thickness GridTreeHeaderInnerBorderThickness
        {
            get { return new Thickness(0.2d); }
        }

        public Brush GridTreeSortWidgetBrush
        {
            get
            {
#if !SILVERLIGHT
                return GridUtil.GetXamlConvertedValue<Brush>("#FF849DBD");
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD"));
#endif
                
            }
        }

        public Brush GridTreeSortWidgetBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF849DBD"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEEF4FA"),0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE1EBF9"),1d),
                     });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFEEF4FA"), Offset= 0d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFE1EBF9"), Offset= 1d}, 
                     },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEAF3FE"), 0.07d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFD0E5FE"), 0.9d),
                    });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFEAF3FE"),Offset= 0.07d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFD0E5FE"),Offset= 0.9d},
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHighlightSelectionForeground
        {
            get
            {
                return InBuiltBlackBrush;
            }
        }

        public Brush GridTreeCurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEAF3FE"), 0.07),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFD0E5FE"), 0.9),  
                    });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFEAF3FE"),Offset= 0.07},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFD0E5FE"),Offset= 0.9},  
                },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeCurrentCellSelectionForeground
        {
            get { return InBuiltBlackBrush; }
        }

        public Brush GridTreeCurrentCellBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return GridUtil.GetXamlConvertedValue<Brush>("#FF7DA2CE");
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF7DA2CE"));
#endif

            }
        }

        public double GridTreeCurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public Brush GridTreeRowHeaderBackgroundBrush
        {
            get
            {
                return this.GridTreeHeaderBackgroundBrush;
            }
        }

        public Brush GridTreeRowHeaderForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        public Brush GridTreeRowHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFAFBFD"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEDF4FD"), 0.798d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF0F7FE"), 1d),
                    });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFFAFBFD"),Offset= 0d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFEDF4FD"),Offset= 0.798d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFF0F7FE"),Offset= 1d}
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeNodeHighlightBrush
        {
            get
            {
                return this.GridTreeCellBackgroundBrush;
            }
        }
#if !SILVERLIGHT
        public Geometry GridTreeExpanderPlusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 288.334,183.208L 288.334,174.125L 294.958,178.542L 288.334,183.208 Z ");
            }
        }

        public Geometry GridTreeExpanderMinusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 274.487,165.737L 282.487,165.737L 282.487,157.681L 274.487,165.737 Z ");

            }
        }
#else
        public Path GridTreeExpanderPlusPath
        {
            get
            {
                string data = "M-91.702,-146.448L68.0101,-31.5536L-91.702,99.2988z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;

            }
        }

        public Path GridTreeExpanderMinusPath
        {
            get
            {
                string data = "M105.264,-136.319L106.268,65.2559L-91.702,65.2559z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;
            }
        }
#endif

       

        public Brush GridTreeExpanderBackground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Brush GridTreeExpanderBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFA8A8A8"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA8A8A8"));

#endif
            }
        }

        public Brush GridTreeExpanderForeground
        {

            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Brush GridTreeExpanderExpandedBackground
        {
            get
            {
                return Brushes.Black;
                //return this.GridTreeExpanderBackground;
            }
        }

        public Brush GridTreeExpanderExpandedBorderBrush
        {
            get { return GridTreeExpanderBorderBrush; }
        }

        public Brush GridTreeExpanderExpandedForeground
        {
            get { return GridTreeExpanderForeground; }
        }

        public Brush GridTreeExpanderHoverBackground
        {
            get
            {

                //<LinearGradientBrush EndPoint="0.5,1" StartPoint="0.5,0">
                //    <GradientStop Color="White" Offset="0"/>
                //    <GradientStop Color="#FFC2EAFA" Offset="1"/>
                //</LinearGradientBrush>

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                     
#if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("White"),0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFC2EAFA"),1d),           
                   
                    });
#else
                     new GradientStop(){Color= Colors.White, Offset= 0d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFC2EAFA"),Offset=1d},           
                   
                    },0.0d);
#endif

                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;


//#if !SILVERLIGHT
//                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF595959"));

//#else
//                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

//#endif
            }
        }

        public Brush GridTreeExpanderHoverBorderBrush
        {
            get 
            {
#if !SILVERLIGHT
                return GridUtil.GetXamlConvertedValue<Brush>("#FF70DAFA");
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF70DAFA"));
#endif
                //return this.GridTreeExpanderHoverBackground; 
            }
        }

        public Brush GridTreeExpanderHoverForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF595959"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

#endif
            }
        }

        public CellBordersInfo GridTreeCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    
#if !SILVERLIGHT
                    Top = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFE3E3E3")), 0.5d),
                    Bottom = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFE3E3E3")), 0.5d),
                    Right = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFE3E3E3")), 0.5d),
                    Left = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFE3E3E3")), 0.5d)
#else
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE3E3E3")), 0.5d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE3E3E3")), 0.5d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE3E3E3")), 0.5d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE3E3E3")), 0.5d)
#endif
                };
            }
        }

        public GridFontInfo GridTreeCellFont
        {
            get
            {
                return GridFontInfo.Default;
            }
        }

        public CellMarginsInfo GridTreeCellTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d
                };
            }
        }

        public Brush GridTreeCellBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush GridTreeCellForegroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return GridUtil.GetXamlConvertedValue<Brush>("#FF6C6D6F");
#else
                return new SolidColorBrush(ColorExtensions.StringToColor ("#FF6C6D6F"));
#endif
                
            }
        }

        public double GridTreeCellBorderWidth
        {
            get
            {
                return 0.2d;
            }
        }

        public Brush GridTreeCellBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return GridUtil.GetXamlConvertedValue<Brush>("#FFA1BFE3");
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA1BFE3"));

#endif
                
            }
        }
    }

    public class GridTreeDefaultGridVisualStyle : IGridTreeVisualStyle
    {
        public Brush GridTreeRowHoverForegroundBrush
        {
            get { return this.GridTreeCellForegroundBrush; }
        }
        public Brush InBuiltBlackBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        private Brush _gridBorderBrush = null;
        public Brush GridTreeBorderBrush
        {
            get
            {
                if (_gridBorderBrush == null)
                {
#if !SILVERLIGHT
                    _gridBorderBrush = GridUtil.GetXamlConvertedValue<Brush>("#FFE2E2E2");
#else
                    _gridBorderBrush = new SolidColorBrush(ColorExtensions.StringToColor("#FFE2E2E2"));
#endif

                }
                return _gridBorderBrush;
            }
        }

        public Thickness GridTreeBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush GridTreeHeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF5FAFF"), 0.099),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE6F0FA"), 0.515), 
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFDCE6F4"), 0.52)
                     });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFF5FAFF"),Offset= 0.099},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFE6F0FA"),Offset= 0.515}, 
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFDCE6F4"),Offset= 0.52}
                     },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHeaderForegroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF2A447F"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF2A447F"));

#endif
            }
        }

        public Brush GridTreeHeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF5FAFF"), 0.099d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE6F0FA"), 0.515d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFDCE6F4"), 0.52d)
                    });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFF5FAFF"),Offset= 0.099d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFE6F0FA"),Offset= 0.515d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFDCE6F4"),Offset= 0.52d}
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.994, 0.101);
                brush.EndPoint = new Point(0.994, 0.922);
                return brush;
            }
        }

        public Brush GridTreeHeaderHoverForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        public GridFontInfo GridTreeHeaderFont
        {
            get
            {
                return GridFontInfo.Default;
            }
        }

        public CellMarginsInfo GridTreeHeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo() { Left = 3d, Right = 3d, };
            }
        }

        public Brush GridTreeHeaderInnerBorderBrush
        {
            get { return Brushes.Transparent; }
        }

        public Thickness GridTreeHeaderInnerBorderThickness
        {
            get { return new Thickness(0.2d); }
        }

        public Brush GridTreeSortWidgetBrush
        {
            get
            {
#if !SILVERLIGHT
                return GridUtil.GetXamlConvertedValue<Brush>("#FF849DBD");
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD"));
#endif

            }
        }

        public Brush GridTreeSortWidgetBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF849DBD"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF849DBD"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEEF4FA"),0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFE1EBF9"),1d), 
                    });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFEEF4FA"),Offset= 0d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFE1EBF9"),Offset= 1d}, 
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEAF3FE"), 0.07d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFD0E5FE"), 0.9d),
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFEAF3FE"),Offset= 0.07d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFD0E5FE"),Offset= 0.9d},
#endif
                },0.0d);
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHighlightSelectionForeground
        {
            get
            {
                return InBuiltBlackBrush;
            }
        }

        public Brush GridTreeCurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEAF3FE"), 0.07),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFD0E5FE"), 0.9),
                     });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFEAF3FE"),Offset= 0.07},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFD0E5FE"), Offset= 0.9},  
                     },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeCurrentCellSelectionForeground
        {
            get { return InBuiltBlackBrush; }
        }

        public Brush GridTreeCurrentCellBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return GridUtil.GetXamlConvertedValue<Brush>("#FFA1BFE3");
#else
                return new SolidColorBrush(ColorExtensions.StringToColor ("#FFA1BFE3"));
#endif

            }
        }

        public double GridTreeCurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public Brush GridTreeRowHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFFAFBFD"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEDF4FD"), 0.798d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFF0F7FE"), 1d)
                     });
#else
 new GradientStop(){Color= ColorExtensions.StringToColor("#FFFAFBFD"),Offset= 0d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFEDF4FD"),Offset= 0.798d},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FFF0F7FE"),Offset= 1d}
                },0.0d);
#endif

                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
                //return Brushes.Red;
            }
        }

        public Brush GridTreeNodeHighlightBrush
        {
            get
            {
                return this.GridTreeCellBackgroundBrush;
            }
        }
#if !SILVERLIGHT
        public Geometry GridTreeExpanderPlusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 288.334,183.208L 288.334,174.125L 294.958,178.542L 288.334,183.208 Z ");
            }
        }

        public Geometry GridTreeExpanderMinusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 274.487,165.737L 282.487,165.737L 282.487,157.681L 274.487,165.737 Z ");

            }
        }
#else
        public Path GridTreeExpanderPlusPath
        {
            get
            {
                string data = "M-91.702,-146.448L68.0101,-31.5536L-91.702,99.2988z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;

            }
        }

        public Path GridTreeExpanderMinusPath
        {
            get
            {
                string data = "M105.264,-136.319L106.268,65.2559L-91.702,65.2559z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;
            }
        }
#endif

      

        private Brush _expanderBackground = null;
        public Brush GridTreeExpanderBackground
        {
            get
            {            


                if (_expanderBackground == null)
#if !SILVERLIGHT
                    _expanderBackground = new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF595959"));

#else
                    _expanderBackground = new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

#endif
                    return _expanderBackground;
            }
        }

        private Brush _expanderBorderBrush = null;
        public Brush GridTreeExpanderBorderBrush
        {
            get
            {
                if (_expanderBorderBrush == null)
#if !SILVERLIGHT
                    new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF25C6F7"));

#else
                    new SolidColorBrush(ColorExtensions.StringToColor("#FF25C6F7"));

#endif
                    return _expanderBorderBrush;
            }
        }

        private Brush _expanderForegroundBrush = null;
        public Brush GridTreeExpanderForeground
        {
            get
            {
                if (_expanderForegroundBrush == null)
#if !SILVERLIGHT
                    _expanderForegroundBrush = new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                    _expanderForegroundBrush = new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
                    return _expanderForegroundBrush;
            }
        }

        public Brush GridTreeExpanderExpandedBackground
        {
            get
            {
                return this.GridTreeExpanderBackground;
            }
        }

        public Brush GridTreeExpanderExpandedBorderBrush
        {
            get
            {
                return this.GridTreeExpanderBorderBrush;
            }
        }

        public Brush GridTreeExpanderExpandedForeground
        {
            get
            {
                return this.GridTreeExpanderForeground;
            }
        }

        public Brush GridTreeExpanderHoverBackground
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                        
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF75DBFB"), 0),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF81DFFB"), 0.591),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF82DFFB"), 1),
                     });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFA2C340"),Offset= 0.177},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF67943A"),Offset= 1},
                     },0.0d);
#endif


                brush.StartPoint = new Point(0.427, -0.294);
                brush.EndPoint = new Point(0.395, 1.865);
                return brush;
            }
            //#if !SILVERLIGHT
            //                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF595959"));

            //#else
            //                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

            //#endif

        }

        public Brush GridTreeExpanderHoverBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return GridUtil.GetXamlConvertedValue<Brush>("#FF25C6F7");

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF25C6F7"));
#endif
                // return GridTreeExpanderHoverBackground;
            }
        }

        public Brush GridTreeExpanderHoverForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF595959"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF595959"));

#endif
            }
        }

        private CellBordersInfo _gridTreeCellBorders = null;
        public CellBordersInfo GridTreeCellBorders
        {
            get
            {
                if (_gridTreeCellBorders == null)
                {
                    _gridTreeCellBorders = new CellBordersInfo()
                    {
                       
#if !SILVERLIGHT
                         Top = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFE3E3E3")), 0.25d),
                        Bottom = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFE3E3E3")), 0.25d),
                        Right = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFE3E3E3")), 0.25d),
                        Left = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFE3E3E3")), 0.25d)
#else
                        Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE3E3E3")), 0.25d),
                        Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE3E3E3")), 0.25d),
                        Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE3E3E3")), 0.25d),
                        Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFE3E3E3")), 0.25d)
#endif
                    };
                }
                return _gridTreeCellBorders;
            }
        }

        public GridFontInfo GridTreeCellFont
        {
            get
            {
                return GridFontInfo.Default;
            }
        }

        public CellMarginsInfo GridTreeCellTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d
                };
            }
        }

        public Brush GridTreeCellBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        private Brush _gridTreeCellForegroundBrush = null;
        public Brush GridTreeCellForegroundBrush
        {
            get
            {
                if (_gridTreeCellForegroundBrush == null)
#if !SILVERLIGHT
                    _gridTreeCellForegroundBrush = GridUtil.GetXamlConvertedValue<Brush>("#FF6C6D6F");
#else
                    _gridTreeCellForegroundBrush = new SolidColorBrush(ColorExtensions.StringToColor("#FF6C6D6F"));
#endif

                    return _gridTreeCellForegroundBrush;
            }
        }

        public double GridTreeCellBorderWidth
        {
            get
            {
                return 1d;
            }
        }

        public Brush GridTreeCellBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return GridUtil.GetXamlConvertedValue<Brush>("#FFA1BFE3");
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFA1BFE3"));
#endif

            }
        }


        public Brush GridTreeRowHeaderBackgroundBrush
        {
            get { return this.GridTreeHeaderBackgroundBrush; }
        }

        public Brush GridTreeRowHeaderForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }
    }

    public class GridTreeGlassyGreenVisualStyle : IGridTreeVisualStyle
    {
        public Brush GridTreeRowHoverForegroundBrush
        {
            get { return this.GridTreeCellForegroundBrush; }
        }
        public Brush GridTreeRowHeaderBackgroundBrush
        {
            get
            {
                return this.GridTreeHeaderBackgroundBrush;
            }
        }

        public Brush GridTreeRowHeaderForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        public Brush InBuiltBlackBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        public Brush GridTreeBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return GridUtil.GetXamlConvertedValue<Brush>("#FF629110"); 
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF629110"));
#endif

            }
        }

        public Thickness GridTreeBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush GridTreeHeaderBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                        
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA2C340"), 0.177),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF67943A"), 1),
                     });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFA2C340"),Offset= 0.177},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF67943A"),Offset= 1},
                     },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHeaderForegroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush GridTreeHeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                      
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA1C240"), 0.16d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF6B963A"), 0.991),
                     });
#else
                     new GradientStop(){Color= ColorExtensions.StringToColor("#FFA1C240"),Offset= 0.16d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF6B963A"),Offset= 0.991},
                     },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHeaderHoverForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        public GridFontInfo GridTreeHeaderFont
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

        public CellMarginsInfo GridTreeHeaderTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 17d,
                    Right = 10d
                };
            }
        }

        public Brush GridTreeHeaderInnerBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF586458"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF586458"));

#endif
            }
        }

        public Thickness GridTreeHeaderInnerBorderThickness
        {
            get
            {
#if SILVERLIGHT
                return new Thickness(0.2d);
#else
                 return new Thickness(0.5d);
#endif
            }
        }

        public Brush GridTreeSortWidgetBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF629110"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF629110"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFA1C240"), 0.16d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF6B963A"), 0.991d),
                     });
#else
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FFA1C240"),Offset= 0.16d},
                    new GradientStop(){Color= ColorExtensions.StringToColor("#FF6B963A"),Offset= 0.991d},
                     },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0.101);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHighlightSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF92B80D"), 0.965),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFB0CC50"), 0.035)
                     });
#else
                     new GradientStop(){ Color= ColorExtensions.StringToColor("#FF92B80D"),Offset= 0.965},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FFB0CC50"),Offset= 0.035}
                     },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHighlightSelectionForeground
        {
            get
            {
#if !SILVERLIGHT
                return GridUtil.GetXamlConvertedValue<Brush>("#FF376E35");
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF376E35"));
#endif

            }
        }

        public Brush GridTreeCurrentCellSelectionBackground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF8C8C8C"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF8C8C8C"));

#endif
            }
        }

        public Brush GridTreeCurrentCellSelectionForeground
        {
            get { return Brushes.White; }
        }

        public Brush GridTreeCurrentCellBorderBrush
        {
            get
            {
#if ! SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF92B80D"));
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF92B80D"));
#endif
            }
        }

        public double GridTreeCurrentCellBorderWidth
        {
            get
            {
                return 0.5d;
            }
        }

        public Brush GridTreeRowHoverBackgroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFEDECEC"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFEDECEC"));

#endif
            }
        }

        public Brush GridTreeNodeHighlightBrush
        {
            get
            {
                return this.GridTreeCellBackgroundBrush;
            }
        }

#if !SILVERLIGHT
        public Geometry GridTreeExpanderPlusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 288.334,183.208L 288.334,174.125L 294.958,178.542L 288.334,183.208 Z ");
            }
        }

        public Geometry GridTreeExpanderMinusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 274.487,165.737L 282.487,165.737L 282.487,157.681L 274.487,165.737 Z ");

            }
        }
#else
        public Path GridTreeExpanderPlusPath
        {
            get
            {
               // string data = "M-91.702,-146.448L68.0101,-31.5536L-91.702,99.2988z";
                string data = "F1 M 288.334,183.208L 288.334,174.125L 294.958,178.542L 288.334,183.208 Z ";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;

            }
        }

        public Path GridTreeExpanderMinusPath
        {
            get
            {
                string data = "F1 M 274.487,165.737L 282.487,165.737L 282.487,157.681L 274.487,165.737 Z ";
               // string data = "M105.264,-136.319L106.268,65.2559L-91.702,65.2559z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;
            }
        }
#endif
        

        public Brush GridTreeExpanderBackground
        {
            //<LinearGradientBrush EndPoint="0.5,1" StartPoint="0.5,0">
            //        <GradientStop Color="#FF80A715" Offset="0.025"/>
            //        <GradientStop Color="#FF40680C" Offset="1"/>
            //    </LinearGradientBrush>
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                       #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF80A715"), 0.025d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF40680C"), 1d),
                    });
#else
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FF80A715"),Offset= 0.025d},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FF40680C"),Offset= 1d},
                    },0.0d);
#endif

                brush.StartPoint = new Point(0, 0.5);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
                        
//                    #if !SILVERLIGHT
//                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("White"), 0.229d),
//                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFB0C8F0"), 0.974d),
//                    });
//#else
//                    new GradientStop(){ Color= Colors.White,Offset= 0.229d},
//                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FFB0C8F0"),Offset= 0.974d},
//                    },0.0d);
//#endif
                
//                brush.StartPoint = new Point(0.101, 0.088);
//                brush.EndPoint = new Point(0.792, 0.86);
//                return brush;
            }
        }

        public Brush GridTreeExpanderBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF141F17"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF141F17"));

#endif
            }
        }

        public Brush GridTreeExpanderForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF3F576F"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));

#endif
            }
        }

        public Brush GridTreeExpanderExpandedBackground
        {
            get
            {
                return this.GridTreeExpanderBackground;
            }
        }

        public Brush GridTreeExpanderExpandedBorderBrush
        {
            get
            {
                return this.GridTreeExpanderBorderBrush;
            }
        }

        public Brush GridTreeExpanderExpandedForeground
        {
            get
            {
                return this.GridTreeExpanderForeground;
            }
        }

        public Brush GridTreeExpanderHoverBackground
        {
            get
            {

#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFBCE93D"));
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFBCE93D"));
#endif
            }
        }

        public Brush GridTreeExpanderHoverBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF3F576F"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));

#endif
            }
        }

        public Brush GridTreeExpanderHoverForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF3F576F"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF3F576F"));

#endif
            }
        }

        public CellBordersInfo GridTreeCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    
#if !SILVERLIGHT
                    Top = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFACBF74")), 0.30d),
                    Bottom = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFACBF74")), 0.30d),
                    Left = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFACBF74")), 0.30d),
                    Right = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFACBF74")), 0.30d)
#else
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFACBF74")), 0.30d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFACBF74")), 0.30d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFACBF74")), 0.30d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FFACBF74")), 0.30d)
#endif
                };
            }
        }

        public GridFontInfo GridTreeCellFont
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

        public CellMarginsInfo GridTreeCellTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d
                };
            }
        }

        public Brush GridTreeCellBackgroundBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush GridTreeCellForegroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF376E35"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF376E35"));

#endif
            }
        }

        public double GridTreeCellBorderWidth
        {
            get
            {
                return 0d;
            }
        }

        public Brush GridTreeCellBorderBrush
        {
            get { throw new NotImplementedException(); }
        }
    }   
     
    public class GridTreeSunBlackVisualStyle : IGridTreeVisualStyle
    {
        public Brush GridTreeRowHoverForegroundBrush
        {
            get { return this.GridTreeCellForegroundBrush; }
        }
        public Brush InBuiltBlackBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        public Brush GridTreeBorderBrush
        {
#if !SILVERLIGHT
            get { return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF1C1918")); }

#else
            get { return new SolidColorBrush(ColorExtensions.StringToColor("#FF1C1918")); }

#endif
        }

        public Thickness GridTreeBorderThickness
        {
            get { return new Thickness(1); }
        }

        public Brush GridTreeHeaderBackgroundBrush
        {
            get
            {
                //<LinearGradientBrush EndPoint="0.5,1" StartPoint="0.5,0">
                //    <GradientStop Color="#FF635552" Offset="0.014"/>
                //    <GradientStop Color="#FF49382D" Offset="0.5"/>
                //    <GradientStop Color="#FF3A2115" Offset="0.5"/>
                //    <GradientStop Color="#FFAF4802" Offset="0.986"/>
                //    <GradientStop Color="#FF1C1918" Offset="1"/>
                //</LinearGradientBrush>
                //LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                //    {
                //     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF635552"), 0.02d),
                //    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF49382D"), 0.5d),
                //    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF3A2115"), 0.5d),
                //    //new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFAF4802"), 0.986d),
                //    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF1C1918"), 1d)

                //    });
                //brush.StartPoint = new Point(0, 0.5);
                //brush.EndPoint = new Point(0.5, 1);
                //<GradientStop Color="#FF655855" Offset="0.002"/>
                //    <GradientStop Color="#FF49372C" Offset="0.5"/>
                //    <GradientStop Color="#FF352016" Offset="0.5"/>
                //    <GradientStop Color="#FFB64A01" Offset="1"/>
                //return brush;
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                  
                    #if !SILVERLIGHT
                      new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF655855"), 0.02),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF49372C"), 0.5d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF352016"), 0.5d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFB64A01"), 1d)
                    });
#else
                      new GradientStop(){ Color= ColorExtensions.StringToColor("#FF655855"),Offset= 0.02},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FF49372C"),Offset= 0.5d},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FF352016"),Offset= 0.5d},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FFB64A01"),Offset= 1d}
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHeaderForegroundBrush
        {
            get
            {
#if !SILVERLIGHT
                return GridUtil.GetXamlConvertedValue<Brush>("#FFEBEBEB"); 
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFEBEBEB"));
#endif

            }
        }

        public Brush GridTreeHeaderHoverBackgroundBrush
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF61544F"), 0.014d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF4A382D"), 0.5),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF372217"), 0.5),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF4D2D17"), 0.999)
                    });
#else
                     new GradientStop(){ Color= ColorExtensions.StringToColor("#FF61544F"),Offset= 0.014d},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FF4A382D"),Offset= 0.5},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FF372217"),Offset= 0.5},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FF4D2D17"),Offset= 0.999}
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Thickness GridTreeHeaderInnerBorderThickness
        {
            get
            {
#if SILVERLIGHT
                return new Thickness(0.2d);
#else
                 return new Thickness(0.5d);
#endif
            }
        }

        public Brush GridTreeSortWidgetBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF1C1918"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF1C1918"));

#endif
            }
        }

        public Brush GridTreeSortWidgetBorderHoverBackgroundBrush
        {
            get
            {

                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                      
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF645753"), 0.03d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF49372C"), 0.5d),         
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF362117"), 0.5d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFC0672A"), 1d), 
                    });
#else
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FF645753"), Offset= 0.03d},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FF49372C"),Offset= 0.5d},         
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FF362117"),Offset= 0.5d},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FFC0672A"),Offset= 1d}, 
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHeaderInnerBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFD99466"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD99466"));

#endif
            }
        }

        public Brush GridTreeHeaderHoverForegroundBrush
        {
            get { return this.GridTreeHeaderForegroundBrush; }
        }

        public GridFontInfo GridTreeHeaderFont
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

        public CellMarginsInfo GridTreeHeaderTextMargins
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

        public Brush GridTreeSortWidgetBrush
        {
            get
            {
                return Brushes.White;
            }
        }

        public Brush GridTreeHighlightSelectionBackground
        {
            get
            {
                //return GridUtil.GetXamlConvertedValue<Brush>("#FFED8E2E");
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                       
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF5D5350"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFCC6431"), 1d),
                    });
#else
                    new GradientStop(){ Color= ColorExtensions.StringToColor ("#FF5D5350"),Offset= 0d},
                    new GradientStop(){ Color= ColorExtensions.StringToColor("#FFCC6431"),Offset= 1d}, 
                    },0.0d);
#endif

                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeHighlightSelectionForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF24201F"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF24201F"));

#endif
            }
        }

        public Brush GridTreeCurrentCellSelectionBackground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                   
                    #if !SILVERLIGHT
                     new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF2B2929"), 0),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF2C2A2A"), 0.027), 
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF2E2C2C"), 1)
                     });
#else
                     new GradientStop(){ Color=ColorExtensions.StringToColor ("#FF2B2929"),Offset= 0},
                    new GradientStop(){ Color=ColorExtensions.StringToColor("#FF2C2A2A"),Offset= 0.027}, 
                    new GradientStop(){ Color=ColorExtensions.StringToColor("#FF2E2C2C"),Offset= 1}
                     },0.0d);
#endif
               
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public Brush GridTreeRowHeaderBackgroundBrush
        {
            get
            {
                return this.GridTreeHeaderBackgroundBrush;
            }
        }

        public Brush GridTreeRowHeaderForegroundBrush
        {
            get
            {
                return this.GridTreeHeaderForegroundBrush;
            }
        }

        public Brush GridTreeRowHoverBackgroundBrush
        {
            get
            {
                return this.GridTreeCellBackgroundBrush;
            }
        }


        public Brush GridTreeCurrentCellSelectionForeground
        {
            get
            { 
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFAAA092"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFAAA092"));

#endif
            }

        }
#if !SILVERLIGHT
        public Geometry GridTreeExpanderPlusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 288.334,183.208L 288.334,174.125L 294.958,178.542L 288.334,183.208 Z ");
            }
        }

        public Geometry GridTreeExpanderMinusPath
        {
            get
            {
                return GridUtil.GetXamlConvertedValue<Geometry>("F1 M 274.487,165.737L 282.487,165.737L 282.487,157.681L 274.487,165.737 Z ");

            }
        }
#else
        public Path GridTreeExpanderPlusPath
        {
            get
            {
                string data = "M-91.702,-146.448L68.0101,-31.5536L-91.702,99.2988z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;

            }
        }

        public Path GridTreeExpanderMinusPath
        {
            get
            {
                string data = "M105.264,-136.319L106.268,65.2559L-91.702,65.2559z";
                string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
                return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;
            }
        }
#endif

       

        public Brush GridTreeExpanderBackground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF8D8D8D"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF8D8D8D"));

#endif
            }
        }

        public Brush GridTreeExpanderBorderBrush
        {
            get
            {
                return GridTreeExpanderBackground;
            }
        }

        public Brush GridTreeExpanderForeground
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFFFFFFF"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFFFFFFF"));

#endif
            }
        }

        public Brush GridTreeExpanderExpandedBackground
        {
            get
            {
                return this.GridTreeExpanderBackground;
            }
        }

        public Brush GridTreeExpanderExpandedBorderBrush
        {
            get
            {
                return this.GridTreeExpanderBackground;
            }
        }

        public Brush GridTreeExpanderExpandedForeground
        {
            get
            {
                return this.GridTreeExpanderForeground;
            }
        }

        public Brush GridTreeExpanderHoverBackground
        {
            get
            {
                return Brushes.White;
//                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
//                {
                    
//                    #if !SILVERLIGHT
//                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF635552"), 0.14d),
//                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF49382D"), 0.5d),         
//                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF3A2115"), 0.5d),
//                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFAF4802"), 0.986d),   
//                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFAF4802"), 1d),
//#else
//                    new GradientStop(){ Color=ColorExtensions.StringToColor ("#FF635552"),Offset= 0.14d},
//                    new GradientStop(){ Color=ColorExtensions.StringToColor("#FF49382D"),Offset= 0.5d},         
//                    new GradientStop(){ Color=ColorExtensions.StringToColor("#FF3A2115"),Offset= 0.5d},
//                    new GradientStop(){ Color=ColorExtensions.StringToColor("#FFAF4802"),Offset= 0.986d},   
//                    new GradientStop(){ Color=ColorExtensions.StringToColor("#FFAF4802"),Offset= 1d},
//#endif
//                },0.0d);
//                brush.StartPoint = new Point(0.5, 0);
//                brush.EndPoint = new Point(0.5, 1);
//                return brush;
            }
        }

        public Brush GridTreeExpanderHoverBorderBrush
        {
            get
            {
                return GridTreeExpanderHoverBackground;
            }
        }

        public Brush GridTreeExpanderHoverForeground
        {
            get
            {
                LinearGradientBrush brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    
                    #if !SILVERLIGHT
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF635552"), 0.14d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF49382D"), 0.5d),         
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF3A2115"), 0.5d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFAF4802"), 0.986d),   
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFAF4802"), 1d),
                    });
#else
                    new GradientStop(){Color=ColorExtensions.StringToColor ("#FF635552"),Offset= 0.14d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF49382D"),Offset= 0.5d},         
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FF3A2115"),Offset= 0.5d},
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFAF4802"),Offset= 0.986d},   
                    new GradientStop(){Color=ColorExtensions.StringToColor("#FFAF4802"),Offset= 1d},
                    },0.0d);
#endif
                
                brush.StartPoint = new Point(0.5, 0);
                brush.EndPoint = new Point(0.5, 1);
                return brush;
            }
        }

        public CellBordersInfo GridTreeCellBorders
        {
            get
            {
                return new CellBordersInfo()
                {
                    
#if !SILVERLIGHT
                    Top = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF353332")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF353332")), 0.25d),
                    Left = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF353332")), 0.25d),
                    Right = new Pen(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF353332")), 0.25d)
#else
                    Top = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")), 0.25d),
                    Bottom = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")), 0.25d),
                    Left = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")), 0.25d),
                    Right = new Pen(new SolidColorBrush(ColorExtensions.StringToColor("#FF353332")), 0.25d)
#endif
                };
            }
        }

        public GridFontInfo GridTreeCellFont
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

        public CellMarginsInfo GridTreeCellTextMargins
        {
            get
            {
                return new CellMarginsInfo()
                {
                    Left = 4d
                };
            }
        }

        public Brush GridTreeCellBackgroundBrush
        {
            get
            {
                 
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF413E3B"));
#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF413E3B"));
#endif
            }
        }

        public Brush GridTreeCellForegroundBrush
        {
            get
            {
#if !SILVERLIGHT
               return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFD0D0D0"));

#else
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFD0D0D0"));

#endif
            }
        }

        public double GridTreeCellBorderWidth
        {
            get
            {
                return 0.25d;
            }
        }

        public Brush GridTreeCellBorderBrush
        {
            get
            {              
                return Brushes.Transparent;            
            }
        }


        public Brush GridTreeCurrentCellBorderBrush
        {
            get
            {
#if !SILVERLIGHT
                return new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FFCC6431"));
#else
                return GridTreeCellBorderBrush; 
#endif
            }
        }

        public double GridTreeCurrentCellBorderWidth
        {
            get { return GridTreeCellBorderWidth; }
        }


        public Brush GridTreeNodeHighlightBrush
        {
            get
            {
                return this.GridTreeCellBackgroundBrush;
            }
        }
    }

}
