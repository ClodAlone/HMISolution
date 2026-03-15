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
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media.Imaging;
using Syncfusion.Windows.Controls.Scroll;
using System.Windows.Controls;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// CellRenderer that displays indented tree-like node layout.
    /// </summary>
    public class GridTreeExpandCellRenderer : GridCellTextBoxRenderer
    {
        private Brush expandWidgetBrush = null;
        private Brush hotExpandWidgetBrush = null;

        /// <summary>
        /// Deault constructor.
        /// </summary>
        public GridTreeExpandCellRenderer()
        {
            IsControlTextShown = true;
            this.SupportsRenderOptimization = true;
        }

        /// <summary>
        /// Event used to provide access to the glyph in the expand cell.
        /// </summary>
        /// <remarks>
        /// Setting ExpandGlyphType = Custom will cause this event to be raised when the glyph is drawn.
        /// </remarks>
        public event GridTreeGlyphDrawingHandler GlyphDrawing;

        /// <summary>
        /// Raises the GlyphDrawing event.
        /// </summary>
        /// <param name="e">The event argument.</param>
        protected virtual void OnGlyphDrawing(GridTreeGlyphDrawingEventArgs e)
        {
            if (GlyphDrawing != null)
                GlyphDrawing(this, e);
        }

        Point lastPoint;

        void GridControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            isHot = false;
            lastPoint = e.GetPosition(this.GridControl);
            RowColumnIndex cell = this.GridControl.PointToCellRowColumnIndex(e);
            if (cell.ColumnIndex == 1 && cell.RowIndex > 0)
            {
                GridStyleInfo style = GridControl.GetRenderStyleInfo(cell);
                isHot = lastPoint.X < style.TextMargins.Left && lastPoint.X > style.TextMargins.Left - 10;
            }

            if (isHot != oldHot || (oldHotCell.RowIndex != cell.RowIndex && cell.ColumnIndex == 1))
            {
                if (!oldHotCell.IsEmpty)
                    this.GridControl.InvalidateCell(oldHotCell);
                this.GridControl.InvalidateCell(cell);
                oldHotCell = cell;
            }
            oldHot = isHot;
        }


        /// </override>
        protected override void OnRender(DrawingContext dc, RenderCellArgs rca, GridRenderStyleInfo style)
        {
            if (!hooked)
            {
                this.GridControl.PreviewMouseMove += new MouseEventHandler(GridControl_PreviewMouseMove);
                hooked = true;
            }
            GridTreeNode n = style.Tag as GridTreeNode;
            GridTreeControlImpl tree = GridControl as GridTreeControlImpl;

            if (n == null)
            {
                return;
            }


            if (tree.EnableHotRowMarker && rca.ColumnIndex == 1 && tree.rowSpanUnderMouse != null && tree.rowSpanUnderMouse.Top == rca.RowIndex)
            {                
                if (tree.EnableLegacyStyle)
                    dc.DrawRectangle(tree.MarkRowBrush, null, rca.CellRect);
                else if (!n.IsSelected)
                {
                    dc.DrawRectangle(tree.GetVisualStyle(tree.VisualStyle).HoveringRecordCellBackground, null, rca.CellRect);
                }
            }

            bool opened = n.Expanded;
            BitmapImage image = null;
            double imageWidth = 0;
            double imageHeight = 0;
            if (tree.SupportNodeImages)
            {
                GridTreeRequestNodeImageEventArgs args = new GridTreeRequestNodeImageEventArgs(n.Item, GridTreeControlImpl.RequestNodeImageEvent, tree);
                tree.OnRequestNodeImage(args);
                image = args.NodeImage;
                if (image != null)
                {
                    imageWidth = image.Width + 2;
                    imageHeight = image.Height;
                    style.TextMargins.Left += imageWidth;
                }
            }

            base.OnRender(dc, rca, style);
            if (tree.SupportNodeImages && image != null)
            {
                style.TextMargins.Left -= imageWidth;
            }

            if (tree.HideEmptyChildGlyphs && (n.ChildNodes == null || n.ChildNodes.Count == 0)&& tree.parentTreeControl.EnableRenderCheckIfGlyphNeeded)
            {
                GridTreeRequestChildListEventArgs e = new GridTreeRequestChildListEventArgs(n, n.Item, false);
                tree.OnRequestChildList(e);

                if (!n.HasChildNodes)
                {
                    n.HasChildNodes = e.ParentNode.ChildNodes != null && e.ParentNode.ChildNodes.Count > 0;
                }
            }
            
            bool skipGlyph;
            if (tree.ParentTreeControl.EnableRenderCheckIfGlyphNeeded)
            {
                skipGlyph = (!n.HasChildNodes && ExpandGlyphType != GridTreeExpandGlyph.PlusMinusLines
               || (!tree.HideEmptyChildGlyphs && n.ChildNodes.Count == 0 && n.Expanded)
               || (tree.HideEmptyChildGlyphs && (n.ChildNodes == null || n.ChildNodes.Count == 0)));

                if (ExpandGlyphType == GridTreeExpandGlyph.PlusMinusLines)
                {
                    skipGlyph = false;
                }
            }
            else
            {
                skipGlyph = (!n.HasChildNodes && ExpandGlyphType != GridTreeExpandGlyph.PlusMinusLines
                  || (!tree.HideEmptyChildGlyphs && n.ChildNodes.Count == 0 && n.Expanded));
            }
                  

            if (!skipGlyph)
            {
                switch (ExpandGlyphType)
                {
                    case GridTreeExpandGlyph.Triangle:
                        {
                            double xoffSet = (opened ? 3 : 2) + style.TextMargins.Left - NodeColumnWidth;
                            //these changes are regarding to set the ExpanderGlyph in the middle of the row
                            int yoffSet = (int) (style.TextMargins.Top + NodeColumnWidth/2); //3;
                            int w = 3;
                            int h = 5;
                            Point pt0 = new Point(rca.CellRect.Left + xoffSet, rca.CellRect.Top + yoffSet);

                            if (rca.CellRect.Width < (pt0.X + w))
                                w = (w > 0) ? (int)rca.CellRect.Width - (int)pt0.X : 3;

                            if (w > 0)
                            {
                                PathGeometry pg = new PathGeometry();
                                PathFigure pfRight = new PathFigure();
                                pfRight.StartPoint = new Point(pt0.X, pt0.Y);
                                pfRight.IsClosed = true;
                                pfRight.IsFilled = true;

                                PolyLineSegment pls = new PolyLineSegment(new Point[]{
                                                            new Point(pt0.X + 2 * w , pt0.Y + h),
                                                            new Point(pt0.X, pt0.Y + 2 * h)
                                                           }, true);

                                pfRight.Segments.Add(pls);
                                pg.Figures.Add(pfRight);
                                if (opened)
                                {
                                    pg.Transform = new RotateTransform(45, pt0.X + w, pt0.Y + h);
                                }

                                Point pt = Mouse.GetPosition(this.GridControl);

                                RowColumnIndex cell = GridControl.PointToCellRowColumnIndex(pt);
                                isHot = cell.RowIndex == style.RowIndex && pt.X < style.TextMargins.Left && pt.X > style.TextMargins.Left - NodeColumnWidth;

                                dc.DrawGeometry(isHot ? HotExpandWidgetBrush : ExpandWidgetBrush, ExpandWidgetPen, pg);
                            }
                        }
                        break;
                    case GridTreeExpandGlyph.PlusMinus:
                    case GridTreeExpandGlyph.PlusMinusLines:
                        {
                            Point pt = Mouse.GetPosition(this.GridControl);
                            RowColumnIndex cell = GridControl.PointToCellRowColumnIndex(pt);
                            isHot = cell.RowIndex == style.RowIndex && pt.X < style.TextMargins.Left && pt.X > style.TextMargins.Left - NodeColumnWidth - 8;

                            double xoffSet = style.TextMargins.Left - NodeColumnWidth - 4;

                            double yoffSet = style.TextMargins.Top + NodeColumnWidth / 2 -2; //3;
                            double w = 5;
                            double h = 5;
                            double adjustment = 1;

                            Point pt0 = new Point(rca.CellRect.Left + xoffSet - 1, rca.CellRect.Top + yoffSet);                            
                            if (rca.CellRect.Width < pt0.X + w)
                                w = (w > 0) ? (int)rca.CellRect.Width - (int)pt0.X : 5;
                            
                            if (w > 0)
                            {                                
                                PathGeometry pg = new PathGeometry();
                                PathFigure pf = new PathFigure();
                                pf.StartPoint = new Point(pt0.X, pt0.Y);
                                pf.IsClosed = false;
                                pf.IsFilled = false;

                                PolyLineSegment pls = new PolyLineSegment(new Point[]{
                                                            new Point(pt0.X + 2 * w, pt0.Y),
                                                            new Point(pt0.X + 2 * w, pt0.Y + 2 * h),
                                                            new Point(pt0.X, pt0.Y + 2 * h),
                                                            new Point(pt0.X, pt0.Y)
                                                           }, true); //draw the square
                                pf.Segments.Add(pls);

                                pls = new PolyLineSegment(new Point[]{
                                                            new Point(pt0.X, pt0.Y),
                                                            new Point(pt0.X + .5 * w, pt0.Y + h)
                                                           }, false);  //move to left side of minus mark
                                pf.Segments.Add(pls);

                                pls = new PolyLineSegment(new Point[]{
                                                            new Point(pt0.X + .5 * w, pt0.Y + h),
                                                            new Point(pt0.X + 1.5 * w , pt0.Y + h)

                                                           }, true); //draw minus mark
                                pf.Segments.Add(pls);

                                if (!opened)
                                {
                                    pls = new PolyLineSegment(new Point[]{
                                                            new Point(pt0.X + 1.5 * w , pt0.Y + h),
                                                            new Point(pt0.X + 1 * w , pt0.Y + .5 * h)
                                                           }, false); //move to top of plus mark
                                    pf.Segments.Add(pls);

                                    pls = new PolyLineSegment(new Point[]{
                                                            new Point(pt0.X + 1 * w , pt0.Y + .5 * h),
                                                             new Point(pt0.X + 1 * w , pt0.Y + 1.5 * h)
                                                           }, true); //draw to bottom of plus mark
                                    pf.Segments.Add(pls);
                                }

                                pg.Figures.Add(pf);

                                if (n.HasChildNodes)
                                {
                                    dc.DrawGeometry(ExpandWidgetBrush, ExpandWidgetPen, pg);
                                    if (ExpandGlyphType == GridTreeExpandGlyph.PlusMinus)
                                    {   //if only doing +-, then double draw it to re-inforce the glyph
                                        dc.DrawGeometry(ExpandWidgetBrush, ExpandWidgetPen, pg);
                                    }

                                    if (isHot)
                                    {
                                        pf = new PathFigure();
                                        pf.StartPoint = new Point(pt0.X, pt0.Y);
                                        pf.IsClosed = false;
                                        pls = new PolyLineSegment(new Point[]{
                                                            new Point(pt0.X + 2 * w , pt0.Y),
                                                            new Point(pt0.X + 2 * w , pt0.Y + 2 * h),
                                                            new Point(pt0.X, pt0.Y + 2 * h),
                                                            new Point(pt0.X, pt0.Y)
                                                           }, true);

                                        pf.IsFilled = false;
                                        pf.Segments.Add(pls);
                                        pg.Figures.Add(pf);
                                        dc.DrawGeometry(HotExpandWidgetBrush, ExpandWidgetPen, pg);

                                    }
                                }
                                else
                                {
                                    pg.Figures.Clear();
                                }

                                if (ExpandGlyphType == GridTreeExpandGlyph.PlusMinusLines)
                                {
                                    //do not try to draw treelines if row is sizing...
                                    if (GridTreeResizeRowsMouseController.inSizing)
                                        return;

                                    //GridTreeNode nextNode = tree.GetNodeAtRowIndex(style.RowIndex + 1);
                                    GridTreeNode previousNode = (style.RowIndex > 1) ? tree.GetNodeAtRowIndex(style.RowIndex - 1) : null;
                                    bool isLast = IsNodeLast(style.RowIndex);
                                    bool isFirst = style.RowIndex == 1;
                                    bool isFirstChild = previousNode != null && previousNode.Level < n.Level;

                                    pf = new PathFigure();

                                    pf.StartPoint = new Point(pt0.X + w, (isFirstChild ? rca.CellRect.Top - tree.Model.RowHeights[style.RowIndex - 1] + h + yoffSet
                                        : rca.CellRect.Top + .5));

                                    pf.IsClosed = false;
                                    pf.IsFilled = false;

                                    if (!isFirst)
                                    {
                                        pls = new PolyLineSegment(new Point[]{
                                                            new Point(pt0.X + w , pt0.Y + (n.HasChildNodes ? 0 : h))
                                                           }, true);
                                        pf.Segments.Add(pls);
                                    }
                                    if (!isLast)
                                    {
                                        if (n.HasChildNodes)
                                        {
                                            pls = new PolyLineSegment(new Point[]{
                                                            new Point(pt0.X + w , pt0.Y + 2 * h)
                                                           }, false);
                                            pf.Segments.Add(pls);
                                        }
                                        pls = new PolyLineSegment(new Point[]{
                                                            new Point(pt0.X + w , rca.CellRect.Bottom + 1)
                                                           }, true);
                                        pf.Segments.Add(pls);
                                    }

                                    double rightArm = 6 + (n.HasChildNodes ? 0 : w);
                                    double xPos = pt0.X + 2 * w - (n.HasChildNodes ? 0 : w);
                                    pls = new PolyLineSegment(new Point[]{
                                                            new Point(xPos , pt0.Y + h)
                                                           }, false);
                                    pf.Segments.Add(pls);
                                    pls = new PolyLineSegment(new Point[]{
                                                            new Point(xPos + rightArm, pt0.Y + h)
                                                           }, true);
                                    pf.Segments.Add(pls);
                                    pg.Figures.Add(pf);

                                    dc.DrawGeometry(ExpandWidgetBrush, ExpandWidgetPen, pg);

                                    double xLoc = pt0.X - 1;
                                    int indent = n.Level;
                                    bool firstTimeOnly = true;

                                    while (indent >= 0 && n.ParentNode != null)
                                    {
                                        int childPos = n.ParentNode.ChildNodes.IndexOf(n);
                                        n = n.ParentNode;
                                        xLoc -= nodeColumnWidth - 8 - adjustment;

                                        if (n.ParentNode != null)
                                        {
                                            int pos = n.ParentNode.ChildNodes.IndexOf(n);
                                            if (pos != n.ParentNode.ChildNodes.Count - 1)
                                            {
                                                double offset = 0;
                                                if (childPos == 0 && firstTimeOnly)
                                                {
                                                    offset = (double.IsNaN(n.NodeHeight) ? GridControl.Model.RowHeights.DefaultLineSize : n.NodeHeight)
                                                                 - (pt0.Y + 2 * h - rca.CellRect.Top);
                                                }
                                                double top = rca.CellRect.Top - offset;

                                                pls = new PolyLineSegment(new Point[]{
                                                                    new Point(xLoc , top)
                                                                   }, false);
                                                pf.Segments.Add(pls);
                                                pls = new PolyLineSegment(new Point[]{
                                                                    new Point(xLoc , rca.CellRect.Bottom + adjustment
                                                                        )
                                                                   }, true);
                                                pf.Segments.Add(pls);
                                                pg.Figures.Add(pf);
                                            }
                                        }
                                        else if (n.Level == 0 && tree.RootNodes.Count == 0)
                                        {
                                            double offset = 0;
                                            if (childPos == 0 && firstTimeOnly)
                                            {
                                                offset = (double.IsNaN(n.NodeHeight) ? GridControl.Model.RowHeights.DefaultLineSize : n.NodeHeight)
                                                             - (pt0.Y + 2 * h - rca.CellRect.Top);
                                            }
                                            double top = rca.CellRect.Top - offset;

                                            pls = new PolyLineSegment(new Point[]{
                                                                    new Point(xLoc , top)
                                                                   }, false);
                                            pf.Segments.Add(pls);
                                            pls = new PolyLineSegment(new Point[]{
                                                                    new Point(xLoc , rca.CellRect.Bottom + adjustment
                                                                        )
                                                                   }, true);
                                            pf.Segments.Add(pls);
                                            pg.Figures.Add(pf);
                                        }

                                        firstTimeOnly = false;
                                        indent--;
                                        xLoc -= w + adjustment;
                                    }
                                    dc.DrawGeometry(ExpandWidgetBrush, ExpandWidgetPen, pg);
                                }
                            }
                        }
                        break;
                    case GridTreeExpandGlyph.Custom:
                        {
                            double xoffSet = 2 + style.TextMargins.Left - NodeColumnWidth;

                            int yoffSet = (int)(style.TextMargins.Top + 2); //4;
                            Point pt0 = new Point(rca.CellRect.Left + xoffSet - 1, rca.CellRect.Top + yoffSet);

                            PathGeometry pg = new PathGeometry();

                            Point pt = Mouse.GetPosition(this.GridControl);
                            RowColumnIndex cell = GridControl.PointToCellRowColumnIndex(pt);
                            isHot = cell.RowIndex == style.RowIndex && pt.X < style.TextMargins.Left && pt.X > style.TextMargins.Left - NodeColumnWidth - 2;

                            GridTreeGlyphDrawingEventArgs e1 = new GridTreeGlyphDrawingEventArgs(pg, pt0, isHot, opened, dc);
                            OnGlyphDrawing(e1);
                            if (e1.Geometry.Figures.Count > 0)
                            {
                                dc.DrawGeometry(isHot ? HotExpandWidgetBrush : ExpandWidgetBrush, ExpandWidgetPen, e1.Geometry);
                            }
                        }

                        break;
                    case GridTreeExpandGlyph.Themed:
                        {
                            double xoffSet = style.TextMargins.Left - NodeColumnWidth  + 7;

                            double yoffSet = style.TextMargins.Top + NodeColumnWidth/2-5;// +2; //3;
                            double w = style.Font.FontSize/2;   //5;
                            double h = style.Font.FontSize/2;
                           
                            Point pt0 = new Point(rca.CellRect.Left + xoffSet - 1, rca.CellRect.Top + yoffSet);

                             if (rca.CellRect.Width < pt0.X + w)
                            {
                                w = (w > 0) ? (int)rca.CellRect.Width - (int)pt0.X : style.Font.FontSize / 2;
                            }
                             if (w > 0)
                             {

                                 Rect textRectangle = new Rect(pt0, new Size(2 * w, 2 * h));

                                 var border = this.GetContent(style, textRectangle);
                                 if (border == null)
                                 {
                                     return;
                                 }
                                 var visualBrush = new VisualBrush(border);
                                 dc.DrawRectangle(visualBrush, null, textRectangle);
                             }
                        }
                        break;
                    default:
                        break;
                }
            }
            if (image != null)
            {
                Thickness margins = style.TextMargins.ToThickness();
                Rect imageRectangle = this.ClipNodeImage(rca.CellRect, imageWidth, imageHeight, style);
                if (rca.CellRect.Width - (imageRectangle.Left - rca.CellRect.Left) >= 0)
                {
                    if (rca.CellRect.Width < imageRectangle.Width + (imageRectangle.Left - rca.CellRect.Left))
                        imageRectangle.Width = rca.CellRect.Width - (imageRectangle.Left - rca.CellRect.Left);
                    else
                        imageRectangle = this.ClipNodeImage(rca.CellRect, imageWidth, imageHeight, style);

                    dc.DrawImage(image, imageRectangle);
                }
                else
                {
                    imageRectangle.Width = 0;
                    dc.DrawImage(image, imageRectangle);
                }           
            }
            
            
        }

        /// <summary>
        /// This function used to Find the image rectangle Hight, width and positions
        /// </summary>
        /// <param name="cellRect"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        private Rect ClipNodeImage(Rect cellRect, double imageWidth, double imageHeight, GridRenderStyleInfo style)
        {           
            cellRect.X = cellRect.X + style.TextMargins.Left;
            cellRect.Width = imageWidth - 2;
            cellRect.Height = GridControl.Model.RowHeights.DefaultLineSize - style.TextMargins.Top - style.TextMargins.Bottom + 1;
            if (imageHeight < cellRect.Height)
            {
                cellRect.Y += (cellRect.Height - imageHeight) / 2;
                cellRect.Height = imageHeight;
            }            
            return cellRect;
        }
       
        private bool IsNodeLast(int rowIndex)
        {
            GridTreeControlImpl tree = this.GridControl as GridTreeControlImpl;
            GridTreeNode n = tree.GetNodeAtRowIndex(rowIndex);
            GridTreeNode nextNode = tree.GetNodeAtRowIndex(rowIndex + 1);
            return nextNode == null || n.Level != nextNode.Level ||
                (n.ParentNode != null && n.ParentNode.ChildNodes.IndexOf(n) == n.ParentNode.ChildNodes.Count - 1);
        }
        
        bool isHot = false;
        bool oldHot = false;
        RowColumnIndex oldHotCell = RowColumnIndex.Empty;
        bool hooked = false;

        Pen expandWidgetPen;

        /// <summary>
        /// Gets or sets the Pen used to draw the borders of the expand glyph.
        /// </summary>
        public Pen ExpandWidgetPen
        {
            get
            {
                if (expandWidgetPen == null)
                {
                    expandWidgetPen = new Pen(new SolidColorBrush(Colors.Blue), .02);
                    expandWidgetPen.Thickness = .2;
                }
                return expandWidgetPen;
            }
            set { expandWidgetPen = value; }
        }

        /// <summary>
        /// Gets or sets the Brush used for drawing the epand glyph when it is under the mouse.
        /// </summary>
        public Brush HotExpandWidgetBrush
        {
            get
            {
                if (hotExpandWidgetBrush == null)
                {
                    hotExpandWidgetBrush = Brushes.LightBlue;
                }
                return hotExpandWidgetBrush;
            }
            set { hotExpandWidgetBrush = value; }
        }

        /// <summary>
        /// Gets or sets the Brush used for drawing of the primary expand glyph drawing.
        /// </summary>
        public Brush ExpandWidgetBrush
        {
            get
            {
                if (expandWidgetBrush == null)
                {
                    expandWidgetBrush = Brushes.Blue;
                }
                return expandWidgetBrush;
            }
            set { expandWidgetBrush = value; }
        }

        private double nodeColumnWidth = 10; //was 10

        private GridTreeExpandGlyph expandGlyphType = GridTreeExpandGlyph.Triangle;

        /// <summary>
        /// Gets or sets the type of the glyph shown in the expand cell.
        /// </summary>
        /// <remarks>
        /// The default value is a triangle. You can also set a +- glyph, or a +-glyph with tree lines, or
        /// a custom drawn glyph. The property NodeColumnWidth reserves the required width of your glyph. The
        /// default value of NodeColumnWidth is 10 which is the setting used for the triangle glyph. For the
        /// +- glyph, the value of NodeColumnWidth is set to 14. If you want to explicitly provide a particular
        /// NodeColumnWidth, then you need to explicitly reset its value after you set ExpandGlyphType as setting
        /// ExpandGlypType also possibly resets NodeColumnWidth.
        /// </remarks>
        public GridTreeExpandGlyph ExpandGlyphType
        {
            get { return expandGlyphType; }
            set
            {
                expandGlyphType = value;
                switch (expandGlyphType)
                {
                    case GridTreeExpandGlyph.Custom:
                        NodeColumnWidth = 14;
                        break;
                    case GridTreeExpandGlyph.PlusMinus:
                    case GridTreeExpandGlyph.PlusMinusLines:
                        NodeColumnWidth = 14;
                        break;
                    case GridTreeExpandGlyph.Triangle:
                        NodeColumnWidth = 10;
                        break;
                    case GridTreeExpandGlyph.Themed:
                        NodeColumnWidth = 19;
                        break;
                    default:
                        break;
                }
            }
        }

        #region Theme Brushes

        private Brush currentBrush = GridDataResourceWrapper.Plus;

        private VisualStyle? currentVisualStyle = null;

        private bool? currentValue = null;
        
        private IGridDataVisualStyle gridVisualStyle = null;

        public IGridDataVisualStyle TreeGridVisualStyle
        {
            get
            {
                if (this.currentVisualStyle != null)
                {
                    switch (this.currentVisualStyle.Value)
                    {
                        case VisualStyle.Default:
                            gridVisualStyle = new GridDataDefaultGridVisualStyle();
                            break;
                        case VisualStyle.Office2007Blue:
                            gridVisualStyle = new GridDataBlueVisualStyle();
                            break;
                        case VisualStyle.Office2007Black:
                            gridVisualStyle = new GridDataBlackVisualStyle();
                            break;
                        case VisualStyle.Office2007Silver:
                            gridVisualStyle = new GridDataSilverVisualStyle();
                            break;
                        case VisualStyle.Office2003:
                            gridVisualStyle = new GridDataSyncBlueVisualStyle();
                            break;
                        case VisualStyle.Blend:
                            gridVisualStyle = new GridDataBlendVisualStyle();
                            break;
                        default:
                            gridVisualStyle = new GridDataDefaultGridVisualStyle();
                            break;
                    }
                }

                return gridVisualStyle;
               // return null;
            }
        }

        private Brush GetBrush(bool opened)
        {
            bool isChanged = false;
            this.UpdateCurrentVisualStyle(out isChanged);

            if (!this.currentValue.HasValue)
            {
                this.currentValue = opened;
                isChanged = true;
            }
            else
            {
                if (this.currentValue.Value != opened)
                {
                    this.currentValue = opened;
                    isChanged = true;
                }
            }

         //   if (isChanged)
            {
                if (!opened)
                {
                    this.currentBrush = GridDataResourceWrapper.Plus;
                }
                else
                {
                    this.currentBrush = GridDataResourceWrapper.Minus;
                }
            }

            return this.currentBrush;
        }

        private void UpdateCurrentVisualStyle(out bool isChanged)
        {
            GridTreeControlImpl tree = GridControl as GridTreeControlImpl;

            if (!this.currentVisualStyle.HasValue)
            {
                this.currentVisualStyle = tree.VisualStyle;
                isChanged = true;
                return;
            }
            else
            {
                if (this.currentVisualStyle.Value != tree.VisualStyle)
                {
                    this.currentVisualStyle = tree.VisualStyle;
                    isChanged = true;
                    return;
                }
            }

            isChanged = false;
        }

        private Border GetContent(GridRenderStyleInfo style, Rect rect)
        {
            GridTreeNode n = style.Tag as GridTreeNode;
            if (n == null)
            {
                return null;
            }

            var border = new Border();
            border.Width = rect.Width;
            border.Height = rect.Height;
            border.Measure(new Size(rect.Width, rect.Height));
            VisualContainer.SetWantsMouseInput(border, false);
            bool opened = n.Expanded;
            var innerBorder = new Border();
            innerBorder.Background = this.GetBrush(opened);
            border.Child = innerBorder;
            border.DataContext = this.TreeGridVisualStyle.PlusMinusButtonBackground;
            border.BorderBrush = this.TreeGridVisualStyle.PlusMinusButtonBorderBrush;
            border.Tag = this.TreeGridVisualStyle.PlusMinusButtonForeground;
           // border.Padding = new Thickness(4, 3, 4, 3);
            border.BorderThickness = new Thickness(0);
            border.Measure(new Size(rect.Width, rect.Height));
            return border;
        }
        #endregion

        /// <summary>
        /// Gets or sets the width of the expand glyph area.
        /// </summary>
        /// <remarks>
        /// When the expand glyph is the triangle, this value is set to 10. When the expand
        /// glyph is a +-, this value is set to 14. If you want to explicitly provide a particular
        /// NodeColumnWidth, then you need to explicitly reset its value after you set ExpandGlyphType as setting
        /// ExpandGlypType also possibly resets NodeColumnWidth.
        /// </remarks>
        public double NodeColumnWidth
        {
            get { return nodeColumnWidth; }
            set { nodeColumnWidth = value; }
        }
    }
}
