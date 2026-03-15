#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
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

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Selected", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "SelectedMouseOver", GroupName = "CommonStates")]
    public class TabItemAdvBorderVS2010 : TabItemAdvBorder
    {
        #region Private members
        /// <summary>
        /// Used for drawing the border.
        /// </summary>
        private Path selectedOutterLine;

        /// <summary>
        /// Used for drawing the border.
        /// </summary>
        private Path hoverSelectedLine;

        ///// <summary>
        ///// Used for drawing the border.
        ///// </summary>
        //private Path hoverOutterLine;

        /// <summary>
        /// Used for drawing the border.
        /// </summary>
        private Path hoverInnerLine;

        /// <summary>
        /// Used for drawing the border.
        /// </summary>
        private Path bottomLine;

        /// <summary>
        /// Used for drawing the border.
        /// </summary>
        private Path selectedMouseOverOutterLine;

        /// <summary>
        /// Indicates whether mouse pointer is over the border.
        /// </summary>
        private bool isMouseOver = false;

        /// <summary>
        /// Indicates whether tab item parent is selected.
        /// </summary>
        private bool isSelected = false;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes new instance of the TabItemAdvBorderOffice2007 class.
        /// </summary>
        public TabItemAdvBorderVS2010()
        {
            DefaultStyleKey = typeof(TabItemAdvBorderOffice2007);
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Occurs when tab item parent is changed.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnTabParentChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnTabParentChanged(e);
            if (this.TabParent != null)
            {
                this.TabParent.IsSelectedChanged += new PropertyChangedCallback(TabItemParentIsSelectedChanged);
            }

            this.UpdateSelectedState();
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            this.isMouseOver = true;
            this.UpdateVisualState();
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeave"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            this.isMouseOver = false;
            this.UpdateVisualState();
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) 
        /// call System.Windows.Controls.Control.ApplyTemplate() method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.selectedOutterLine = this.GetTemplateChild("SelectedOutterLine") as Path;
            this.hoverSelectedLine = this.GetTemplateChild("HoverSelectedLine") as Path;
            //this.hoverOutterLine = this.GetTemplateChild("HoverOutterLine") as Path;
            this.hoverInnerLine = this.GetTemplateChild("HoverInnerLine") as Path;
            this.bottomLine = this.GetTemplateChild("BottomLine") as Path;
            this.selectedMouseOverOutterLine = this.GetTemplateChild("SelectedMouseOverOutterLine") as Path;
            this.UpdateSelectedState();
        }

        /// <summary>
        /// Initializes paths with data.
        /// </summary>
        private void InitializePaths(Size borderSize)
        {
            borderSize.Width += 0.5;

            if (this.selectedOutterLine != null)
            {
                this.selectedOutterLine.Data = this.GetSelectedOutterLineGeometry(borderSize);
            }

            if (this.hoverSelectedLine != null)
            {
                this.hoverSelectedLine.Data = this.GetHoverSelectedLineGeometry(borderSize);
            }
            if (this.selectedMouseOverOutterLine != null)
            {
                this.selectedMouseOverOutterLine.Data = this.GetHoverSelectedLineGeometry(borderSize);
            }

            //if (this.hoverOutterLine != null)
            //{
            //    this.hoverOutterLine.Data = this.GetHoverOutterLineGeometry(borderSize);
            //}

            if (this.hoverInnerLine != null)
            {
                this.hoverInnerLine.Data = this.GetHoverInnerLineGeometry(borderSize);
            }

            if (this.bottomLine != null)
            {
                this.bottomLine.Data = this.GetBottomLineGeometry(borderSize);
            }
        }

        /// <summary>
        /// Refreshs border paths.
        /// </summary>
        /// <param name="borderSize">Size of the border.</param>
        protected override void RefreshBorderPaths(Size borderSize)
        {
            this.InitializePaths(borderSize);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates the look of the border. 
        /// </summary>
        private void UpdateSelectedState()
        {
            if (this.TabParent != null && this.TabParent.IsSelected)
            {
                this.isSelected = true;
            }
            else
            {
                this.isSelected = false;
            }

            this.UpdateVisualState();
        }

        /// <summary>
        /// Update visual state of the button.
        /// </summary>
        internal virtual void UpdateVisualState()
        {
            if (this.isSelected)
            {
                if (this.isMouseOver)
                {
                    VisualStateManager.GoToState(this, "SelectedMouseOver", true);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Selected", true);
                }
            }
            else if (this.isMouseOver)
            {
                VisualStateManager.GoToState(this, "MouseOver", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        /// <summary>
        /// Updates visual state of the border.
        /// </summary>
        /// <param name="isOver">Indicates whether mouse is over the border.</param>
        /// <param name="isRightRotated">Indicates whether tab is rotated when tab is placed on the right.</param>
        /// <param name="isLeftRotated">Indicates whether tab is rotated when tab is placed on the left.</param>
        internal override void SetVisualState(bool isOver, bool isRightRotated, bool isLeftRotated)
        {
            if (this.isSelected)
            {
                if (isOver)
                {
                    VisualStateManager.GoToState(this, "SelectedMouseOver", true);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Selected", true);
                }
            }
            else if (isOver)
            {
                VisualStateManager.GoToState(this, "MouseOver", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        /// <summary>
        /// Occurs when tab item parent is selected or deselected.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void TabItemParentIsSelectedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateSelectedState();
        }

        /// <summary>
        /// Gets geometry used for drawing the border.
        /// </summary>
        /// <param name="borderSize">Border size.</param>
        /// <returns>Needed geometry.</returns>
        private PathGeometry GetSelectedOutterLineGeometry(Size borderSize)
        {
            PathGeometry path = new PathGeometry();

            PathFigure selectedOutterLineFigure = new PathFigure();
            selectedOutterLineFigure.StartPoint = new Point(0.5, borderSize.Height);

            ArcSegment arc = new ArcSegment();
            arc.Point = new Point(3.5, borderSize.Height - 2);
            arc.Size = new Size(3.5, 3);
            arc.SweepDirection = SweepDirection.Counterclockwise;
            arc.IsLargeArc = false;
            arc.RotationAngle = 0;
            selectedOutterLineFigure.Segments.Add(arc);

            LineSegment line = new LineSegment();
            line.Point = new Point(3.5, 5);
            selectedOutterLineFigure.Segments.Add(line);

            arc = new ArcSegment();
            arc.Point = new Point(5, 0.5);
            arc.Size = new Size(5, 5);
            arc.SweepDirection = SweepDirection.Clockwise;
            arc.IsLargeArc = false;
            arc.RotationAngle = 0;
            selectedOutterLineFigure.Segments.Add(arc);

            line = new LineSegment();
            line.Point = new Point(borderSize.Width - 8, 0.5);
            selectedOutterLineFigure.Segments.Add(line);

            arc = new ArcSegment();
            arc.Point = new Point(borderSize.Width - 3, 2);
            arc.Size = new Size(5, 5);
            arc.SweepDirection = SweepDirection.Clockwise;
            arc.IsLargeArc = false;
            arc.RotationAngle = 0;
            selectedOutterLineFigure.Segments.Add(arc);

            line = new LineSegment();
            line.Point = new Point(borderSize.Width - 3, borderSize.Height - 2);
            selectedOutterLineFigure.Segments.Add(line);

            arc = new ArcSegment();
            arc.Point = new Point(borderSize.Width, borderSize.Height);
            arc.Size = new Size(3, 3);
            arc.SweepDirection = SweepDirection.Counterclockwise;
            arc.IsLargeArc = false;
            arc.RotationAngle = 0;
            selectedOutterLineFigure.Segments.Add(arc);

            path.Figures.Add(selectedOutterLineFigure);

            return path;
        }

        /// <summary>
        /// Gets geometry used for drawing the border.
        /// </summary>
        /// <param name="borderSize">Border size.</param>
        /// <returns>Needed geometry.</returns>
        private PathGeometry GetHoverSelectedLineGeometry(Size borderSize)
        {
            PathGeometry path = new PathGeometry();

            PathFigure hoverSelectedLineFigure = new PathFigure();
            hoverSelectedLineFigure.StartPoint = new Point(3.5, borderSize.Height - 3);

            LineSegment line = new LineSegment();
            line.Point = new Point(3.5, borderSize.Height - 3);
            hoverSelectedLineFigure.Segments.Add(line);

            line = new LineSegment();
            line.Point = new Point(3.5, 5);
            hoverSelectedLineFigure.Segments.Add(line);

            ArcSegment arc = new ArcSegment();
            arc.Point = new Point(8, 0.5);
            arc.Size = new Size(5, 5);
            arc.SweepDirection = SweepDirection.Clockwise;
            arc.IsLargeArc = false;
            arc.RotationAngle = 0;
            hoverSelectedLineFigure.Segments.Add(arc);

            line = new LineSegment();
            line.Point = new Point(borderSize.Width - 8, 0.5);
            hoverSelectedLineFigure.Segments.Add(line);

            arc = new ArcSegment();
            arc.Point = new Point(borderSize.Width - 3, 5);
            arc.Size = new Size(5, 5);
            arc.SweepDirection = SweepDirection.Clockwise;
            arc.IsLargeArc = false;
            arc.RotationAngle = 0;
            hoverSelectedLineFigure.Segments.Add(arc);

            line = new LineSegment();
            line.Point = new Point(borderSize.Width - 3, borderSize.Height - 2);
            hoverSelectedLineFigure.Segments.Add(line);

            path.Figures.Add(hoverSelectedLineFigure);

            return path;
        }

        ///// <summary>
        ///// Gets geometry used for drawing the border.
        ///// </summary>
        ///// <param name="borderSize">Border size.</param>
        ///// <returns>Needed geometry.</returns>
        //private PathGeometry GetHoverOutterLineGeometry(Size borderSize)
        //{
        //    PathGeometry path = new PathGeometry();

        //    PathFigure hoverOutterLineFigure = new PathFigure();
        //    hoverOutterLineFigure.StartPoint = new Point(3.5, borderSize.Height - 0.5);

        //    LineSegment line = new LineSegment();
        //    line.Point = new Point(3.5, borderSize.Height - 0.5);
        //    hoverOutterLineFigure.Segments.Add(line);

        //    line = new LineSegment();
        //    line.Point = new Point(3.5, 5);
        //    hoverOutterLineFigure.Segments.Add(line);

        //    ArcSegment arc = new ArcSegment();
        //    arc.Point = new Point(8, 0.5);
        //    arc.Size = new Size(5, 5);
        //    arc.SweepDirection = SweepDirection.Clockwise;
        //    arc.IsLargeArc = false;
        //    arc.RotationAngle = 0;
        //    hoverOutterLineFigure.Segments.Add(arc);

        //    line = new LineSegment();
        //    line.Point = new Point(borderSize.Width - 8, 0.5);
        //    hoverOutterLineFigure.Segments.Add(line);

        //    arc = new ArcSegment();
        //    arc.Point = new Point(borderSize.Width - 3, 5);
        //    arc.Size = new Size(5, 5);
        //    arc.SweepDirection = SweepDirection.Clockwise;
        //    arc.IsLargeArc = false;
        //    arc.RotationAngle = 0;
        //    hoverOutterLineFigure.Segments.Add(arc);

        //    line = new LineSegment();
        //    line.Point = new Point(borderSize.Width - 3, borderSize.Height);
        //    hoverOutterLineFigure.Segments.Add(line);

        //    path.Figures.Add(hoverOutterLineFigure);

        //    return path;
        //}

        /// <summary>
        /// Gets geometry used for drawing the border.
        /// </summary>
        /// <param name="borderSize">Border size.</param>
        /// <returns>Needed geometry.</returns>
        private PathGeometry GetHoverInnerLineGeometry(Size borderSize)
        {
            PathGeometry path = new PathGeometry();

            PathFigure hoverInnerLineFigure = new PathFigure();
            hoverInnerLineFigure.StartPoint = new Point(4.5, borderSize.Height - 0.5);

            LineSegment line = new LineSegment();
            line.Point = new Point(4.5, borderSize.Height - 0.5);
            hoverInnerLineFigure.Segments.Add(line);

            line = new LineSegment();
            line.Point = new Point(4.5, 5);
            hoverInnerLineFigure.Segments.Add(line);

            ArcSegment arc = new ArcSegment();
            arc.Point = new Point(6, 1.5);
            arc.Size = new Size(4, 4);
            arc.SweepDirection = SweepDirection.Clockwise;
            arc.IsLargeArc = false;
            arc.RotationAngle = 0;
            hoverInnerLineFigure.Segments.Add(arc);

            line = new LineSegment();
            line.Point = new Point(borderSize.Width - 8, 1.5);
            hoverInnerLineFigure.Segments.Add(line);

            arc = new ArcSegment();
            arc.Point = new Point(borderSize.Width - 4, 3);
            arc.Size = new Size(4, 4);
            arc.SweepDirection = SweepDirection.Clockwise;
            arc.IsLargeArc = false;
            arc.RotationAngle = 0;
            hoverInnerLineFigure.Segments.Add(arc);

            line = new LineSegment();
            line.Point = new Point(borderSize.Width - 4, borderSize.Height);
            hoverInnerLineFigure.Segments.Add(line);

            path.Figures.Add(hoverInnerLineFigure);

            return path;
        }

        /// <summary>
        /// Gets geometry used for drawing the border.
        /// </summary>
        /// <param name="borderSize">Border size.</param>
        /// <returns>Needed geometry.</returns>
        private PathGeometry GetBottomLineGeometry(Size borderSize)
        {
            PathGeometry path = new PathGeometry();

            PathFigure bottomLineFigure = new PathFigure();
            bottomLineFigure.StartPoint = new Point(0, borderSize.Height);

            LineSegment line = new LineSegment();
            line.Point = new Point(0, borderSize.Height - 0.5);
            bottomLineFigure.Segments.Add(line);

            line = new LineSegment();
            line.Point = new Point(borderSize.Width, borderSize.Height);
            bottomLineFigure.Segments.Add(line);

            path.Figures.Add(bottomLineFigure);

            return path;
        }
        #endregion
    }
}
