// <copyright file="CharacterSeven.cs" company="Syncfusion Software">
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
    /// Represents the seven segmented character element.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="CharacterSevenSample.Window1" Title="CharacterSevenSample" Height="400"
    /// Width="400" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <Grid>
    ///         <syncfusion:DigitalGauge Name="digitalGauge1" Width="350" Height="100"
    ///                                  CharacterCount="9" CharacterHeight="50" CenterFrameFillColor="Gray"
    ///                                  Foreground="Red" DimmedBrush="LightGray" SegmentWidth="5" 
    ///                                  SkewAngleX="-10" SegmentSpacing="3" CharacterSpacing="5"
    ///                                  Value="8:50 AM" CharacterType="SegmentSeven" />
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
    /// namespace CharacterSevenSample
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
    ///                 digitalGauge1.CharacterType = CharacterType.SegmentSeven;
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
    internal class CharacterSeven : CharacterBase
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
        private List<bool> m_listSegments = new List<bool>(7);

        /// <summary>
        /// Geometry used for drawing the segment.
        /// </summary>
        private Geometry m_segment;
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
            m_segment = new PathGeometry();
            double height = finalSize.Height / 2;
            if (height > 0)
            {
                double segmentSpacing = this.SegmentsSpacing;
                if (segmentSpacing > height - (this.SegmentWidth / 2))
                {
                    segmentSpacing = height - (this.SegmentWidth / 2);
                }

                if (segmentSpacing < 0)
                {
                    segmentSpacing = 0;
                }

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
                (m_segment as PathGeometry).Figures.Add(figure1);
            }

            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Participates in rendering operations that are directed by the layout system.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (m_listSegments.Count >= 7)
            {
                Brush segmentBrush = this.DimmedBrush;
                if (m_listSegments.Count > 0 && m_listSegments[0])
                {
                    segmentBrush = this.ForegroundBrush;
                }

                drawingContext.DrawGeometry(segmentBrush, null, m_segment);
                segmentBrush = this.DimmedBrush;
                if (m_listSegments.Count > 5 && m_listSegments[5])
                {
                    segmentBrush = this.ForegroundBrush;
                }

                drawingContext.PushTransform(new TranslateTransform(0, this.DesiredSize.Height / 2));
                drawingContext.DrawGeometry(segmentBrush, null, m_segment);
                m_countPushes += 1;

                segmentBrush = this.DimmedBrush;
                if (m_listSegments.Count > 2 && m_listSegments[2])
                {
                    segmentBrush = this.ForegroundBrush;
                }

                drawingContext.Pop();
                drawingContext.PushTransform(new TranslateTransform(this.DesiredSize.Width, 0));
                drawingContext.DrawGeometry(segmentBrush, null, m_segment);

                segmentBrush = this.DimmedBrush;
                if (m_listSegments.Count > 3 && m_listSegments[3])
                {
                    segmentBrush = this.ForegroundBrush;
                }

                drawingContext.PushTransform(new TranslateTransform(0, this.DesiredSize.Height / 2));
                drawingContext.DrawGeometry(segmentBrush, null, m_segment);
                m_countPushes += 1;

                segmentBrush = this.DimmedBrush;
                if (m_listSegments.Count > 1 && m_listSegments[1])
                {
                    segmentBrush = this.ForegroundBrush;
                }

                drawingContext.Pop();
                drawingContext.Pop();
                drawingContext.PushTransform(new RotateTransform(-90d));
                drawingContext.PushTransform(new TranslateTransform(-this.SegmentWidth / 2, this.SegmentWidth / 2));
                drawingContext.DrawGeometry(segmentBrush, null, m_segment);

                segmentBrush = this.DimmedBrush;
                if (m_listSegments.Count > 6 && m_listSegments[6])
                {
                    segmentBrush = this.ForegroundBrush;
                }

                drawingContext.PushTransform(new TranslateTransform(-this.DesiredSize.Height / 2, 0));
                drawingContext.DrawGeometry(segmentBrush, null, m_segment);
                m_countPushes += 1;

                segmentBrush = this.DimmedBrush;
                if (m_listSegments.Count > 4 && m_listSegments[4])
                {
                    segmentBrush = this.ForegroundBrush;
                }

                drawingContext.PushTransform(new TranslateTransform(-this.DesiredSize.Height / 2, 0));
                drawingContext.DrawGeometry(segmentBrush, null, m_segment);
                m_countPushes += 1;
                for (int i = 0; i < m_countPushes; i++)
                {
                    drawingContext.Pop();
                }

                m_countPushes = 0;
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

            base.OnRender(drawingContext);
        }
        #endregion Overrides
    }
}
