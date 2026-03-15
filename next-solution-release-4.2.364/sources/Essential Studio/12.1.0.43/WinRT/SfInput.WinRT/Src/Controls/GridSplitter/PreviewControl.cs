#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;
using Syncfusion.UI.Xaml.Controls.Extensions;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Syncfusion.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents the control that shows a preview of the SfGridSplitter's
    /// redistribution of space between columns or rows of a Grid control.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    [TemplatePart(Name = PreviewControl.ElementHorizontalTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PreviewControl.ElementVerticalTemplateName, Type = typeof(FrameworkElement))]
    internal partial class PreviewControl : Control
    {
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementHorizontalTemplateName = "HorizontalTemplate";

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementVerticalTemplateName = "VerticalTemplate";

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal FrameworkElement ElementHorizontalTemplateFrameworkElement { get; set; }

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal FrameworkElement ElementVerticalTemplateFrameworkElement { get; set; }

        /// <summary>
        /// Is Null until the PreviewControl is bound to a SfGridSplitter.
        /// </summary>
        private SfGridSplitter.GridResizeDirection _currentGridResizeDirection;

        /// <summary>
        /// Tracks the bound SfGridSplitter's location for calculating the
        /// PreviewControl's offset.
        /// </summary>
        private Point _gridSplitterOrigin;

        /// <summary>
        /// Instantiate the PreviewControl.
        /// </summary>
        public PreviewControl()
        {
            _gridSplitterOrigin = new Point();
        }

        /// <summary>
        /// Called when template should be applied to the control.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ElementHorizontalTemplateFrameworkElement = this.GetTemplateChild(PreviewControl.ElementHorizontalTemplateName) as FrameworkElement;
            ElementVerticalTemplateFrameworkElement = this.GetTemplateChild(PreviewControl.ElementVerticalTemplateName) as FrameworkElement;

            if (_currentGridResizeDirection == SfGridSplitter.GridResizeDirection.Columns)
            {
                if (ElementHorizontalTemplateFrameworkElement != null)
                {
                    ElementHorizontalTemplateFrameworkElement.Visibility = Visibility.Collapsed;
                }
                if (ElementVerticalTemplateFrameworkElement != null)
                {
                    ElementVerticalTemplateFrameworkElement.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (ElementHorizontalTemplateFrameworkElement != null)
                {
                    ElementHorizontalTemplateFrameworkElement.Visibility = Visibility.Visible;
                }
                if (ElementVerticalTemplateFrameworkElement != null)
                {
                    ElementVerticalTemplateFrameworkElement.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Bind the the dimensions of the preview control to the associated
        /// grid splitter.
        /// </summary>
        /// <param name="gridSplitter">SfGridSplitter instance to target.</param>
        public void Bind(SfGridSplitter gridSplitter)
        {
            Debug.Assert(gridSplitter != null, "gridSplitter should not be null!");
            Debug.Assert(gridSplitter.Parent != null, "gridSplitter.Parent should not be null!");

            this.Style = gridSplitter.PreviewStyle;
            this.Height = gridSplitter.ResizeDataInternal != null &&
                          gridSplitter.ResizeDataInternal.ResizeDirection.Equals(SfGridSplitter.GridResizeDirection.Rows)
                              ? gridSplitter.ActualHeight/2
                              : gridSplitter.ActualHeight;
            this.Width = gridSplitter.ResizeDataInternal != null &&
                         gridSplitter.ResizeDataInternal.ResizeDirection.Equals(SfGridSplitter.GridResizeDirection.Columns)
                             ? gridSplitter.ActualWidth/2
                             : gridSplitter.ActualWidth;

            if (gridSplitter.ResizeDataInternal != null)
            {
                _currentGridResizeDirection = gridSplitter.ResizeDataInternal.ResizeDirection;
                if (gridSplitter.ResizeDataInternal.ResizeDirection.Equals(SfGridSplitter.GridResizeDirection.Columns))
                {
                    FrameworkElementExtensions.SetCursor(this, new CoreCursor(CoreCursorType.SizeWestEast, 1));
                }
                else
                {
                    FrameworkElementExtensions.SetCursor(this, new CoreCursor(CoreCursorType.SizeNorthSouth, 1));
                }
            }

            GeneralTransform gt = gridSplitter.TransformToVisual((UIElement)gridSplitter.Parent);
            Point p = new Point(0, 0);
            p = gt.TransformPoint(p);

            _gridSplitterOrigin.X = gridSplitter.ResizeDataInternal != null &&
                                    gridSplitter.ResizeDataInternal.ResizeDirection.Equals(
                                        SfGridSplitter.GridResizeDirection.Columns)
                                        ? p.X + this.Width/2
                                        : p.X;
            _gridSplitterOrigin.Y = gridSplitter.ResizeDataInternal != null &&
                                    gridSplitter.ResizeDataInternal.ResizeDirection.Equals(
                                        SfGridSplitter.GridResizeDirection.Rows)
                                        ? p.Y + this.Height/2
                                        : p.Y;
            
            SetValue(Canvas.LeftProperty, _gridSplitterOrigin.X);
            SetValue(Canvas.TopProperty, _gridSplitterOrigin.Y);
        }

        /// <summary>
        /// Gets or sets the x-axis offset for the underlying render transform.
        /// </summary>
        public double OffsetX
        {
            get { return (double)GetValue(Canvas.LeftProperty) - _gridSplitterOrigin.X; }
            set { SetValue(Canvas.LeftProperty, _gridSplitterOrigin.X + value); }
        }

        /// <summary>
        /// Gets or sets the y-axis offset for the underlying render transform.
        /// </summary>
        public double OffsetY
        {
            get { return (double)GetValue(Canvas.TopProperty) - _gridSplitterOrigin.Y; }
            set { SetValue(Canvas.TopProperty, _gridSplitterOrigin.Y + value); }
        }
    }
}