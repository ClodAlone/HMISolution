// <copyright file="Office2007SplitterItemBorder.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents Office 2007 Splitter Item Border control.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class Office2007SplitterItemBorder : SplitterItemBorder
    {
        #region Private members
        /// <summary>
        /// Presents selected outer line
        /// </summary>
        private PathGeometry m_selectedOutterLine;

        /// <summary>
        /// Presents selected selected line
        /// </summary>
        private PathGeometry m_hoverSelectedLine;

        /// <summary>
        /// Presents selected sublime
        /// </summary>
        private PathGeometry m_subLine;

        /// <summary>
        /// Presents selected hover outer line
        /// </summary>
        private PathGeometry m_hoverOutterLine;

        /// <summary>
        /// Presents selected hover inner line
        /// </summary>
        private PathGeometry m_hoverInnerLine;
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the state of the tab.
        /// </summary>
        /// <value>The state of the tab.</value>
        public TabStates TabState
        {
            get
            {
                return (TabStates)GetValue(TabStateProperty);
            }

            set
            {
                SetValue(TabStateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the sub line border brush.
        /// </summary>
        /// <value>The sub line border brush.</value>
        [DefaultValue((string)null)]
        public Brush SublineBorderBrush
        {
            get
            {
                return (Brush)base.GetValue(SublineBorderBrushProperty);
            }

            set
            {
                base.SetValue(SublineBorderBrushProperty, value);
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Arranges the content of a <see cref="T:System.Windows.Controls.Decorator"/> element.
        /// </summary>
        /// <param name="arrangeSize">The <see cref="T:System.Windows.Size"/> this element uses to arrange its child content.</param>
        /// <returns>
        /// The <see cref="T:System.Windows.Size"/> that represents the arranged size of this <see cref="T:System.Windows.Controls.Decorator"/> element and its child.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double awidth = arrangeSize.Width;
            double aheight = arrangeSize.Height;

            Rect arrangeRect = new Rect
            {
                Width = Math.Max(0.0, awidth - 8.0),
                Height = Math.Max(0.0, aheight - 4.0),
                X = Math.Min(aheight, awidth),
                Y = Math.Min(2.0, aheight)
            };

            if (Child != null)
            {
                Child.Arrange(arrangeRect);
            }

            m_selectedOutterLine = new PathGeometry();
            m_hoverSelectedLine = new PathGeometry();
            m_hoverOutterLine = new PathGeometry();
            m_hoverInnerLine = new PathGeometry();
            m_subLine = new PathGeometry();

            PathFigure selectedOutterLineFigure = new PathFigure(
                new Point(0.0, aheight),
                new PathSegment[]  
                {
                  new ArcSegment(new Point(3, aheight - 2), new Size(3, 3), 0.0, false, SweepDirection.Counterclockwise, true), 
                  new LineSegment(new Point(3, 5), true), 
                  new ArcSegment(new Point(8, 0), new Size(5, 5), 0.0, false, SweepDirection.Clockwise, true), 
                  new LineSegment(new Point(awidth - 8, 0), true),                         
                  new ArcSegment(new Point(awidth - 3, 5), new Size(5, 5), 0.0, false, SweepDirection.Clockwise, true),                   
                  new LineSegment(new Point(awidth - 3, aheight - 2), true),   
                  new ArcSegment(new Point(awidth, aheight), new Size(3, 3), 0.0, false, SweepDirection.Counterclockwise, true),     
                }, 
                false);
            
            PathFigure hoverSelectedLineFigure = new PathFigure(
                new Point(0.0, aheight),
                new PathSegment[]  
                {                  
                  new LineSegment(new Point(3, aheight - 3), false), 
                  new LineSegment(new Point(3, 5), true), 
                  new ArcSegment(new Point(8, 0), new Size(5, 5), 0.0, false, SweepDirection.Clockwise, true), 
                  new LineSegment(new Point(awidth - 8, 0), true),                         
                  new ArcSegment(new Point(awidth - 3, 5), new Size(5, 5), 0.0, false, SweepDirection.Clockwise, true),                   
                  new LineSegment(new Point(awidth - 3, aheight - 2), true),                     
                }, 
                false);

            PathFigure hoverOutterLineFigure = new PathFigure(
                new Point(0.0, aheight),
                new PathSegment[]  
                {                  
                  new LineSegment(new Point(3, aheight - 0.5), false), 
                  new LineSegment(new Point(3, 5), true), 
                  new ArcSegment(new Point(8, 0), new Size(5, 5), 0.0, false, SweepDirection.Clockwise, true), 
                  new LineSegment(new Point(awidth - 8, 0), true),                         
                  new ArcSegment(new Point(awidth - 3, 5), new Size(5, 5), 0.0, false, SweepDirection.Clockwise, true),                   
                  new LineSegment(new Point(awidth - 3, aheight), true),                     
                }, 
                false);

            PathFigure hoverInnerLineFigure = new PathFigure(
                new Point(0.0, aheight),
                new PathSegment[]  
                {                  
                  new LineSegment(new Point(4, aheight - 0.5), false), 
                  new LineSegment(new Point(4, 5), true), 
                  new ArcSegment(new Point(8, 1), new Size(4, 4), 0.0, false, SweepDirection.Clockwise, true), 
                  new LineSegment(new Point(awidth - 8, 1), true),                         
                  new ArcSegment(new Point(awidth - 4, 5), new Size(4, 4), 0.0, false, SweepDirection.Clockwise, true),                   
                  new LineSegment(new Point(awidth - 4, aheight), true),                     
                }, 
                false);

            PathFigure subLineFigure = new PathFigure(
                new Point(0.0, aheight),
                new PathSegment[] 
                { 
                  new LineSegment(new Point(4, aheight - 3), false),                     
                  new LineSegment(new Point(4, 5), true),                     
                  new LineSegment(new Point(awidth - 4, 5), false),                    
                  new LineSegment(new Point(awidth - 4, aheight - 3), true),   
                },
                false);

            switch (TabState)
            {
                case TabStates.None:
                    m_hoverOutterLine.Figures.Add(hoverOutterLineFigure);
                    break;
                case TabStates.Hover:
                    m_hoverOutterLine.Figures.Add(hoverOutterLineFigure);
                    m_hoverInnerLine.Figures.Add(hoverInnerLineFigure);
                    break;
                case TabStates.Selected:
                    m_selectedOutterLine.Figures.Add(selectedOutterLineFigure);
                    m_subLine.Figures.Add(subLineFigure);
                    break;
                case TabStates.HoverSelected:
                    m_selectedOutterLine.Figures.Add(selectedOutterLineFigure);
                    m_hoverSelectedLine.Figures.Add(hoverSelectedLineFigure);
                    m_subLine.Figures.Add(subLineFigure);
                    break;
                default:
                    break;
            }

            return arrangeSize;
        }

        /// <summary>
        /// Measures the child element of a <see cref="T:System.Windows.Controls.Decorator"/> to prepare for arranging it during the <see cref="M:System.Windows.Controls.Decorator.ArrangeOverride(System.Windows.Size)"/> pass.
        /// </summary>
        /// <param name="constraint">An upper limit <see cref="T:System.Windows.Size"/> that should not be exceeded.</param>
        /// <returns>
        /// The target <see cref="T:System.Windows.Size"/> of the element.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (Child != null)
            {
                Size size = constraint;
                size.Width = Math.Max(0.0, size.Width - 8.0);
                size.Height = Math.Max(0.0, size.Height - 4.0);
                this.Child.Measure(size);
                Size desiredSize = Child.DesiredSize;
                desiredSize.Width = desiredSize.Width + 8.0;
                desiredSize.Height = desiredSize.Height + 4.0;
                return desiredSize;
            }

            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            Pen pen = null;

            switch (TabState)
            {
                case TabStates.None:
                    pen = new Pen(BorderBrush, 1.0);
                    drawingContext.DrawGeometry(Background, pen, m_hoverOutterLine);
                    break;
                case TabStates.Hover:
                    pen = new Pen(BorderBrush, 1.0);
                    drawingContext.DrawGeometry(null, pen, m_hoverOutterLine);
                    pen = new Pen(BottomBorderBrush, 1.0);
                    drawingContext.DrawGeometry(Background, pen, m_hoverInnerLine);
                    break;
                case TabStates.Selected:
                    pen = new Pen(BorderBrush, 1.0);
                    drawingContext.DrawGeometry(Background, pen, m_selectedOutterLine);
                    pen = new Pen(SublineBorderBrush, 1.0);
                    drawingContext.DrawGeometry(null, pen, m_subLine);
                    break;
                case TabStates.HoverSelected:
                    pen = new Pen(BorderBrush, 1.0);
                    drawingContext.DrawGeometry(Background, pen, m_selectedOutterLine);
                    pen = new Pen(SublineBorderBrush, 1.0);
                    drawingContext.DrawGeometry(null, pen, m_subLine);
                    pen = new Pen(BorderInnerBrush, 1.5);
                    drawingContext.DrawGeometry(null, pen, m_hoverSelectedLine);
                    break;
                default:
                    break;
            }

            if (TabState == TabStates.Selected || TabState == TabStates.HoverSelected)
            {
                drawingContext.DrawLine(new Pen(BottomBorderBrush, 0.4), new Point(2.2, ActualHeight), new Point(ActualWidth - 2.2, ActualHeight));
                drawingContext.DrawLine(new Pen(BottomBorderBrush, 0.8), new Point(1.9, ActualHeight + 0.4), new Point(ActualWidth - 1.9, ActualHeight + 0.4));
                drawingContext.DrawLine(new Pen(BottomBorderBrush, 0.9), new Point(1.4, ActualHeight + 1.0), new Point(ActualWidth - 1.4, ActualHeight + 1.0));
            }

            if (base.VisualXSnappingGuidelines == null)
            {
                double[] array1 = new double[2];
                array1[1] = base.ActualWidth;
                base.VisualXSnappingGuidelines = new DoubleCollection(array1);
            }

            if (base.VisualYSnappingGuidelines == null)
            {
                double[] array2 = new double[2];
                array2[1] = base.ActualHeight;
                base.VisualYSnappingGuidelines = new DoubleCollection(array2);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Draws the geometry.
        /// </summary>
        /// <param name="context">The context value.</param>
        /// <param name="brush">The brush value.</param>
        /// <param name="background">The background value.</param>
        /// <param name="path">The path value.</param>
        private static void DrawGeometry(DrawingContext context, Brush brush, Brush background, PathGeometry path)
        {
            if (null != brush)
            {
                Pen pen = new Pen(brush, 1.0);
                context.DrawGeometry(background, pen, path);
            }
        }
        #endregion

        #region dependency properties
        /// <summary>
        /// TabState DependencyProperty
        /// </summary>
        public static readonly DependencyProperty TabStateProperty = DependencyProperty.Register("TabState", typeof(TabStates), typeof(Office2007SplitterItemBorder), new FrameworkPropertyMetadata(TabStates.None, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
        
        /// <summary>
        /// SublineBorderBrush DependencyProperty
        /// </summary>
        public static readonly DependencyProperty SublineBorderBrushProperty = DependencyProperty.Register("SublineBorderBrush", typeof(Brush), typeof(Office2007SplitterItemBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion
    }
}
