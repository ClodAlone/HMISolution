#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    /// Represents tab item's office2007 border.
    /// </summary>
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Selected", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "RightRotatedMouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "RightRotatedSelected", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "LeftRotatedMouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "LeftRotatedSelected", GroupName = "CommonStates")]
    public class TabItemAdvBorderOffice2003 : TabItemAdvBorder
    {
        #region Private members
        /// <summary>
        /// Used for drawing the border.
        /// </summary>
        private Path borderPath;

        /// <summary>
        /// Used for drawing the border.
        /// </summary>
        private Path borderBottomPath;

        /// <summary>
        /// Indicates whether mouse pointer is over the border.
        /// </summary>
        private bool isMouseOver = false;

        /// <summary>
        /// Indicates whether tab item parent is selected.
        /// </summary>
        private bool isSelected = false;

        /// <summary>
        /// Indicates whether border is rotated when tab placed to the right.
        /// </summary>
        private bool isRightRotated = false;

        /// <summary>
        /// Indicates whether border is rotated when tab placed to the left.
        /// </summary>
        private bool isLeftRotated = false;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes new instance of the TabItemAdvBorderOffice2003 class.
        /// </summary>
        public TabItemAdvBorderOffice2003()
        {
            DefaultStyleKey = typeof(TabItemAdvBorderOffice2003);
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

            this.borderPath = this.GetTemplateChild("BorderPath") as Path;
            this.borderBottomPath = this.GetTemplateChild("BorderBottomPath") as Path;

            this.UpdateSelectedState();
        }

        /// <summary>
        /// Initializes paths with data.
        /// </summary>
        private void InitializePaths(Size borderSize)
        {
            borderSize.Width += 0.5;

            if (this.borderPath != null)
            {
                this.borderPath.Data = this.GetBorderGeometry(borderSize);
            }

            if (this.borderBottomPath != null)
            {
                this.borderBottomPath.Data = this.GetBottomBorderGeometry(borderSize);
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
        /// Updates visual appearance of the border.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void UpdateIsRotated(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.TabControlParent != null && this.TabControlParent.RotateTextWhenVertical)
            {
                if (this.TabControlParent.TabStripPlacement == TabStripPlacement.Left)
                {
                    this.isLeftRotated = true;
                    this.isRightRotated = false;
                }
                else if (this.TabControlParent.TabStripPlacement == TabStripPlacement.Right)
                {
                    this.isRightRotated = true;
                    this.isLeftRotated = false;
                }
                else
                {
                    this.isLeftRotated = false;
                    this.isRightRotated = false;
                }
            }
            else
            {
                this.isLeftRotated = false;
                this.isRightRotated = false;
            }

            this.UpdateVisualState();
        }

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
            if (this.isLeftRotated)
            {
                if (this.isSelected)
                {
                    VisualStateManager.GoToState(this, "LeftRotatedSelected", true);
                }
                else if (this.isMouseOver)
                {
                    VisualStateManager.GoToState(this, "LeftRotatedMouseOver", true);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Normal", true);
                }
            }
            else if (this.isRightRotated)
            {
                if (this.isSelected)
                {
                    VisualStateManager.GoToState(this, "RightRotatedSelected", true);
                }
                else if (this.isMouseOver)
                {
                    VisualStateManager.GoToState(this, "RightRotatedMouseOver", true);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Normal", true);
                }
            }
            else
            {
                if (this.isSelected)
                {
                    VisualStateManager.GoToState(this, "Selected", true);
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
        }

        /// <summary>
        /// Updates visual state of the border.
        /// </summary>
        /// <param name="isOver">Indicates whether mouse is over the border.</param>
        /// <param name="isRightRotated">Indicates whether tab is rotated when tab is placed on the right.</param>
        /// <param name="isLeftRotated">Indicates whether tab is rotated when tab is placed on the left.</param>
        internal override void SetVisualState(bool isOver, bool isRightRotated, bool isLeftRotated)
        {
            this.isMouseOver = isOver;
            this.isRightRotated = isRightRotated;
            this.isLeftRotated = isLeftRotated;
            this.UpdateVisualState();
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
        private PathGeometry GetBorderGeometry(Size borderSize)
        {
            PathGeometry path = new PathGeometry();

            PathFigure borderFigure = new PathFigure();
            borderFigure.StartPoint = new Point(0.5, borderSize.Height - 4);

            LineSegment line = new LineSegment();
            line.Point = new Point(0.5, 5);
            borderFigure.Segments.Add(line);

            line = new LineSegment();
            line.Point = new Point(0.5, 0.5);
            borderFigure.Segments.Add(line);

            line = new LineSegment();
            line.Point = new Point(borderSize.Width, 0.5);
            borderFigure.Segments.Add(line);

            line = new LineSegment();
            line.Point = new Point(borderSize.Width, borderSize.Height - 4);
            borderFigure.Segments.Add(line);

            path.Figures.Add(borderFigure);

            return path;
        }

        /// <summary>
        /// Gets geometry used for drawing the border.
        /// </summary>
        /// <param name="borderSize">Border size.</param>
        /// <returns>Needed geometry.</returns>
        private PathGeometry GetBottomBorderGeometry(Size borderSize)
        {
            PathGeometry path = new PathGeometry();

            if (this.isSelected)
            {
                PathFigure lineFigure = new PathFigure();
                lineFigure.StartPoint = new Point(0, borderSize.Height - 4.5);

                LineSegment line = new LineSegment();
                line.Point = new Point(borderSize.Width, borderSize.Height - 4.5);
                lineFigure.Segments.Add(line);

                path.Figures.Add(lineFigure);
            }

            return path;
        }

        /// <summary>
        /// Occurs when Tabcontrol parent is changed.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnTabControlParentChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnTabControlParentChanged(e);
            if (this.TabControlParent != null)
            {
                this.TabControlParent.RotateTextWhenVerticalChanged += new PropertyChangedCallback(UpdateIsRotated);
                this.TabControlParent.TabStripPlacementChanged += new PropertyChangedCallback(UpdateIsRotated);
            }
        }
        #endregion
    }
}
