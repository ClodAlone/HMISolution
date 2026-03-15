// <copyright file="CharacterFourteen.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represent the fourteen segmented character element.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="CharacterFourteenSample.Window1" Title="CharacterFourteenSample" Height="400"
    /// Width="400" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <Grid>
    ///         <syncfusion:DigitalGauge Name="digitalGauge1" Width="350" Height="100"
    ///                                  CharacterCount="9" CharacterHeight="50" CenterFrameFillColor="Gray"
    ///                                  Foreground="Red" DimmedBrush="LightGray" SegmentWidth="5" 
    ///                                  SkewAngleX="-10" SegmentSpacing="3" CharacterSpacing="5" 
    ///                                  Value="8:50 AM" CharacterType="SegmentFourteen" />
    ///     </Grid>
    /// </Window>
    /// </code>
    /// <code lang="C#">
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Text;
    /// using System.Windows;
    /// using System.Windows.Media;
    /// using System.Windows.Controls;
    /// using System.Windows.Data;
    /// using Syncfusion.Windows.Shared;
    /// using Syncfusion.Windows.Gauge;<para/>
    /// namespace CharacterFourteenSample
    /// {
    ///         public partial class Window1 : Window
    ///         {
    ///             private DigitalGauge digitalGauge1;
    ///             public Window1()
    ///             {                
    ///                 InitializeComponent();<para/>
    ///                 digitalGauge1 = new DigitalGauge();
    ///                 digitalGauge1.Height = 100;
    ///                 digitalGauge1.Width = 350;
    ///                 digitalGauge1.CharacterCount = 9;
    ///                 digitalGauge1.CharacterHeight = 50;
    ///                 digitalGauge1.CenterFrameFillColor = Colors.Gray;
    ///                 digitalGauge1.Foreground = new SolidColorBrush(Colors.Red);
    ///                 digitalGauge1.DimmedBrush = new SolidColorBrush(Colors.LightGray);
    ///                 digitalGauge1.CharacterType = CharacterType.SegmentFourteen;
    ///                 digitalGauge1.SkewAngleX = -10;
    ///                 digitalGauge1.SegmentWidth = 5;
    ///                 digitalGauge1.SegmentSpacing = 3;
    ///                 digitalGauge1.CharacterSpacing = 5;
    ///                 digitalGauge1.Value = "8:50 AM";
    ///                 this.Content = digitalGauge1;
    ///             }
    ///         }
    /// }   
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class CharacterFourteen : CharacterBase
    {
        #region Private Members
        /// <summary>
        /// Value indicating the count of transform pushes into the drawing context.
        /// </summary>
        private int m_countPushes = 0;

        /// <summary>
        /// Collection of boolean values indicating what character segments should be 
        /// drawn with foreground brush.
        /// </summary>
        private List<bool> m_listSegments = new List<bool>(14);

        /// <summary>
        /// Geometry used for drawing the segment.
        /// </summary>
        private Geometry m_segment;

        /// <summary>
        /// Geometry used for drawing the central horizontal segments.
        /// </summary>
        private Geometry m_shortSegment;

        /// <summary>
        /// Geometry used for drawing the sloping segments.
        /// </summary>
        private Geometry m_slopingSegment;
        #endregion Private Members

        #region CLR Getters & Setters
        /// <summary>
        /// Gets or sets the collection of boolean values indicating what character 
        /// segments should be drawn with foreground brush.
        /// </summary>
        /// <value>
        /// Type: <see cref="List{Boolean}"/>
        /// </value>
        internal override List<bool> Segments
        {
            get
            {
                return m_listSegments;
            }

            set
            {
                m_listSegments = value;
            }
        }
        #endregion CLR Getters & Setters

        #region Overrides
        /// <summary>
        /// Positions child elements and determines a size for the element.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use 
        /// to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double height = finalSize.Height / 2;
            double width = finalSize.Width / 2;
            double segmentSpacing = this.SegmentsSpacing;
            if (segmentSpacing > height - (this.SegmentWidth / 2))
            {
                segmentSpacing = height - (this.SegmentWidth / 2);
            }

            if (segmentSpacing < 0)
            {
                segmentSpacing = 0;
            }

            m_slopingSegment = this.GetSegmentPath(Math.Sqrt((height * height) + (width * width)) - (this.SegmentWidth / 2), segmentSpacing);
            m_shortSegment = this.GetSegmentPath(width, segmentSpacing);
            m_segment = this.GetSegmentPath(height, segmentSpacing);
            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Participates in rendering operations that are directed by the layout system.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            this.DrawSegments(drawingContext, this.DimmedBrush, false);
            this.DrawShortSegments(drawingContext, this.DimmedBrush, false);
            this.DrawSlopingSegments(drawingContext, this.DimmedBrush, false);
            this.DrawSlopingSegments(drawingContext, this.ForegroundBrush, true);
            this.DrawSegments(drawingContext, this.ForegroundBrush, true);
            this.DrawShortSegments(drawingContext, this.ForegroundBrush, true);

            base.OnRender(drawingContext);
        }
        #endregion Overrides

        #region Implementation
        /// <summary>
        /// Draws character segments.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions.</param>
        /// <param name="segmentBrush">Brush used for drawing the segment.</param>
        /// <param name="isActive">Indicates whether segment is active.</param>
        private void DrawSegments(DrawingContext drawingContext, Brush segmentBrush, bool isActive)
        {
            if (m_listSegments.Count >= 7)
            {
                if ((isActive && m_listSegments[0]) || (!isActive && !m_listSegments[0]))
                {
                    for (int i = 0; i < m_countPushes; i++)
                    {
                        drawingContext.Pop();
                    }

                    drawingContext.DrawGeometry(segmentBrush, null, m_segment);
                }

                if ((isActive && m_listSegments[1]) || (!isActive && !m_listSegments[1]))
                {
                    for (int i = 0; i < m_countPushes; i++)
                    {
                        drawingContext.Pop();
                    }

                    drawingContext.PushTransform(new RotateTransform(-90d));
                    drawingContext.PushTransform(new TranslateTransform(-this.SegmentWidth / 2, this.SegmentWidth / 2));
                    drawingContext.DrawGeometry(segmentBrush, null, m_segment);
                    m_countPushes = 2;
                }

                if ((isActive && m_listSegments[2]) || (!isActive && !m_listSegments[2]))
                {
                    for (int i = 0; i < m_countPushes; i++)
                    {
                        drawingContext.Pop();
                    }

                    drawingContext.PushTransform(new TranslateTransform(this.DesiredSize.Width, 0));
                    drawingContext.DrawGeometry(segmentBrush, null, m_segment);
                    m_countPushes = 1;
                }

                if ((isActive && m_listSegments[3]) || (!isActive && !m_listSegments[3]))
                {
                    for (int i = 0; i < m_countPushes; i++)
                    {
                        drawingContext.Pop();
                    }

                    drawingContext.PushTransform(new TranslateTransform(this.DesiredSize.Width, this.DesiredSize.Height / 2));
                    drawingContext.DrawGeometry(segmentBrush, null, m_segment);
                    m_countPushes = 1;
                }

                if ((isActive && m_listSegments[4]) || (!isActive && !m_listSegments[4]))
                {
                    for (int i = 0; i < m_countPushes; i++)
                    {
                        drawingContext.Pop();
                    }

                    drawingContext.PushTransform(new RotateTransform(-90d));
                    drawingContext.PushTransform(new TranslateTransform(-this.SegmentWidth / 2, this.SegmentWidth / 2));
                    drawingContext.PushTransform(new TranslateTransform(-this.DesiredSize.Height, 0));
                    drawingContext.DrawGeometry(segmentBrush, null, m_segment);
                    m_countPushes = 3;
                }

                if ((isActive && m_listSegments[5]) || (!isActive && !m_listSegments[5]))
                {
                    for (int i = 0; i < m_countPushes; i++)
                    {
                        drawingContext.Pop();
                    }

                    drawingContext.PushTransform(new TranslateTransform(0, this.DesiredSize.Height / 2));
                    drawingContext.DrawGeometry(segmentBrush, null, m_segment);
                    m_countPushes = 1;
                }

                if (m_listSegments.Count >= 14)
                {
                    if ((isActive && m_listSegments[8]) || (!isActive && !m_listSegments[8]))
                    {
                        for (int i = 0; i < m_countPushes; i++)
                        {
                            drawingContext.Pop();
                        }

                        drawingContext.PushTransform(new TranslateTransform(this.DesiredSize.Width / 2, 0));
                        drawingContext.DrawGeometry(segmentBrush, null, m_segment);
                        m_countPushes = 1;
                    }

                    if ((isActive && m_listSegments[9]) || (!isActive && !m_listSegments[9]))
                    {
                        for (int i = 0; i < m_countPushes; i++)
                        {
                            drawingContext.Pop();
                        }

                        drawingContext.PushTransform(new TranslateTransform(this.DesiredSize.Width / 2, this.DesiredSize.Height / 2));
                        drawingContext.DrawGeometry(segmentBrush, null, m_segment);
                        m_countPushes = 1;
                    }
                }

                for (int i = 0; i < m_countPushes; i++)
                {
                    drawingContext.Pop();
                }

                if (this.DrawDot)
                {
                    drawingContext.DrawEllipse(this.ForegroundBrush, null, new Point(this.DesiredSize.Width + this.SegmentWidth + (this.CharactersSpacing / 2), this.DesiredSize.Height - (this.SegmentWidth / 2)), this.SegmentWidth / 2, this.SegmentWidth / 2);
                }

                if (this.DrawColon)
                {
                    drawingContext.DrawEllipse(this.ForegroundBrush, null, new Point(this.DesiredSize.Width + this.SegmentWidth + (this.CharactersSpacing / 2), this.DesiredSize.Height / 4), this.SegmentWidth / 2, this.SegmentWidth / 2);
                    drawingContext.DrawEllipse(this.ForegroundBrush, null, new Point(this.DesiredSize.Width + this.SegmentWidth + (this.CharactersSpacing / 2), 3 * this.DesiredSize.Height / 4), this.SegmentWidth / 2, this.SegmentWidth / 2);
                }

                m_countPushes = 0;
            }
        }

        /// <summary>
        /// Draws central horizontal character segments.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions.</param>
        /// <param name="segmentBrush">Brush used for drawing the segment.</param>
        /// <param name="isActive">Indicates whether segment is active.</param>
        private void DrawShortSegments(DrawingContext drawingContext, Brush segmentBrush, bool isActive)
        {
            if (m_listSegments.Count >= 14)
            {
                if ((isActive && m_listSegments[6]) || (!isActive && !m_listSegments[6]))
                {
                    for (int i = 0; i < m_countPushes; i++)
                    {
                        drawingContext.Pop();
                    }

                    drawingContext.PushTransform(new RotateTransform(-90d));
                    drawingContext.PushTransform(new TranslateTransform(-this.SegmentWidth / 2, this.SegmentWidth / 2));
                    drawingContext.PushTransform(new TranslateTransform(-this.DesiredSize.Height / 2, 0));
                    drawingContext.DrawGeometry(segmentBrush, null, m_shortSegment);
                    m_countPushes = 3;
                }

                if ((isActive && m_listSegments[7]) || (!isActive && !m_listSegments[7]))
                {
                    for (int i = 0; i < m_countPushes; i++)
                    {
                        drawingContext.Pop();
                    }

                    drawingContext.PushTransform(new RotateTransform(-90d));
                    drawingContext.PushTransform(new TranslateTransform(-this.SegmentWidth / 2, this.SegmentWidth / 2));
                    drawingContext.PushTransform(new TranslateTransform(-this.DesiredSize.Height / 2, this.DesiredSize.Width / 2));
                    drawingContext.DrawGeometry(segmentBrush, null, m_shortSegment);
                    m_countPushes = 3;
                }

                for (int i = 0; i < m_countPushes; i++)
                {
                    drawingContext.Pop();
                }

                m_countPushes = 0;
            }
        }

        /// <summary>
        /// Draws sloping character segments.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions.</param>
        /// <param name="segmentBrush">Brush used for drawing the segment.</param>
        /// <param name="isActive">Indicates whether segment is active.</param>
        private void DrawSlopingSegments(DrawingContext drawingContext, Brush segmentBrush, bool isActive)
        {
            if (m_listSegments.Count >= 14)
            {
                if ((isActive && m_listSegments[10]) || (!isActive && !m_listSegments[10]))
                {
                    for (int i = 0; i < m_countPushes; i++)
                    {
                        drawingContext.Pop();
                    }

                    drawingContext.PushTransform(new TranslateTransform(this.SegmentWidth / 8, this.SegmentWidth / 2));
                    drawingContext.PushTransform(new RotateTransform(-25d - (this.SegmentWidth / 4)));
                    drawingContext.DrawGeometry(segmentBrush, null, m_slopingSegment);
                    m_countPushes = 2;
                }

                if ((isActive && m_listSegments[11]) || (!isActive && !m_listSegments[11]))
                {
                    for (int i = 0; i < m_countPushes; i++)
                    {
                        drawingContext.Pop();
                    }

                    drawingContext.PushTransform(new TranslateTransform(this.DesiredSize.Width - (this.SegmentWidth / 8), 0));
                    drawingContext.PushTransform(new RotateTransform(25d + (this.SegmentWidth / 8)));
                    drawingContext.DrawGeometry(segmentBrush, null, m_slopingSegment);
                    m_countPushes = 2;
                }

                if ((isActive && m_listSegments[12]) || (!isActive && !m_listSegments[12]))
                {
                    for (int i = 0; i < m_countPushes; i++)
                    {
                        drawingContext.Pop();
                    }

                    drawingContext.PushTransform(new TranslateTransform(this.DesiredSize.Width / 2, this.DesiredSize.Height / 2));
                    drawingContext.PushTransform(new RotateTransform(25d + (this.SegmentWidth / 4)));
                    drawingContext.DrawGeometry(segmentBrush, null, m_slopingSegment);
                    m_countPushes = 2;
                }

                if ((isActive && m_listSegments[13]) || (!isActive && !m_listSegments[13]))
                {
                    for (int i = 0; i < m_countPushes; i++)
                    {
                        drawingContext.Pop();
                    }

                    drawingContext.PushTransform(new TranslateTransform((this.DesiredSize.Width / 2) + (this.SegmentWidth / 8), (this.DesiredSize.Height / 2) + (this.SegmentWidth / 2)));
                    drawingContext.PushTransform(new RotateTransform(-25d - (this.SegmentWidth / 4)));
                    drawingContext.DrawGeometry(segmentBrush, null, m_slopingSegment);
                    m_countPushes = 2;
                }

                for (int i = 0; i < m_countPushes; i++)
                {
                    drawingContext.Pop();
                }

                m_countPushes = 0;
            }
        }

        /// <summary>
        /// Gets the geometry used for drawing the character segment.
        /// </summary>
        /// <param name="height">Height of the character segment.</param>
        /// <param name="segmentSpacing">Spacing between the segments</param>
        /// <returns>Geometry of the segment</returns>
        private Geometry GetSegmentPath(double height, double segmentSpacing)
        {
            PathGeometry segment = new PathGeometry();
            if (height > this.SegmentWidth + (this.SegmentsSpacing * 2))
            {
                Point bottomCenter = new Point(this.SegmentWidth / 2, height - segmentSpacing);
                Point bottomLeft = new Point(0, height - (this.SegmentWidth / 2) - segmentSpacing);
                Point bottomRight = new Point(this.SegmentWidth, height - (this.SegmentWidth / 2) - segmentSpacing);
                Point topCenter = new Point(this.SegmentWidth / 2, segmentSpacing);
                Point topLeft = new Point(0, (this.SegmentWidth / 2) + segmentSpacing);
                Point topRight = new Point(this.SegmentWidth, (this.SegmentWidth / 2) + segmentSpacing);

                LineSegment bc = new LineSegment(bottomCenter, true);
                LineSegment bl = new LineSegment(bottomLeft, true);
                LineSegment tl = new LineSegment(topLeft, true);
                LineSegment tc = new LineSegment(topCenter, true);
                LineSegment tr = new LineSegment(topRight, true);
                LineSegment br = new LineSegment(bottomRight, true);

                PathFigure figure1 = new PathFigure(
                    bottomCenter,
                    new PathSegment[] { bc, bl, tl, tc, tr, br },
                    false);
                (segment as PathGeometry).Figures.Add(figure1);
            }

            return segment;
        }
        #endregion Implementation
    }
}
