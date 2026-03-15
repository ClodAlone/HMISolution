#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#if !SILVERLIGHT

using System;
using System.Drawing;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
using Syncfusion.DocIO.Rendering;
using System.Collections.Generic;
namespace Syncfusion.Layouting
{
    /// <summary>
    /// Represents the layout context for layouting text.
    /// </summary>
    internal class LeafLayoutContext : LayoutContext
    {
        #region Fields
        //Specifies whether the client area has been updated based on text wrap
        private bool m_isXPositionUpdated;
        private bool m_isYPositionUpdated;
        private bool m_isWrapText;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LCLeaf"/> class.
        /// </summary>
        /// <param name="strWidget">The STR widget.</param>
        /// <param name="lcOperator">The lc operator.</param>
        public LeafLayoutContext(ILeafWidget strWidget, ILCOperator lcOperator)
            : base(strWidget, lcOperator)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the leaf widget.
        /// </summary>
        /// <value>The leaf widget.</value>
        public ILeafWidget LeafWidget
        {
            get
            {
                return m_widget as ILeafWidget;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Layouts the specified widget.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns></returns>
        public override LayoutedWidget Layout(RectangleF rect)
        {
            CreateLayoutArea(rect);
            float clientActiveAreawidth = rect.Width;
            ILeafWidget leafWidget = LeafWidget;
            SizeF size = leafWidget.Measure(DrawingContext);
			//If the text range is in empty paragraph then update width as zero
            if (leafWidget is WTextRange && (leafWidget as WTextRange).Owner == null)
                size.Width = 0;
            TabsLayoutInfo tabsInfo = leafWidget.LayoutInfo as TabsLayoutInfo;

            if (tabsInfo != null)
            {
                UpdateTabWidth(ref rect, ref size);
                if (((m_lcOperator as Layouter).UpdatingTOC
                    || (m_lcOperator as Layouter).UpdatingPageFields) && (leafWidget is WTextRange))
                    (leafWidget as WTextRange).Text = "\t";
                // If tab position is beyond the right margin and Document compatibility mode is not Word 2013
                if (tabsInfo.m_currTab.Position > ClientLayoutAreaRight && !m_isTabStopBeyondRightMarginExists && (leafWidget is WTextRange)
                    && !(tabsInfo.m_currTab.Justification == TabJustification.Decimal && IsLeafWidgetIsInCell(LeafWidget as WTextRange)) 
                    && ((leafWidget as WTextRange).Document.Settings.CompatibilityMode!= CompatibilityMode.Word2013))
                    m_isTabStopBeyondRightMarginExists = true;
            }
            bool IsAdjust = false;
            // Get Current Textrange
            WTextRange textRange = GetCurrTextRange();
            AdjustClientAreaBasedOnTextWrap(leafWidget, size, ref rect);
            if (textRange != null)
            {
                m_ltWidget = WordLayout(rect, size, textRange);
                if (m_ltWidget != null)
                {
                    // Update layoute state as wrap text to layout the text based on text wrap.
                    if (m_ltState == LayoutState.Splitted && m_isWrapText && (m_lcOperator as Layouter).FloatingTableBottom == float.MinValue)
                        m_ltState = LayoutState.WrapText;
                    return m_ltWidget;
                }
                //Update width of the Footnote separator
                if (textRange.Text == ((char)3).ToString())
                {
                    if (rect.Width < 144)
                        size.Width = rect.Width;
                    else
                        size.Width = 144;
                }
                //Update width of the Footnote Continuation Separator
                else if (textRange.Text == ((char)4).ToString())
                    size.Width = rect.Width;
            }
            float tabBoundsRight = rect.X + size.Width;
            if (!IsLeafWidgetNeedToBeSplitted(size, clientActiveAreawidth) || (tabsInfo != null && (tabBoundsRight <= (tabsInfo.m_currTab.Position + tabsInfo.PageMarginLeft)) && size.Height <= m_layoutArea.ClientArea.Height))
            {
                FitWidget(size, leafWidget,false);

                ParagraphLayoutInfo paragraphInfo = LayoutInfo as ParagraphLayoutInfo;
                bool isPageBreak = (paragraphInfo != null) ? paragraphInfo.IsPageBreak : false;

                if (!isPageBreak)
                {
                    m_ltState = LayoutState.Fitted;
                }
                else
                {
                    m_ltState = LayoutState.Breaked;
                }

                if (LayoutInfo.IsPageBreakItem)
                {
                    m_ltState = LayoutState.Fitted;
                }
            }
            else
            {
                m_isTabStopBeyondRightMarginExists = false;
                ISplitLeafWidget splitLeafWidget = LeafWidget as ISplitLeafWidget;
                bool isClipped = (LeafWidget is WTextRange || LeafWidget is SplitStringWidget) && LeafWidget.LayoutInfo.IsClipped;
                if (splitLeafWidget != null
                    && ((size.Height <= m_layoutArea.ClientArea.Height)
                    || (textRange != null
                    && textRange.OwnerParagraph != null
                    && textRange.OwnerParagraph.ParagraphFormat.LineSpacingRule == LineSpacingRule.Exactly
                    && Math.Abs(textRange.OwnerParagraph.ParagraphFormat.LineSpacing) <= m_layoutArea.ClientActiveArea.Height)
                    || (isClipped && (m_layoutArea.ClientArea.Height > 0
                    || (IsInFrame(textRange.OwnerParagraph)) && m_layoutArea.ClientArea.Height >= 0))))
                {
                    SplitUpWidget(splitLeafWidget, clientActiveAreawidth);
                }
                else
                {
                    m_ltState = LayoutState.NotFitted;
                    m_bIsVerticalNotFitted = (size.Height > m_layoutArea.ClientArea.Height);
                }
            }
            // Update layout state as wrap text to layout the text based on text wrap.
            if (textRange != null && m_ltState == LayoutState.Splitted && m_isWrapText)
            {
                float minWidth = size.Width;
                if (SplittedWidget is SplitStringWidget)
                {
                    minWidth = DrawingContext.MeasureTextRange((SplittedWidget as SplitStringWidget).RealStringWidget as WTextRange, (SplittedWidget as SplitStringWidget).SplittedText.Split(' ')[0]).Width;
                }
                else if ((SplittedWidget is WTextRange) && (SplittedWidget.LayoutInfo as TabsLayoutInfo) != null)
                    minWidth = size.Width;
                if (m_ltWidget.Bounds.Right + minWidth + (GetOwnerParagraph().m_layoutInfo as ParagraphLayoutInfo).Margins.Right < (m_lcOperator as Layouter).ClientLayoutArea.Right)
                    m_ltState = LayoutState.WrapText;
            }

#if DEBUG_LAYOUTING
      /* Debug code */ DBG_CommitChildContext( this );
#endif
            DoLayoutAfter();
            //Layout Shape TextBody
            if (m_ltWidget != null && m_ltWidget.Widget is Shape)
            {
                LayoutShapeTextBody();
            }
            return m_ltWidget;
        }
        /// <summary>
        /// 
        /// </summary>
        private void LayoutShapeTextBody()
        {
            Shape shape = (m_ltWidget.Widget as Shape);
            RectangleF layoutRect = GetBoundsToLayoutShapeTextBody(shape, m_ltWidget.Bounds);
            UpdateShapeBoundsToLayoutTextBody(ref layoutRect, shape.TextFrame.InternalMargin);
            LayoutContext context = Create(shape.TextBody, m_lcOperator, layoutRect.Width);
            if (shape.m_layoutInfo.IsVerticalText)
                layoutRect = new RectangleF(layoutRect.X, layoutRect.Y, layoutRect.Height, layoutRect.Width);
            (m_ltWidget.Widget.LayoutInfo as ShapeLayoutInfo).TextLayoutingBounds = layoutRect;
            LayoutedWidget ltWidget = context.Layout(layoutRect);
            UpdateLayoutedWidgetBasedOnVerticalAlignment(layoutRect, ltWidget, shape.TextFrame.TextVerticalAlignment);
            m_ltWidget.ChildWidgets.Add(ltWidget);
            ltWidget.Owner = m_ltWidget;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ltWidget"></param>
        private void UpdateLayoutedWidgetBasedOnVerticalAlignment(RectangleF bounds, LayoutedWidget ltWidget, VerticalAlignment textVerticalAlignment)
        {
            float displacement = 0;
            switch (textVerticalAlignment)
            {
                case VerticalAlignment.Middle:
                    displacement = (bounds.Height - ltWidget.Bounds.Height) / 2;
                    break;
                case VerticalAlignment.Bottom:
                    displacement = bounds.Height - ltWidget.Bounds.Height;
                    break;
            }
            if (displacement > 0)
                ltWidget.ShiftLocation(0, displacement, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="layoutRect"></param>
        /// <param name="textBox"></param>
        private void UpdateShapeBoundsToLayoutTextBody(ref RectangleF layoutRect, InternalMargin internalMargin)
        {
            //Update Actual TextLayout Bounds
            layoutRect.Height -= layoutRect.Y;
            layoutRect.Y += m_ltWidget.Bounds.Y;
            layoutRect.Width -= layoutRect.X;
            layoutRect.X += m_ltWidget.Bounds.X;
            //Update bounds based on internal margin and linewidth
            layoutRect.X += internalMargin.Left + (m_ltWidget.Widget as Shape).LineFormat.Weight;
            layoutRect.Y += internalMargin.Top + (m_ltWidget.Widget as Shape).LineFormat.Weight;
            layoutRect.Width -= (internalMargin.Left + internalMargin.Right + ((m_ltWidget.Widget as Shape).LineFormat.Weight * 2));
            layoutRect.Height -= (internalMargin.Top + internalMargin.Bottom + ((m_ltWidget.Widget as Shape).LineFormat.Weight * 2));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private RectangleF GetBoundsToLayoutShapeTextBody(Shape shape, RectangleF bounds)
        {
            ShapePath shapePath = new ShapePath(bounds, shape.ShapeGuide);
            Dictionary<string, float> formulaValues = shapePath.ParseShapeFormula(shape.AutoShapeType);
            switch (shape.AutoShapeType)
            {
                case AutoShapeType.Arc:
                case AutoShapeType.BlockArc:
                case AutoShapeType.Chord:
                case AutoShapeType.CircularArrow:
                case AutoShapeType.Cloud:
                case AutoShapeType.CloudCallout:
                case AutoShapeType.DoubleWave:
                case AutoShapeType.Donut:
                case AutoShapeType.Oval:
                case AutoShapeType.FlowChartConnector:
                case AutoShapeType.FlowChartSequentialAccessStorage:
                    return new RectangleF(formulaValues["il"], formulaValues["it"], formulaValues["ir"], formulaValues["ib"]);
                case AutoShapeType.DoubleBrace:
                case AutoShapeType.DoubleBracket:
                case AutoShapeType.FlowChartAlternateProcess:
                    return new RectangleF(formulaValues["il"], formulaValues["il"], formulaValues["ir"], formulaValues["ib"]);
                case AutoShapeType.BentUpArrow:
                    return new RectangleF(0, formulaValues["y2"], formulaValues["x4"], bounds.Height);
                case AutoShapeType.Bevel:
                    return new RectangleF(formulaValues["x1"], formulaValues["x1"], formulaValues["x2"], formulaValues["y2"]);
                case AutoShapeType.Can:
                    return new RectangleF(0, formulaValues["y2"], bounds.Width, formulaValues["y3"]);
                case AutoShapeType.L_Shape:
                    return new RectangleF(0, formulaValues["it"], formulaValues["ir"], bounds.Height);
                case AutoShapeType.FlowChartDelay:
                    return new RectangleF(0, formulaValues["it"], formulaValues["ir"], formulaValues["ib"]);
                case AutoShapeType.Cube:
                    return new RectangleF(0, formulaValues["y1"], formulaValues["x4"], bounds.Height);
                case AutoShapeType.Decagon:
                    return new RectangleF(formulaValues["x1"], formulaValues["y2"], formulaValues["x4"], formulaValues["y3"]);
                case AutoShapeType.DiagonalStripe:
                    return new RectangleF(0, 0, formulaValues["x3"], formulaValues["y3"]);
                case AutoShapeType.Diamond:
                case AutoShapeType.FlowChartCollate:
                case AutoShapeType.FlowChartDecision:
                    return new RectangleF((bounds.Width / 4), (bounds.Height / 4), formulaValues["ir"], formulaValues["ib"]);
                case AutoShapeType.FlowChartDisplay:
                    return new RectangleF((bounds.Width / 6), 0, formulaValues["x2"], bounds.Height);
                case AutoShapeType.Dodecagon:
                    return new RectangleF(formulaValues["x1"], formulaValues["y1"], formulaValues["x4"], formulaValues["y4"]);
                case AutoShapeType.DownArrow:
                    return new RectangleF(formulaValues["x1"], 0, formulaValues["x2"], formulaValues["y2"]);
                case AutoShapeType.DownArrowCallout:
                    return new RectangleF(0, 0, bounds.Width, formulaValues["y2"]);
                case AutoShapeType.FlowChartDocument:
                    return new RectangleF(0, 0, bounds.Width, formulaValues["y1"]);
                case AutoShapeType.FlowChartExtract:
                    return new RectangleF((bounds.Width / 4), (bounds.Height / 2), formulaValues["x2"], bounds.Height);
                case AutoShapeType.FlowChartData:
                    return new RectangleF((bounds.Width / 5), 0, formulaValues["x5"], bounds.Height);
                case AutoShapeType.FlowChartInternalStorage:
                    return new RectangleF((bounds.Width / 8), (bounds.Height / 8), bounds.Width, bounds.Height);
                case AutoShapeType.FlowChartMagneticDisk:
                    return new RectangleF(0, (bounds.Height / 3), bounds.Width, formulaValues["y3"]);
                case AutoShapeType.FlowChartDirectAccessStorage:
                    return new RectangleF((bounds.Width / 6), 0, formulaValues["x2"], bounds.Height);
                case AutoShapeType.FlowChartManualInput:
                case AutoShapeType.FlowChartCard:
                    return new RectangleF(0, (bounds.Height / 5), bounds.Width, bounds.Height);
                case AutoShapeType.FlowChartManualOperation:
                    return new RectangleF((bounds.Width / 5), 0, formulaValues["x3"], bounds.Height);
                case AutoShapeType.FlowChartMerge:
                    return new RectangleF((bounds.Width / 4), 0, formulaValues["x2"], +(bounds.Height / 2));
                case AutoShapeType.FlowChartMultiDocument:
                    return new RectangleF(0, formulaValues["y2"], formulaValues["x5"], formulaValues["y8"]);
                //case AutoShapeType.FlowChartOfflineStorage:
                //return new RectangleF((bounds.Width / 4), 0, formulaValues["x4"], +(bounds.Height / 2)); 
                case AutoShapeType.FlowChartOffPageConnector:
                    return new RectangleF(0, 0, bounds.Width, formulaValues["y1"]);
                case AutoShapeType.FlowChartStoredData:
                    return new RectangleF((bounds.Width / 6), 0, formulaValues["x2"], bounds.Height);
                case AutoShapeType.FlowChartOr:
                case AutoShapeType.FlowChartSummingJunction:
                case AutoShapeType.FlowChartTerminator:
                case AutoShapeType.Hexagon:
                //case AutoShapeType.leftCircularArrow:
                //case AutoShapeType.leftRightCircularArrow:  
                case AutoShapeType.NoSymbol:
                case AutoShapeType.Parallelogram:
                case AutoShapeType.Cross:
                case AutoShapeType.SmileyFace:
                case AutoShapeType.SnipSameSideCornerRectangle:
                case AutoShapeType.Star16Point:
                case AutoShapeType.Star24Point:
                case AutoShapeType.Teardrop:
                case AutoShapeType.Star32Point:
                case AutoShapeType.Wave:
                case AutoShapeType.OvalCallout:
                    return new RectangleF(formulaValues["il"], formulaValues["it"], formulaValues["ir"], formulaValues["ib"]);
                case AutoShapeType.FlowChartPredefinedProcess:
                    return new RectangleF((bounds.Width / 8), 0, formulaValues["x2"], bounds.Height);
                case AutoShapeType.FlowChartPreparation:
                    return new RectangleF((bounds.Width / 5), 0, formulaValues["x2"], bounds.Height);
                case AutoShapeType.FlowChartProcess:
                //case AutoShapeType.funnel:  
                case AutoShapeType.StraightConnector:
                //case AutoShapeType.swooshArrow:  
                case AutoShapeType.UTurnArrow:
                case AutoShapeType.RectangularCallout:
                    return new RectangleF(0, 0, bounds.Width, bounds.Height);
                case AutoShapeType.FlowChartPunchedTape:
                    return new RectangleF(0, (bounds.Height / 5), bounds.Width, formulaValues["ib"]);
                case AutoShapeType.FlowChartSort:
                    return new RectangleF((bounds.Width / 4), (bounds.Height / 4), formulaValues["ir"], formulaValues["ib"]);
                case AutoShapeType.FoldedCorner:
                    return new RectangleF(0, 0, bounds.Width, formulaValues["y2"]);
                case AutoShapeType.Frame:
                    return new RectangleF(formulaValues["x1"], formulaValues["x1"], formulaValues["x4"], formulaValues["y4"]);
                //case AutoShapeType.gear6:  
                //    return new RectangleF(0+formulaValues["xD5"],0+formulaValues["yA1"],formulaValues["xA1"],formulaValues["yD2"]); 
                //case AutoShapeType.gear9:  
                //    return new RectangleF(0+formulaValues["xA8"],0+formulaValues["yD1"],formulaValues["xD1"],formulaValues["yD3"]); 
                case AutoShapeType.Heart:
                    return new RectangleF(formulaValues["il"], (bounds.Height / 4), formulaValues["ir"], formulaValues["ib"]);
                case AutoShapeType.Heptagon:
                    return new RectangleF(formulaValues["x2"], formulaValues["y1"], formulaValues["x5"], formulaValues["ib"]);
                case AutoShapeType.Pentagon:
                case AutoShapeType.RoundSingleCornerRectangle:
                    return new RectangleF(0, 0, formulaValues["ir"], bounds.Height);
                case AutoShapeType.HorizontalScroll:
                    return new RectangleF(formulaValues["ch"], formulaValues["ch"], formulaValues["x4"], formulaValues["y6"]);
                case AutoShapeType.Explosion1:
                    return new RectangleF(formulaValues["x5"], formulaValues["y3"], formulaValues["x21"], formulaValues["y9"]);
                case AutoShapeType.Explosion2:
                    return new RectangleF(formulaValues["x5"], formulaValues["y3"], formulaValues["x19"], formulaValues["y17"]);
                case AutoShapeType.LeftArrow:
                    return new RectangleF(formulaValues["x1"], formulaValues["y1"], bounds.Width, formulaValues["y2"]);
                case AutoShapeType.LeftArrowCallout:
                    return new RectangleF(formulaValues["x2"], 0, bounds.Width, bounds.Height);
                case AutoShapeType.LeftBrace:
                case AutoShapeType.LeftBracket:
                    return new RectangleF(formulaValues["il"], formulaValues["it"], bounds.Width, formulaValues["ib"]);
                case AutoShapeType.LeftRightArrow:
                    return new RectangleF(formulaValues["x1"], formulaValues["y1"], formulaValues["x4"], formulaValues["y2"]);
                case AutoShapeType.LeftRightArrowCallout:
                    return new RectangleF(formulaValues["x2"], 0, formulaValues["x3"], bounds.Height);
                //case AutoShapeType.LeftRightRibbon:  
                //    return new RectangleF(0+formulaValues["x1"],0+formulaValues["ly1"],formulaValues["x4"],formulaValues["ry4"]); 
                case AutoShapeType.LeftRightUpArrow:
                    return new RectangleF(formulaValues["il"], formulaValues["y3"], formulaValues["ir"], formulaValues["y5"]);
                case AutoShapeType.LeftUpArrow:
                    return new RectangleF(formulaValues["il"], formulaValues["y3"], formulaValues["x4"], formulaValues["y5"]);
                case AutoShapeType.LightningBolt:
                    return new RectangleF(formulaValues["x4"], formulaValues["y4"], formulaValues["x9"], formulaValues["y10"]);
                case AutoShapeType.MathDivision:
                    return new RectangleF(formulaValues["x1"], formulaValues["y3"], formulaValues["x3"], formulaValues["y4"]);
                case AutoShapeType.MathEqual:
                case AutoShapeType.UpArrow:
                    return new RectangleF(formulaValues["x1"], formulaValues["y1"], formulaValues["x2"], bounds.Height);
                case AutoShapeType.UpDownArrow:
                    return new RectangleF(formulaValues["x1"], formulaValues["y1"], formulaValues["x2"], formulaValues["y4"]);
                case AutoShapeType.MathMinus:
                    return new RectangleF(formulaValues["x1"], formulaValues["y1"], formulaValues["x2"], formulaValues["y2"]);
                case AutoShapeType.MathMultiply:
                    return new RectangleF(formulaValues["xA"], formulaValues["yB"], formulaValues["xE"], formulaValues["yH"]);
                case AutoShapeType.MathNotEqual:
                    return new RectangleF(formulaValues["x1"], formulaValues["y1"], formulaValues["x8"], formulaValues["y4"]);
                case AutoShapeType.MathPlus:
                    return new RectangleF(formulaValues["x1"], formulaValues["y2"], formulaValues["x4"], formulaValues["y3"]);
                case AutoShapeType.Moon:
                    return new RectangleF(formulaValues["g12w"], formulaValues["g15h"], formulaValues["g0w"], formulaValues["g16h"]);
                //.nnonIsoscelesTrapezoid:
                case AutoShapeType.Trapezoid:
                    return new RectangleF(formulaValues["il"], formulaValues["it"], formulaValues["ir"], bounds.Height);
                case AutoShapeType.NotchedRightArrow:
                    return new RectangleF(formulaValues["x1"], formulaValues["y1"], formulaValues["x3"], formulaValues["y2"]);
                case AutoShapeType.Octagon:
                case AutoShapeType.RoundedRectangularCallout:
                case AutoShapeType.SnipDiagonalCornerRectangle:
                case AutoShapeType.RoundedRectangle:
                case AutoShapeType.Plaque:
                    return new RectangleF(formulaValues["il"], formulaValues["il"], formulaValues["ir"], formulaValues["ib"]);
                case AutoShapeType.RegularPentagon:
                    return new RectangleF(formulaValues["x2"], formulaValues["it"], formulaValues["x3"], formulaValues["y2"]);
                case AutoShapeType.Pie:
                    return new RectangleF(formulaValues["il"], formulaValues["it"], formulaValues["ir"], formulaValues["ib"]);
                //case AutoShapeType.pieWedge:
                //    return new RectangleF(formulaValues["x1"], formulaValues["y1"], bounds.Width, bounds.Height);
                //case AutoShapeType.plaqueTabs:
                //case AutoShapeType.squareTabs:
                //    return new RectangleF(formulaValues["dx"], formulaValues["dx"], formulaValues["x1"], formulaValues["y1"]); 
                case AutoShapeType.QuadArrow:
                    return new RectangleF(formulaValues["il"], formulaValues["y3"], formulaValues["ir"], formulaValues["y4"]);
                case AutoShapeType.QuadArrowCallout:
                    return new RectangleF(formulaValues["x2"], formulaValues["y2"], formulaValues["x7"], formulaValues["y7"]);
                case AutoShapeType.DownRibbon:
                    return new RectangleF(formulaValues["x2"], formulaValues["y2"], formulaValues["x9"], bounds.Height);
                case AutoShapeType.UpRibbon:
                    return new RectangleF(formulaValues["x2"], 0, formulaValues["x9"], formulaValues["y2"]);
                case AutoShapeType.RightArrow:
                    return new RectangleF(0, formulaValues["y1"], formulaValues["x2"], formulaValues["y2"]);
                case AutoShapeType.RightArrowCallout:
                    return new RectangleF(0, 0, formulaValues["x2"], bounds.Height);
                case AutoShapeType.RightBrace:
                case AutoShapeType.RightBracket:
                    return new RectangleF(0, formulaValues["it"], formulaValues["ir"], formulaValues["ib"]);
                case AutoShapeType.RoundDiagonalCornerRectangle:
                    return new RectangleF(formulaValues["dx"], formulaValues["dx"], formulaValues["ir"], formulaValues["ib"]);
                case AutoShapeType.RoundSameSideCornerRectangle:
                    return new RectangleF(formulaValues["il"], formulaValues["tdx"], formulaValues["ir"], formulaValues["ib"]);
                case AutoShapeType.RightTriangle:
                    return new RectangleF(bounds.Width/12, formulaValues["it"], formulaValues["ir"], formulaValues["ib"]);
                case AutoShapeType.SnipSingleCornerRectangle:
                    return new RectangleF(0, formulaValues["it"], formulaValues["ir"], bounds.Height);
                case AutoShapeType.SnipAndRoundSingleCornerRectangle:
                    return new RectangleF(formulaValues["il"], formulaValues["il"], formulaValues["ir"], bounds.Height);
                case AutoShapeType.Star10Point:
                    return new RectangleF(formulaValues["sx2"], formulaValues["sy2"], formulaValues["sx5"], formulaValues["sy3"]);
                case AutoShapeType.Star12Point:
                    return new RectangleF(formulaValues["sx2"], formulaValues["sy2"], formulaValues["sx5"], formulaValues["sy5"]);
                case AutoShapeType.Star4Point:
                    return new RectangleF(formulaValues["sx1"], formulaValues["sy1"], formulaValues["sx2"], formulaValues["sy2"]);
                case AutoShapeType.Star5Point:
                    return new RectangleF(formulaValues["sx1"], formulaValues["sy1"], formulaValues["sx4"], formulaValues["sy3"]);
                case AutoShapeType.Star6Point:
                    return new RectangleF(formulaValues["sx1"], formulaValues["sy1"], formulaValues["sx4"], formulaValues["sy2"]);
                case AutoShapeType.Star7Point:
                    return new RectangleF(formulaValues["sx2"], formulaValues["sy1"], formulaValues["sx5"], formulaValues["sy3"]);
                case AutoShapeType.Star8Point:
                    return new RectangleF(formulaValues["sx1"], formulaValues["sy1"], formulaValues["sx4"], formulaValues["sy4"]);
                case AutoShapeType.StripedRightArrow:
                    return new RectangleF(formulaValues["x4"], formulaValues["y1"], formulaValues["x6"], formulaValues["y2"]);
                case AutoShapeType.Sun:
                    return new RectangleF(formulaValues["x9"], formulaValues["y9"], formulaValues["x8"], formulaValues["y8"]);
                case AutoShapeType.IsoscelesTriangle:
                    return new RectangleF(formulaValues["x1"], (bounds.Height / 2), formulaValues["x3"], bounds.Height);
                case AutoShapeType.UpArrowCallout:
                    return new RectangleF(0, formulaValues["y2"], bounds.Width, bounds.Height);
                case AutoShapeType.UpDownArrowCallout: return new RectangleF(0, formulaValues["y2"], bounds.Width, formulaValues["y3"]);
                case AutoShapeType.VerticalScroll:
                    return new RectangleF(formulaValues["ch"], formulaValues["ch"], formulaValues["x6"], formulaValues["y4"]);
                case AutoShapeType.CurvedUpRibbon:
                    return new RectangleF(formulaValues["x2"], formulaValues["y6"], formulaValues["x5"], formulaValues["rh"]);
                case AutoShapeType.CurvedDownRibbon:
                    return new RectangleF(formulaValues["x2"], formulaValues["q1"], formulaValues["x5"], formulaValues["y6"]);
                case AutoShapeType.Chevron:
                    return new RectangleF(formulaValues["il"], 0, formulaValues["ir"], bounds.Height);
                default:
                    return new RectangleF(0, 0, bounds.Width, bounds.Height);
            }
        }
        /// <summary>
        /// Get Current TextRange of the leafWidget
        /// </summary>
        /// <returns></returns>
        private WTextRange GetCurrTextRange()
        {
            WTextRange textRange = (LeafWidget is WTextRange) ? (LeafWidget as WTextRange)
                : (LeafWidget is SplitStringWidget) && ((LeafWidget as SplitStringWidget).RealStringWidget is WTextRange)
                ? ((LeafWidget as SplitStringWidget).RealStringWidget as WTextRange) : null;
            return textRange;
        }
        /// <summary>
        /// Update Tab Width.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="size">The size.</param>
        private void UpdateTabWidth(ref RectangleF rect, ref SizeF size)
        {
            float xPosition = (float)Math.Round(rect.X, 2);
            float pageMarginLeft = GetPageMarginLeft();
            float prevTabPosition = (m_lcOperator as Layouter).PreviousTab.Position + pageMarginLeft;
            float width = rect.X - (prevTabPosition - (m_lcOperator as Layouter).PreviousTabWidth);
            if ((m_lcOperator as Layouter).PreviousTab.Justification == TabJustification.Centered
                && width / 2 < (m_lcOperator as Layouter).PreviousTabWidth)
            {
                xPosition = prevTabPosition + width / 2;
                if (rect.Right < xPosition)
                    xPosition = rect.Right;
                rect.Width -= (xPosition - rect.X);
                rect.X = xPosition;
                CreateLayoutArea(rect);
            }
            else if ((m_lcOperator as Layouter).PreviousTab.Justification == TabJustification.Right
                && rect.X < prevTabPosition)
            {
                xPosition = prevTabPosition;
                rect.Width -= (xPosition - rect.X);
                rect.X = xPosition;
                CreateLayoutArea(rect);
            }
            else if ((m_lcOperator as Layouter).PreviousTab.Justification == TabJustification.Decimal)
            {
                UpdateLeafWidgetPosition(ref rect, ref xPosition, width);
            }
            ILeafWidget leafWidget = LeafWidget;
            TabsLayoutInfo tabsInfo = leafWidget.LayoutInfo as TabsLayoutInfo;
            float firstLineIndent = 0;
            WParagraph paragraph = (leafWidget as ParagraphItem).OwnerParagraph as WParagraph;
            if ((leafWidget as ParagraphItem).Owner is SDTInlineContent)
                paragraph = (leafWidget as ParagraphItem).GetOwnerParagraph();
            firstLineIndent = paragraph.ParagraphFormat.FirstLineIndent < 0 ? paragraph.ParagraphFormat.LeftIndent : 0;

            tabsInfo.PageMarginLeft = pageMarginLeft;
            float leftIndent = 0;
            ParagraphLayoutInfo paraInfo = paragraph.m_layoutInfo as ParagraphLayoutInfo;
            if (paraInfo.IsFirstLine)
                leftIndent = (float)paraInfo.Margins.Left + paraInfo.FirstLineIndent;
            else
                leftIndent = (float)paraInfo.Margins.Left;

            float tabWidth = (float)tabsInfo.GetNextTabPosition(xPosition - pageMarginLeft);

            if (firstLineIndent != 0
                && xPosition - pageMarginLeft < firstLineIndent
                && (tabsInfo.m_currTab.Position != xPosition - pageMarginLeft + tabWidth
                || tabsInfo.m_currTab.Position > firstLineIndent))
                tabWidth = (float)firstLineIndent - (xPosition - (float)pageMarginLeft);
            if (IsLeafWidgetIsInCell(LeafWidget as WTextRange)
                && tabWidth > m_layoutArea.ClientActiveArea.Width
                && xPosition - leftIndent == pageMarginLeft
                && tabsInfo.m_currTab.Position == 0
                && tabsInfo.m_currTab.Justification == TabJustification.Left)
                tabWidth = m_layoutArea.ClientActiveArea.Width;

            size.Width = tabWidth;
            if (tabsInfo.CurrTabJustification == TabJustification.Centered
                || tabsInfo.CurrTabJustification == TabJustification.Right
                || (tabsInfo.CurrTabJustification == TabJustification.Decimal && LeafWidget is ParagraphItem && !IsLeafWidgetIsInCell(LeafWidget as ParagraphItem)))
                size.Width = 0;
            (m_lcOperator as Layouter).PreviousTabWidth = tabWidth;
            tabsInfo.TabWidth = tabWidth;
            LeafWidget.LayoutInfo.Size = new SizeF(size.Width, LeafWidget.LayoutInfo.Size.Height);//set the Layout size into the LeafWidget layout info
        }
        /// <summary>
        /// Get Left Margin of the Page
        /// </summary>
        /// <returns></returns>
        private float GetPageMarginLeft()
        {
            float pageMarginLeft = (m_lcOperator as Layouter).ClientLayoutArea.Left;
            ParagraphItem paraItem = (LeafWidget is ParagraphItem) ? (LeafWidget as ParagraphItem) :
                ((LeafWidget is SplitStringWidget) ? (LeafWidget as SplitStringWidget).RealStringWidget as ParagraphItem
                : ((LeafWidget is ParagraphItem) ? (LeafWidget as ParagraphItem) : null));
            if (IsLeafWidgetIsInCell(paraItem))
            {
                WParagraph paragraph = paraItem.OwnerParagraph as WParagraph;
                if (paraItem.Owner is SDTInlineContent)
                    paragraph = paraItem.GetOwnerParagraph();
                WTableCell tableCell = paragraph.Owner as WTableCell;
                //TableCellLeftMargin value is assigned when the Tab is inside Table cell.
                pageMarginLeft = (tableCell.m_layoutInfo as TableLayoutInfo).TableCellLeftMargin;
            }
            return pageMarginLeft;
        }
        /// <summary>
        /// Update xPosition of the LeafWidget which have Previous Tab justification is Decimal
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="xPosition"></param>
        /// <param name="width"></param>
        private void UpdateLeafWidgetPosition(ref RectangleF rect, ref float xPosition, float width)
        {
            WParagraph paragraph = (LeafWidget as WTextRange).GetOwnerParagraph();
            int index = paragraph.ChildEntities.IndexOf(LeafWidget as WTextRange);
            int previousTabIndex = 0;
            for (int i = index - 1; i >= 0; i--)
            {
                if (paragraph.ChildEntities[i] is WTextRange && ((paragraph.ChildEntities[i] as ILeafWidget).LayoutInfo as TabsLayoutInfo) != null)
                {
                    previousTabIndex = i;
                    break;
                }
            }
            float leftWidth = DrawingContext.GetLeftWidth(paragraph, previousTabIndex, index);
            if (leftWidth < (m_lcOperator as Layouter).PreviousTabWidth)
            {
                width -= leftWidth;
                xPosition = ((m_lcOperator as Layouter).PreviousTab.Position + GetPageMarginLeft()) + width;
                rect.Width -= (xPosition - rect.X);
                rect.X = xPosition;
                CreateLayoutArea(rect);
            }
        }
        /// <summary>
        /// Determines whether leaf widget is need to be splitted
        /// </summary>
        /// <param name="size">size</param>
        /// <param name="rect">rect</param>
        /// <returns>
        /// 	<c>true</c> if leaf widget need to be splitted, set to <c>true</c>.
        /// </returns>
        internal bool IsLeafWidgetNeedToBeSplitted(SizeF size, float clientActiveAreaWidth)
        {
            bool isLeafWidgetNeedToBeSplitted = false;

            bool clipped = IsClipped(size);

            if (IsTextContainsLineBreakCharacters())
                return true;
            if ((((LeafWidget is WPicture) || (LeafWidget is WOleObject) || (LeafWidget is Shape)) ?
               (IsPictureFit(size, clientActiveAreaWidth) && (m_layoutArea.Width != 0.0f ? true : IsParagraphItemNeedToFit(LeafWidget as ParagraphItem)))
                : TryFit(size))
                || clipped
                || ((LeafWidget is WTextRange || LeafWidget is SplitStringWidget) ? IsTextRangeFitInClientActiveArea(size) : false))
            {
                isLeafWidgetNeedToBeSplitted = false;
            }
            else
                isLeafWidgetNeedToBeSplitted = true;
            return isLeafWidgetNeedToBeSplitted;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool TryFit(SizeF s)
        {
            WTextRange textRange = (LeafWidget is WTextRange) ? (LeafWidget as WTextRange) :
               ((LeafWidget is SplitStringWidget) ? (LeafWidget as SplitStringWidget).RealStringWidget as WTextRange : null);
            WParagraph paragraph = textRange != null ? GetOwnerParagraph() : null;
            return (s.Width <= m_layoutArea.ClientActiveArea.Width 
                  && (s.Height <= m_layoutArea.ClientActiveArea.Height
                  || (paragraph != null
                  && paragraph.ParagraphFormat.LineSpacingRule == LineSpacingRule.Exactly
                  && Math.Abs(paragraph.ParagraphFormat.LineSpacing) <= m_layoutArea.ClientActiveArea.Height)
                  || IsNeedToFitItemOfLastParagraph()));
        }
        /// <summary>
        /// Determine whether need to fit item of the last paragraph
        /// </summary>
        /// <returns></returns>
        private bool IsNeedToFitItemOfLastParagraph()
        {
            WParagraph paragraph = GetOwnerParagraph();
            return (paragraph != null && ((paragraph as IWidget).LayoutInfo as ParagraphLayoutInfo).IsNotFitted);
        }
        /// <summary>
        /// Determines whether text contains line break characters.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if text contains line break characters; otherwise, <c>false</c>.
        /// </returns>
        private bool IsTextContainsLineBreakCharacters()
        {
            return (LeafWidget is WTextRange && ((LeafWidget as WTextRange).Text.Contains("\n") || (LeafWidget as WTextRange).Text.Contains("\r"))
                || LeafWidget is SplitStringWidget && (LeafWidget as SplitStringWidget).SplittedText != null
                && ((LeafWidget as SplitStringWidget).SplittedText.Contains("\n") || (LeafWidget as SplitStringWidget).SplittedText.Contains("\r")));
        }
        /// <summary>
        /// Determines whether the specified leaf widget is to be clipped or not.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        /// 	<c>true</c> if the specified leaf widget is to be clipped; otherwise, <c>false</c>.
        /// </returns>
        private bool IsClipped(SizeF size)
        {
            bool isClipped = false;
            Entity ent = (LeafWidget is WPicture) ? (LeafWidget as WPicture)
                : ((LeafWidget is WOleObject) ? ((LeafWidget as WOleObject).OlePicture as WPicture) : LeafWidget as Entity);
            if (LayoutInfo.IsClipped && !(LeafWidget is WTextRange || LeafWidget is SplitStringWidget) && m_layoutArea.Height != 0 && m_layoutArea.Width != 0)
            {
                if (IsLeafWidgetHeightNeedToBeUpdate(ent as ParagraphItem))
                {
                    WParagraph ownerParagraph = (ent as ParagraphItem).GetOwnerParagraph();
                    if ((size.Height > m_layoutArea.ClientActiveArea.Height
                        && (ownerParagraph as IWidget).LayoutInfo.IsClipped
                        && !LayoutInfo.IsVerticalText)
                        || (LayoutInfo.IsVerticalText && size.Height > m_layoutArea.ClientActiveArea.Width))
                        isClipped = true;
                    bool isPictureSizeMaxCellWidth = ownerParagraph.IsInCell ? (float)DrawingContext.GetCellWidth(ent as ParagraphItem) < size.Width : true;
                    if ((size.Height <= m_layoutArea.ClientActiveArea.Height && isPictureSizeMaxCellWidth && IsFirstItemInLine() && !LayoutInfo.IsVerticalText)
                        || (LayoutInfo.IsVerticalText && size.Height <= m_layoutArea.ClientActiveArea.Width && size.Width > m_layoutArea.ClientActiveArea.Height))
                        isClipped = true;
                }
                else if (!((ent is WPicture) || (ent is Shape) ))
                    isClipped = true;
            }
            WTextRange textRange = (LeafWidget is WTextRange) ? (LeafWidget as WTextRange) :
                ((LeafWidget is SplitStringWidget) ? (LeafWidget as SplitStringWidget).RealStringWidget as WTextRange : null);
            float width = m_layoutArea.ClientActiveArea.Width;
            if (((textRange != null
                && textRange.Text.Length == 1
                || (LeafWidget is WCheckBox))
                || ((LeafWidget is SplitStringWidget)
                && (LeafWidget as SplitStringWidget).SplittedText != null
                && (LeafWidget as SplitStringWidget).SplittedText.Length == 1)))
            {
                //Get Owner paragraph of the text range
                WParagraph ownerParagraph = textRange.OwnerParagraph;
                if (ownerParagraph == null)
                {
                    if (textRange.Owner is SDTInlineContent)
                        ownerParagraph = textRange.GetOwnerParagraph();
                    else if (textRange.CharacterFormat.BaseFormat.OwnerBase is WParagraph)
                        ownerParagraph = textRange.CharacterFormat.BaseFormat.OwnerBase as WParagraph;
                }
                if (ownerParagraph != null)
                {
                    if (ownerParagraph.IsInCell)
                    {
                        width = DrawingContext.GetCellWidth(textRange);
                        if (size.Width > width)
                            isClipped = true;
                    }
                    if (IsParagraphItemNeedToFit(textRange))
                        isClipped = true;
                }
            }
            if (LeafWidget is WSymbol
                && (LeafWidget as WSymbol).GetOwnerParagraph().IsInCell)
            {
                width = DrawingContext.GetCellWidth(LeafWidget as WSymbol);
                if (size.Width > width)
                    isClipped = true;
            }
            return isClipped;
        }
        /// <summary>
        /// Determine whether the paragraph Item is the first item in the line.
        /// </summary>
        /// <param name="paraItem"></param>
        /// <returns></returns>
        private bool IsFirstItemInLine()
        {
            //Get Owner paragraph of the paragraph item.
            WParagraph ownerParagraph = GetOwnerParagraph();
            if (ownerParagraph != null)
            {
                ParagraphLayoutInfo parainfo = ownerParagraph.m_layoutInfo as ParagraphLayoutInfo;
                //Get the paragraph X position
                float xPosition = (float)(parainfo.XPosition + GetPararaphLeftIndent());
                if (Math.Round(xPosition, 2) == Math.Round(m_layoutArea.ClientActiveArea.X, 2))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Determine whether the paragraph Item is need to Fit
        /// </summary>
        /// <param name="paraItem"></param>
        /// <returns></returns>
        private bool IsParagraphItemNeedToFit(ParagraphItem paraItem)
        {
            float leftIndent = 0;
            float columnWidth = 0;
            if (paraItem != null && paraItem.OwnerParagraph != null)
            {
                Entity ent = paraItem as Entity;
                while (!(ent is WSection))
                {
                    if (ent is WTable)
                        break;
                    if (ent.Owner == null)
                        break;
                    else
                        ent = ent.Owner as Entity;
                }
                if (ent is WTable && (paraItem.OwnerParagraph.OwnerTextBody is WTableCell))
                    columnWidth = (paraItem.OwnerParagraph.OwnerTextBody as WTableCell).Width;
                else
                    columnWidth = (float)(m_lcOperator as Layouter).ClientLayoutArea.Width;
                ParagraphLayoutInfo paraInfo = (paraItem.OwnerParagraph as IWidget).LayoutInfo as ParagraphLayoutInfo;
                if (paraInfo.IsFirstLine)
                    leftIndent = (float)paraInfo.Margins.Left + paraInfo.FirstLineIndent;
                else
                    leftIndent = (float)paraInfo.Margins.Left;
                if (IsFirstItemInLine() && paraItem is WPicture && (paraItem.OwnerParagraph.OwnerTextBody is WTableCell))
                    return true;
            }
            if (leftIndent > columnWidth)
                return true;
            return false;
        }
        /// <summary>
        /// Get's Left Indent of the paragraph
        /// </summary>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        private float GetPararaphLeftIndent()
        {
            WTextRange textRange = (LeafWidget is WTextRange) ? (LeafWidget as WTextRange) :
                ((LeafWidget is SplitStringWidget) ? (LeafWidget as SplitStringWidget).RealStringWidget as WTextRange : null);
            WParagraph paragraph = (LeafWidget is ParagraphItem) ? (LeafWidget as ParagraphItem).OwnerParagraph : null;
            if (paragraph == null && (LeafWidget is ParagraphItem) && (LeafWidget as ParagraphItem).Owner is SDTInlineContent)
                paragraph = (LeafWidget as ParagraphItem).GetOwnerParagraph();
            else if (textRange != null && paragraph == null)
            {
                if (textRange.Owner is SDTInlineContent)
                    paragraph = textRange.GetOwnerParagraph();
                else
                    paragraph = textRange.CharacterFormat.BaseFormat.OwnerBase as WParagraph;
            }
            if (paragraph != null)
            {
                ParagraphLayoutInfo paraInfo = (paragraph as IWidget).LayoutInfo as ParagraphLayoutInfo;
                if (paraInfo.IsFirstLine)
                    return (float)paraInfo.Margins.Left + paraInfo.FirstLineIndent;
                else
                    return (float)paraInfo.Margins.Left;
            }
            else
                return 0;
        }
        /// <summary>
        /// Determines whether Picture Fit
        /// </summary>
        /// <param name="size">size</param>
        /// <returns>
        /// 	<c>true</c> if Picture Fit, set to <c>true</c>.
        /// </returns>
        private bool IsPictureFit(SizeF size, float clientActiveAreaWidth)
        {
            bool isPictureFit = false;
            //Get empty text range height
            float emptyTextHeight = GetEmptyTextRangeHeight();
            ParagraphItem paraItem = (LeafWidget is WOleObject) ? ((LeafWidget as WOleObject).OlePicture as WPicture) : LeafWidget as ParagraphItem;
            bool isInlineImage = paraItem != null && (paraItem is Shape ? (paraItem as Shape).WrapFormat.TextWrappingStyle == TextWrappingStyle.Inline
                                 : (paraItem as WPicture).TextWrappingStyle == TextWrappingStyle.Inline);
            if (isInlineImage)
            {
                float pageHeight = 0.0f;
                float columnWidth = 0.0f;
                Entity ent = GetBaseEntity(paraItem);
                WParagraph ownerParagraph = paraItem.GetOwnerParagraph();
                ParagraphLayoutInfo paragraphInfo = (ownerParagraph as IWidget).LayoutInfo as ParagraphLayoutInfo;
                float topMargin = (float)(paragraphInfo.Margins.Top + paragraphInfo.Paddings.Top);
                if (ent is WSection)
                {
                    pageHeight = (m_lcOperator as Layouter).ClientLayoutArea.Height;
                    columnWidth = (m_lcOperator as Layouter).ClientLayoutArea.Width;
                    if (paragraphInfo.IsFirstLine)
                        columnWidth = (m_lcOperator as Layouter).ClientLayoutArea.Width - (float)(paragraphInfo.Margins.Left + paragraphInfo.Margins.Right + paragraphInfo.FirstLineIndent + paragraphInfo.ListTab);
                    else
                        columnWidth = (m_lcOperator as Layouter).ClientLayoutArea.Width - (float)(paragraphInfo.Margins.Left + paragraphInfo.Margins.Right + paragraphInfo.ListTab);
                    if ((size.Height <= m_layoutArea.ClientActiveArea.Height
                        || (size.Height > pageHeight
                        && ((m_lcOperator as Layouter).IsLayoutingHeaderFooter ||Math.Round(m_layoutArea.ClientActiveArea.Y - topMargin, 2) == Math.Round((m_lcOperator as Layouter).PageTopMargin, 2))))
                       && (size.Width <= m_layoutArea.ClientActiveArea.Width || (size.Width > columnWidth && IsFirstItemInLine())
                        || Math.Round(clientActiveAreaWidth, 2) != Math.Round(m_layoutArea.ClientActiveArea.Width, 2)))
                        isPictureFit = true;
                }
                else if (ent is WTable)
                {
                    ILayoutSpacingsInfo spacingInfo = (ownerParagraph.OwnerTextBody as WTableCell).m_layoutInfo as ILayoutSpacingsInfo;
                    float cellTopMargin = (float)(spacingInfo.Margins.Top + spacingInfo.Paddings.Top);
                    float cellWidth = (float)DrawingContext.GetCellWidth(paraItem);
                    if ((size.Height <= m_layoutArea.ClientActiveArea.Height
                        && !(size.Width > m_layoutArea.ClientActiveArea.Width
                        && Math.Round(m_layoutArea.ClientActiveArea.Width, 2) != Math.Round(cellWidth, 2))
                        && !LayoutInfo.IsVerticalText)
                        || (LayoutInfo.IsVerticalText
                        && size.Height <= m_layoutArea.ClientActiveArea.Width)
                        || (size.Height > (m_lcOperator as Layouter).ClientLayoutArea.Height
                        && !LayoutInfo.IsClipped
                        && m_layoutArea.ClientActiveArea.Y - topMargin - cellTopMargin == (m_lcOperator as Layouter).PageTopMargin))
                        isPictureFit = true;
                }
            }
            else if (emptyTextHeight < m_layoutArea.ClientActiveArea.Height)
            {
                isPictureFit = true;
            }
            return isPictureFit;
        }
        /// <summary>
        /// Get the empty TextRange height
        /// </summary>
        /// <returns></returns>
        private float GetEmptyTextRangeHeight()
        {
            WParagraph paragraph = GetOwnerParagraph();
            if (paragraph != null)
            {
                float emptyTextHeight = DrawingContext.MeasureString(" ", paragraph.BreakCharacterFormat.Font, null, paragraph.BreakCharacterFormat, false).Height;
                float lineSpacing = Math.Abs(paragraph.ParagraphFormat.LineSpacing);
                if ((paragraph.ParagraphFormat.LineSpacingRule == LineSpacingRule.Exactly)
                    || (paragraph.ParagraphFormat.LineSpacingRule == LineSpacingRule.AtLeast && lineSpacing > emptyTextHeight))
                    return lineSpacing;
                else
                    return emptyTextHeight;
            }
            return 0;
        }
        /// <summary>
        /// Get Base Entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        private Entity GetBaseEntity(ParagraphItem entity)
        {
            Entity ent = entity;
            while (!(ent is WSection))
            {
                if ((ent is WTable) && IsLeafWidgetHeightNeedToBeUpdate(entity))
                {
                    break;
                }
                if (ent.Owner == null)
                    break;
                else
                    ent = ent.Owner as Entity;
            }
            return ent;
        }
        /// <summary>
        /// Determines whether TextRange Fit in ClientActiveArea
        /// </summary>
        /// <param name="size">size</param>
        /// <returns>
        /// 	<c>true</c> if TextRange Fit in ClientActiveArea, set to <c>true</c>.
        /// </returns>
        private bool IsTextRangeFitInClientActiveArea(SizeF size)
        {
            bool isTextRangeFit = false;
            //Checks for owner row ? height type = exactly
            if ((LayoutInfo.IsClipped && size.Width <= m_layoutArea.Width)
                || IsTextRangeNeedToFit())
                isTextRangeFit = true;
            float pageHeight = (m_lcOperator as Layouter).ClientLayoutArea.Height;
            WParagraph paragraph = GetOwnerParagraph();
            //skip to fit the text in current page when text in the table cell. 
            if (paragraph != null && !paragraph.IsInCell && size.Height > pageHeight && m_layoutArea.ClientActiveArea.Height > 0)
                isTextRangeFit = true;
            return isTextRangeFit;
        }
        /// <summary>
        /// Determine whether the text range is need to fit
        /// </summary>
        /// <returns>
        /// <c>true</c> if previous tab position is greater than the ClientActiveArea, set to <c>true</c>.
        /// </returns>
        private bool IsTextRangeNeedToFit()
        {
            WTextRange textRange = (LeafWidget is WTextRange) ? (LeafWidget as WTextRange) :
                ((LeafWidget is SplitStringWidget) ? (LeafWidget as SplitStringWidget).RealStringWidget as WTextRange : null);
            float prevTabPosition = (m_lcOperator as Layouter).PreviousTab.Position;
            bool isLeafWidgetIsInCell=IsLeafWidgetIsInCell(textRange);
            float cellWidth = 0;
            return (isLeafWidgetIsInCell ? textRange != null && (prevTabPosition > (cellWidth = DrawingContext.GetCellWidth(textRange))
                || ((textRange.m_layoutInfo as TabsLayoutInfo) != null 
                && ((textRange.m_layoutInfo as TabsLayoutInfo).m_currTab.Position > cellWidth
                || (textRange.m_layoutInfo as TabsLayoutInfo).m_currTab.Position == 0
                && (textRange.m_layoutInfo as TabsLayoutInfo).m_currTab.Justification == TabJustification.Left
                && textRange.m_layoutInfo.Size.Width > cellWidth))
                || cellWidth == 0) && !m_isTabStopBeyondRightMarginExists
                : (prevTabPosition + (m_lcOperator as Layouter).ClientLayoutArea.Left >= m_layoutArea.ClientActiveArea.Right
                || ((textRange != null && (textRange.m_layoutInfo as TabsLayoutInfo) != null
                && (textRange.m_layoutInfo as TabsLayoutInfo).m_currTab.Position + (m_lcOperator as Layouter).ClientLayoutArea.Left >= m_layoutArea.ClientActiveArea.Right))
                || (m_lcOperator as Layouter).ClientLayoutArea.Left >= m_layoutArea.ClientActiveArea.Right));
        }
        /// <summary>
        /// Layout for Word 
        /// </summary>
        /// <param name="rect">The rect</param>
        /// <param name="size">size</param>
        /// <returns></returns>
        internal LayoutedWidget WordLayout(RectangleF rect, SizeF size, WTextRange textRange)
        {
            //Get NextSibling of leafwidget
            WTextRange nextSiblingTextRange = GetNextSibling(textRange) as WTextRange;

            if (size.Width != 0.0f
                && nextSiblingTextRange != null
                && ((nextSiblingTextRange as IWidget).LayoutInfo as TabsLayoutInfo) == null)
            {
                float clientWidth = m_isTabStopBeyondRightMarginExists ? MAX_WIDTH : textRange.GetClientWidth(DrawingContext, (m_lcOperator as Layouter).ClientLayoutArea.Width);
                if (IsTextRangeNeedToFit())
                    return null;
                //Word by word layouting for CJK (unicode text)
                if (DrawingContext.IsUnicodeText(GetText()))
                {
                    SplitUnicodeTextByWord(textRange, rect, size, clientWidth);
                    DoLayoutAfter();
                    return m_ltWidget;
                }
                else if (IsTextNeedToBeSplitted(size, rect, textRange)
                         && IsTextNeedToBeSplittedByWord(size, rect, textRange, clientWidth))//Checks for text need to be splitted
                {
                    ISplitLeafWidget splitLeafWidget = LeafWidget as ISplitLeafWidget;
                    SplitByWord(splitLeafWidget, size, textRange, clientWidth, false);
                    DoLayoutAfter();
                    return m_ltWidget;
                }
            }
            else if (nextSiblingTextRange != null && ((nextSiblingTextRange as IWidget).LayoutInfo as TabsLayoutInfo) != null && size.Width < rect.Width)
            {
                TabsLayoutInfo tabsInfo = (nextSiblingTextRange as IWidget).LayoutInfo as TabsLayoutInfo;
                tabsInfo.GetNextTabPosition(m_layoutArea.ClientArea.X);

                if (tabsInfo.m_currTab.Position > ClientLayoutAreaRight)
                {
                    if (IsTextRangeNeedToFit())
                        return null;
                    if (IsTextNeedToBeSplitted(size, rect, textRange)
                             && IsTextNeedToBeSplittedByWord(size, new RectangleF(rect.X, rect.Y, MAX_WIDTH - rect.Width, rect.Height), nextSiblingTextRange, MAX_WIDTH))//Checks for text need to be splitted
                    {
                        ISplitLeafWidget splitLeafWidget = LeafWidget as ISplitLeafWidget;
                        SplitByWord(splitLeafWidget, size, textRange, m_layoutArea.ClientArea.Width, false);
                        DoLayoutAfter();
                        if (!(m_ltWidget.Widget as SplitStringWidget).SplittedText.Equals(""))
                            return m_ltWidget;
                    }
                }
            }
            //Check whether the remaining client area enough to fit the single word.
            if ((m_lcOperator as Layouter).FloatingTableBottom != float.MinValue
                && GetMinWidth(textRange, size, rect) > Math.Round(rect.Width))
            {
                ISplitLeafWidget splitLeafWidget = LeafWidget as ISplitLeafWidget;
                SplitByWord(splitLeafWidget, size, textRange, rect.Width, true);
                DoLayoutAfter();
                return m_ltWidget;
            }
            else
                (m_lcOperator as Layouter).FloatingTableBottom = float.MinValue;
            return m_ltWidget = null;
        }
        /// <summary>
        /// Word by word layouting for Unicode text
        /// </summary>
        private void SplitUnicodeTextByWord(WTextRange textRange,RectangleF rect,SizeF size, float clientWidth)
        {
            //Get NextSibling of leafwidget
            WTextRange nextSiblingTextRange = GetNextSibling(textRange) as WTextRange;
            if (nextSiblingTextRange != null && nextSiblingTextRange.Text.Length > 0
                 && DrawingContext.IsBeginCharacter(nextSiblingTextRange.Text[0]))
            {
                float width = DrawingContext.MeasureTextRange((textRange.NextSibling) as WTextRange, nextSiblingTextRange.Text[0].ToString()).Width;
                string text = GetText();
                int index = text.Length;
                if ((IsBeginCJKCharacter(text, ref index)
                    || index > 0)
                    && (size.Width + width > rect.Width
                    || IsUnicodeTextNeedToBeSplittedByWord(size, rect, textRange, clientWidth)))
                {
                    IStringWidget strWidget = textRange as IStringWidget;
                    ISplitLeafWidget[] spLeafWidgets = new ISplitLeafWidget[2];
                    //Split the last character to next line
                    string text1 = textRange.Text.Substring(0, index - 1);
                    string text2 = textRange.Text.Substring(index - 1);
                    spLeafWidgets[0] = new SplitStringWidget(strWidget, text1);
                    spLeafWidgets[1] = new SplitStringWidget(strWidget, text2);
                    m_ltState = LayoutState.NotFitted;

                    if (spLeafWidgets != null)
                    {
                        size = spLeafWidgets[0].Measure(DrawingContext);

                        if (!TryFit(size))
                        {
                            size.Width = m_layoutArea.ClientArea.Width;
                        }
                        FitWidget(size, spLeafWidgets[0],false);
                        m_sptWidget = spLeafWidgets[1];
                        m_ltState = LayoutState.Splitted;
                    }
                }
            }
        }
        /// <summary>
        /// Check whether the text need to splitted by word
        /// </summary>
        /// <param name="size"></param>
        /// <param name="rect"></param>
        /// <param name="nextSiblingText"></param>
        /// <param name="sizeNextWidth"></param>
        /// <returns></returns>
        private bool IsUnicodeTextNeedToBeSplittedByWord(SizeF size, RectangleF rect, WTextRange textRange, float clientWidth)
        {
            bool isTextNeedToBeSplitted = false;
            WTextRange nextSiblingTextRange = GetNextSibling(textRange) as WTextRange;
            string text=GetText();
            //Check whether the text need to splitted by word
            if (nextSiblingTextRange != null
                && nextSiblingTextRange is WTextRange
                && size.Width < rect.Width)
            {
                string nextSiblingText = nextSiblingTextRange.Text;
                float nextTextRangeWidth = GetUnicodeNextTextRangeWidth(nextSiblingTextRange, ref nextSiblingText, size, rect);
                if ((size.Width + nextTextRangeWidth) > rect.Width
                   && clientWidth >= rect.Width
                   && nextTextRangeWidth < clientWidth)
                {
                    isTextNeedToBeSplitted = true;
                }
            }
            return isTextNeedToBeSplitted;
        }
        /// <summary>
        /// Get Text Whether the LeafWidget is WTextRange or SplitStringWidget
        /// </summary>
        /// <returns></returns>
        private string GetText()
        {
            return (LeafWidget is WTextRange) ? (LeafWidget as WTextRange).Text : (LeafWidget as SplitStringWidget).SplittedText;
        }
        /// <summary>
        /// Check whether the text need to splitted by word
        /// </summary>
        /// <param name="size"></param>
        /// <param name="rect"></param>
        /// <param name="nextSiblingText"></param>
        /// <param name="sizeNextWidth"></param>
        /// <returns></returns>
        private bool IsTextNeedToBeSplittedByWord(SizeF size, RectangleF rect, WTextRange textRange, float clientWidth)
        {
            bool isTextNeedToBeSplitted = false;
            WTextRange nextSiblingTextRange = GetNextSibling(textRange) as WTextRange;
            //Check whether the text need to splitted by word
            if (nextSiblingTextRange != null
                && nextSiblingTextRange is WTextRange
                && !(nextSiblingTextRange.Text.StartsWith(" ")
                || GetText().EndsWith(" ")
                || ((nextSiblingTextRange as IWidget).LayoutInfo as TabsLayoutInfo) != null)
                && !(size.Width > rect.Width))
            {
                string nextSiblingText = nextSiblingTextRange.Text;
                float nextTextRangeWidth = GetNextTextRangeWidth(nextSiblingTextRange, ref nextSiblingText, size, rect);
                float prevTextRangeWidth = DrawingContext.GetPreviousTextRangeWidth(textRange);
                if (!DrawingContext.IsUnicodeText(nextSiblingText) && (size.Width + nextTextRangeWidth) > rect.Width
                   && clientWidth >= rect.Width
                   && (prevTextRangeWidth + GetWidthToFitText(textRange, nextTextRangeWidth) < clientWidth))
                {
                    isTextNeedToBeSplitted = true;
                }
            }
            return isTextNeedToBeSplitted;
        }
        /// <summary>
        /// Get width to fit text in current line
        /// </summary>
        /// <param name="textRange"></param>
        /// <returns></returns>
        private float GetWidthToFitText(WTextRange textRange, float nextTextRangeWidth)
        {
            string text = "";
            string[] st = GetText().Split(' ');
            if (st.Length == 1)
            {
                if (textRange.Text != st[0])
                    return DrawingContext.MeasureTextRange(textRange, st[0]).Width + nextTextRangeWidth;
                else
                    return (textRange as IWidget).LayoutInfo.Size.Width + nextTextRangeWidth;
            }
            else
            {
                for (int i = 0; i < st.Length - 1; i++)
                {
                    text += st[i] + " ";
                }
                string[] hypenText = st[st.Length - 1].Split(StringParser.Hyphen);
                for (int i = 0; i < hypenText.Length - 1; i++)
                {
                    text += hypenText[i] + StringParser.Hyphen.ToString();
                }
                if (textRange.Text != text)
                    return DrawingContext.MeasureTextRange(textRange, text).Width;
                else
                    return (textRange as IWidget).LayoutInfo.Size.Width;
            }
        }
        /// <summary>
        /// Get Next TextRanges width
        /// </summary>
        /// <param name="nextSiblingText"></param>
        /// <param name="size"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        private float GetNextTextRangeWidth(WTextRange nextSiblingTextRange, ref string nextSiblingText, SizeF size, RectangleF rect)
        {
            SizeF sizeNext = new SizeF();
            bool isNextSiblingSizeNeedToBeMeasure = IsNextSibligSizeNeedToBeMeasure(ref sizeNext, nextSiblingTextRange, rect, size);
            while (isNextSiblingSizeNeedToBeMeasure
                && IsLeafWidgetNextSiblingIsTextRange(nextSiblingTextRange)
                && (size + sizeNext).Width < rect.Width)
            {
                nextSiblingTextRange = GetNextSibling(nextSiblingTextRange) as WTextRange;
                if (!IsNextSibligSizeNeedToBeMeasure(ref sizeNext, nextSiblingTextRange, rect, size))
                    break;
                nextSiblingText += nextSiblingTextRange.Text;
            }
            return sizeNext.Width;
        }
        /// <summary>
        /// Get Next TextRanges width
        /// </summary>
        /// <param name="nextSiblingText"></param>
        /// <param name="size"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        private float GetUnicodeNextTextRangeWidth(WTextRange nextSiblingTextRange, ref string nextSiblingText, SizeF size, RectangleF rect)
        {
            SizeF sizeNext = new SizeF();
            bool isNextSiblingSizeNeedToBeMeasure = IsUnicodeNextSibligSizeNeedToBeMeasure(ref sizeNext, nextSiblingTextRange, rect, size);
            while (isNextSiblingSizeNeedToBeMeasure
                && IsLeafWidgetNextSiblingIsTextRange(nextSiblingTextRange)
                && (size + sizeNext).Width < rect.Width)
            {
                nextSiblingTextRange = GetNextSibling(nextSiblingTextRange) as WTextRange;
                if (!IsUnicodeNextSibligSizeNeedToBeMeasure(ref sizeNext, nextSiblingTextRange, rect, size))
                    break;
                nextSiblingText += nextSiblingTextRange.Text;
            }
            return sizeNext.Width;
        }
        /// <summary>
        /// Determine Whether the Nextsibling Textrange size is need to measure
        /// </summary>
        /// <param name="sizeNext"></param>
        /// <param name="nextSiblingTextRange"></param>
        /// <param name="rect"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private bool IsNextSibligSizeNeedToBeMeasure(ref SizeF sizeNext, WTextRange nextSiblingTextRange, RectangleF rect, SizeF size)
        {
            if (nextSiblingTextRange.Text.Contains(" ")
                    || ((nextSiblingTextRange as IWidget).LayoutInfo as TabsLayoutInfo) != null)
            {
                float width = (nextSiblingTextRange as IWidget).LayoutInfo.Size.Width;
                if (nextSiblingTextRange.Text != nextSiblingTextRange.Text.Split(' ')[0])
                    width = DrawingContext.MeasureTextRange(nextSiblingTextRange, nextSiblingTextRange.Text.Split(' ')[0]).Width;
                if ((size.Width + sizeNext.Width + width) > rect.Width && nextSiblingTextRange.Text.Contains(StringParser.Hyphen.ToString()))
                {
                    if (nextSiblingTextRange.Text != nextSiblingTextRange.Text.Split(StringParser.Hyphen)[0] + StringParser.Hyphen.ToString())
                        width = DrawingContext.MeasureTextRange(nextSiblingTextRange, nextSiblingTextRange.Text.Split(StringParser.Hyphen)[0] + StringParser.Hyphen.ToString()).Width;
                }
                sizeNext.Width += width;
                return false;
            }
            else
                sizeNext += (nextSiblingTextRange as IWidget).LayoutInfo.Size;
            return true;
        }
        /// <summary>
        /// Determine Whether the Nextsibling Textrange size is need to measure
        /// </summary>
        /// <param name="sizeNext"></param>
        /// <param name="nextSiblingTextRange"></param>
        /// <param name="rect"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private bool IsUnicodeNextSibligSizeNeedToBeMeasure(ref SizeF sizeNext, WTextRange nextSiblingTextRange, RectangleF rect, SizeF size)
        {
            int index = 0;
            if (IsBeginCJKCharacter(nextSiblingTextRange.Text, ref index))
            {
                float width = (nextSiblingTextRange as IWidget).LayoutInfo.Size.Width;
                string text = string.Empty;
                for (int i = index; i < nextSiblingTextRange.Text.Length; i++)
                {
                    if (DrawingContext.IsBeginCharacter(nextSiblingTextRange.Text[i]))
                        text += nextSiblingTextRange.Text[i];
                    else
                        break;
                }
                width = DrawingContext.MeasureTextRange(nextSiblingTextRange, text).Width;
                sizeNext.Width += width;
                if (text == nextSiblingTextRange.Text)
                    return true;
                else
                    return false;
            }
            return false;
        }
        /// <summary>
        /// Determine whether the text contain CJK begin characters
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool IsBeginCJKCharacter(string text,ref int index)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (DrawingContext.IsBeginCharacter(text[i]))
                {
                    index = i;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Get Next text range
        /// </summary>
        /// <returns></returns>
        private IWidget GetNextSibling(WTextRange textRange)
        {
            WParagraph paragraph = GetOwnerParagraph();
            IWidget nextSibling = paragraph.GetNextSibling(textRange) as IWidget;
            while (nextSibling != null && !((nextSibling is WTextRange)
                && !(nextSibling.LayoutInfo.IsSkip)))
            {
                if (!(nextSibling is WFootnote) && (nextSibling is BookmarkStart
                    || nextSibling is BookmarkEnd
                    || nextSibling is WFieldMark || nextSibling.LayoutInfo.IsSkip))
                    nextSibling = paragraph.GetNextSibling(nextSibling) as IWidget;
                else
                    return nextSibling;
            }
            return nextSibling;
        }
        /// <summary>
        /// Determines whether leaf widget is in cell
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if leaf widget is in cell, set to <c>true</c>.
        /// </returns>
        internal bool IsLeafWidgetIsInCell(ParagraphItem paraItem)
        {
            bool isLeafWidgetIsInCell = false;
            if (paraItem != null
                && ((paraItem.OwnerParagraph != null
                && paraItem.OwnerParagraph.IsInCell)
                || ((paraItem.Owner is SDTInlineContent)
                && paraItem.GetOwnerParagraph() != null
                && paraItem.GetOwnerParagraph().IsInCell)))
                isLeafWidgetIsInCell = true;
            return isLeafWidgetIsInCell;
        }
        /// <summary>
        /// Determines whether next sibling of leaf widget is text range.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if next sibling of leaf widget is text range, set to <c>true</c>.
        /// </returns>
        internal bool IsLeafWidgetNextSiblingIsTextRange(WTextRange textRange)
        {
            WTextRange nextSiblingTextRange = GetNextSibling(textRange) as WTextRange;
            if (nextSiblingTextRange != null && nextSiblingTextRange is WTextRange)
                return true;
            return false;
        }
        /// <summary>
        /// Split by Word
        /// </summary>
        /// <param name="splitLeafWidget">SplitLeafWidget</param>
        /// <param name="size">Size</param>
        internal void SplitByWord(ISplitLeafWidget splitLeafWidget, SizeF size,WTextRange textRange, float clientWidth,bool isWrapTextBasedOnAbsTable)
        {
            if (splitLeafWidget != null && size.Height <= m_layoutArea.ClientArea.Height)
            {
                SizeF sizesplit;
                ISplitLeafWidget[] splitedLeafWidgets = null;
                if ((LayoutInfo as TabsLayoutInfo) != null)
                {
                    splitedLeafWidgets = new ISplitLeafWidget[] { splitLeafWidget, splitLeafWidget };
                }
                else
                {
                    IStringWidget strWidget = textRange as IStringWidget;
                    string text = "";
                    string[] st = GetText().Split(' ');

                    ISplitLeafWidget[] spLeafWidgets = new ISplitLeafWidget[2];
                    if (st.Length == 1 && size.Width > clientWidth)
                    {
                        spLeafWidgets[0] = new SplitStringWidget(strWidget, st[0]);
                        spLeafWidgets[1] = new SplitStringWidget(strWidget, text);
                    }
                    else if (st[st.Length - 1].EndsWith(StringParser.Hyphen.ToString()))
                    {
                        spLeafWidgets[0] = new SplitStringWidget(strWidget, GetText());
                        spLeafWidgets[1] = new SplitStringWidget(strWidget, text);
                    }
                    else
                    {
                        for (int i = 0; i < st.Length - 1; i++)
                            text += st[i] + " ";
                        string[] hypenText = st[st.Length - 1].Split(StringParser.Hyphen);
                        for (int i = 0; i < hypenText.Length - 1; i++)
                        {
                            text += hypenText[i] + StringParser.Hyphen.ToString();
                        }
                        //Split the entire word into the bottom of the absolute table when remaining client area not enough to fit for the single word. 
                        if (isWrapTextBasedOnAbsTable)
                        {
                            spLeafWidgets[0] = new SplitStringWidget(strWidget, "");
                            spLeafWidgets[1] = new SplitStringWidget(strWidget, GetText());
                        }
                        else
                        {
                            spLeafWidgets[0] = new SplitStringWidget(strWidget, text);
                            spLeafWidgets[1] = new SplitStringWidget(strWidget, hypenText[hypenText.Length - 1]);
                        }
                    }
                    splitedLeafWidgets = spLeafWidgets;
                }

                m_ltState = LayoutState.NotFitted;

                if (splitedLeafWidgets != null)
                {
                    size = splitedLeafWidgets[0].Measure(DrawingContext);

                    if (!TryFit(size))
                    {
                        size.Width = m_layoutArea.ClientArea.Width;
                    }
                    FitWidget(size, splitedLeafWidgets[0],false);
                    m_sptWidget = splitedLeafWidgets[1];
                    m_ltState = LayoutState.Splitted;
                }
            }
            else
            {
                m_ltState = LayoutState.NotFitted;
                m_bIsVerticalNotFitted = (size.Height > m_layoutArea.ClientArea.Height);
            }
        }
        /// <summary>
        /// Determines whether text is need to be splitted
        /// </summary>
        /// <param name="size">size</param>
        /// <param name="rect">rect</param>
        /// <returns>
        /// 	<c>true</c> if text need to be splitted, set to <c>true</c>.
        /// </returns>
        internal bool IsTextNeedToBeSplitted(SizeF size, RectangleF rect,WTextRange textRange)
        {
            WTextRange prevSiblingTextRange = DrawingContext.GetPreviousSibling(textRange) as WTextRange;
            WTextRange nextSiblingTextRange = GetNextSibling(textRange) as WTextRange;
            bool isTextNeedToBeSplitted = IsTextNeedToBeSplitted(prevSiblingTextRange, nextSiblingTextRange);
            // Checks for previous sibling text ends with empty string "" and 
            // next sibling and current leaf widget text starts and ends with empty string ""
            if ((prevSiblingTextRange != null                                  // if previous sibling not equal to null  
                && (prevSiblingTextRange is WTextRange)                       // if previous sibling is text range    
                && prevSiblingTextRange.Text.EndsWith("")     // if previous sibling text ends with empty string ""
                || prevSiblingTextRange == null)                              // if previous sibling is null
                && GetText().StartsWith("")                                   // if text starts and
                && GetText().EndsWith("")                                     // ends with empty string ""
                && (nextSiblingTextRange != null
                && nextSiblingTextRange.Text.StartsWith("")      // if next sibling text starts and
                && nextSiblingTextRange.Text.EndsWith("")))       // ends with empty string ""
            {
                int index = nextSiblingTextRange.Text.IndexOf(" ");
                if (index != -1)
                {
                    SizeF sizeNext = DrawingContext.MeasureTextRange(nextSiblingTextRange, nextSiblingTextRange.Text.Substring(0, nextSiblingTextRange.Text.IndexOf(" ") + 1));
                    if (!(size.Width > rect.Width) && (size + sizeNext).Width > rect.Width)
                    {
                        isTextNeedToBeSplitted = true;
                    }
                    else
                        isTextNeedToBeSplitted = false;
                }
            }
            return isTextNeedToBeSplitted;
        }
        /// <summary>
        /// Determines whether text is need to be splitted
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if text need to be splitted, set to <c>true</c>.
        /// </returns>
        internal bool IsTextNeedToBeSplitted(WTextRange prevSiblingTextRange, WTextRange nextSiblingTextRange)
        {
            bool isTextNeedToBeSplitted = true;
            string text=GetText();
            if (text.EndsWith(" ")       // if text ends with space " "
                || text.EndsWith(".")    // if text ends with full stop "."
                || text.EndsWith(",")    // if text ends with comma  ","
                || (nextSiblingTextRange != null
                && (nextSiblingTextRange.Text.StartsWith(" ")  // if next sibling text starts with space " "
                || nextSiblingTextRange.Text.StartsWith(".")  // if next sibling text starts with full stop "."
                || nextSiblingTextRange.Text.StartsWith(",")))  // if next sibling text starts with comma ","
                || (prevSiblingTextRange != null                          //if previous sibling not equal to null  
                    && (prevSiblingTextRange is WTextRange)               //if previous sibling is text range    
                    && prevSiblingTextRange.Text.EndsWith("")     //if previous sibling text ends with empty string ""
                    && text.StartsWith("")                           //if text starts and
                    && text.EndsWith("")                             // ends with empty string ""
                //checks for next sibling text not starts and ends with empty string ""
                    && !(nextSiblingTextRange != null
                    && nextSiblingTextRange.Text.StartsWith("")
                    && (nextSiblingTextRange.Text.EndsWith("")))))
                isTextNeedToBeSplitted = false;
            return isTextNeedToBeSplitted;
        }
        /// <summary>
        /// Get OwnerParagraph of the LeafWidget
        /// </summary>
        /// <returns></returns>
        private WParagraph GetOwnerParagraph()
        {
            ParagraphItem paraItem = (LeafWidget is SplitStringWidget) ? (LeafWidget as SplitStringWidget).RealStringWidget as WTextRange
                : (LeafWidget is ParagraphItem) ? (LeafWidget as ParagraphItem) : null;
            if ((paraItem is WTextRange) && (paraItem as WTextRange).Owner == null)
                return (paraItem as WTextRange).CharacterFormat.BaseFormat.OwnerBase as WParagraph;
            else if (paraItem != null)
                return paraItem.GetOwnerParagraph();
            return null;
        }
        /// <summary>
        /// Adjust Client Area for Text Wrapping
        /// </summary>
        /// <param name="leafWidget">leaf widget</param>
        /// <param name="size">Size</param>
        /// <param name="rect">The rect</param>
        internal void AdjustClientAreaBasedOnTextWrap(ILeafWidget leafWidget, SizeF size, ref RectangleF rect)
        {
            #region textwrap
            WParagraph paragraph = GetOwnerParagraph();
            //Update Layout area based on text wrap
            if ((m_lcOperator as Layouter).FloatingItems.Count > 0
                && m_bIsNeedToWrap
                && !(m_lcOperator as Layouter).IsLayoutingHeaderFooter
                && !((LeafWidget is WPicture
                && ((LeafWidget as WPicture).TextWrappingStyle == TextWrappingStyle.InFrontOfText
                || (LeafWidget as WPicture).TextWrappingStyle == TextWrappingStyle.Behind))
                || (LeafWidget is Shape
                && ((LeafWidget as Shape).WrapFormat.TextWrappingStyle == TextWrappingStyle.InFrontOfText
                || (LeafWidget as Shape).WrapFormat.TextWrappingStyle == TextWrappingStyle.Behind)))
                && !IsLeafWidgetOwnerIsTextBox()
                && !(paragraph != null && (IsInFrame(paragraph) || IsInFootnote(paragraph))))
            {
                RectangleF clientLayoutArea = (m_lcOperator as Layouter).ClientLayoutArea;
                int wrapOwnerIndex = GetFloattingItemIndex(paragraph as Entity);
                int wrapItemIndex = -1;
                if (LeafWidget is WPicture)
                    wrapItemIndex = (LeafWidget as WPicture).WrapCollectionIndex;
                if (LeafWidget is Shape)
                    wrapItemIndex = (LeafWidget as Shape).WrapFormat.WrapCollectionIndex;
                bool isTextRangeInTextBox=false ;
                if (leafWidget is WTextRange)
                {
                    WParagraph ownerPara = (leafWidget as WTextRange).OwnerParagraph;
                    if (ownerPara!=null && ownerPara.IsInCell)
                    {
                        WTable table = (ownerPara.OwnerTextBody as WTableCell).OwnerRow.OwnerTable;
                        if (table.m_isTextBox && table.m_textBoxFormat.TextWrappingStyle !=TextWrappingStyle.Inline)
                            isTextRangeInTextBox = table.m_textBoxFormat.AllowOverlap;
                    }
                    else if(ownerPara != null && ownerPara.OwnerTextBody.Owner is Shape)
                    {
                        Shape shape = ownerPara.OwnerTextBody.Owner as Shape;
                        if (shape.WrapFormat.TextWrappingStyle != TextWrappingStyle.Inline)
                            isTextRangeInTextBox = shape.WrapFormat.AllowOverlap;
                    }
                }
                for (int i = 0; i < (m_lcOperator as Layouter).FloatingItems.Count; i++)
                {
                    RectangleF textWrappingBounds = (m_lcOperator as Layouter).FloatingItems[i].TextWrappingBounds;
                    TextWrappingStyle textWrappingStyle = (m_lcOperator as Layouter).FloatingItems[i].TextWrappingStyle;
                    TextWrappingType textWrappingType = (m_lcOperator as Layouter).FloatingItems[i].TextWrappingType;
                    bool allowOverlap = (m_lcOperator as Layouter).FloatingItems[i].AllowOverlap;
                    if (!(clientLayoutArea.X > textWrappingBounds.Right + DEF_MIN_WIDTH || clientLayoutArea.Right < textWrappingBounds.X - DEF_MIN_WIDTH))
                    {
                        if ((m_lcOperator as Layouter).FloatingItems.Count > 0
                            && wrapOwnerIndex != i
                            && wrapItemIndex != i
                            && Math.Round((rect.Y + size.Height), 2) > Math.Round(textWrappingBounds.Y, 2)
                            && Math.Round(rect.Y, 2) < Math.Round((textWrappingBounds.Bottom), 2)
                            && textWrappingStyle != TextWrappingStyle.Inline
                            && textWrappingStyle != TextWrappingStyle.TopAndBottom
                            && textWrappingStyle != TextWrappingStyle.InFrontOfText
                            && textWrappingStyle != TextWrappingStyle.Behind
                            && !(allowOverlap && (isTextRangeInTextBox || (leafWidget is WPicture && (leafWidget as WPicture).TextWrappingStyle != TextWrappingStyle.Inline && (leafWidget as WPicture).AllowOverlap) 
                            || (leafWidget is Shape && (leafWidget as Shape).WrapFormat.TextWrappingStyle != TextWrappingStyle.Inline && (leafWidget as Shape).WrapFormat.AllowOverlap))))
                        {
                            Entity ent = leafWidget as Entity;
                            while (!(ent is WParagraph) && ent != null)
                            {
                                if (ent is WParagraph || ent == null)
                                    break;
                                ent = ent.Owner;
                            }
                            float rightIndent = 0;
                            float leftIndent = 0;
                            WTextRange currTextRange = GetCurrTextRange();
                            if (leafWidget is ParagraphItem)
                            {
                                if ((leafWidget as ParagraphItem).OwnerParagraph != null)
                                {
                                    rightIndent = (float)(((leafWidget as ParagraphItem).OwnerParagraph as IWidget).LayoutInfo as ParagraphLayoutInfo).Margins.Right;
                                    leftIndent = (float)(((leafWidget as ParagraphItem).OwnerParagraph as IWidget).LayoutInfo as ParagraphLayoutInfo).Margins.Left;
                                }
                                else if (currTextRange != null)
                                {
                                    WParagraph para = currTextRange.CharacterFormat.BaseFormat.OwnerBase as WParagraph;
                                    if (para != null)
                                    {
                                        rightIndent = (float)((para as IWidget).LayoutInfo as ParagraphLayoutInfo).Margins.Right;
                                        leftIndent = (float)((para as IWidget).LayoutInfo as ParagraphLayoutInfo).Margins.Left;
                                    }
                                }
                            }
                            rightIndent = rightIndent < 0 ? Math.Abs(rightIndent) : 0;
                            if (rect.X >= textWrappingBounds.X && rect.X < textWrappingBounds.Right
                                      && textWrappingType != TextWrappingType.Left) // Skip to update when the wrap type as left
                            {
                                rect.Width = rect.Width - (textWrappingBounds.Right - rect.X) - rightIndent;
                                m_isWrapText = true;
                                //checks minimum width
                                if (Math.Round(rect.Width) < DEF_MIN_WIDTH || (rect.Width < size.Width && (leafWidget.LayoutInfo as TabsLayoutInfo) != null)
                                    || (textWrappingBounds.X <
                                    (GetOwnerParagraph().m_layoutInfo as ParagraphLayoutInfo).XPosition + GetPararaphLeftIndent())) // check whether the TextWrap X position is less than the paragraph X position
                                {
                                    rect.Width = m_layoutArea.ClientActiveArea.Right - textWrappingBounds.Right;
                                    //checks minimum width of the single word
                                    float minwidth = 0;
                                    if (currTextRange != null)
                                        minwidth = GetMinWidth(currTextRange, size, rect);
                                    else
                                        minwidth = size.Width;
                                    if (Math.Round(rect.Width) < DEF_MIN_WIDTH)
                                    {
                                        rect.Y = textWrappingBounds.Bottom;
                                        m_isYPositionUpdated = true;
                                        rect.Width = m_layoutArea.ClientArea.Width;
                                        rect.Height = rect.Height - textWrappingBounds.Height;
                                        CreateLayoutArea(rect);
                                        m_isWrapText = false;
                                    }
                                    else
                                    {
                                        rect.X = textWrappingBounds.Right;
                                        m_isXPositionUpdated = true;
                                        m_isWrapText = true;
                                        CreateLayoutArea(rect);
                                        if ((m_lcOperator as Layouter).FloatingItems[i].FloatingEntity is WTable)
                                            (m_lcOperator as Layouter).FloatingTableBottom = textWrappingBounds.Bottom;
                                    }
                                }
                                else
                                {
                                    rect.X = textWrappingBounds.Right;
                                    m_isXPositionUpdated = true;
                                    m_isWrapText = true;
                                    CreateLayoutArea(rect);
                                }
                            }
                            else if (textWrappingBounds.X >= rect.X && rect.Right > textWrappingBounds.X)
                            {
                                rect.Width = textWrappingBounds.X - rect.X - rightIndent;
                                //Remaining right side width
                                float remainingClientWidth = m_layoutArea.ClientActiveArea.Right - textWrappingBounds.Right;
                                m_isWrapText = true;

                                //checks minimum width
                                float minwidth = 0;
                                if (currTextRange != null)
                                    minwidth = GetMinWidth(currTextRange, size, rect);
                                else
                                    minwidth = size.Width;
                                if (remainingClientWidth < DEF_MIN_WIDTH || remainingClientWidth < minwidth) // Check whether the text wrap have minimum width in right side
                                    m_isWrapText = false;
                                if ((remainingClientWidth > DEF_MIN_WIDTH
                                    && (((Math.Round(rect.Width, 2) <= Math.Round(minwidth, 2)
                                    || (rect.Width < size.Width && (leafWidget.LayoutInfo as TabsLayoutInfo) != null))
                                    && textWrappingType != TextWrappingType.Left                 // Skip to update width when the wrap type as left
                                    && textWrappingType != TextWrappingType.Largest)           
                                    || textWrappingType == TextWrappingType.Right            //To layout right side when the wrap type as right
                                    || (rect.Width < remainingClientWidth && textWrappingType == TextWrappingType.Largest))) // Check whether the right side width is greater than the left side when the wrap type as largest
                                    || ((textWrappingBounds.X -
                                    (paragraph.m_layoutInfo as ParagraphLayoutInfo).XPosition + GetPararaphLeftIndent() < DEF_MIN_WIDTH)    // Check whether the left side of text wrap object is have minimum width to layout or not
                                    && (textWrappingType != TextWrappingType.Left || remainingClientWidth < DEF_MIN_WIDTH)))
                                {
                                    rect.Width = remainingClientWidth;
                                    m_isWrapText = true;
                                    if (rect.X + minwidth > textWrappingBounds.X
                                        || textWrappingType == TextWrappingType.Right
                                        || textWrappingType == TextWrappingType.Largest)       //Update X position when the wrap type as largest or right or the minimum width + rect.X > wrap x position
                                    {
                                        rect.X = textWrappingBounds.Right;
                                        m_isXPositionUpdated = true;
                                        if (rect.Width > minwidth || textWrappingType == TextWrappingType.Right
                                            || textWrappingType == TextWrappingType.Largest)
                                            CreateLayoutArea(rect);
                                    }
                                    //Reset the rectangle position when it doesn't have width to layout the cionetent in current line and the text wrap type must be bothsides.
                                    if (rect.Width < DEF_MIN_WIDTH || (rect.Width < minwidth && rect.Right == (m_lcOperator as Layouter).ClientLayoutArea.Right
                                        && textWrappingType == TextWrappingType.Both))
                                    {
                                        if (Math.Round(rect.X, 2) == Math.Round(GetPageMarginLeft() + GetPararaphLeftIndent(), 2))
                                        {
                                            rect.Y = textWrappingBounds.Bottom;
                                            m_isYPositionUpdated = true;
                                            rect.Width = m_layoutArea.ClientArea.Width;
                                            rect.Height = rect.Height - textWrappingBounds.Height;
                                            CreateLayoutArea(rect);
                                            m_isWrapText = false;
                                        }
                                            // Reset the rectangle position when the rectangle right position is equialent to layout area right position
                                        else if (rect.Right == (m_lcOperator as Layouter).ClientLayoutArea.Right && textWrappingType == TextWrappingType.Both)
                                        {
                                            rect.Y = textWrappingBounds.Bottom;
                                            rect.Width = (m_lcOperator as Layouter).ClientLayoutArea.Width;
                                            rect.Height = rect.Height - textWrappingBounds.Height;
                                            rect.X = (m_lcOperator as Layouter).ClientLayoutArea.X;
                                            CreateLayoutArea(rect);
                                            m_isXPositionUpdated = true;
                                            m_isYPositionUpdated = true;
                                            m_isWrapText = false;
                                        }
                                        else
                                        {
                                            rect.Width = 0;
                                            CreateLayoutArea(rect);
                                        }
                                    }
                                }
                                else
                                {
                                    if (Math.Round(rect.Width, 2) <= Math.Round(minwidth, 2) && Math.Round(rect.X - leftIndent, 2) != Math.Round((m_lcOperator as Layouter).ClientLayoutArea.X, 2))
                                        rect.Width = 0;
                                    CreateLayoutArea(rect);
                                }
                            }
                            else if (rect.X > textWrappingBounds.X && rect.X > textWrappingBounds.Right && textWrappingType != TextWrappingType.Left)
                            {
                                rect.Width = m_layoutArea.ClientArea.Width;
                                CreateLayoutArea(rect);
                                m_isWrapText = true;
                            }
                            else if (rect.X > textWrappingBounds.X && rect.X < textWrappingBounds.Right
                                && textWrappingType != TextWrappingType.Left)
                            {
                                rect.Width = rect.Width - (textWrappingBounds.Right - rect.X);
                                rect.X = textWrappingBounds.Right;
                                CreateLayoutArea(rect);
                                m_isXPositionUpdated = true;
                                m_isWrapText = true;
                            }
                            if (textWrappingType != TextWrappingType.Both)
                                m_isWrapText = false;
                        }
                        else if ((m_lcOperator as Layouter).FloatingItems.Count > 0
                            && wrapOwnerIndex != i
                            && wrapItemIndex != i
                            && ((rect.Y >= textWrappingBounds.Y
                            && rect.Y < (textWrappingBounds.Bottom))
                            || ((rect.Y + size.Height >= textWrappingBounds.Y)
                            && (rect.Y + size.Height < (textWrappingBounds.Bottom))))
                            && textWrappingStyle == TextWrappingStyle.TopAndBottom
                            && !(allowOverlap && (isTextRangeInTextBox || (leafWidget is WPicture && (leafWidget as WPicture).TextWrappingStyle != TextWrappingStyle.Inline && (leafWidget as WPicture).AllowOverlap) 
                            || (leafWidget is Shape && (leafWidget as Shape).WrapFormat.TextWrappingStyle != TextWrappingStyle.Inline && (leafWidget as Shape).WrapFormat.AllowOverlap))))
                        {
                            rect.Y = textWrappingBounds.Bottom;
                            m_isYPositionUpdated = true;
                            rect.Height = rect.Height - textWrappingBounds.Height;
                            CreateLayoutArea(rect);
                        }
                    }
                }
            }
            #endregion
        }
        /// <summary>
        /// Get Minium width of the word
        /// </summary>
        /// <returns></returns>
        private float GetMinWidth(WTextRange currTextRange, SizeF size, RectangleF rect)
        {
            string[] split = GetText().Split(' ');
            float minwidth = DrawingContext.MeasureTextRange(currTextRange, split[0]).Width;
            WTextRange nextSibling = GetNextSibling(currTextRange) as WTextRange;
            if (split.Length == 1 && nextSibling != null)
            {
                string nextSiblingText = nextSibling.Text;
                minwidth += GetNextTextRangeWidth(nextSibling, ref nextSiblingText, size, rect);
            }
            return minwidth;
        }
        /// <summary>
        /// Check whether the leafwidget owner is textbox or not
        /// </summary>
        private bool IsLeafWidgetOwnerIsTextBox()
        {
            //Get owner Paragraph of leafWidget
            Entity ent = GetOwnerParagraph() as Entity;
            while (ent != null)
            {
                if (ent.EntityType == EntityType.HeaderFooter || ent.EntityType == EntityType.Section || ent.Owner == null)
                    return false;
                else
                    ent = ent.Owner;
                //check whether the Leafwidget is textbox and wrapping style is 'InfrontofText' or 'behind'
                if (ent is WTable && (ent as WTable).m_isTextBox
                    && ((ent as WTable).m_textBoxFormat.TextWrappingStyle == TextWrappingStyle.InFrontOfText
                    || (ent as WTable).m_textBoxFormat.TextWrappingStyle == TextWrappingStyle.Behind))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Does the layout after.
        /// </summary>
        protected override void DoLayoutAfter()
        {
            FieldLayoutInfo fieldInfo = LayoutInfo as FieldLayoutInfo;
            bool cont = (fieldInfo != null) ? (fieldInfo.FieldType > -1) : false;

            if (cont && m_ltWidget != null)
            {
                m_lcOperator.SendLeafLayoutAfter(m_ltWidget);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Fits the widget.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="widget">The widget.</param>
        private void FitWidget(SizeF size, IWidget widget, bool isLastWordFit)
        {
            if ((widget.LayoutInfo as TabsLayoutInfo) != null)
                (m_lcOperator as Layouter).PreviousTab = (widget.LayoutInfo as TabsLayoutInfo).m_currTab;
            bool IsPageOrigin = false;
            double width = size.Width + LayoutInfo.Paddings.Left + LayoutInfo.Paddings.Right;
            double height = size.Height + LayoutInfo.Paddings.Top + LayoutInfo.Paddings.Bottom;
            double indentX = 0f;
            double indentY = 0f;
            bool IsPictureAligned = false;
            ParagraphItem paraItem = (LeafWidget is WOleObject) ? ((LeafWidget as WOleObject).OlePicture as WPicture) as ParagraphItem : LeafWidget as ParagraphItem;
            Shape shape = paraItem as Shape;
            WPicture pic = paraItem as WPicture;
            #region Absolute Position of Picture
            if (paraItem is WPicture || paraItem is Shape)
            {
                IsPictureAligned = true;
                float leftMargin = 0.0f, topMargin = 0.0f, bottomMargin = 0.0f, headerDistance = 0.0f, footerDistance = 0.0f, rightMargin = 0.0f;
                WSection currentSection = null;
                float pageWidth = 0.0f;
                float pageHeight = 0.0f;
                float pageClientWidth = 0.0f, pageClientHeight = 0.0f;
                bool isSingleColumn = true;
                WParagraph ownerPara = paraItem.GetOwnerParagraph();
                if (paraItem.Owner != null)
                {
                    Entity ent = paraItem.Owner as Entity;
                    while (!(ent is WSection))
                    {
                        if (ent is WTable && (ent as WTable).m_isTextBox)
                            ent = (ent as WTable).m_textBoxFormat.OwnerBase as WTextBox;
                        if (ent.Owner == null)
                            break;
                        else
                            ent = ent.Owner as Entity;
                    }
                    if (ent is WSection)
                    {
                        currentSection = ent as WSection;
                        leftMargin = currentSection.PageSetup.Margins.Left;
                        rightMargin = currentSection.PageSetup.Margins.Right;
                        topMargin = ((currentSection.PageSetup.Margins.Top > 0) ? currentSection.PageSetup.Margins.Top : 36);
                        bottomMargin = ((currentSection.PageSetup.Margins.Bottom > 0) ? currentSection.PageSetup.Margins.Bottom : 36);
                        pageHeight = currentSection.PageSetup.PageSize.Height;
                        pageWidth = currentSection.PageSetup.PageSize.Width;
                        pageClientWidth = currentSection.PageSetup.ClientWidth;
                        footerDistance = currentSection.PageSetup.FooterDistance;
                        headerDistance = currentSection.PageSetup.HeaderDistance;
                        pageClientHeight = (float)currentSection.PageSetup.PageSize.Height - topMargin - bottomMargin;
                        if (currentSection.Columns.Count > 1)
                            isSingleColumn = false;
                    }
                }
                TextWrappingStyle textWrapStyle = shape != null ? shape.WrapFormat.TextWrappingStyle : pic.TextWrappingStyle;

                if (textWrapStyle != TextWrappingStyle.Inline)
                {
                    bool isLayoutInCell = false;
                    VerticalOrigin vertOrigin = shape != null ? shape.VerticalOrigin : pic.VerticalOrigin;
                    HorizontalOrigin horzOrigin = shape != null ? shape.HorizontalOrigin : pic.HorizontalOrigin;
                    ShapeHorizontalAlignment horzAlignment = shape != null ? shape.HorizontalAlignment : pic.HorizontalAlignment;
                    ShapeVerticalAlignment vertAlignment = shape != null ? shape.VerticalAlignment : pic.VerticalAlignment;
                    float shapeHeight = shape != null ? shape.Height : pic.Height;
                    float shapeWidth = shape != null ? shape.Width : pic.Width;
                    float vertPosition = shape != null ? shape.VerticalPosition : pic.VerticalPosition;
                    float horzPosition = shape != null ? shape.HorizontalPosition : pic.HorizontalPosition;
                    bool layoutInCell = shape != null ? shape.LayoutInCell : pic.LayoutInCell;
                    if (ownerPara.IsInCell && layoutInCell)
                    {
                        isLayoutInCell = true;
                        indentY = GetVerticalPosition(paraItem, vertPosition, vertOrigin, textWrapStyle);
                        indentX = GetHorizontalPosition(size, paraItem, horzAlignment, horzOrigin, horzPosition, textWrapStyle);
                    }
                    else
                    {
                        if (m_isYPositionUpdated)//Upadte the Y Coordinate of floating image when floating image postion is changed based on the wrapping style.
                        {
                            indentY = m_layoutArea.ClientArea.Y;
                        }
                        else
                        {
                            switch (vertOrigin)
                            {
                                case VerticalOrigin.Page:
                                case VerticalOrigin.TopMargin:
                                    {
                                        indentY = vertPosition;
                                        switch (vertAlignment)
                                        {
                                            case ShapeVerticalAlignment.Top:
                                                indentY = vertPosition;
                                                break;
                                            case ShapeVerticalAlignment.Center:
                                                indentY = (pageHeight - shapeHeight) / 2;
                                                break;
                                            case ShapeVerticalAlignment.Bottom:
                                                indentY = (pageHeight - shapeHeight);
                                                break;
                                            case ShapeVerticalAlignment.None:
                                                break;
                                        }
                                    }
                                    break;
                                case VerticalOrigin.Paragraph:
                                    {
                                        indentY = m_layoutArea.ClientArea.Y + vertPosition;
                                    }
                                    break;
                                case VerticalOrigin.Margin:
                                    {
                                        indentY = topMargin + vertPosition;
                                        switch (vertAlignment)
                                        {
                                            case ShapeVerticalAlignment.Top:
                                                indentY = topMargin;
                                                break;
                                            case ShapeVerticalAlignment.Center:
                                                indentY = topMargin + (pageClientHeight - shapeHeight) / 2;
                                                break;
                                            case ShapeVerticalAlignment.Bottom:
                                                indentY = topMargin + pageClientHeight - shapeHeight;
                                                break;
                                            case ShapeVerticalAlignment.None:
                                                break;
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        indentY = m_layoutArea.ClientArea.Y - LayoutInfo.Paddings.Top + vertPosition;
                                    }
                                    break;
                            }
                        }
                        if (m_isXPositionUpdated //Upadte the Y Coordinate of floating image when floating image
                            && horzOrigin != HorizontalOrigin.Column// postion is changed based on the wrapping style.
                            && horzAlignment != ShapeHorizontalAlignment.None)
                            indentX = m_layoutArea.ClientArea.X;
                        else
                        {
                            switch (horzOrigin)
                            {
                                case HorizontalOrigin.Page:
                                    {
                                        indentX = horzPosition;
                                        switch (horzAlignment)
                                        {
                                            case ShapeHorizontalAlignment.Center:
                                                if (isLayoutInCell)
                                                    indentX = (DrawingContext.GetCellWidth(paraItem) - shapeWidth) / 2;
                                                else
                                                    indentX = (pageWidth - shapeWidth) / 2;
                                                break;
                                            case ShapeHorizontalAlignment.Left:
                                                indentX = 0.0f;
                                                break;
                                            case ShapeHorizontalAlignment.Right:
                                                if (isLayoutInCell)
                                                    indentX = DrawingContext.GetCellWidth(paraItem) - shapeWidth;
                                                else
                                                    indentX = pageWidth - shapeWidth;
                                                break;
                                            case ShapeHorizontalAlignment.None:
                                                if (isLayoutInCell)
                                                {
                                                    ILayoutSpacingsInfo spacings = (ownerPara.OwnerTextBody as WTableCell).m_layoutInfo as ILayoutSpacingsInfo;
                                                    indentX = ((ownerPara.OwnerTextBody as WTableCell).m_layoutInfo as TableLayoutInfo).TableCellLeftMargin - spacings.Margins.Left - spacings.Paddings.Left + horzPosition;
                                                }
                                                else
                                                    indentX = horzPosition;
                                                break;
                                        }
                                        if (indentX < 0 && isLayoutInCell)
                                        {
                                            ILayoutSpacingsInfo spacings = (ownerPara.OwnerTextBody as WTableCell).m_layoutInfo as ILayoutSpacingsInfo;
                                            indentX = ((ownerPara.OwnerTextBody as WTableCell).m_layoutInfo as TableLayoutInfo).TableCellLeftMargin - spacings.Margins.Left - spacings.Paddings.Left;
                                        }
                                    }
                                    break;
                                case HorizontalOrigin.Column:
                                    {
                                        if (!ownerPara.IsXpositionUpated)//Update the Xposition while wrapping element exsit in the paragraph
                                            indentX = (ownerPara.m_layoutInfo as ParagraphLayoutInfo).XPosition + horzPosition;
                                        else
                                            indentX = (m_lcOperator as Layouter).ClientLayoutArea.Left + horzPosition;
                                        switch (horzAlignment)
                                        {
                                            case ShapeHorizontalAlignment.Center:
                                                indentX = (m_lcOperator as Layouter).ClientLayoutArea.Left + ((m_lcOperator as Layouter).ClientLayoutArea.Width - shapeWidth) / 2;
                                                break;
                                            case ShapeHorizontalAlignment.Left:
                                                indentX = (m_lcOperator as Layouter).ClientLayoutArea.Left;
                                                break;
                                            case ShapeHorizontalAlignment.Right:
                                                indentX = (m_lcOperator as Layouter).ClientLayoutArea.Left + (m_lcOperator as Layouter).ClientLayoutArea.Width - shapeWidth;//- TextBoxFormat.InternalMargin.Right;
                                                break;
                                            case ShapeHorizontalAlignment.None:
                                                break;
                                        }
                                    }
                                    break;
                                case HorizontalOrigin.Margin:
                                    {
                                        if (currentSection != null)
                                        {
                                            indentX = leftMargin + horzPosition;
                                            switch (horzAlignment)
                                            {
                                                case ShapeHorizontalAlignment.Center:
                                                    indentX = leftMargin + (pageClientWidth - shapeWidth) / 2;
                                                    break;
                                                case ShapeHorizontalAlignment.Left:
                                                    indentX = leftMargin;
                                                    break;
                                                case ShapeHorizontalAlignment.Right:
                                                    indentX = leftMargin + pageClientWidth - shapeWidth;//- TextBoxFormat.InternalMargin.Right;
                                                    break;
                                                case ShapeHorizontalAlignment.None:
                                                    break;
                                            }
                                        }
                                        else
                                            indentX = m_layoutArea.ClientArea.X + horzPosition;
                                    }
                                    break;
                                case HorizontalOrigin.LeftMargin:
                                    indentX = GetLeftMarginHorizPosition(leftMargin, horzAlignment, horzPosition, shapeWidth, textWrapStyle);
                                    break;
                                case HorizontalOrigin.RightMargin:
                                    indentX = GetRightMarginHorizPosition(pageWidth, rightMargin, horzAlignment, horzPosition, shapeWidth, textWrapStyle);
                                    break;
                                case HorizontalOrigin.InsideMargin:
                                    if ((m_lcOperator as Layouter).CurrPageIndex % 2 == 0)
                                        indentX = GetRightMarginHorizPosition(pageWidth, rightMargin, horzAlignment, horzPosition, shapeWidth, textWrapStyle);
                                    else
                                        indentX = GetLeftMarginHorizPosition(leftMargin, horzAlignment, horzPosition, shapeWidth, textWrapStyle);
                                    break;
                                case HorizontalOrigin.OutsideMargin:
                                    if ((m_lcOperator as Layouter).CurrPageIndex % 2 == 0)
                                        indentX = GetLeftMarginHorizPosition(leftMargin, horzAlignment, horzPosition, shapeWidth, textWrapStyle);
                                    else
                                        indentX = GetRightMarginHorizPosition(pageWidth, rightMargin, horzAlignment, horzPosition, shapeWidth, textWrapStyle);
                                    break;
                                default:
                                    {
                                        indentX = m_layoutArea.ClientArea.X + horzPosition;
                                    }
                                    break;
                            }
                        }
                    }
                }
                else
                    IsPictureAligned = false;
            }
            #endregion
            //Skip to update the widget when current textrange fit on the end of the line 
            if (!isLastWordFit)
                width = UpdateLeafWidgetWidth(width, widget);
            ParagraphItem item = widget as ParagraphItem;
            if (widget is SplitStringWidget)
                item = (widget as SplitStringWidget).RealStringWidget as ParagraphItem;
            WParagraph ownerParagraph = (item as ParagraphItem).OwnerParagraph as WParagraph;
            if ((item as ParagraphItem).Owner is SDTInlineContent)
                ownerParagraph = (item as ParagraphItem).GetOwnerParagraph();
            if (((pic != null || shape != null) ? ownerParagraph.IsInCell && IsLeafWidgetHeightNeedToBeUpdate(paraItem) : true) && height > m_layoutArea.ClientArea.Height)
                height = m_layoutArea.ClientArea.Height;
            if (LayoutInfo.IsVerticalText && pic != null
                && pic.TextWrappingStyle == TextWrappingStyle.Inline
                && height > m_layoutArea.ClientArea.Width)
            {
                ILayoutInfo layoutInfo = (ownerParagraph.OwnerTextBody as WTableCell).m_layoutInfo;
                height = m_layoutArea.ClientArea.Width
                    + ownerParagraph.m_layoutInfo.Margins.Right - (2 * (layoutInfo.Margins.Top + layoutInfo.Margins.Bottom))
                - (layoutInfo.Margins.Left + layoutInfo.Margins.Right);
            }


            m_ltWidget = new LayoutedWidget(widget);
            if (!IsPictureAligned)
                m_ltWidget.Bounds = new RectangleF(
                  (float)(m_layoutArea.ClientArea.X - LayoutInfo.Paddings.Left + indentX),
                  (float)(m_layoutArea.ClientArea.Y - LayoutInfo.Paddings.Top + indentY),
                  (float)width, (float)height);
            else
                m_ltWidget.Bounds = new RectangleF(
              (float)(indentX - LayoutInfo.Paddings.Left),
              (float)(indentY - LayoutInfo.Paddings.Top),
              (float)width, (float)height);
            UpdateSkipLeftPosition();
            m_ltWidget.PrevTabJustification = (m_lcOperator as Layouter).PreviousTab.Justification;
            //for skip the subwidth update to zero when the isLastWordFit flag is true for the justifed line.  
            if (isLastWordFit)
                m_ltWidget.TextTag = "IsLastWordFit";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageWidth"></param>
        /// <param name="rightMargin"></param>
        /// <param name="horzAlignment"></param>
        /// <param name="horzPosition"></param>
        /// <param name="shapeWidth"></param>
        /// <param name="textWrapStyle"></param>
        /// <returns></returns>
        private float GetRightMarginHorizPosition(float pageWidth, float rightMargin, ShapeHorizontalAlignment horzAlignment, float horzPosition, float shapeWidth, TextWrappingStyle textWrapStyle)
        {
            float xPosition = pageWidth - rightMargin;
            float indentX = xPosition + horzPosition;
            switch (horzAlignment)
            {
                case ShapeHorizontalAlignment.Center:
                    indentX = xPosition + (rightMargin - shapeWidth) / 2;
                    break;
                case ShapeHorizontalAlignment.Left:
                    indentX = xPosition;
                    break;
                case ShapeHorizontalAlignment.Right:
                    indentX = pageWidth - shapeWidth;
                    break;
                case ShapeHorizontalAlignment.None:
                    break;
            }
            if ((indentX < 0 || indentX + shapeWidth > pageWidth) && textWrapStyle != TextWrappingStyle.InFrontOfText && textWrapStyle != TextWrappingStyle.Behind)
                indentX = pageWidth - shapeWidth;
            return indentX;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftMargin"></param>
        /// <param name="horzAlignment"></param>
        /// <param name="horzPosition"></param>
        /// <param name="shapeWidth"></param>
        /// <param name="textWrapStyle"></param>
        /// <returns></returns>
        private float GetLeftMarginHorizPosition(float leftMargin, ShapeHorizontalAlignment horzAlignment, float horzPosition, float shapeWidth, TextWrappingStyle textWrapStyle)
        {
            float indentX = horzPosition;
            switch (horzAlignment)
            {
                case ShapeHorizontalAlignment.Center:
                    indentX = (leftMargin - shapeWidth) / 2;
                    break;
                case ShapeHorizontalAlignment.Left:
                    indentX = 0;
                    break;
                case ShapeHorizontalAlignment.Right:
                    indentX = leftMargin - shapeWidth;
                    break;
                case ShapeHorizontalAlignment.None:
                    break;
            }
            if (indentX < 0 && textWrapStyle != TextWrappingStyle.InFrontOfText && textWrapStyle != TextWrappingStyle.Behind)
                indentX = 0;
            return indentX;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paraItem"></param>
        /// <param name="ownerParagraph"></param>
        /// <returns></returns>
        private bool IsLeafWidgetHeightNeedToBeUpdate(ParagraphItem paraItem)
        {
            bool isPictureNeedToClip = (paraItem is WPicture) ? ((paraItem as WPicture).LayoutInCell
                                       || (paraItem as WPicture).TextWrappingStyle == TextWrappingStyle.Inline) : false;
            bool isShapeNeedToClip = (paraItem is Shape) ? ((paraItem as Shape).LayoutInCell
                                      || (paraItem as Shape).WrapFormat.TextWrappingStyle == TextWrappingStyle.Inline) : false;
            return (isPictureNeedToClip || isShapeNeedToClip);
        }
        /// <summary>
        /// Get vertical position of the picture
        /// </summary>
        /// <param name="picture"></param>
        /// <returns></returns>
        private double GetVerticalPosition(ParagraphItem paraItem, float vertPosition, VerticalOrigin vertOrigin, TextWrappingStyle textWrapStyle)
        {
            double indentY = 0;
            double topMargin = ((paraItem.GetOwnerParagraph().OwnerTextBody as WTableCell).m_layoutInfo as TableLayoutInfo).TableCellTopMargin;
            switch (vertOrigin)
            {
                case VerticalOrigin.Page:
                case VerticalOrigin.TopMargin:
                case VerticalOrigin.Margin:
                    {
                        indentY = topMargin + vertPosition;
                    }
                    break;
                case VerticalOrigin.Paragraph:
                    indentY = (paraItem.GetOwnerParagraph().m_layoutInfo as ParagraphLayoutInfo).YPosition + vertPosition;
                    break;
                default:
                    indentY = m_layoutArea.ClientActiveArea.Y + vertPosition;
                    break;
            }
            if (indentY < topMargin//Check whether the current picture wrapping style infrontoftext or behindtext 
                && textWrapStyle != TextWrappingStyle.InFrontOfText//just skip the picture y position updation 
                && textWrapStyle != TextWrappingStyle.Behind)//when picture y position less than the page top margin
                indentY = topMargin;
            return indentY;
        }
        /// <summary>
        /// Get Horizontal position of the picture
        /// </summary>
        /// <param name="picture"></param>
        /// <returns></returns>
        private double GetHorizontalPosition(SizeF size, ParagraphItem paraItem, ShapeHorizontalAlignment horzAlignment, HorizontalOrigin horzOrigin, float horzPosition, TextWrappingStyle textWrapStyle)
        {
            double indentX = 0;
            ILayoutSpacingsInfo spacings = (paraItem.GetOwnerParagraph().OwnerTextBody as WTableCell).m_layoutInfo as ILayoutSpacingsInfo;
            double cellWidth = DrawingContext.GetCellWidth(paraItem) + spacings.Paddings.Left + spacings.Paddings.Right;
            double leftMargin = ((paraItem.GetOwnerParagraph().OwnerTextBody as WTableCell).m_layoutInfo as TableLayoutInfo).TableCellLeftMargin - spacings.Paddings.Left;
            switch (horzOrigin)
            {
                case HorizontalOrigin.Page:
                    {
                        indentX = horzPosition;
                        switch (horzAlignment)
                        {
                            case ShapeHorizontalAlignment.Center:
                                indentX = leftMargin + (cellWidth - size.Width) / 2;
                                break;
                            case ShapeHorizontalAlignment.Left:
                                indentX = leftMargin;
                                break;
                            case ShapeHorizontalAlignment.Right:
                                indentX = leftMargin + (cellWidth - size.Width);
                                break;
                            case ShapeHorizontalAlignment.None:
                                indentX = leftMargin + horzPosition;
                                break;
                        }
                    }
                    break;
                case HorizontalOrigin.Column:
                case HorizontalOrigin.Margin:
                    {
                        switch (horzAlignment)
                        {
                            case ShapeHorizontalAlignment.Center:
                                indentX = leftMargin + spacings.Paddings.Left + (cellWidth - spacings.Paddings.Left - spacings.Paddings.Right - size.Width) / 2;
                                break;
                            case ShapeHorizontalAlignment.Left:
                                indentX = leftMargin + spacings.Paddings.Left;
                                break;
                            case ShapeHorizontalAlignment.Right:
                                indentX = leftMargin + spacings.Paddings.Left + (cellWidth - spacings.Paddings.Left - spacings.Paddings.Right - size.Width);
                                break;
                            case ShapeHorizontalAlignment.None:
                                indentX = leftMargin + spacings.Paddings.Left + horzPosition;
                                break;
                        }
                    }
                    break;
                default:
                    {
                        indentX = leftMargin + spacings.Paddings.Left + horzPosition;
                    }
                    break;
            }
            if (textWrapStyle != TextWrappingStyle.InFrontOfText && textWrapStyle != TextWrappingStyle.Behind &&
                (indentX < leftMargin || cellWidth < size.Width))
                indentX = leftMargin;
            return indentX;
        }
        /// <summary>
        /// Update Skip Left position
        /// </summary>
        private void UpdateSkipLeftPosition()
        {
            if (m_ltWidget.Widget is WTextRange
                && (m_ltWidget.Widget as WTextRange).OwnerParagraph != null
                && (m_ltWidget.Widget as WTextRange).OwnerParagraph.IsInCell)
            {
                WParagraphFormat paraFormat = (Widget as WTextRange).OwnerParagraph.ParagraphFormat;
                WTableCell tableCell = (Widget as WTextRange).OwnerParagraph.Owner as WTableCell;
                ILayoutInfo layoutInfo = (tableCell as IWidget).LayoutInfo;
                if (m_ltWidget.Bounds.X < (layoutInfo as TableLayoutInfo).TableCellLeftMargin)
                {
                    float cellSpacing = 0;
                    if (tableCell.OwnerRow.OwnerTable.TableFormat.CellSpacing > 0)
                        cellSpacing = tableCell.OwnerRow.OwnerTable.TableFormat.CellSpacing * 2;
                    float leftPadding = (float)(layoutInfo.Paddings.Left + layoutInfo.Margins.Left - cellSpacing);
                    m_ltWidget.SkipLeftPosition = (layoutInfo as TableLayoutInfo).TableCellLeftMargin - m_ltWidget.Bounds.X - leftPadding;
                }
            }
        }
        /// <summary>
        /// Update Width of the LeafWidget
        /// </summary>
        /// <param name="width"></param>
        /// <param name="widget"></param>
        /// <returns></returns>
        private double UpdateLeafWidgetWidth(double width, IWidget widget)
        {
            ParagraphItem item = widget as ParagraphItem;
            if (widget is SplitStringWidget)
                item = (widget as SplitStringWidget).RealStringWidget as ParagraphItem;
            WParagraph ownerParagraph = (item as ParagraphItem).OwnerParagraph as WParagraph;
            if ((item as ParagraphItem).Owner is SDTInlineContent)
                ownerParagraph = (item as ParagraphItem).GetOwnerParagraph();
            WPicture picture = (LeafWidget is WOleObject) ? ((LeafWidget as WOleObject).OlePicture as WPicture) : LeafWidget as WPicture;
            Shape shape = LeafWidget as Shape;
            if (picture == null
                && shape == null
                && width > m_layoutArea.ClientArea.Width
                && !(!ownerParagraph.IsInCell && IsTextRangeNeedToFit())
                && (((m_lcOperator as Layouter).PreviousTab.Justification != TabJustification.Right
                && (m_lcOperator as Layouter).PreviousTab.Justification != TabJustification.Centered)
                || (IsLeafWidgetIsInCell(item)
                && (m_lcOperator as Layouter).PreviousTab.Justification == TabJustification.Right)))
            {
                if (ownerParagraph.IsInCell)
                    return m_layoutArea.ClientActiveArea.Width + (ownerParagraph.Owner as WTableCell).m_layoutInfo.Paddings.Right;
                else
                    return m_layoutArea.ClientArea.Width;
            }
            else if (((picture != null
                && picture.TextWrappingStyle != TextWrappingStyle.InFrontOfText && picture.TextWrappingStyle != TextWrappingStyle.Behind)
                || (shape != null
                && shape.WrapFormat.TextWrappingStyle != TextWrappingStyle.InFrontOfText && shape.WrapFormat.TextWrappingStyle != TextWrappingStyle.Behind))
                && ownerParagraph.OwnerTextBody is WTableCell)
            {
                if (width > m_layoutArea.ClientActiveArea.Width + (ownerParagraph.Owner as WTableCell).m_layoutInfo.Paddings.Right
                   && !LayoutInfo.IsVerticalText)
                    return (width > (ownerParagraph.Owner as WTableCell).Width) ? (ownerParagraph.Owner as WTableCell).Width : width;
                else if (width > m_layoutArea.ClientActiveArea.Height + (ownerParagraph.Owner as WTableCell).m_layoutInfo.Paddings.Bottom
                   && LayoutInfo.IsVerticalText)
                    return m_layoutArea.ClientArea.Height + (ownerParagraph.Owner as WTableCell).m_layoutInfo.Paddings.Bottom;
            }

            return width;
        }
        /// <summary>
        /// Splits up widget.
        /// </summary>
        /// <param name="splitLeafWidget">The split leaf widget.</param>
        private void SplitUpWidget(ISplitLeafWidget splitLeafWidget, float clientActiveAreaWidth)
        {
            SizeF size;
            ISplitLeafWidget[] splitedLeafWidgets;
            bool isLastWordFit = false;
            if ((LayoutInfo as TabsLayoutInfo) != null)
            {
                splitedLeafWidgets = new ISplitLeafWidget[] { splitLeafWidget, splitLeafWidget };
            }
            else
            {
                float clientWidth = (m_lcOperator as Layouter).ClientLayoutArea.Width;
                WParagraph ownerPara = GetOwnerParagraph();
                if (ownerPara != null && IsInFrame(ownerPara))
                    clientWidth = (m_lcOperator as Layouter).FrameLayoutArea.Width;
                if (ownerPara != null &&  ownerPara.OwnerTextBody is WTableCell)
                    clientWidth = (ownerPara.OwnerTextBody as WTableCell).Width;
                splitedLeafWidgets = splitLeafWidget.SplitBySize(DrawingContext, m_layoutArea.ClientArea.Size, clientWidth, clientActiveAreaWidth, ref isLastWordFit);
            }

            m_ltState = LayoutState.NotFitted;

            if (splitedLeafWidgets != null)
            {
                size = splitedLeafWidgets[0].Measure(DrawingContext);
                if ((splitedLeafWidgets[0].LayoutInfo as TabsLayoutInfo) != null)
                    size.Width = 0;
                //Skip to update the client area bounds when word fit on the end of the line.
                if (!TryFit(size) && !isLastWordFit)
                {
#if DEBUG_LAYOUTING
          System.Diagnostics.Trace.WriteLine( "Split string not fitted to line" );
#endif
                    size.Width = m_layoutArea.ClientArea.Width;
                }
                FitWidget(size, splitedLeafWidgets[0], isLastWordFit);
                m_sptWidget = splitedLeafWidgets[1];
                m_ltState = LayoutState.Splitted;
            }
        }
        #endregion
    }
}

#endif
