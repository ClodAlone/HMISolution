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
using System.Collections.Specialized;
using System.Windows.Shapes;
using System.ComponentModel;
using Syncfusion.Windows.Controls.Cells;

namespace Syncfusion.Windows.Controls.Grid
{

    public class GridDataStyleManager : DependencyObject
    {
        #region fields

        internal GridDataControl gridDataControl;

        #endregion

        #region Groups

        /// <summary>
        /// Gets or sets the row appearence.
        /// </summary>
        /// <value>The row appearence.</value>
        public RowAppearence RowAppearence
        {
            get { return (RowAppearence)GetValue(RowAppearenceProperty); }
            set { SetValue(RowAppearenceProperty, value); }
        }

        public static readonly DependencyProperty RowAppearenceProperty =
            DependencyProperty.Register("RowAppearence", typeof(RowAppearence), typeof(GridDataStyleManager), new PropertyMetadata(OnRowAppearenceChanged));

        private static void OnRowAppearenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                (e.NewValue as RowAppearence).StyleManager = d as GridDataStyleManager;
            }
        }

        /// <summary>
        /// Gets or sets the header appearence.
        /// </summary>
        /// <value>The header appearence.</value>
        public HeaderAppearence HeaderAppearence
        {
            get { return (HeaderAppearence)GetValue(HeaderAppearenceProperty); }
            set { SetValue(HeaderAppearenceProperty, value); }
        }

        public static readonly DependencyProperty HeaderAppearenceProperty =
            DependencyProperty.Register("HeaderAppearence", typeof(HeaderAppearence), typeof(GridDataStyleManager), new PropertyMetadata(OnHeaderAppearenceChanged));

        private static void OnHeaderAppearenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                (e.NewValue as HeaderAppearence).StyleManager = d as GridDataStyleManager;
            }
        }

        /// <summary>
        /// Gets or sets the column appearence.
        /// </summary>
        /// <value>The column appearence.</value>
        public ColumnAppearence ColumnAppearence
        {
            get { return (ColumnAppearence)GetValue(ColumnAppearenceProperty); }
            set { SetValue(ColumnAppearenceProperty, value); }
        }

        public static readonly DependencyProperty ColumnAppearenceProperty =
            DependencyProperty.Register("ColumnAppearence", typeof(ColumnAppearence), typeof(GridDataStyleManager), new PropertyMetadata(OnColumnAppearenceChanged));

        private static void OnColumnAppearenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                (e.NewValue as ColumnAppearence).StyleManager = d as GridDataStyleManager;
            }
        }

        /// <summary>
        /// Gets or sets the group area appearence.
        /// </summary>
        /// <value>The group area appearence.</value>
        public GroupAreaAppearence GroupAreaAppearence
        {
            get { return (GroupAreaAppearence)GetValue(GroupAreaAppearenceProperty); }
            set { SetValue(GroupAreaAppearenceProperty, value); }
        }

        public static readonly DependencyProperty GroupAreaAppearenceProperty =
            DependencyProperty.Register("GroupAreaAppearence", typeof(GroupAreaAppearence), typeof(GridDataStyleManager), new PropertyMetadata(OnGroupAreaAppearenceChanged));

        private static void OnGroupAreaAppearenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                (e.NewValue as GroupAreaAppearence).StyleManager = d as GridDataStyleManager;
            }
        }

        /// <summary>
        /// Gets or sets the expander appearence.
        /// </summary>
        /// <value>The expander appearence.</value>
        public ExpanderAppearence ExpanderAppearence
        {
            get { return (ExpanderAppearence)GetValue(ExpanderAppearenceProperty); }
            set { SetValue(ExpanderAppearenceProperty, value); }
        }

        public static readonly DependencyProperty ExpanderAppearenceProperty =
            DependencyProperty.Register("ExpanderAppearence", typeof(ExpanderAppearence), typeof(GridDataStyleManager), new PropertyMetadata(OnExpanderAppearenceChanged));

        private static void OnExpanderAppearenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                (e.NewValue as ExpanderAppearence).StyleManager = d as GridDataStyleManager;
            }
        }

        /// <summary>
        /// Gets or sets the value cell appearance.
        /// </summary>
        /// <value>The value cell appearance.</value>
        public ValueCellAppearance ValueCellAppearance
        {
            get { return (ValueCellAppearance)GetValue(ValueCellAppearanceProperty); }
            set { SetValue(ValueCellAppearanceProperty, value); }
        }

        public static readonly DependencyProperty ValueCellAppearanceProperty =
            DependencyProperty.Register("ValueCellAppearance", typeof(ValueCellAppearance), typeof(GridDataStyleManager), new PropertyMetadata(OnValueCellAppearanceChanged));

        private static void OnValueCellAppearanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                (e.NewValue as ValueCellAppearance).StyleManager = d as GridDataStyleManager;
            }
        }

        public NestedGridAppearance NestedGridAppearance
        {
            get { return (NestedGridAppearance)GetValue(NestedGridAppearanceProperty); }
            set { SetValue(NestedGridAppearanceProperty, value); }
        }

        public static readonly DependencyProperty NestedGridAppearanceProperty =
            DependencyProperty.Register("NestedGridAppearance", typeof(NestedGridAppearance), typeof(GridDataStyleManager), new PropertyMetadata(OnNestedGridAppearanceChanged));

        private static void OnNestedGridAppearanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                (e.NewValue as NestedGridAppearance).StyleManager = d as GridDataStyleManager;
            }
        }

        #endregion

    }

    public class Appearence : DependencyObject
    {
        /// <summary>
        /// Gets or sets the style manager.
        /// </summary>
        /// <value>The style manager.</value>
        internal virtual GridDataStyleManager StyleManager
        {
            get;
            set;
        }
    }

    public class GroupAreaAppearence : Appearence
    {
        /// <summary>
        /// Gets or Sets the background brush for the GroupDropArea.
        /// </summary>
        /// <value>The group area background brush.</value>
        public Brush GroupAreaBackgroundBrush
        {
            get { return (Brush)GetValue(GroupAreaBackgroundBrushProperty); }
            set { SetValue(GroupAreaBackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty GroupAreaBackgroundBrushProperty =
            DependencyProperty.Register("GroupAreaBackgroundBrush", typeof(Brush), typeof(GroupAreaAppearence), null);

        /// <summary>
        /// Gets or Sets the foreground brush for the GroupDropArea’s text.
        /// </summary>
        /// <value>The group area foreground brush.</value>
        public Brush GroupAreaForegroundBrush
        {
            get { return (Brush)GetValue(GroupAreaForegroundBrushProperty); }
            set { SetValue(GroupAreaForegroundBrushProperty, value); }
        }

        public static readonly DependencyProperty GroupAreaForegroundBrushProperty =
            DependencyProperty.Register("GroupAreaForegroundBrush", typeof(Brush), typeof(GroupAreaAppearence), null);

        /// <summary>
        /// Gets or Sets the Font information for the grouped header cell in the GroupDropArea.
        /// </summary>
        /// <value>The group header font.</value>
        public GridFontInfo GroupHeaderFont
        {
            get { return (GridFontInfo)GetValue(GroupHeaderFontProperty); }
            set { SetValue(GroupHeaderFontProperty, value); }
        }

        public static readonly DependencyProperty GroupHeaderFontProperty =
            DependencyProperty.Register("GroupHeaderFont", typeof(GridFontInfo), typeof(GroupAreaAppearence), null);

        /// <summary>
        /// Gets or Sets the CellBorder information for the grouped header cell in the GroupDropArea. 
        /// </summary>
        /// <value>The group cell borders.</value>
        public CellBordersInfo GroupCellBorders
        {
            get { return (CellBordersInfo)GetValue(GroupCellBordersProperty); }
            set { SetValue(GroupCellBordersProperty, value); }
        }

        public static readonly DependencyProperty GroupCellBordersProperty =
            DependencyProperty.Register("GroupCellBorders", typeof(CellBordersInfo), typeof(GroupAreaAppearence), null);


        /// <summary>
        /// Gets or Sets the CellMargin information for the grouped header cell in the GroupDropArea.
        /// </summary>
        /// <value>The group cell border margins.</value>
        public CellMarginsInfo GroupCellBorderMargins
        {
            get { return (CellMarginsInfo)GetValue(GroupCellBorderMarginsProperty); }
            set { SetValue(GroupCellBorderMarginsProperty, value); }
        }

        public static readonly DependencyProperty GroupCellBorderMarginsProperty =
            DependencyProperty.Register("GroupCellBorderMargins", typeof(CellMarginsInfo), typeof(GroupAreaAppearence), null);


        /// <summary>
        /// Gets or Sets the Background for the DragDropIndicator.
        /// </summary>
        /// <value>The drag drop indicator brush.</value>
        public Brush DragDropIndicatorBrush
        {
            get { return (Brush)GetValue(DragDropIndicatorBrushProperty); }
            set { SetValue(DragDropIndicatorBrushProperty, value); }
        }

        public static readonly DependencyProperty DragDropIndicatorBrushProperty =
            DependencyProperty.Register("DragDropIndicatorBrush", typeof(Brush), typeof(GroupAreaAppearence), null);

        /// <summary>
        /// Gets or Sets the Outer Border Brush for the DragDropIndicator.
        /// </summary>
        /// <value>The drag drop indicator outer brush.</value>
        public Brush DragDropIndicatorOuterBrush
        {
            get { return (Brush)GetValue(DragDropIndicatorOuterBrushProperty); }
            set { SetValue(DragDropIndicatorOuterBrushProperty, value); }
        }

        public static readonly DependencyProperty DragDropIndicatorOuterBrushProperty =
            DependencyProperty.Register("DragDropIndicatorOuterBrush", typeof(Brush), typeof(GroupAreaAppearence), null);

    }

    public class ExpanderAppearence : Appearence
    {
        /// <summary>
        /// Gets or Sets the Vector path of Clopsed Parent and the Group Caption rows
        /// </summary>
        /// <value>The plus path.</value>
        public Path PlusPath
        {
            get { return (Path)GetValue(PlusPathProperty); }
            set { SetValue(PlusPathProperty, value); }
        }

        public static readonly DependencyProperty PlusPathProperty =
            DependencyProperty.Register("PlusPath", typeof(Path), typeof(ExpanderAppearence), null);

        /// <summary>
        /// Gets or Sets the Vector path of Expaned Parent and the Group Caption rows
        /// </summary>
        /// <value>The minus path.</value>
        public Path MinusPath
        {
            get { return (Path)GetValue(MinusPathProperty); }
            set { SetValue(MinusPathProperty, value); }
        }

        public static readonly DependencyProperty MinusPathProperty =
            DependencyProperty.Register("MinusPath", typeof(Path), typeof(ExpanderAppearence), null);

        /// <summary>
        /// Gets or sets the background brush for the default Expander icon(i.e. In Clopsed state)
        /// </summary>
        /// <value>The plus minus button background.</value>
        public Brush PlusMinusButtonBackground
        {
            get { return (Brush)GetValue(PlusMinusButtonBackgroundProperty); }
            set { SetValue(PlusMinusButtonBackgroundProperty, value); }
        }

        public static readonly DependencyProperty PlusMinusButtonBackgroundProperty =
            DependencyProperty.Register("PlusMinusButtonBackground", typeof(Brush), typeof(ExpanderAppearence), null);

        /// <summary>
        /// Gets or sets the border brush for the default Expander icon (i.e. In Clopsed state)
        /// </summary>
        /// <value>The plus minus button border brush.</value>
        public Brush PlusMinusButtonBorderBrush
        {
            get { return (Brush)GetValue(PlusMinusButtonBorderBrushProperty); }
            set { SetValue(PlusMinusButtonBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty PlusMinusButtonBorderBrushProperty =
            DependencyProperty.Register("PlusMinusButtonBorderBrush", typeof(Brush), typeof(ExpanderAppearence), null);

        /// <summary>
        /// Gets or sets the foreground brush for the default Expander icon (i.e. In Clopsed state)
        /// </summary>
        /// <value>The plus minus button foreground.</value>
        public Brush PlusMinusButtonForeground
        {
            get { return (Brush)GetValue(PlusMinusButtonForegroundProperty); }
            set { SetValue(PlusMinusButtonForegroundProperty, value); }
        }

        public static readonly DependencyProperty PlusMinusButtonForegroundProperty =
            DependencyProperty.Register("PlusMinusButtonForeground", typeof(Brush), typeof(ExpanderAppearence), null);

        /// <summary>
        /// Gets or sets the background brush for the default expanded Expander icon
        /// </summary>
        /// <value>The plus minus expanded button background.</value>
        public Brush PlusMinusExpandedButtonBackground
        {
            get { return (Brush)GetValue(PlusMinusExpandedButtonBackgroundProperty); }
            set { SetValue(PlusMinusExpandedButtonBackgroundProperty, value); }
        }

        public static readonly DependencyProperty PlusMinusExpandedButtonBackgroundProperty =
            DependencyProperty.Register("PlusMinusExpandedButtonBackground", typeof(Brush), typeof(ExpanderAppearence), null);

        /// <summary>
        /// Gets or sets the border brush for the default expanded Expander icon
        /// </summary>
        /// <value>The plus minus expanded button border brush.</value>
        public Brush PlusMinusExpandedButtonBorderBrush
        {
            get { return (Brush)GetValue(PlusMinusExpandedButtonBorderBrushProperty); }
            set { SetValue(PlusMinusExpandedButtonBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty PlusMinusExpandedButtonBorderBrushProperty =
            DependencyProperty.Register("PlusMinusExpandedButtonBorderBrush", typeof(Brush), typeof(ExpanderAppearence), null);

        /// <summary>
        /// Gets or sets the foreground brush for the default expanded Expander icon
        /// </summary>
        /// <value>The plus minus expanded button foreground.</value>
        public Brush PlusMinusExpandedButtonForeground
        {
            get { return (Brush)GetValue(PlusMinusExpandedButtonForegroundProperty); }
            set { SetValue(PlusMinusExpandedButtonForegroundProperty, value); }
        }

        public static readonly DependencyProperty PlusMinusExpandedButtonForegroundProperty =
            DependencyProperty.Register("PlusMinusExpandedButtonForeground", typeof(Brush), typeof(ExpanderAppearence), null);

        /// <summary>
        /// Gets or sets the Hover background brush for the default Expander icon(i.e. In Clopsed state)
        /// </summary>
        /// <value>The plus minus hover button background.</value>
        public Brush PlusMinusHoverButtonBackground
        {
            get { return (Brush)GetValue(PlusMinusHoverButtonBackgroundProperty); }
            set { SetValue(PlusMinusHoverButtonBackgroundProperty, value); }
        }

        public static readonly DependencyProperty PlusMinusHoverButtonBackgroundProperty =
            DependencyProperty.Register("PlusMinusHoverButtonBackground", typeof(Brush), typeof(ExpanderAppearence), null);

        /// <summary>
        /// Gets or sets the Hover border brush for the default Expander icon (i.e. In Clopsed state)
        /// </summary>
        /// <value>The plus minus hover button border brush.</value>
        public Brush PlusMinusHoverButtonBorderBrush
        {
            get { return (Brush)GetValue(PlusMinusHoverButtonBorderBrushProperty); }
            set { SetValue(PlusMinusHoverButtonBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty PlusMinusHoverButtonBorderBrushProperty =
            DependencyProperty.Register("PlusMinusHoverButtonBorderBrush", typeof(Brush), typeof(ExpanderAppearence), null);

        /// <summary>
        /// Gets or sets the Hover foreground brush for the default Expander icon (i.e. In Clopsed state)
        /// </summary>
        /// <value>The plus minus hover button foreground.</value>
        public Brush PlusMinusHoverButtonForeground
        {
            get { return (Brush)GetValue(PlusMinusHoverButtonForegroundProperty); }
            set { SetValue(PlusMinusHoverButtonForegroundProperty, value); }
        }

        public static readonly DependencyProperty PlusMinusHoverButtonForegroundProperty =
            DependencyProperty.Register("PlusMinusHoverButtonForeground", typeof(Brush), typeof(ExpanderAppearence), null);

        /// <summary>
        /// Gets or sets the plus minus caption selected button background.
        /// </summary>
        /// <value>The plus minus caption selected button background.</value>
        [Obsolete]
        public Brush PlusMinusCaptionSelectedButtonBackground
        {
            get { return (Brush)GetValue(PlusMinusCaptionSelectedButtonBackgroundProperty); }
            set { SetValue(PlusMinusCaptionSelectedButtonBackgroundProperty, value); }
        }


        public static readonly DependencyProperty PlusMinusCaptionSelectedButtonBackgroundProperty =
            DependencyProperty.Register("PlusMinusCaptionSelectedButtonBackground", typeof(Brush), typeof(ExpanderAppearence), null);

        /// <summary>
        /// Gets or sets the plus minus caption selected button border brush.
        /// </summary>
        /// <value>The plus minus caption selected button border brush.</value>
        [Obsolete]
        public Brush PlusMinusCaptionSelectedButtonBorderBrush
        {
            get { return (Brush)GetValue(PlusMinusCaptionSelectedButtonBorderBrushProperty); }
            set { SetValue(PlusMinusCaptionSelectedButtonBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty PlusMinusCaptionSelectedButtonBorderBrushProperty =
            DependencyProperty.Register("PlusMinusCaptionSelectedButtonBorderBrush", typeof(Brush), typeof(ExpanderAppearence), null);

        /// <summary>
        /// Gets or sets the plus minus caption selected button foreground.
        /// </summary>
        /// <value>The plus minus caption selected button foreground.</value>
        [Obsolete]
        public Brush PlusMinusCaptionSelectedButtonForeground
        {
            get { return (Brush)GetValue(PlusMinusCaptionSelectedButtonForegroundProperty); }
            set { SetValue(PlusMinusCaptionSelectedButtonForegroundProperty, value); }
        }

        public static readonly DependencyProperty PlusMinusCaptionSelectedButtonForegroundProperty =
            DependencyProperty.Register("PlusMinusCaptionSelectedButtonForeground", typeof(Brush), typeof(ExpanderAppearence), null);

    }

    public class HeaderAppearence : Appearence
    {
        /// <summary>
        /// Gets or Sets the background brush for the Header cell.
        /// </summary>
        /// <value>The header background brush.</value>
        public Brush HeaderBackgroundBrush
        {
            get { return (Brush)GetValue(HeaderBackgroundBrushProperty); }
            set { SetValue(HeaderBackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty HeaderBackgroundBrushProperty =
            DependencyProperty.Register("HeaderBackgroundBrush", typeof(Brush), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets or Sets the foreground brush for the Header cell.
        /// </summary>
        /// <value>The header foreground brush.</value>
        public Brush HeaderForegroundBrush
        {
            get { return (Brush)GetValue(HeaderForegroundBrushProperty); }
            set { SetValue(HeaderForegroundBrushProperty, value); }
        }

        public static readonly DependencyProperty HeaderForegroundBrushProperty =
            DependencyProperty.Register("HeaderForegroundBrush", typeof(Brush), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets or Sets the hover background brush for the Header cell.
        /// </summary>
        /// <value>The header hover background brush.</value>
        public Brush HeaderHoverBackgroundBrush
        {
            get { return (Brush)GetValue(HeaderHoverBackgroundBrushProperty); }
            set { SetValue(HeaderHoverBackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty HeaderHoverBackgroundBrushProperty =
            DependencyProperty.Register("HeaderHoverBackgroundBrush", typeof(Brush), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets or Sets the hover foreground brush for the Header cell.
        /// </summary>
        /// <value>The header hover foreground brush.</value>
        public Brush HeaderHoverForegroundBrush
        {
            get { return (Brush)GetValue(HeaderHoverForegroundBrushProperty); }
            set { SetValue(HeaderHoverForegroundBrushProperty, value); }
        }

        public static readonly DependencyProperty HeaderHoverForegroundBrushProperty =
            DependencyProperty.Register("HeaderHoverForegroundBrush", typeof(Brush), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets or Sets the Cell border information for the Header cell area.
        /// </summary>
        /// <value>The header cell borders.</value>
        public CellBordersInfo HeaderCellBorders
        {
            get { return (CellBordersInfo)GetValue(HeaderCellBordersProperty); }
            set { SetValue(HeaderCellBordersProperty, value); }
        }

        public static readonly DependencyProperty HeaderCellBordersProperty =
            DependencyProperty.Register("HeaderCellBorders", typeof(CellBordersInfo), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets the header control's border which says as inner border's brush for all directions.
        /// </summary>
        /// <value>The header inner border.</value>
        public Brush HeaderInnerBorder
        {
            get { return (Brush)GetValue(HeaderInnerBorderProperty); }
            set { SetValue(HeaderInnerBorderProperty, value); }
        }

        public static readonly DependencyProperty HeaderInnerBorderProperty =
            DependencyProperty.Register("HeaderInnerBorder", typeof(Brush), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets the header control's border which says as inner border's thickness for all directions.
        /// </summary>
        /// <value>The header inner border thickness.</value>
        public Thickness HeaderInnerBorderThickness
        {
            get { return (Thickness)GetValue(HeaderInnerBorderThicknessProperty); }
            set { SetValue(HeaderInnerBorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty HeaderInnerBorderThicknessProperty =
            DependencyProperty.Register("HeaderInnerBorderThickness", typeof(Thickness), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets or Sets the font information of the Header cell.
        /// </summary>
        /// <value>The header font.</value>
        public GridFontInfo HeaderFont
        {
            get { return (GridFontInfo)GetValue(HeaderFontProperty); }
            set { SetValue(HeaderFontProperty, value); }
        }

        public static readonly DependencyProperty HeaderFontProperty =
            DependencyProperty.Register(" HeaderFont", typeof(GridFontInfo), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets or Sets the text margins of the Header cell.
        /// </summary>
        /// <value>The header text margins.</value>
        public CellMarginsInfo HeaderTextMargins
        {
            get { return (CellMarginsInfo)GetValue(HeaderTextMarginsProperty); }
            set { SetValue(HeaderTextMarginsProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextMarginsProperty =
            DependencyProperty.Register("HeaderTextMargins", typeof(CellMarginsInfo), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets or Sets the brush for the sort icon.
        /// </summary>
        /// <value>The sort widget brush.</value>
        public Brush SortWidgetBrush
        {
            get { return (Brush)GetValue(SortWidgetBrushProperty); }
            set { SetValue(SortWidgetBrushProperty, value); }
        }

        public static readonly DependencyProperty SortWidgetBrushProperty =
            DependencyProperty.Register("SortWidgetBrush", typeof(Brush), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets or Sets the background brush for the Filter icon.
        /// </summary>
        /// <value>The filter button inner brush.</value>
        public Brush FilterButtonInnerBrush
        {
            get { return (Brush)GetValue(FilterButtonInnerBrushProperty); }
            set { SetValue(FilterButtonInnerBrushProperty, value); }
        }

        public static readonly DependencyProperty FilterButtonInnerBrushProperty =
            DependencyProperty.Register("FilterButtonInnerBrush", typeof(Brush), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets or Sets the border brush for the Filter icon.
        /// </summary>
        /// <value>The filter button outer brush.</value>
        public Brush FilterButtonOuterBrush
        {
            get { return (Brush)GetValue(FilterButtonOuterBrushProperty); }
            set { SetValue(FilterButtonOuterBrushProperty, value); }
        }

        public static readonly DependencyProperty FilterButtonOuterBrushProperty =
            DependencyProperty.Register("FilterButtonOuterBrush", typeof(Brush), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets or Sets the hover background brush for the Filter icon.
        /// </summary>
        /// <value>The filter button hover inner brush.</value>
        public Brush FilterButtonHoverInnerBrush
        {
            get { return (Brush)GetValue(FilterButtonHoverInnerBrushProperty); }
            set { SetValue(FilterButtonHoverInnerBrushProperty, value); }
        }

        public static readonly DependencyProperty FilterButtonHoverInnerBrushProperty =
            DependencyProperty.Register("FilterButtonHoverInnerBrush", typeof(Brush), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets or Sets the hover border brush for the Filter icon.
        /// </summary>
        /// <value>The filter button hover outer brush.</value>
        public Brush FilterButtonHoverOuterBrush
        {
            get { return (Brush)GetValue(FilterButtonHoverOuterBrushProperty); }
            set { SetValue(FilterButtonHoverOuterBrushProperty, value); }
        }

        public static readonly DependencyProperty FilterButtonHoverOuterBrushProperty =
            DependencyProperty.Register("FilterButtonHoverOuterBrush", typeof(Brush), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets or Sets the background brush of the filter icon when the filter is applied.
        /// </summary>
        /// <value>The filter button applied brush.</value>
        public Brush FilterButtonAppliedBrush
        {
            get { return (Brush)GetValue(FilterButtonAppliedBrushProperty); }
            set { SetValue(FilterButtonAppliedBrushProperty, value); }
        }

        public static readonly DependencyProperty FilterButtonAppliedBrushProperty =
            DependencyProperty.Register("FilterButtonAppliedBrush", typeof(Brush), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets or Sets the background brush for the column options popup.
        /// </summary>
        /// <value>The column options popup background.</value>
        public Brush ColumnOptionsPopupBackground
        {
            get { return (Brush)GetValue(ColumnOptionsPopupBackgroundProperty); }
            set { SetValue(ColumnOptionsPopupBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ColumnOptionsPopupBackgroundProperty =
            DependencyProperty.Register("ColumnOptionsPopupBackground", typeof(Brush), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets or Sets the foreground brush for the column options popup.
        /// </summary>
        /// <value>The column options popup foreground.</value>
        public Brush ColumnOptionsPopupForeground
        {
            get { return (Brush)GetValue(ColumnOptionsPopupForegroundProperty); }
            set { SetValue(ColumnOptionsPopupForegroundProperty, value); }
        }

        public static readonly DependencyProperty ColumnOptionsPopupForegroundProperty =
            DependencyProperty.Register("ColumnOptionsPopupForeground", typeof(Brush), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets or Sets the background brush for the column options icon.
        /// </summary>
        /// <value>The column options button background.</value>
        public Brush ColumnOptionsButtonBackground
        {
            get { return (Brush)GetValue(ColumnOptionsButtonBackgroundProperty); }
            set { SetValue(ColumnOptionsButtonBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ColumnOptionsButtonBackgroundProperty =
            DependencyProperty.Register("ColumnOptionsButtonBackground", typeof(Brush), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets or Sets the border brush for the column options icon.
        /// </summary>
        /// <value>The column options button border brush.</value>
        public Brush ColumnOptionsButtonBorderBrush
        {
            get { return (Brush)GetValue(ColumnOptionsButtonBorderBrushProperty); }
            set { SetValue(ColumnOptionsButtonBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty ColumnOptionsButtonBorderBrushProperty =
            DependencyProperty.Register("ColumnOptionsButtonBorderBrush", typeof(Brush), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets or sets the column options close button brush.
        /// </summary>
        /// <value>The column options close button brush.</value>
        public Brush ColumnOptionsCloseButtonBrush
        {
            get { return (Brush)GetValue(ColumnOptionsCloseButtonBrushProperty); }
            set { SetValue(ColumnOptionsCloseButtonBrushProperty, value); }
        }

        public static readonly DependencyProperty ColumnOptionsCloseButtonBrushProperty =
            DependencyProperty.Register("ColumnOptionsCloseButtonBrush", typeof(Brush), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets or Sets the hover background brush for the options of header cell 
        /// control such as sort icon, filter icon and column options icon.
        /// </summary>
        /// <value>The header options hover background.</value>
        public Brush HeaderOptionsHoverBackground
        {
            get { return (Brush)GetValue(HeaderOptionsHoverBackgroundProperty); }
            set { SetValue(HeaderOptionsHoverBackgroundProperty, value); }
        }

        public static readonly DependencyProperty HeaderOptionsHoverBackgroundProperty =
            DependencyProperty.Register("HeaderOptionsHoverBackground", typeof(Brush), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets or Sets the border brush for the options of header cell 
        /// control such as sort icon, filter icon and column options icon.
        /// </summary>
        /// <value>The header options border brush.</value>
        public Brush HeaderOptionsBorderBrush
        {
            get { return (Brush)GetValue(HeaderOptionsBorderBrushProperty); }
            set { SetValue(HeaderOptionsBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty HeaderOptionsBorderBrushProperty =
            DependencyProperty.Register("HeaderOptionsBorderBrush", typeof(Brush), typeof(HeaderAppearence), null);

        /// <summary>
        /// Gets or Sets the background brush for the options of header cell control such as sort icon, 
        /// filter icon and column options icon when it is checked.
        /// </summary>
        /// <value>The header options checked background.</value>
        public Brush HeaderOptionsCheckedBackground
        {
            get { return (Brush)GetValue(HeaderOptionsCheckedBackgroundProperty); }
            set { SetValue(HeaderOptionsCheckedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty HeaderOptionsCheckedBackgroundProperty =
            DependencyProperty.Register("HeaderOptionsCheckedBackground", typeof(Brush), typeof(HeaderAppearence), null);

    }

    public class RowAppearence : Appearence
    {
        /// <summary>
        /// Gets or sets the row header background.
        /// </summary>
        /// <value>The row header background.</value>
        public Brush RowHeaderBackground
        {
            get { return (Brush)GetValue(RowHeaderBackgroundProperty); }
            set { SetValue(RowHeaderBackgroundProperty, value); }
        }

        public static readonly DependencyProperty RowHeaderBackgroundProperty =
            DependencyProperty.Register("RowHeaderBackground", typeof(Brush), typeof(RowAppearence), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Path that indicating the Current row in Row header.
        /// </summary>
        /// <value>The Row Header Icon Path.</value>
        public Path RowHeaderIconPath
        {
            get { return (Path)GetValue(RowHeaderIconPathProperty); }
            set { SetValue(RowHeaderIconPathProperty, value); }
        }

        public static readonly DependencyProperty RowHeaderIconPathProperty =
            DependencyProperty.Register("RowHeaderIconPath", typeof(Path), typeof(RowAppearence), null);

        /// <summary>
        /// Gets of Sets the brush for Row background
        /// </summary>
        /// <value>The row background.</value>
        public Brush RowBackground
        {
            get { return (Brush)GetValue(RowBackgroundProperty); }
            set { SetValue(RowBackgroundProperty, value); }
        }

        public static readonly DependencyProperty RowBackgroundProperty =
            DependencyProperty.Register("RowBackground", typeof(Brush), typeof(RowAppearence), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the background brush for the selected row.
        /// </summary>
        /// <value>The highlight selection background.</value>
        public Brush HighlightSelectionBackground
        {
            get { return (Brush)GetValue(HighlightSelectionBackgroundProperty); }
            set { SetValue(HighlightSelectionBackgroundProperty, value); }
        }

        public static readonly DependencyProperty HighlightSelectionBackgroundProperty =
            DependencyProperty.Register("HighlightSelectionBackground", typeof(Brush), typeof(RowAppearence), null);

        /// <summary>
        /// Gets or Sets the foreground brush for the selected row.
        /// </summary>
        /// <value>The highlight selection foreground.</value>
        public Brush HighlightSelectionForeground
        {
            get { return (Brush)GetValue(HighlightSelectionForegroundProperty); }
            set { SetValue(HighlightSelectionForegroundProperty, value); }
        }

        public static readonly DependencyProperty HighlightSelectionForegroundProperty =
            DependencyProperty.Register("HighlightSelectionForeground", typeof(Brush), typeof(RowAppearence), null);

        /// <summary>
        /// Gets or Sets the background brush for the summary caption row.
        /// </summary>
        /// <value>The summary caption background.</value>
        public Brush SummaryCaptionBackground
        {
            get { return (Brush)GetValue(SummaryCaptionBackgroundProperty); }
            set { SetValue(SummaryCaptionBackgroundProperty, value); }
        }

        public static readonly DependencyProperty SummaryCaptionBackgroundProperty =
            DependencyProperty.Register("SummaryCaptionBackground", typeof(Brush), typeof(RowAppearence), null);

        /// <summary>
        /// Gets or Sets the foreground brush for the summary caption row.
        /// </summary>
        /// <value>The summary caption foreground.</value>
        public Brush SummaryCaptionForeground
        {
            get { return (Brush)GetValue(SummaryCaptionForegroundProperty); }
            set { SetValue(SummaryCaptionForegroundProperty, value); }
        }

        public static readonly DependencyProperty SummaryCaptionForegroundProperty =
            DependencyProperty.Register("SummaryCaptionForeground", typeof(Brush), typeof(RowAppearence), null);

        /// <summary>
        /// Gets or Sets the foreground brush for the row where the mouse is pointed.
        /// </summary>
        /// <value>The hovering group caption cell background.</value>
        public Brush HoveringGroupCaptionCellBackground
        {
            get { return (Brush)GetValue(HoveringGroupCaptionCellBackgroundProperty); }
            set { SetValue(HoveringGroupCaptionCellBackgroundProperty, value); }
        }

        public static readonly DependencyProperty HoveringGroupCaptionCellBackgroundProperty =
            DependencyProperty.Register("HoveringGroupCaptionCellBackground", typeof(Brush), typeof(RowAppearence), null);

        /// <summary>
        /// Gets or Sets the background brush for the row where the mouse is pointed.
        /// </summary>
        /// <value>The hovering record cell background.</value>
        public Brush HoveringRecordCellBackground
        {
            get { return (Brush)GetValue(HoveringRecordCellBackgroundProperty); }
            set { SetValue(HoveringRecordCellBackgroundProperty, value); }
        }

        public static readonly DependencyProperty HoveringRecordCellBackgroundProperty =
            DependencyProperty.Register("HoveringRecordCellBackground", typeof(Brush), typeof(RowAppearence), null);

        /// <summary>
        /// Gets or sets the highlight brush.
        /// </summary>
        /// <value>The highlight brush.</value>
        public Brush HighlightBrush
        {
            get { return (Brush)GetValue(HighlightBrushProperty); }
            set { SetValue(HighlightBrushProperty, value); }
        }

        public static readonly DependencyProperty HighlightBrushProperty =
            DependencyProperty.Register("HighlightBrush", typeof(Brush), typeof(RowAppearence), null);

        /// <summary>
        /// Gets or Sets the foreground brush for the selected row.
        /// </summary>
        /// <value>The hovering record cell foreground.</value>
        public Brush HoveringRecordCellForeground
        {
            get { return (Brush)GetValue(HoveringRecordCellForegroundProperty); }
            set { SetValue(HoveringRecordCellForegroundProperty, value); }
        }

        public static readonly DependencyProperty HoveringRecordCellForegroundProperty =
            DependencyProperty.Register("HoveringRecordCellForeground", typeof(Brush), typeof(RowAppearence), null);

        /// <summary>
        /// Gets or Sets the font information that to be displayed on the summary caption row.
        /// </summary>
        /// <value>The summary caption font.</value>
        public GridFontInfo SummaryCaptionFont
        {
            get { return (GridFontInfo)GetValue(SummaryCaptionFontProperty); }
            set { SetValue(SummaryCaptionFontProperty, value); }
        }

        public static readonly DependencyProperty SummaryCaptionFontProperty =
            DependencyProperty.Register("SummaryCaptionFont", typeof(GridFontInfo), typeof(RowAppearence), null);

        /// <summary>
        /// Gets or Sets the background brush for the summary row.
        /// </summary>
        /// <value>The summary row background.</value>
        public Brush SummaryRowBackground
        {
            get { return (Brush)GetValue(SummaryRowBackgroundProperty); }
            set { SetValue(SummaryRowBackgroundProperty, value); }
        }

        public static readonly DependencyProperty SummaryRowBackgroundProperty =
            DependencyProperty.Register("SummaryRowBackground", typeof(Brush), typeof(RowAppearence), null);

        /// <summary>
        /// Gets or Sets the foreground brush for the summary row.
        /// </summary>
        /// <value>The summary row foreground.</value>
        public Brush SummaryRowForeground
        {
            get { return (Brush)GetValue(SummaryRowForegroundProperty); }
            set { SetValue(SummaryRowForegroundProperty, value); }
        }

        public static readonly DependencyProperty SummaryRowForegroundProperty =
            DependencyProperty.Register("SummaryRowForeground", typeof(Brush), typeof(RowAppearence), null);

        /// <summary>
        /// Gets or Sets the font information that to be displayed on the summary row.
        /// </summary>
        /// <value>The summary row font.</value>
        public GridFontInfo SummaryRowFont
        {
            get { return (GridFontInfo)GetValue(SummaryRowFontProperty); }
            set { SetValue(SummaryRowFontProperty, value); }
        }

        public static readonly DependencyProperty SummaryRowFontProperty =
            DependencyProperty.Register("SummaryRowFont", typeof(GridFontInfo), typeof(RowAppearence), null);

        /// <summary>
        /// Gets or sets the row header selection background.
        /// </summary>
        /// <value>The row header selection background.</value>
        public Brush RowHeaderSelectionBackground
        {
            get { return (Brush)GetValue(RowHeaderSelectionBackgroundProperty); }
            set { SetValue(RowHeaderSelectionBackgroundProperty, value); }
        }

        public static readonly DependencyProperty RowHeaderSelectionBackgroundProperty =
            DependencyProperty.Register("RowHeaderSelectionBackground", typeof(Brush), typeof(RowAppearence), null);

        /// <summary>
        /// Gets or sets the row header foreground.
        /// </summary>
        /// <value>The row header foreground.</value>
        public Brush RowHeaderForeground
        {
            get { return (Brush)GetValue(RowHeaderForegroundProperty); }
            set { SetValue(RowHeaderForegroundProperty, value); }
        }

        public static readonly DependencyProperty RowHeaderForegroundProperty =
            DependencyProperty.Register("RowHeaderForeground", typeof(Brush), typeof(RowAppearence), null);

        /// <summary>
        /// Gets or Sets the background brush for the selected group caption row.
        /// </summary>
        /// <value>The group caption selection background.</value>
        public Brush GroupCaptionSelectionBackground
        {
            get { return (Brush)GetValue(GroupCaptionSelectionBackgroundProperty); }
            set { SetValue(GroupCaptionSelectionBackgroundProperty, value); }
        }

        public static readonly DependencyProperty GroupCaptionSelectionBackgroundProperty =
            DependencyProperty.Register("GroupCaptionSelectionBackground", typeof(Brush), typeof(RowAppearence), null);

        /// <summary>
        /// Gets or Sets the foreground brush for a selected cell’s foreground in a row.
        /// </summary>
        /// <value>The current cell selection background.</value>
        public Brush CurrentCellSelectionBackground
        {
            get { return (Brush)GetValue(CurrentCellSelectionBackgroundProperty); }
            set { SetValue(CurrentCellSelectionBackgroundProperty, value); }
        }

        public static readonly DependencyProperty CurrentCellSelectionBackgroundProperty =
            DependencyProperty.Register("CurrentCellSelectionBackground", typeof(Brush), typeof(RowAppearence), null);

        /// <summary>
        /// Gets or Sets the foreground brush for the selected group caption row.
        /// </summary>
        /// <value>The group caption selection foreground.</value>
        public Brush GroupCaptionSelectionForeground
        {
            get { return (Brush)GetValue(GroupCaptionSelectionForegroundProperty); }
            set { SetValue(GroupCaptionSelectionForegroundProperty, value); }
        }

        public static readonly DependencyProperty GroupCaptionSelectionForegroundProperty =
            DependencyProperty.Register("GroupCaptionSelectionForeground", typeof(Brush), typeof(RowAppearence), null);

        /// <summary>
        /// Gets or sets the current cell selection foreground.
        /// </summary>
        /// <value>The current cell selection foreground.</value>
        public Brush CurrentCellSelectionForeground
        {
            get { return (Brush)GetValue(CurrentCellSelectionForegroundProperty); }
            set { SetValue(CurrentCellSelectionForegroundProperty, value); }
        }

        public static readonly DependencyProperty CurrentCellSelectionForegroundProperty =
            DependencyProperty.Register("CurrentCellSelectionForeground", typeof(Brush), typeof(RowAppearence), null);
    }

    public class ColumnAppearence : Appearence
    {
        /// <summary>
        /// Gets or sets the check box visible column style.
        /// </summary>
        /// <value>The check box visible column style.</value>
        public Style CheckBoxVisibleColumnStyle
        {
            get { return (Style)GetValue(CheckBoxVisibleColumnStyleProperty); }
            set { SetValue(CheckBoxVisibleColumnStyleProperty, value); }
        }

        public static readonly DependencyProperty CheckBoxVisibleColumnStyleProperty =
            DependencyProperty.Register("CheckBoxVisibleColumnStyle", typeof(Style), typeof(ColumnAppearence), new PropertyMetadata(OnCheckBoxVisibleColumnStyleChanged));

        private static void OnCheckBoxVisibleColumnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                (d as ColumnAppearence).ApplyStyle();
            }
        }

        private void ApplyStyle()
        {
            //if (StyleManager != null)
            //    StyleManager.ApplyStyle();
        }

        /// <summary>
        /// Gets or sets the currency edit visible column style.
        /// </summary>
        /// <value>The currency edit visible column style.</value>
        public Style CurrencyEditVisibleColumnStyle
        {
            get { return (Style)GetValue(CurrencyEditVisibleColumnStyleProperty); }
            set { SetValue(CurrencyEditVisibleColumnStyleProperty, value); }
        }

        public static readonly DependencyProperty CurrencyEditVisibleColumnStyleProperty =
            DependencyProperty.Register("CurrencyEditVisibleColumnStyle", typeof(Style), typeof(ColumnAppearence), new PropertyMetadata(OnCurrencyEditVisibleColumnStyleChanged));

        private static void OnCurrencyEditVisibleColumnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                (d as ColumnAppearence).ApplyStyle();
            }
        }

        /// <summary>
        /// Gets or sets the date time visible column style.
        /// </summary>
        /// <value>The date time visible column style.</value>
        public Style DateTimeVisibleColumnStyle
        {
            get { return (Style)GetValue(DateTimeVisibleColumnStyleProperty); }
            set { SetValue(DateTimeVisibleColumnStyleProperty, value); }
        }

        public static readonly DependencyProperty DateTimeVisibleColumnStyleProperty =
            DependencyProperty.Register("DateTimeVisibleColumnStyle", typeof(Style), typeof(ColumnAppearence), new PropertyMetadata(OnDateTimeVisibleColumnStyleChanged));

        private static void OnDateTimeVisibleColumnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                (d as ColumnAppearence).ApplyStyle();
            }
        }

        [Obsolete]
        /// <summary>
        /// Gets or sets the double edit visible column style.
        /// </summary>
        /// <value>The double edit visible column style.</value>
        public Style DouleEditVisibleColumnStyle
        {
            get { return (Style)GetValue(DouleEditVisibleColumnStyleProperty); }
            set { SetValue(DouleEditVisibleColumnStyleProperty, value); }
        }

        [Obsolete]
        public static readonly DependencyProperty DouleEditVisibleColumnStyleProperty =
            DependencyProperty.Register("DouleEditVisibleColumnStyle", typeof(Style), typeof(ColumnAppearence), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the double edit visible column style.
        /// </summary>
        /// <value>The double edit visible column style.</value>
        public Style DoubleEditVisibleColumnStyle
        {
            get { return (Style)GetValue(DoubleEditVisibleColumnStyleProperty); }
            set { SetValue(DoubleEditVisibleColumnStyleProperty, value); }
        }

        public static readonly DependencyProperty DoubleEditVisibleColumnStyleProperty =
            DependencyProperty.Register("DoubleEditVisibleColumnStyle", typeof(Style), typeof(ColumnAppearence), new PropertyMetadata(OnDoubleEditVisibleColumnStyleChanged));

        private static void OnDoubleEditVisibleColumnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                (d as ColumnAppearence).ApplyStyle();
            }
        }

        /// <summary>
        /// Gets or sets the integer edit visible column style.
        /// </summary>
        /// <value>The integer edit visible column style.</value>
        public Style IntegerEditVisibleColumnStyle
        {
            get { return (Style)GetValue(IntegerEditVisibleColumnStyleProperty); }
            set { SetValue(IntegerEditVisibleColumnStyleProperty, value); }
        }

        public static readonly DependencyProperty IntegerEditVisibleColumnStyleProperty =
            DependencyProperty.Register("IntegerEditVisibleColumnStyle", typeof(Style), typeof(ColumnAppearence), new PropertyMetadata(OnIntegerEditVisibleColumnStyleChanged));

        private static void OnIntegerEditVisibleColumnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                (d as ColumnAppearence).ApplyStyle();
            }
        }

        /// <summary>
        /// Gets or sets the mask edit visible column style.
        /// </summary>
        /// <value>The mask edit visible column style.</value>
        public Style MaskEditVisibleColumnStyle
        {
            get { return (Style)GetValue(MaskEditVisibleColumnStyleProperty); }
            set { SetValue(MaskEditVisibleColumnStyleProperty, value); }
        }

        public static readonly DependencyProperty MaskEditVisibleColumnStyleProperty =
            DependencyProperty.Register("MaskEditVisibleColumnStyle", typeof(Style), typeof(ColumnAppearence), new PropertyMetadata(OnMaskEditVisibleColumnStyleChanged));

        private static void OnMaskEditVisibleColumnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                (d as ColumnAppearence).ApplyStyle();
            }
        }

        /// <summary>
        /// Gets or sets the percent edit visible column style.
        /// </summary>
        /// <value>The percent edit visible column style.</value>
        public Style PercentEditVisibleColumnStyle
        {
            get { return (Style)GetValue(PercentEditVisibleColumnStyleProperty); }
            set { SetValue(PercentEditVisibleColumnStyleProperty, value); }
        }

        public static readonly DependencyProperty PercentEditVisibleColumnStyleProperty =
            DependencyProperty.Register("PercentEditVisibleColumnStyle", typeof(Style), typeof(ColumnAppearence), new PropertyMetadata(OnPercentEditVisibleColumnStyleChanged));

        private static void OnPercentEditVisibleColumnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                (d as ColumnAppearence).ApplyStyle();
            }
        }

        /// <summary>
        /// Gets or sets up down edit visible column style.
        /// </summary>
        /// <value>Up down edit visible column style.</value>
        public Style UpDownEditVisibleColumnStyle
        {
            get { return (Style)GetValue(UpDownEditVisibleColumnStyleProperty); }
            set { SetValue(UpDownEditVisibleColumnStyleProperty, value); }
        }

        public static readonly DependencyProperty UpDownEditVisibleColumnStyleProperty =
            DependencyProperty.Register("UpDownEditVisibleColumnStyle", typeof(Style), typeof(ColumnAppearence), new PropertyMetadata(OnUpDownEditVisibleColumnStyleChanged));

        private static void OnUpDownEditVisibleColumnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                (d as ColumnAppearence).ApplyStyle();
            }
        }
    }

    public class ValueCellAppearance : Appearence
    {
        /// <summary>
        /// Gets or sets the value cell borders.
        /// </summary>
        /// <value>The value cell borders.</value>
        public CellBordersInfo ValueCellBorders
        {
            get { return (CellBordersInfo)GetValue(ValueCellBordersProperty); }
            set { SetValue(ValueCellBordersProperty, value); }
        }

        public static readonly DependencyProperty ValueCellBordersProperty =
            DependencyProperty.Register("ValueCellBorders", typeof(CellBordersInfo), typeof(ValueCellAppearance), null);



        /// <summary>
        /// Gets or sets the grid border brush.
        /// </summary>
        /// <value>The grid border brush.</value>
        public Brush GridBorderBrush
        {
            get { return (Brush)GetValue(GridBorderBrushProperty); }
            set { SetValue(GridBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty GridBorderBrushProperty =
            DependencyProperty.Register("GridBorderBrush", typeof(Brush), typeof(ValueCellAppearance), null);

        /// <summary>
        /// Gets or sets the grid border thickness.
        /// </summary>
        /// <value>The grid border thickness.</value>
        public Thickness GridBorderThickness
        {
            get { return (Thickness)GetValue(GridBorderThicknessProperty); }
            set { SetValue(GridBorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty GridBorderThicknessProperty =
            DependencyProperty.Register("GridBorderThickness", typeof(Thickness), typeof(ValueCellAppearance), null);

        /// <summary>
        /// Gets or sets the value font.
        /// </summary>
        /// <value>The value font.</value>
        public GridFontInfo ValueFont
        {
            get { return (GridFontInfo)GetValue(ValueFontProperty); }
            set { SetValue(ValueFontProperty, value); }
        }

        public static readonly DependencyProperty ValueFontProperty =
            DependencyProperty.Register("ValueFont", typeof(GridFontInfo), typeof(ValueCellAppearance), null);

        /// <summary>
        /// Gets or sets the value text margins.
        /// </summary>
        /// <value>The value text margins.</value>
        public CellMarginsInfo ValueTextMargins
        {
            get { return (CellMarginsInfo)GetValue(ValueTextMarginsProperty); }
            set { SetValue(ValueTextMarginsProperty, value); }
        }

        public static readonly DependencyProperty ValueTextMarginsProperty =
            DependencyProperty.Register("ValueTextMargins", typeof(CellMarginsInfo), typeof(ValueCellAppearance), null);

        /// <summary>
        /// Gets or sets the value background brush.
        /// </summary>
        /// <value>The value background brush.</value>
        public Brush ValueBackgroundBrush
        {
            get { return (Brush)GetValue(ValueBackgroundBrushProperty); }
            set { SetValue(ValueBackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty ValueBackgroundBrushProperty =
            DependencyProperty.Register("ValueBackgroundBrush", typeof(Brush), typeof(ValueCellAppearance), null);

        /// <summary>
        /// Gets or sets the value foreground brush.
        /// </summary>
        /// <value>The value foreground brush.</value>
        public Brush ValueForegroundBrush
        {
            get { return (Brush)GetValue(ValueForegroundBrushProperty); }
            set { SetValue(ValueForegroundBrushProperty, value); }
        }

        public static readonly DependencyProperty ValueForegroundBrushProperty =
            DependencyProperty.Register("ValueForegroundBrush", typeof(Brush), typeof(ValueCellAppearance), null);


        /// <summary>
        /// Gets or sets the width of the current cell border.
        /// </summary>
        /// <value>The width of the current cell border.</value>
        public double CurrentCellBorderWidth
        {
            get { return (double)GetValue(CurrentCellBorderWidthProperty); }
            set { SetValue(CurrentCellBorderWidthProperty, value); }
        }

        public static readonly DependencyProperty CurrentCellBorderWidthProperty =
            DependencyProperty.Register("CurrentCellBorderWidth", typeof(double), typeof(ValueCellAppearance), null);

        /// <summary>
        /// Gets or sets the current cell border brush.
        /// </summary>
        /// <value>The current cell border brush.</value>
        public Brush CurrentCellBorderBrush
        {
            get { return (Brush)GetValue(CurrentCellBorderBrushProperty); }
            set { SetValue(CurrentCellBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty CurrentCellBorderBrushProperty =
            DependencyProperty.Register("CurrentCellBorderBrush", typeof(Brush), typeof(ValueCellAppearance), null);

    }

    public class NestedGridAppearance : Appearence
    {
        /// <summary>
        /// Gets or Sets the Cell borders information for the top left header cell of the Nested grid.
        /// </summary>
        /// <value>The top left cell header cell border.</value>
        public CellBordersInfo TopLeftCellHeaderCellBorder
        {
            get { return (CellBordersInfo)GetValue(TopLeftCellHeaderCellBorderProperty); }
            set { SetValue(TopLeftCellHeaderCellBorderProperty, value); }
        }

        public static readonly DependencyProperty TopLeftCellHeaderCellBorderProperty =
            DependencyProperty.Register("TopLeftCellHeaderCellBorder", typeof(CellBordersInfo), typeof(NestedGridAppearance), null);

        /// <summary>
        /// Gets or Sets the Cell borders information for the header cells 
        /// not but the top left header cell of the Nested grid.
        /// </summary>
        /// <value>The nested header cell border.</value>
        public CellBordersInfo NestedHeaderCellBorder
        {
            get { return (CellBordersInfo)GetValue(NestedHeaderCellBorderProperty); }
            set { SetValue(NestedHeaderCellBorderProperty, value); }
        }

        public static readonly DependencyProperty NestedHeaderCellBorderProperty =
            DependencyProperty.Register("NestedHeaderCellBorder", typeof(CellBordersInfo), typeof(NestedGridAppearance), null);

        /// <summary>
        /// Gets or Sets the Cell borders information for the left 
        /// value cells (i.e. top left column) of the Nested grid.
        /// </summary>
        /// <value>The first header column border.</value>
        public CellBordersInfo FirstHeaderColumnBorder
        {
            get { return (CellBordersInfo)GetValue(FirstHeaderColumnBorderProperty); }
            set { SetValue(FirstHeaderColumnBorderProperty, value); }
        }

        public static readonly DependencyProperty FirstHeaderColumnBorderProperty =
            DependencyProperty.Register("FirstHeaderColumnBorder", typeof(CellBordersInfo), typeof(NestedGridAppearance), null);

        /// <summary>
        /// Gets or Sets the Cell borders information for the value cells not 
        /// but the first left column value cells of the Nested grid.
        /// </summary>
        /// <value>The last header column border.</value>
        public CellBordersInfo LastHeaderColumnBorder
        {
            get { return (CellBordersInfo)GetValue(LastHeaderColumnBorderProperty); }
            set { SetValue(LastHeaderColumnBorderProperty, value); }
        }

        public static readonly DependencyProperty LastHeaderColumnBorderProperty =
            DependencyProperty.Register("LastHeaderColumnBorder", typeof(CellBordersInfo), typeof(NestedGridAppearance), null);
    }
}
