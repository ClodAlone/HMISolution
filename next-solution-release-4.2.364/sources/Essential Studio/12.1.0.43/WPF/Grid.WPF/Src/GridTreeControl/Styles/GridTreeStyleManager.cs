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
using System.Windows.Shapes;

namespace Syncfusion.Windows.Controls.Grid
{
    public class GridTreeStyleManager: DependencyObject
    {
        public GridTreeStyleManager()
        {

        }
        #region RowAppearance
        /// <summary>
        /// RowApperance Use to change the Row Appearance of GridTreeControl like HighlighlightSelectionBackground, HiglightSelectionForeground, RowHoverBackground, RowHoverForeground etc,.
        /// </summary>
        public TreeRowAppearance RowAppearance
        {
            get { return (TreeRowAppearance)GetValue(RowAppearanceProperty); }
            set { SetValue(RowAppearanceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowAppearance.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowAppearanceProperty =
            DependencyProperty.Register("RowAppearance", typeof(TreeRowAppearance), typeof(GridTreeStyleManager), new PropertyMetadata(OnRowAppearanceChanged));

        private static void OnRowAppearanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                (e.NewValue as TreeRowAppearance).StyleManager = d as GridTreeStyleManager;
            }
        }
        #endregion

        #region HeaderAppearance
        /// <summary>
        /// HeaderAppearance use to changed the HeaderAppearance of the GridTreeContol like HeaderBackground,HeaderForeground, HeaderHoverBackground, HeaderHoverForeground etc,.
        /// </summary>
        public TreeHeaderAppearance HeaderAppearance
        {
            get { return (TreeHeaderAppearance)GetValue(HeaderAppearanceProperty); }
            set { SetValue(HeaderAppearanceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderAppearance.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderAppearanceProperty =
            DependencyProperty.Register("HeaderAppearance", typeof(TreeHeaderAppearance), typeof(GridTreeStyleManager), new PropertyMetadata(OnHeaderAppearanceChanged));

        private static void OnHeaderAppearanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                (e.NewValue as TreeHeaderAppearance).StyleManager = d as GridTreeStyleManager;
            }
        }
        #endregion

      
       

        #region ExpanderAppearance

        /// <summary>
        /// ExpanderAppearance used to changed the ExpanderAppearance of the GridTreeControl like ExpanderPlusPath, ExpanderMinusPath etc,.
        /// </summary>
        public TreeExpanderAppearance ExpanderAppearance
        {
            get { return (TreeExpanderAppearance)GetValue(ExpanderAppearanceProperty); }
            set { SetValue(ExpanderAppearanceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExpanderAppearance.  This enables animation, styling, binding, etc...

       
        public static readonly DependencyProperty ExpanderAppearanceProperty =
            DependencyProperty.Register("ExpanderAppearance", typeof(TreeExpanderAppearance), typeof(GridTreeStyleManager), new PropertyMetadata(OnExpanderAppearanceChanged));

        private static void OnExpanderAppearanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                (e.NewValue as TreeExpanderAppearance).StyleManager = d as GridTreeStyleManager;
            }
        }
        #endregion

        #region CellAppearance
        /// <summary>
        /// CellAppearance used to change the Cell Appearance of the GridTreeControl like CellBackground, CellForeground,CellFont.
        /// </summary>
        public TreeCellAppearance CellAppearance
        {
            get { return (TreeCellAppearance)GetValue(CellAppearanceProperty); }
            set { SetValue(CellAppearanceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellAppearance.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellAppearanceProperty =
            DependencyProperty.Register("CellAppearance", typeof(TreeCellAppearance), typeof(GridTreeStyleManager), new PropertyMetadata(OnCellAppearanceChanged));

        private static void OnCellAppearanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                (e.NewValue as TreeCellAppearance).StyleManager = d as GridTreeStyleManager;
            }
        }

        #endregion

    }     

    public class TreeHeaderAppearance : TreeAppearance
    {

        #region SortWidgetBorderHoverBackgroundBrush


        /// <summary>
        /// Gets or sets SortWidgetBorderHover Color
        /// </summary>
        public Brush SortWidgetBorderHoverBackgroundBrush
        {
            get { return (Brush)GetValue(SortWidgetBorderHoverBackgroundBrushProperty); }
            set { SetValue(SortWidgetBorderHoverBackgroundBrushProperty, value); }
        }

       
        public static readonly DependencyProperty SortWidgetBorderHoverBackgroundBrushProperty =
            DependencyProperty.Register("SortWidgetBorderHoverBackgroundBrush", typeof(Brush), typeof(TreeHeaderAppearance), null);
        #endregion

        #region SortWidgetBorderBrush

        /// <summary>
        /// Gets or sets SortWidgetBorder Color
        /// </summary>
        public Brush SortWidgetBorderBrush
        {
            get { return (Brush)GetValue(SortWidgetBorderBrushProperty); }
            set { SetValue(SortWidgetBorderBrushProperty, value); }
        }

       
        public static readonly DependencyProperty SortWidgetBorderBrushProperty =
            DependencyProperty.Register("SortWidgetBorderBrush", typeof(Brush), typeof(TreeHeaderAppearance), null);
        #endregion

        //#region HeaderInnerBorderBrush

        ///// <summary>
        ///// Gets or sets HeaderInnerBorder Color
        ///// </summary>
        //public Brush HeaderInnerBorderBrush
        //{
        //    get { return (Brush)GetValue(HeaderInnerBorderBrushProperty); }
        //    set { SetValue(HeaderInnerBorderBrushProperty, value); }
        //}

       
        //public static readonly DependencyProperty HeaderInnerBorderBrushProperty =
        //    DependencyProperty.Register("HeaderInnerBorderBrush", typeof(Brush), typeof(TreeHeaderAppearance), null);
        //#endregion

        //#region HeaderInnerBorderThickness

        ///// <summary>
        ///// Gets or Sets HeaderInnerBorderThickness
        ///// </summary>
        //public Thickness HeaderInnerBorderThickness
        //{
        //    get { return (Thickness)GetValue(HeaderInnerBorderThicknessProperty); }
        //    set { SetValue(HeaderInnerBorderThicknessProperty, value); }
        //}

       
        //public static readonly DependencyProperty HeaderInnerBorderThicknessProperty =
        //    DependencyProperty.Register("HeaderInnerBorderThickness", typeof(Thickness), typeof(TreeHeaderAppearance), null);
        //#endregion
        #region HeaderBackgroundBrush
       /// <summary>
       /// Gets or sets HeaderBackground Color
       /// </summary>
        public Brush HeaderBackgroundBrush
        {
            get { return (Brush)GetValue(HeaderBackgroundBrushProperty); }
            set { SetValue(HeaderBackgroundBrushProperty, value); }
        }

       
        public static readonly DependencyProperty HeaderBackgroundBrushProperty =
            DependencyProperty.Register("HeaderBackgroundBrush", typeof(Brush), typeof(TreeHeaderAppearance), null);

        #endregion

        #region HeaderForegroundBrush
       
        /// <summary>
        /// Gets or sets HeaderForeground Color
        /// </summary>
        public Brush HeaderForegroundBrush
        {
            get { return (Brush)GetValue(HeaderForegroundBrushProperty); }
            set { SetValue(HeaderForegroundBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderBackgroundBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderForegroundBrushProperty =
            DependencyProperty.Register("HeaderForegroundBrush", typeof(Brush), typeof(TreeHeaderAppearance), null);

        #endregion

        #region HeaderHoverBackgroundBrush
        
        /// <summary>
        /// Sets or gets HeaderHoverBackground color
        /// </summary>
        public Brush HeaderHoverBackgroundBrush
        {
            get { return (Brush)GetValue(HeaderHoverBackgroundBrushProperty); }
            set { SetValue(HeaderHoverBackgroundBrushProperty, value); }
        }

        
        public static readonly DependencyProperty HeaderHoverBackgroundBrushProperty =
            DependencyProperty.Register("HeaderHoverBackgroundBrush", typeof(Brush), typeof(TreeHeaderAppearance), null);

        #endregion

        #region HeaderHoverForegroundBrush
        
        /// <summary>
        /// Gets or sets HeaderhoverForeground Color
        /// </summary>
        public Brush HeaderHoverForegroundBrush
        {
            get { return (Brush)GetValue(HeaderHoverForegroundBrushProperty); }
            set { SetValue(HeaderHoverForegroundBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderHoverForegroundBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderHoverForegroundBrushProperty =
            DependencyProperty.Register("HeaderHoverForegroundBrush", typeof(Brush), typeof(TreeHeaderAppearance), null);

        #endregion

        #region HeaderFont
        /// <summary>
        /// Gets or sets HeaderFont
        /// </summary>

        public GridFontInfo HeaderFont
        {
            get { return (GridFontInfo)GetValue(HeaderFontProperty); }
            set { SetValue(HeaderFontProperty, value); }
        }

        // Using a DependencyProperty as the backing store for  HeaderFont.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderFontProperty =
            DependencyProperty.Register(" HeaderFont", typeof(GridFontInfo), typeof(TreeHeaderAppearance), null);

        #endregion

        #region HeaderTextMargins
        /// <summary>
        /// Gets or Sets HeaderTextMargins
        /// </summary>
        public CellMarginsInfo HeaderTextMargins
        {
            get { return (CellMarginsInfo)GetValue(HeaderTextMarginsProperty); }
            set { SetValue(HeaderTextMarginsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderTextMargins.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTextMarginsProperty =
            DependencyProperty.Register("HeaderTextMargins", typeof(CellMarginsInfo), typeof(TreeHeaderAppearance), null);

        #endregion

        #region SortWidgetBrush
        /// <summary>
        /// Gets or sets SortWidgetBackground Color
        /// </summary>

        public Brush SortWidgetBrush
        {
            get { return (Brush)GetValue(SortWidgetBrushProperty); }
            set { SetValue(SortWidgetBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SortWidgetBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortWidgetBrushProperty =
            DependencyProperty.Register("SortWidgetBrush", typeof(Brush), typeof(TreeHeaderAppearance), null);
        #endregion

    }

    public class TreeRowAppearance : TreeAppearance
    {

        #region RowHoverBackgroundBrush

        public Brush RowHoverBackgroundBrush
        {
            get { return (Brush)GetValue(RowHoverBackgroundBrushProperty); }
            set { SetValue(RowHoverBackgroundBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightSelectionBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowHoverBackgroundBrushProperty =
            DependencyProperty.Register("RowHoverBackgroundBrush", typeof(Brush), typeof(TreeRowAppearance), null);
        #endregion

        #region RowHoverForegroundBrush

        public Brush RowHoverForegroundBrush
        {
            get { return (Brush)GetValue(RowHoverForegroundBrushProperty); }
            set { SetValue(RowHoverForegroundBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightSelectionBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowHoverForegroundBrushProperty =
            DependencyProperty.Register("RowHoverForegroundBrush", typeof(Brush), typeof(TreeRowAppearance), null);
        #endregion
       

        #region HighlightSelectionBackground

        public Brush HighlightSelectionBackground
        {
            get { return (Brush)GetValue(HighlightSelectionBackgroundProperty); }
            set { SetValue(HighlightSelectionBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightSelectionBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightSelectionBackgroundProperty =
            DependencyProperty.Register("HighlightSelectionBackground", typeof(Brush), typeof(TreeRowAppearance), null);
        #endregion


        #region HighlightSelectionForeground

        public Brush HighlightSelectionForeground
        {
            get { return (Brush)GetValue(HighlightSelectionForegroundProperty); }
            set { SetValue(HighlightSelectionForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightSelectionBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightSelectionForegroundProperty =
            DependencyProperty.Register("HighlightSelectionForeground", typeof(Brush), typeof(TreeRowAppearance), null);

        #endregion

        #region CurrentCellSelectionBackground

        public Brush CurrentCellSelectionBackground
        {
            get { return (Brush)GetValue(CurrentCellSelectionBackgroundProperty); }
            set { SetValue(CurrentCellSelectionBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightSelectionBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentCellSelectionBackgroundProperty =
            DependencyProperty.Register("CurrentCellSelectionBackground", typeof(Brush), typeof(TreeRowAppearance), null);

        #endregion

        #region CurrentCellSelectionForeground

        public Brush CurrentCellSelectionForeground
        {
            get { return (Brush)GetValue(CurrentCellSelectionForegroundProperty); }
            set { SetValue(CurrentCellSelectionForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightSelectionBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentCellSelectionForegroundProperty =
            DependencyProperty.Register("CurrentCellSelectionForeground", typeof(Brush), typeof(TreeRowAppearance), null);
        #endregion

        #region CurrentCellBorderBrush

        public Brush CurrentCellBorderBrush
        {
            get { return (Brush)GetValue(CurrentCellBorderBrushProperty); }
            set { SetValue(CurrentCellBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightSelectionBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentCellBorderBrushProperty =
            DependencyProperty.Register("CurrentCellBorderBrush", typeof(Brush), typeof(TreeRowAppearance), null);
        #endregion

        #region CurrentCellBorderWidth

        public double CurrentCellBorderWidth
        {
            get { return (double)GetValue(CurrentCellBorderWidthProperty); }
            set { SetValue(CurrentCellBorderWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightSelectionBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentCellBorderWidthProperty =
            DependencyProperty.Register("CurrentCellBorderWidth", typeof(double), typeof(TreeRowAppearance), null);


        #endregion

        #region NodeHighlightBrush

        //public Brush NodeHighlightBrush
        //{
        //    get { return (Brush)GetValue(NodeHighlightBrushProperty); }
        //    set { SetValue(NodeHighlightBrushProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for HighlightSelectionBackground.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty NodeHighlightBrushProperty =
        //    DependencyProperty.Register("NodeHighlightBrush", typeof(Brush), typeof(TreeRowAppearance), null);


        #endregion

        #region RowHeaderBackgroundBrush
        public Brush RowHeaderBackgroundBrush
        {
            get { return (Brush)GetValue(RowHeaderBackgroundBrushProperty); }
            set { SetValue(RowHeaderBackgroundBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowHeaderBackgroundBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowHeaderBackgroundBrushProperty =
            DependencyProperty.Register("RowHeaderBackgroundBrush", typeof(Brush), typeof(TreeRowAppearance), null);


        #endregion

        #region RowHeaderForegroundBrush
        public Brush RowHeaderForegroundBrush
        {
            get { return (Brush)GetValue(RowHeaderForegroundBrushProperty); }
            set { SetValue(RowHeaderForegroundBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowHeaderForegroundBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowHeaderForegroundBrushProperty =
            DependencyProperty.Register("RowHeaderForegroundBrush", typeof(Brush), typeof(TreeRowAppearance), null);


        #endregion

    }

    public class TreeExpanderAppearance : TreeAppearance
    {

       
        
#if !SILVERLIGHT
        #region ExpanderPlusPath
        public Geometry ExpanderPlusPath
        {
            get { return (Geometry)GetValue(ExpanderPlusPathProperty); }
            set { SetValue(ExpanderPlusPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlusPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpanderPlusPathProperty =
            DependencyProperty.Register("ExpanderPlusPath", typeof(Geometry), typeof(TreeExpanderAppearance), null);

        #endregion
#region ExpanderMinusPath

          public Geometry ExpanderMinusPath
        {
            get { return (Geometry)GetValue(ExpanderMinusPathProperty); }
            set { SetValue(ExpanderMinusPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinusPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpanderMinusPathProperty =
            DependencyProperty.Register("ExpanderMinusPath", typeof(Geometry), typeof(TreeExpanderAppearance), null);

#endregion
#else
        public Path ExpanderPlusPath
        {
            get { return (Path)GetValue(ExpanderPlusPathProperty); }
            set { SetValue(ExpanderPlusPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlusPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpanderPlusPathProperty =
            DependencyProperty.Register("ExpanderPlusPath", typeof(Path), typeof(TreeExpanderAppearance), null);

        public Path ExpanderMinusPath
        {
            get { return (Path)GetValue(ExpanderMinusPathProperty); }
            set { SetValue(ExpanderMinusPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlusPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpanderMinusPathProperty =
            DependencyProperty.Register("ExpanderMinusPath", typeof(Path), typeof(TreeExpanderAppearance), null);
#endif



        public Brush ExpanderBackground
        {
            get { return (Brush)GetValue(ExpanderBackgroundProperty); }
            set { SetValue(ExpanderBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlusMinusButtonBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpanderBackgroundProperty =
            DependencyProperty.Register("ExpanderBackground", typeof(Brush), typeof(TreeExpanderAppearance), null);

        public Brush ExpanderBorderBrush
        {
            get { return (Brush)GetValue(ExpanderBorderBrushProperty); }
            set { SetValue(ExpanderBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlusMinusButtonBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpanderBorderBrushProperty =
            DependencyProperty.Register("ExpanderBorderBrush", typeof(Brush), typeof(TreeExpanderAppearance), null);


        //public Brush ExpanderForeground
        //{
        //    get { return (Brush)GetValue(ExpanderForegroundProperty); }
        //    set { SetValue(ExpanderForegroundProperty, value); }
        //}

        
        //public static readonly DependencyProperty ExpanderForegroundProperty =
        //    DependencyProperty.Register("ExpanderForeground", typeof(Brush), typeof(TreeExpanderAppearance), null);

        public Brush ExpanderExpandedBackground
        {
            get { return (Brush)GetValue(ExpanderExpandedBackgroundProperty); }
            set { SetValue(ExpanderExpandedBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlusMinusExpandedButtonBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpanderExpandedBackgroundProperty =
            DependencyProperty.Register("ExpanderExpandedBackground", typeof(Brush), typeof(TreeExpanderAppearance), null);

        public Brush ExpanderExpandedBorderBrush
        {
            get { return (Brush)GetValue(ExpanderExpandedBorderBrushProperty); }
            set { SetValue(ExpanderExpandedBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlusMinusButtonBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpanderExpandedBorderBrushProperty =
            DependencyProperty.Register("ExpanderExpandedBorderBrush", typeof(Brush), typeof(TreeExpanderAppearance), null);



        public Brush ExpanderHoverBackground
        {
            get { return (Brush)GetValue(ExpanderHoverBackgroundProperty); }
            set { SetValue(ExpanderHoverBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlusMinusHoverButtonBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpanderHoverBackgroundProperty =
            DependencyProperty.Register("ExpanderHoverBackground", typeof(Brush), typeof(TreeExpanderAppearance), null);

        public Brush ExpanderHoverBorderBrush
        {
            get { return (Brush)GetValue(ExpanderHoverBorderBrushProperty); }
            set { SetValue(ExpanderHoverBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlusMinusHoverButtonBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpanderHoverBorderBrushProperty =
            DependencyProperty.Register("ExpanderHoverBorderBrush", typeof(Brush), typeof(TreeExpanderAppearance), null);
    }

    public class TreeCellAppearance : TreeAppearance
    {


        public CellBordersInfo CellBorders
        {
            get { return (CellBordersInfo)GetValue(CellBordersProperty); }
            set { SetValue(CellBordersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueCellBorders.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellBordersProperty =
            DependencyProperty.Register("CellBorders", typeof(CellBordersInfo), typeof(TreeCellAppearance), null);



        public GridFontInfo CellFont
        {
            get { return (GridFontInfo)GetValue(CellFontProperty); }
            set { SetValue(CellFontProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueFont.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellFontProperty =
            DependencyProperty.Register("CellFont", typeof(GridFontInfo), typeof(TreeCellAppearance), null);



        public CellMarginsInfo CellTextMargins
        {
            get { return (CellMarginsInfo)GetValue(CellTextMarginsProperty); }
            set { SetValue(CellTextMarginsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueTextMargins.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellTextMarginsProperty =
            DependencyProperty.Register("CellTextMargins", typeof(CellMarginsInfo), typeof(TreeCellAppearance), null);




        public Brush CellBackgroundBrush
        {
            get { return (Brush)GetValue(CellBackgroundBrushProperty); }
            set { SetValue(CellBackgroundBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueBackgroundBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellBackgroundBrushProperty =
            DependencyProperty.Register("CellBackgroundBrush", typeof(Brush), typeof(TreeCellAppearance), null);



        public Brush CellForegroundBrush
        {
            get { return (Brush)GetValue(CellForegroundBrushProperty); }
            set { SetValue(CellForegroundBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueForegroundBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellForegroundBrushProperty =
            DependencyProperty.Register("CellForegroundBrush", typeof(Brush), typeof(TreeCellAppearance), null);


       
    }

    public class TreeAppearance : DependencyObject
    {
        internal virtual GridTreeStyleManager StyleManager
        {
            get;
            set;
        }
    }
}
