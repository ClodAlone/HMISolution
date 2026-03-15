// <copyright file="OneNoteTabBorder.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the class for the OneNoteTabBorder
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class OneNoteTabBorder : Decorator
    {
        #region Private members
        /// <summary>
        /// Stores the pathGeometry in path1.
        /// </summary>
        private PathGeometry path1;

        /// <summary>
        /// Stores the pathGeometry in path2.
        /// </summary>
        private PathGeometry path2;
        #endregion

        #region DP properties
        /// <summary>
        /// Dependency property for storing the Background.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty = Panel.BackgroundProperty.AddOwner(typeof(OneNoteTabBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Dependency property for storing the BorderBrush.
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(OneNoteTabBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Dependency property for storing the BorderInnerBrush.
        /// </summary>
        public static readonly DependencyProperty BorderInnerBrushProperty = DependencyProperty.Register("BorderInnerBrush", typeof(Brush), typeof(OneNoteTabBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Dependency property for storing the BottomBorderBrush.
        /// </summary>
        public static readonly DependencyProperty BottomBorderBrushProperty = DependencyProperty.Register("BottomBorderBrush", typeof(Brush), typeof(OneNoteTabBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>The background.</value>
        [DefaultValue((string)null)]
        public Brush Background
        {
            get
            {
                return (Brush)GetValue(BackgroundProperty);
            }

            set
            {
                SetValue(BackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the border brush.
        /// </summary>
        /// <value>The border brush.</value>
        [DefaultValue((string)null)]
        public Brush BorderBrush
        {
            get
            {
                return (Brush)GetValue(BorderBrushProperty);
            }

            set
            {
                SetValue(BorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the border inner brush.
        /// </summary>
        /// <value>The border inner brush.</value>
        [DefaultValue((string)null)]
        public Brush BorderInnerBrush
        {
            get
            {
                return (Brush)GetValue(BorderInnerBrushProperty);
            }

            set
            {
                SetValue(BorderInnerBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the bottom border brush.
        /// </summary>
        /// <value>The bottom border brush.</value>
        [DefaultValue((string)null)]
        public Brush BottomBorderBrush
        {
            get
            {
                return (Brush)GetValue(BottomBorderBrushProperty);
            }

            set
            {
                SetValue(BottomBorderBrushProperty, value);
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
            TabControlExt parent = (TabControlExt)VisualUtils.FindAncestor(this, typeof(TabControlExt));
            
            // TabControlExt parent = (TemplatedParent as TabItemExt).Parent as TabControlExt;
            TabLayoutPanel layoutPanel = parent.TabLayoutPanel;
            bool needShrink = parent.TabLayoutPanel.NeedShrink;
            bool multiline = parent.TabItemLayout != TabItemLayoutType.SingleLine && layoutPanel.m_NumRows == layoutPanel.VisibleItemsCount;
            Rect arrangeRect = new Rect
                                {
                                    Width = Math.Max(0.0, arrangeSize.Width - 2.0 - arrangeSize.Height),
                                    Height = Math.Max(0.0, arrangeSize.Height),
                                    X = Math.Min(arrangeSize.Height, arrangeSize.Width),
                                    Y = Math.Min(2.0, arrangeSize.Height)
                                };

            if (needShrink && Child.DesiredSize.Width > arrangeRect.Width)
            {
                arrangeRect.Width = Child.DesiredSize.Width;
            }

            if (multiline)
            {
                arrangeRect.Width = layoutPanel.AverageWidth;
            }

            if (Child != null)
            {
                Child.Arrange(arrangeRect);
            }

            path1 = new PathGeometry();
            path2 = new PathGeometry();
            PathFigure figure1 = new PathFigure(
                new Point(1.0, arrangeSize.Height),
                new PathSegment[] 
                { 
                  new LineSegment(new Point(arrangeSize.Height - 3.0, 4.0), true), 
                  new LineSegment(new Point(arrangeSize.Height, 2.5), true), 
                  new LineSegment(new Point(arrangeSize.Height + 3.0, 1.5), true), 
                  new LineSegment(new Point(arrangeSize.Width - 5.0, 1.5), true), 
                  new LineSegment(new Point(arrangeSize.Width - 1.5, 5.0), true), 

                  // new ArcSegment(new Point(arrangeSize.Width - 1.5, 5.0), new Size(3.5, 3.5), 0.0, false, SweepDirection.Clockwise, true), 
                  new LineSegment(new Point(arrangeSize.Width - 1.5, arrangeSize.Height), true) 
                },
                false);

            PathFigure figure2 = new PathFigure(
                new Point(0.0, arrangeSize.Height),
                new PathSegment[] 
                { 
                    new LineSegment(new Point(arrangeSize.Height - 3.0, 3.0), true), 
                    new LineSegment(new Point(arrangeSize.Height, 1.5), true), 
                    new LineSegment(new Point(arrangeSize.Height + 3.0, 0.5), true), 
                    new LineSegment(new Point(arrangeSize.Width - 4.0, 0.5), true), 
                    new LineSegment(new Point(arrangeSize.Width - 0.5, 4.0), true), 

                    // new ArcSegment(new Point(arrangeSize.Width - 0.5, 4.0), new Size(3.5, 3.5), 0.0, false, SweepDirection.Clockwise, true), 
                    new LineSegment(new Point(arrangeSize.Width - 0.5, arrangeSize.Height), true) 
                },
                false);

            path1.Figures.Add(figure1);
            path2.Figures.Add(figure2);
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
                TabControlExt parent = (TabControlExt)VisualUtils.FindAncestor(this, typeof(TabControlExt));
                
                // TabControlExt parent = (TemplatedParent as TabItemExt).Parent as TabControlExt;
                bool needShrink = parent.TabLayoutPanel.NeedShrink;
                Size size = constraint;

                size.Width = needShrink ? Math.Max(0.0, size.Width - constraint.Height)
                    : Math.Max(0.0, size.Width - 8);

                Child.Measure(size);
                Size desiredSize = Child.DesiredSize;

                if (!needShrink)
                {
                    desiredSize.Width = desiredSize.Width + 8;
                }

                if (!needShrink)
                {
                    if (!double.IsInfinity(constraint.Height) && !parent.RotateTextWhenVertical)
                    {
                        desiredSize.Width = desiredSize.Width + constraint.Height;  // + 4;                                
                    }

                    if (parent.RotateTextWhenVertical)
                    {
                        desiredSize.Width = desiredSize.Width + desiredSize.Height;  // + 4;                                
                    }
                }

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
            if ((path2 != null) && ((Background != null) || (BorderBrush != null)))
            {
                Pen pen = null;

                if (BorderBrush != null)
                {
                    pen = new Pen(BorderBrush, 1.0);
                }

                drawingContext.DrawGeometry(Background, pen, path2);

                if (BorderInnerBrush != null)
                {
                    pen = new Pen(BorderInnerBrush, 1.0);
                    drawingContext.DrawGeometry(null, pen, path1);
                }

                if (BottomBorderBrush != null)
                {
                    drawingContext.DrawLine(new Pen(BottomBorderBrush, 1.0), new Point(1.8, ActualHeight), new Point(ActualWidth - 2.0, ActualHeight));
                    drawingContext.DrawLine(new Pen(BottomBorderBrush, 1.5), new Point(1.2, ActualHeight + 1), new Point(ActualWidth - 2.0, ActualHeight + 1));
                }

                // if (base.VisualXSnappingGuidelines == null)
                // {
                //    double[] array1 = new double[2];
                //    array1[1] = base.ActualWidth;
                //    base.VisualXSnappingGuidelines = new DoubleCollection(array1);
                // }

                // if (base.VisualYSnappingGuidelines == null)
                // {
                //    double[] array2 = new double[2];
                //    array2[1] = base.ActualHeight;
                //    base.VisualYSnappingGuidelines = new DoubleCollection(array2);
                // }
            }
        }
        #endregion
    }
}
