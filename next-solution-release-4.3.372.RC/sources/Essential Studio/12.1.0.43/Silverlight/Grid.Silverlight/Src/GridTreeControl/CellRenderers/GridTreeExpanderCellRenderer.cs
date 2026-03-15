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
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Media.Imaging;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Scroll;


    public class GridTreeExpanderCellModel : GridCellModel<GridTreeExpanderCellRenderer>
    {
        public GridTreeExpanderCellModel()
        {
        }
    }

    public class GridTreeExpanderCellRenderer : GridVirtualizingCellRenderer<GridTreeExpanderCellControl>
    {
        public GridTreeExpanderCellRenderer()
        {
            this.AllowRecycle = false;
            this.SupportsRenderOptimization = false;
            this.IsControlTextShown = true;
            this.IsFocusable = true;
            this.IsEditable = false;           
        }

        public GridTreeExpanderCellModel ExpanderCellModel
        {
            get
            {
                return this.CellModel as GridTreeExpanderCellModel;
            }
        }

        /// <summary>
        /// Gets and Sets the ExpanderGlyphType of CustomPlusPathData of the GridTreeControl.
        /// </summary>
        public Path CustomPlusExpanderGlyphPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and Sets the ExpanderGlyphType of CustomMinusPathData of the GridTreeControl.
        /// </summary>
        public Path CustomMinusExpanderGlyphPath
        {
            get;
            set;
        }

        public override void OnInitializeContent(GridTreeExpanderCellControl uiElement, GridRenderStyleInfo style)
        {
            uiElement.IsInSuspend = true;
            uiElement.IsContentInitialized = true;           
            base.OnInitializeContent(uiElement, style);
            uiElement.Text = style.CellValue != null && style.CellValue.ToString() != string.Empty ? style.CellValue.ToString() : string.Empty;
            uiElement.GridControl = style.GridControl;
            //uiElement.RenderStyle = style;
            uiElement.PlusMinusButtonBackground = visualStyle.HeaderBackgroundBrush;
            uiElement.PlusMinusButtonForeground = visualStyle.HeaderForegroundBrush;
            uiElement.Node = style.Tag as GridTreeNode;
            uiElement.IsInSuspend = false;
            uiElement.ExpandGlyphType = this.ExpandGlyphType;
            uiElement.Foreground = style.Foreground;

            var font = style.ReadOnlyFont;
            var tb = uiElement;

            //Thickness margins = style.TextMargins.ToThickness();
            //margins.Left = Math.Max(0, margins.Left - 4);
            //margins.Right = Math.Max(0, margins.Right - 2);
            //tb.Margin = style.ErrorInfo.AdjustErrorInfoMargin(margins, style.GridControl, style.CellRowColumnIndex);
            tb.FontFamily = font.FontFamily;
            tb.FontSize = font.FontSize;
            tb.FontStretch = font.FontStretch;
            tb.FontWeight = font.FontWeight;
            tb.FontStyle = font.FontStyle;
            tb.Foreground = style.Foreground;
            tb.IsEnabled = !style.ReadOnly;
            //tb.IsReadOnly = style.ReadOnly;
            tb.HorizontalAlignment = style.HorizontalAlignment;
            //tb.Padding = tb.Margin;
            tb.VerticalAlignment = style.VerticalAlignment;
            //switch (style.HorizontalAlignment)
            //{
            //    case HorizontalAlignment.Center:
            //        tb.TextAlignment = TextAlignment.Center;
            //        break;
            //    case HorizontalAlignment.Left:
            //        tb.TextAlignment = TextAlignment.Left;
            //        break;
            //    case HorizontalAlignment.Right:
            //        tb.TextAlignment = TextAlignment.Right;
            //        break;
            //    case HorizontalAlignment.Stretch:
            //        tb.TextAlignment = TextAlignment.Justify;
            //        break;
            //    default:
            //        tb.TextAlignment = TextAlignment.Left;
            //        break;
            //}
            if (this.CustomPlusExpanderGlyphPath != null && this.CustomMinusExpanderGlyphPath != null)
            {
                (uiElement as GridTreeExpanderCellControl).CustomPlusPath = this.CustomPlusExpanderGlyphPath;
                (uiElement as GridTreeExpanderCellControl).CustomMinusPath = this.CustomMinusExpanderGlyphPath;
            }
        }

        protected override void ArrangeUIElement(Syncfusion.Windows.Controls.Cells.ArrangeCellArgs aca, GridTreeExpanderCellControl uiElement, GridRenderStyleInfo style)
        {
            var n = style.Tag as GridTreeNode;
            uiElement.Margin = new Thickness((n.Level + 1) * nodeColumnWidth, 0, 0, 0);
            base.ArrangeUIElement(aca, uiElement, style);
        }

        private bool IsNodeLast(int rowIndex)
        {
            GridTreeControlImpl tree = this.GridControl as GridTreeControlImpl;
            GridTreeNode n = tree.GetNodeAtRowIndex(rowIndex);
            GridTreeNode nextNode = tree.GetNodeAtRowIndex(rowIndex + 1);
            return nextNode == null || n.Level != nextNode.Level ||
                (n.ParentNode != null && n.ParentNode.ChildNodes.IndexOf(n) == n.ParentNode.ChildNodes.Count - 1);
        }

        private double nodeColumnWidth = 10;

        public double NodeColumnWidth
        {
            get { return nodeColumnWidth; }
            set { nodeColumnWidth = value; }
        }

        private IGridDataVisualStyle visualStyle = new GridDataDefaultGridVisualStyle();

        public IGridDataVisualStyle VisualStyle
        {
            get { return visualStyle; }
            set { visualStyle = value; }
        }

        private GridTreeExpandGlyph expandGlyphType = GridTreeExpandGlyph.Triangle;

        /// <summary>
        /// Gets and Sets the ExpanderGlyphType of the GridTreeControl.
        /// </summary>
        public GridTreeExpandGlyph ExpandGlyphType
        {
            get { return expandGlyphType; }
            set
            {
                expandGlyphType = value;                
                switch (expandGlyphType)
                {                    
                    case GridTreeExpandGlyph.PlusMinus:                    
                        NodeColumnWidth = 10;
                        break;
                    case GridTreeExpandGlyph.Triangle:
                        NodeColumnWidth = 10;
                        break;
                    case GridTreeExpandGlyph.Custom:
                        NodeColumnWidth = 10;
                        break;
                    default:
                        this.ExpandGlyphType = GridTreeExpandGlyph.Triangle;
                        break;
                }                
            }             
        }


        #region GLYPH Drawing


        //private Brush expandWidgetBrush = null;
        //private Brush hotExpandWidgetBrush = null;
        //Pen expandWidgetPen;

        ///// <summary>
        ///// Gets or sets the Pen used to draw the borders of the expand glyph.
        ///// </summary>
        //public Pen ExpandWidgetPen
        //{
        //    get
        //    {
        //        if (expandWidgetPen == null)
        //        {
        //            expandWidgetPen = new Pen(new SolidColorBrush(Colors.Blue), .02);
        //            expandWidgetPen.Thickness = .2;
        //        }
        //        return expandWidgetPen;
        //    }
        //    set { expandWidgetPen = value; }
        //}

        ///// <summary>
        ///// Gets or sets the Brush used for drawing the epand glyph when it is under the mouse.
        ///// </summary>
        //public Brush HotExpandWidgetBrush
        //{
        //    get
        //    {
        //        if (hotExpandWidgetBrush == null)
        //        {
        //            hotExpandWidgetBrush = Brushes.AliceBlue;
        //        }
        //        return hotExpandWidgetBrush;
        //    }
        //    set { hotExpandWidgetBrush = value; }
        //}

        ///// <summary>
        ///// Gets or sets the Brush used for drawing of the primary expand glyph drawing.
        ///// </summary>
        //public Brush ExpandWidgetBrush
        //{
        //    get
        //    {
        //        if (expandWidgetBrush == null)
        //        {
        //            expandWidgetBrush = Brushes.MidnightBlue;
        //        }
        //        return expandWidgetBrush;
        //    }
        //    set { expandWidgetBrush = value; }
        //}




        //private GridTreeExpandGlyph expandGlyphType = GridTreeExpandGlyph.Triangle;

        ///// <summary>
        ///// Gets or sets the type of the glyph shown in the expand cell.
        ///// </summary>
        ///// <remarks>
        ///// The default value is a triangle. You can also set a +- glyph, or a +-glyph with tree lines, or
        ///// a custom drawn glyph. The property NodeColumnWidth reserves the required width of your glyph. The
        ///// default value of NodeColumnWidth is 10 which is the setting used for the triangle glyph. For the
        ///// +- glyph, the value of NodeColumnWidth is set to 14. If you want to explicitly provide a particular
        ///// NodeColumnWidth, then you need to explicitly reset its value after you set ExpandGlyphType as setting
        ///// ExpandGlypType also possibly resets NodeColumnWidth.
        ///// </remarks>
        //public GridTreeExpandGlyph ExpandGlyphType
        //{
        //    get { return expandGlyphType; }
        //    set
        //    {
        //        expandGlyphType = value;
        //        switch (expandGlyphType)
        //        {
        //            case GridTreeExpandGlyph.Custom:
        //                NodeColumnWidth = 14;
        //                break;
        //            case GridTreeExpandGlyph.PlusMinus:
        //            case GridTreeExpandGlyph.PlusMinusLines:
        //                NodeColumnWidth = 14;
        //                break;
        //            case GridTreeExpandGlyph.Triangle:
        //                NodeColumnWidth = 10;
        //                break;
        //            case GridTreeExpandGlyph.Themed:
        //                NodeColumnWidth = 19;
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Event used to provide access to the glyph in the expand cell.
        ///// </summary>
        ///// <remarks>
        ///// Setting ExpandGlyphType = Custom will cause this event to be raised when the glyph is drawn.
        ///// </remarks>
        //public event GridTreeGlyphDrawingHandler GlyphDrawing;

        ///// <summary>
        ///// Raises the GlyphDrawing event.
        ///// </summary>
        ///// <param name="e">The event argument.</param>
        //protected virtual void OnGlyphDrawing(GridTreeGlyphDrawingEventArgs e)
        //{
        //    if (GlyphDrawing != null)
        //        GlyphDrawing(this, e);
        //}

        //Point lastPoint;
        //bool isHot = false;
        //bool oldHot = false;
        //RowColumnIndex oldHotCell = RowColumnIndex.Empty;
        //bool hooked = false;
        //private WriteableBitmap bp;

        //private Border GetContent(GridRenderStyleInfo style, Rect rect)
        //{
        //    GridTreeNode n = style.Tag as GridTreeNode;
        //    if (n == null)
        //    {
        //        return null;
        //    }

        //    var border = new Border();
        //    border.Width = rect.Width;
        //    border.Height = rect.Height;
        //    border.Measure(new Size(rect.Width, rect.Height));
        //    VisualContainer.SetWantsMouseInput(border, false);
        //    bool opened = n.Expanded;

        //    //Work on this
        //    //var innerBorder = new Border();
        //    //innerBorder.Background = this.GetBrush(opened);
        //    //border.Child = innerBorder;
        //    border.DataContext = this.visualStyle.PlusMinusButtonBackground;
        //    border.BorderBrush = this.visualStyle.PlusMinusButtonBorderBrush;
        //    border.Tag = this.visualStyle.PlusMinusButtonForeground;
        //    // border.Padding = new Thickness(4, 3, 4, 3);
        //    border.BorderThickness = new Thickness(0);
        //    border.Measure(new Size(rect.Width, rect.Height));
        //    return border;
        //}


        //protected override GridTreeExpanderCellControl CreateUIElement(ArrangeCellArgs aca, GridRenderStyleInfo cellInfo)
        //{
        //    this.DrawGlyph(aca, cellInfo);
        //    return base.CreateUIElement(aca, cellInfo);
        //}

        //public void DrawGlyph(ArrangeCellArgs rca, GridRenderStyleInfo style)
        //{
        //    GridTreeNode n = style.Tag as GridTreeNode;
        //    GridTreeControlImpl tree = GridControl as GridTreeControlImpl;

        //    //WriteableBitmap bp = null;

        //    if (n == null)
        //    {
        //        return;
        //    }


        //    bool opened = n.Expanded;
        //    BitmapImage image = null;
        //    double imageWidth = 0;
        //    double imageHeight = 0;
        //    if (tree.SupportNodeImages)
        //    {
        //        GridTreeRequestNodeImageEventArgs args = new GridTreeRequestNodeImageEventArgs(n.Item, tree);
        //        tree.OnRequestNodeImage(args);
        //        image = args.NodeImage;
        //        //imageWidth = image.Width + 2;
        //        //imageHeight = image.Height;
        //        style.TextMargins.Left += imageWidth;
        //    }

        //    if (tree.SupportNodeImages)
        //    {
        //        style.TextMargins.Left -= imageWidth;
        //    }

        //    bool skipGlyph = (!n.HasChildNodes && ExpandGlyphType != GridTreeExpandGlyph.PlusMinusLines
        //       || (!tree.HideEmptyChildGlyphs && n.ChildNodes.Count == 0 && n.Expanded));

        //    if (!skipGlyph)
        //    {
        //        switch (ExpandGlyphType)
        //        {
        //            case GridTreeExpandGlyph.Triangle:
        //                {
        //                    double xoffSet = (opened ? 3 : 2) + style.TextMargins.Left - NodeColumnWidth;
        //                    int yoffSet = (int)(style.TextMargins.Top + 2); //3;
        //                    int w = 3;
        //                    int h = 5;
        //                    Point pt0 = new Point(rca.CellRect.Left + xoffSet, rca.CellRect.Top + yoffSet);

        //                    PathGeometry pg = new PathGeometry();
        //                    PathFigure pfRight = new PathFigure();
        //                    pfRight.StartPoint = new Point(pt0.X, pt0.Y);
        //                    pfRight.IsClosed = true;
        //                    pfRight.IsFilled = true;

        //                    //PolyLineSegment pls = new PolyLineSegment(new Point[]{
        //                    //                                new Point(pt0.X + 2 * w , pt0.Y + h),
        //                    //                                new Point(pt0.X, pt0.Y + 2 * h)
        //                    //                               }, true);

        //                    PolyLineSegment pls = new PolyLineSegment()
        //                    {
        //                        Points = new PointCollection(){
        //                                                    new Point(pt0.X + 2 * w , pt0.Y + h),
        //                                                    new Point(pt0.X, pt0.Y + 2 * h)
        //                                                   }


        //                    };

        //                    pfRight.Segments.Add(pls);

        //                    pg.Figures.Add(pfRight);
        //                    if (opened)
        //                    {
        //                        pg.Transform = new RotateTransform() { Angle = 45, CenterX = pt0.X + w, CenterY = pt0.Y + h };

        //                    }

        //                    Point pt = new Point(20,20);// DependencyObjectExtensions.GetMousePosition(this.GridControl);
        //                    RowColumnIndex cell = GridControl.PointToCellRowColumnIndex(pt);
        //                    isHot = cell.RowIndex == style.RowIndex && pt.X < style.TextMargins.Left && pt.X > style.TextMargins.Left - NodeColumnWidth;


        //                    var canvas = new Canvas();
        //                    var path = new Path() { StrokeThickness=1,  Stroke= isHot ? HotExpandWidgetBrush : ExpandWidgetBrush, Data=pg };
        //                    canvas.Children.Add(path);
        //                    canvas.Children.Add(new Rectangle() { Fill = ExpandWidgetBrush, Width = 20, Height = 20});

        //                    canvas.Width=25;
        //                    canvas.Width=25;

        //                    canvas.Background = ExpandWidgetBrush;
        //                    var glyph = new WriteableBitmap(25, 25);
        //                    glyph.Render(canvas, null);
        //                    glyph.Invalidate();

        //                    bp = glyph;




        //                    //dc.DrawGeometry(isHot ? HotExpandWidgetBrush : ExpandWidgetBrush, ExpandWidgetPen, pg);
        //                }
        //                break;
        //            case GridTreeExpandGlyph.PlusMinus:
        //            case GridTreeExpandGlyph.PlusMinusLines:
        //                {
        //                    Point pt = DependencyObjectExtensions.GetMousePosition(this.GridControl);
        //                    RowColumnIndex cell = GridControl.PointToCellRowColumnIndex(pt);
        //                    isHot = cell.RowIndex == style.RowIndex && pt.X < style.TextMargins.Left && pt.X > style.TextMargins.Left - NodeColumnWidth - 8;

        //                    double xoffSet = style.TextMargins.Left - NodeColumnWidth - 4;

        //                    double yoffSet = style.TextMargins.Top + 2; //3;
        //                    double w = 5;
        //                    double h = 5;
        //                    double adjustment = 1;

        //                    Point pt0 = new Point(rca.CellRect.Left + xoffSet - 1, rca.CellRect.Top + yoffSet);
        //                    PathGeometry pg = new PathGeometry();
        //                    PathFigure pf = new PathFigure();
        //                    pf.StartPoint = new Point(pt0.X, pt0.Y);
        //                    pf.IsClosed = false;
        //                    pf.IsFilled = false;



        //                    PolyLineSegment pls = new PolyLineSegment()
        //                    {
        //                        Points = new PointCollection()
        //                                                        { new Point(pt0.X + 2 * w, pt0.Y),
        //                                                    new Point(pt0.X + 2 * w, pt0.Y + 2 * h),
        //                                                    new Point(pt0.X, pt0.Y + 2 * h),
        //                                                    new Point(pt0.X, pt0.Y) }
        //                    }; //draw the square




        //                    pf.Segments.Add(pls);

        //                    //pls = new PolyLineSegment(new Point[]{

        //                    //                               }, false);  //move to left side of minus mark


        //                    pls = new PolyLineSegment()
        //                    {
        //                        Points = new PointCollection()
        //                                                      { 
        //                                                          new Point(pt0.X, pt0.Y),
        //                                                        new Point(pt0.X + .5 * w, pt0.Y + h)
        //                                                      }
        //                    };

        //                    pf.Segments.Add(pls);

        //                    //pls = new PolyLineSegment(new Point[]{
        //                    //                                new Point(pt0.X + .5 * w, pt0.Y + h),
        //                    //                                new Point(pt0.X + 1.5 * w , pt0.Y + h)

        //                    //                               }, true); //draw minus mark
        //                    pls = new PolyLineSegment()
        //                    {
        //                        Points = new PointCollection()
        //                    { new Point(pt0.X + .5 * w, pt0.Y + h),
        //                                                    new Point(pt0.X + 1.5 * w , pt0.Y + h)}
        //                    };

        //                    pf.Segments.Add(pls);

        //                    if (!opened)
        //                    {
        //                        //pls = new PolyLineSegment(new Point[]{
        //                        //                            new Point(pt0.X + 1.5 * w , pt0.Y + h),
        //                        //                            new Point(pt0.X + 1 * w , pt0.Y + .5 * h)
        //                        //                           }, false); //move to top of plus mark
                              
        //                        pls = new PolyLineSegment()
        //                        { Points=new PointCollection()
        //                        { new Point(pt0.X + 1.5 * w , pt0.Y + h),
        //                                                    new Point(pt0.X + 1 * w , pt0.Y + .5 * h)} }
        //                        ; 


        //                        pf.Segments.Add(pls);

        //                        //pls = new PolyLineSegment(new Point[]{
        //                        //                            new Point(pt0.X + 1 * w , pt0.Y + .5 * h),
        //                        //                             new Point(pt0.X + 1 * w , pt0.Y + 1.5 * h)
        //                        //                           }, true); //draw to bottom of plus mark

        //                        pls = new PolyLineSegment(){ Points=new PointCollection(){   new Point(pt0.X + 1 * w , pt0.Y + .5 * h),
        //                                                     new Point(pt0.X + 1 * w , pt0.Y + 1.5 * h)}}; //draw to bottom of plus mark

        //                        pf.Segments.Add(pls);
        //                    }

        //                    pg.Figures.Add(pf);

        //                    if (n.HasChildNodes)
        //                    {
        //                        //dc.DrawGeometry(ExpandWidgetBrush, ExpandWidgetPen, pg);
        //                        if (ExpandGlyphType == GridTreeExpandGlyph.PlusMinus)
        //                        {   //if only doing +-, then double draw it to re-inforce the glyph
        //                            //dc.DrawGeometry(ExpandWidgetBrush, ExpandWidgetPen, pg);
        //                        }

        //                        if (isHot)
        //                        {
        //                            pf = new PathFigure();
        //                            pf.StartPoint = new Point(pt0.X, pt0.Y);
        //                            pf.IsClosed = false;
        //                            //pls = new PolyLineSegment(new Point[]{
        //                            //                        new Point(pt0.X + 2 * w , pt0.Y),
        //                            //                        new Point(pt0.X + 2 * w , pt0.Y + 2 * h),
        //                            //                        new Point(pt0.X, pt0.Y + 2 * h),
        //                            //                        new Point(pt0.X, pt0.Y)
        //                            //                       }, true);

        //                            pls = new PolyLineSegment(){ Points=new PointCollection(){ new Point(pt0.X + 2 * w , pt0.Y),
        //                                                    new Point(pt0.X + 2 * w , pt0.Y + 2 * h),
        //                                                    new Point(pt0.X, pt0.Y + 2 * h),
        //                                                    new Point(pt0.X, pt0.Y)}};




        //                            pf.IsFilled = false;
        //                            pf.Segments.Add(pls);
        //                            pg.Figures.Add(pf);


        //                            var canvas = new Canvas();
        //                            var path = new Path() { Data = pg, Fill = HotExpandWidgetBrush };
        //                            canvas.Children.Add(path);


        //                            var glyph = new WriteableBitmap(25, 25);
        //                            glyph.Render(canvas, null);
        //                            glyph.Invalidate();
        //                            glyph = bp;
        //                            //dc.DrawGeometry(HotExpandWidgetBrush, ExpandWidgetPen, pg);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        pg.Figures.Clear();
        //                    }

        //                    if (ExpandGlyphType == GridTreeExpandGlyph.PlusMinusLines)
        //                    {
        //                        //do not try to draw treelines if row is sizing...
        //                        if (GridTreeResizeRowsMouseController.inSizing)
        //                            return;

        //                        GridTreeNode nextNode = tree.GetNodeAtRowIndex(style.RowIndex + 1);
        //                        GridTreeNode previousNode = (style.RowIndex > 1) ? tree.GetNodeAtRowIndex(style.RowIndex - 1) : null;
        //                        bool isLast = IsNodeLast(style.RowIndex);
        //                        bool isFirst = style.RowIndex == 1;
        //                        bool isFirstChild = previousNode != null && previousNode.Level < n.Level;

        //                        pf = new PathFigure();

        //                        pf.StartPoint = new Point(pt0.X + w, (isFirstChild ? rca.CellRect.Top - tree.Model.RowHeights[style.RowIndex - 1] + h + yoffSet
        //                            : rca.CellRect.Top + .5)
        //                            );

        //                        pf.IsClosed = false;
        //                        pf.IsFilled = false;

        //                        if (!isFirst)
        //                        {
        //                            //pls = new PolyLineSegment(new Point[]{
        //                            //                        new Point(pt0.X + w , pt0.Y + (n.HasChildNodes ? 0 : h))
        //                            //                       }, true);

        //                            pls = new PolyLineSegment(){ Points=new PointCollection(){new Point(pt0.X + w , pt0.Y + (n.HasChildNodes ? 0 : h))
        //                                                   }};

        //                            pf.Segments.Add(pls);
        //                        }
        //                        if (!isLast)
        //                        {
        //                            if (n.HasChildNodes)
        //                            {
        //                                //pls = new PolyLineSegment(new Point[]{
        //                                //                    new Point(pt0.X + w , pt0.Y + 2 * h)
        //                                //                   }, false);

        //                                pls = new PolyLineSegment()
        //                                { Points=new PointCollection(){  new Point(pt0.X + w , pt0.Y + 2 * h)}} ;


        //                                pf.Segments.Add(pls);
        //                            }
        //                            //pls = new PolyLineSegment(new Point[]{
        //                            //                        new Point(pt0.X + w , rca.CellRect.Bottom + 1)
        //                            //                       }, true);

        //                            pls = new PolyLineSegment()
        //                            { Points=new PointCollection(){new Point(pt0.X + w , rca.CellRect.Bottom + 1)}};




        //                            pf.Segments.Add(pls);
        //                        }

        //                        double rightArm = 6 + (n.HasChildNodes ? 0 : w);
        //                        double xPos = pt0.X + 2 * w - (n.HasChildNodes ? 0 : w);
        //                        //pls = new PolyLineSegment(new Point[]{
        //                        //                            new Point(xPos , pt0.Y + h)
        //                        //                           }, false);


        //                        pls = new PolyLineSegment()
        //                        { Points=new PointCollection(){new Point(xPos , pt0.Y + h)}};


        //                        pf.Segments.Add(pls);
        //                        //pls = new PolyLineSegment(new Point[]{
        //                        //                            new Point(xPos + rightArm, pt0.Y + h)
        //                        //                           }, true);

        //                        pls = new PolyLineSegment()
        //                        { Points=new PointCollection(){    new Point(xPos + rightArm, pt0.Y + h)}};



        //                        pf.Segments.Add(pls);

        //                        pg.Figures.Add(pf);

        //                        var canvas = new Canvas();
        //                        var path = new Path() { Data = pg, Fill = ExpandWidgetBrush };
        //                        canvas.Children.Add(path);


        //                        var glyph = new WriteableBitmap(25, 25);
        //                        glyph.Render(canvas, null);
        //                        glyph.Invalidate();
        //                        bp = glyph;
                                
        //                       // dc.DrawGeometry(ExpandWidgetBrush, ExpandWidgetPen, pg);

        //                        double xLoc = pt0.X - 1;
        //                        int indent = n.Level;
        //                        bool firstTimeOnly = true;

        //                        while (indent >= 0 && n.ParentNode != null)
        //                        {
        //                            int childPos = n.ParentNode.ChildNodes.IndexOf(n);
        //                            n = n.ParentNode;
        //                            xLoc -= nodeColumnWidth - 8 - adjustment;

        //                            if (n.ParentNode != null)
        //                            {
        //                                int pos = n.ParentNode.ChildNodes.IndexOf(n);
        //                                if (pos != n.ParentNode.ChildNodes.Count - 1)
        //                                {
        //                                    double offset = 0;
        //                                    if (childPos == 0 && firstTimeOnly)
        //                                    {
        //                                        offset = (double.IsNaN(n.NodeHeight) ? GridControl.Model.RowHeights.DefaultLineSize : n.NodeHeight)
        //                                                     - (pt0.Y + 2 * h - rca.CellRect.Top);
        //                                    }
        //                                    double top = rca.CellRect.Top - offset;

        //                                    //pls = new PolyLineSegment(new Point[]{
        //                                    //                        new Point(xLoc , top)
        //                                    //                       }, false);

        //                                    pls = new PolyLineSegment()
        //                                    { Points=new PointCollection(){ new Point(xLoc , top)}}
        //                                  ;


        //                                    pf.Segments.Add(pls);
        //                                    //pls = new PolyLineSegment(new Point[]{
        //                                    //                        new Point(xLoc , rca.CellRect.Bottom + adjustment
        //                                    //                            )
        //                                    //                       }, true);

        //                                    pls = new PolyLineSegment(){ Points=new PointCollection(){ new Point(xLoc , rca.CellRect.Bottom + adjustment
        //                                                                )}};



        //                                    pf.Segments.Add(pls);
        //                                    pg.Figures.Add(pf);
        //                                }
        //                            }
        //                            firstTimeOnly = false;
        //                            indent--;
        //                            xLoc -= w + adjustment;
        //                        }

        //                        var canvas1 = new Canvas();
        //                        var path1 = new Path() { Data = pg, Fill = ExpandWidgetBrush };
        //                        canvas.Children.Add(path1);


        //                        var glyph1 = new WriteableBitmap(25, 25);
        //                        glyph1.Render(canvas, null);
        //                        glyph1.Invalidate();

        //                        bp = glyph;


        //                        //dc.DrawGeometry(ExpandWidgetBrush, ExpandWidgetPen, pg);
        //                    }
        //                }
        //                break;
        //            case GridTreeExpandGlyph.Custom:
        //                {
        //                    double xoffSet = 2 + style.TextMargins.Left - NodeColumnWidth;

        //                    int yoffSet = (int)(style.TextMargins.Top + 2); //4;
        //                    Point pt0 = new Point(rca.CellRect.Left + xoffSet - 1, rca.CellRect.Top + yoffSet);

        //                    PathGeometry pg = new PathGeometry();

        //                    Point pt = DependencyObjectExtensions.GetMousePosition(this.GridControl);
        //                    RowColumnIndex cell = GridControl.PointToCellRowColumnIndex(pt);
        //                    isHot = cell.RowIndex == style.RowIndex && pt.X < style.TextMargins.Left && pt.X > style.TextMargins.Left - NodeColumnWidth - 2;

        //                    GridTreeGlyphDrawingEventArgs e1 = new GridTreeGlyphDrawingEventArgs(pg, pt0, isHot, opened);
        //                    OnGlyphDrawing(e1);
        //                    if (e1.Geometry.Figures.Count > 0)
        //                    {
        //                        var canvas = new Canvas();
        //                        var path = new Path() { Data = e1.Geometry, Fill = isHot ? HotExpandWidgetBrush : ExpandWidgetBrush };
        //                        canvas.Children.Add(path);
        //                        var glyph = new WriteableBitmap(25, 25);
        //                        glyph.Render(canvas, null);
        //                        glyph.Invalidate();
        //                        //dc.DrawGeometry(isHot ? HotExpandWidgetBrush : ExpandWidgetBrush, ExpandWidgetPen, e1.Geometry);
        //                    }
        //                }

        //                break;
        //            case GridTreeExpandGlyph.Themed:
        //                {
        //                    double xoffSet = style.TextMargins.Left - NodeColumnWidth + 7;

        //                    double yoffSet = style.TextMargins.Top + 2; //3;
        //                    double w = style.Font.FontSize / 2;   //5;
        //                    double h = style.Font.FontSize / 2;

        //                    Point pt0 = new Point(rca.CellRect.Left + xoffSet - 1, rca.CellRect.Top + yoffSet);

        //                    Rect textRectangle = new Rect(pt0, new Size(2 * w, 2 * h));

        //                    var border = this.GetContent(style, textRectangle);
        //                    if (border == null)
        //                    {
        //                        return;
        //                    }
        //                    //verify
        //                    //var visualBrush = new VisualBrush(border);
        //                    //dc.DrawRectangle(visualBrush, null, textRectangle);
        //                }
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //    if (image != null)
        //    {
        //        Rect r = rca.CellRect;

        //        r.X = r.X + style.TextMargins.Left;
        //        r.Width = imageWidth - 2;
        //        r.Height = GridControl.Model.RowHeights.DefaultLineSize - style.TextMargins.Top - style.TextMargins.Bottom + 1;
        //        if (imageHeight < r.Height)
        //        {
        //            r.Y += (r.Height - imageHeight) / 2;
        //            r.Height = imageHeight;
        //        }
        //        //dc.DrawImage(image, r);
        //    }            
        //}    
        #endregion
    }
}
