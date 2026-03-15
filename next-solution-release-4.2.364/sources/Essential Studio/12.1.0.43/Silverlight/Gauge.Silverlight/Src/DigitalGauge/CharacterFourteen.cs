#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;


namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the seven segmented character element.
    /// </summary>
    /// <example>
    /// <code> &lt;Grid&gt;
    ///    &lt;syncfusion:DigitalGauge  Name=&quot;digitalGauge&quot;
    ///                                    CharacterHeight=&quot;50&quot;  Grid.RowSpan=&quot;2&quot;
    ///                                    SegmentBrush=&quot;Brown&quot;   SegmentWidth=&quot;3&quot;
    ///                                    DimmedBrush=&quot;Transparent&quot;  SegmentSpacing=&quot;1&quot; InnerFrameBrush=&quot;CadetBlue&quot; OuterFrameBrush=&quot;Black&quot; Background=&quot;AliceBlue&quot;
    ///                                    CharacterSpacing=&quot;5&quot;  CharacterCount=&quot;10&quot;
    ///                                    Value=&quot;Syncfusion&quot; CharacterType=&quot;SegmentFourteen&quot; /&gt;
    ///  &lt;/Grid&gt;</code>
    /// <para></para>
    /// <para></para>
    /// <para></para>
    /// <para></para>
    /// <para>using System; </para>
    /// <para></para>
    /// <para>using System.Collections.Generic; </para>
    /// <para></para>
    /// <para>using System.Linq; </para>
    /// <para></para>
    /// <para>using System.Text; </para>
    /// <para></para>
    /// <para>using System.Windows; </para>
    /// <para></para>
    /// <para>using System.Windows.Media; </para>
    /// <para></para>
    /// <para>using System.Windows.Controls; </para>
    /// <para></para>
    /// <para>using System.Windows.Data; </para>
    /// <para></para>
    /// <para>using Syncfusion.Windows.Shared; </para>
    /// <para></para>
    /// <para>using Syncfusion.Windows.Gauge; </para>
    /// <para></para>
    /// <para></para>
    /// <para>namespace CharacterSevenSample </para>
    /// <para></para>
    /// <para>{ </para>
    /// <para></para>
    /// <para>        public partial class MainPage : UserControl </para>
    /// <para></para>
    /// <para>        { </para>
    /// <para></para>
    /// <para>            private DigitalGauge digitalGauge1; </para>
    /// <para></para>
    /// <para>            public Window1() </para>
    /// <para></para>
    /// <para>            { </para>
    /// <para></para>
    /// <para>                InitializeComponent(); </para>
    /// <para></para>
    /// <para></para>
    /// <para>                digitalGauge1 = new DigitalGauge(); </para>
    /// <para></para>
    /// <para>                digitalGauge1.Height = 100; </para>
    /// <para></para>
    /// <para>                digitalGauge1.Width = 350; </para>
    /// <para></para>
    /// <para>                digitalGauge1.CharacterCount = 9; </para>
    /// <para></para>
    /// <para>                digitalGauge1.CharacterHeight = 50; </para>
    /// <para></para>
    /// <para>                digitalGauge1.CenterFrameFillColor = Colors.Gray; </para>
    /// <para></para>
    /// <para>                digitalGauge1.Foreground = new
    /// SolidColorBrush(Colors.Red); </para>
    /// <para></para>
    /// <para>                digitalGauge1.DimmedBrush = new
    /// SolidColorBrush(Colors.LightGray); </para>
    /// <para></para>
    /// <para>                digitalGauge1.CharacterType =
    /// CharacterType.SegmentFourteen; </para>
    /// <para></para>
    /// <para>                digitalGauge1.SkewAngleX = -10; </para>
    /// <para></para>
    /// <para>                digitalGauge1.SegmentWidth = 5; </para>
    /// <para></para>
    /// <para>                digitalGauge1.SegmentSpacing = 3; </para>
    /// <para></para>
    /// <para>                digitalGauge1.CharacterSpacing = 5; </para>
    /// <para></para>
    /// <para>                digitalGauge1.Value = &quot;8:50 AM&quot;; </para>
    /// <para></para>
    /// <para>                this.Content = digitalGauge1; </para>
    /// <para></para>
    /// <para>            } </para>
    /// <para></para>
    /// <para>        } </para>
    /// <para></para>
    /// <para>}</para>
    /// </example>
    internal class CharacterFourteen : CharacterBase
    {
        #region Private Members
       
        /// <summary>
        /// Collection of boolean values indicating what character segments should be 
        /// drawn with foreground brush.
        /// </summary>
        private List<bool> m_listSegments = new List<bool>(14);
        
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
            
            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Participates in rendering operations that are directed by the layout system.
        /// </summary>
        /// <returns>The grid containing the segments.</returns>
        public Grid OnRender()
        {
            Grid grid=new Grid();
           if (m_listSegments.Count >= 14)
            {            
                for (int i = 13; i >= 0; i--)
                {
                    Path tempath= RetreiveSegment(i);
                    if (m_listSegments[i])
                    {
                        tempath.Fill = this.SegmentBrush;
                        tempath.Stroke = new SolidColorBrush(Colors.Black);
                        tempath.StrokeThickness = 0.2;
                        tempath.SetValue(Canvas.ZIndexProperty, 1);
                    }
                    else
                    {
                        tempath.Fill = this.DimmedBrush;
                    }

                    grid.Children.Add(tempath);
                }               
           }

           Path dotpath = new Path();
           GeometryGroup pathGeometry = new GeometryGroup();
           EllipseGeometry ellipseGeometry = new EllipseGeometry();
           ellipseGeometry.RadiusX = this.SegmentWidth / 2;
           ellipseGeometry.RadiusY = this.SegmentWidth / 2;
           ellipseGeometry.Center = new Point(this.SegmentWidth + (this.CharacterHeight / 5), this.CharacterHeight - (this.SegmentWidth / 2));
           pathGeometry.Children.Add(ellipseGeometry);
           dotpath.Data = pathGeometry;
           if (this.DrawDot)
           {
               dotpath.Fill = this.SegmentBrush;
           }
           else
           {
               dotpath.Fill = new SolidColorBrush(Colors.Transparent);
           }

           grid.Children.Add(dotpath);

           Path colonpath = new Path();
           pathGeometry = new GeometryGroup();
           ellipseGeometry = new EllipseGeometry();
           ellipseGeometry.RadiusX = this.SegmentWidth / 2;
           ellipseGeometry.RadiusY = this.SegmentWidth / 2;
           ellipseGeometry.Center = new Point(this.SegmentWidth + (this.CharacterHeight / 5), this.CharacterHeight / 4);
           pathGeometry.Children.Add(ellipseGeometry);
           EllipseGeometry ellipseGeometry1 = new EllipseGeometry();
           ellipseGeometry1.RadiusX = this.SegmentWidth / 2;
           ellipseGeometry1.RadiusY = this.SegmentWidth / 2;
           ellipseGeometry1.Center = new Point(this.SegmentWidth + (this.CharacterHeight / 5), 3 * this.CharacterHeight / 4);
           pathGeometry.Children.Add(ellipseGeometry1);
           colonpath.Data = pathGeometry;
           if (this.DrawColon)
           {
               colonpath.Fill = this.SegmentBrush;
           }
           else
           {
               colonpath.Fill = new SolidColorBrush(Colors.Transparent);
           }

           grid.Children.Add(colonpath);
          
           return grid;            
        }

        public Path RetreiveSegment(int position)
        {
            double height = this.Height / 2;
            height = this.CharacterHeight/2;
            double segmentvalue = this.SegmentsSpacing;
            if (segmentvalue > 1)
            {
                segmentvalue = 1;
            }

            if (height > 0)
            {
                double segmentSpacing = this.SegmentsSpacing;              

                if (position == 6 || position == 7)
                {
                    height = height / 2;
                }
               
                Point bottomCenter = new Point(this.SegmentWidth / 2, height - segmentSpacing);
                Point bottomLeft = new Point(0, height - (this.SegmentWidth / 2) - segmentSpacing);
                Point bottomRight = new Point(this.SegmentWidth, height - (this.SegmentWidth / 2) - segmentSpacing);
                Point topCenter = new Point(this.SegmentWidth / 2, segmentSpacing);
                Point topLeft = new Point(0, (this.SegmentWidth / 2) + segmentSpacing);
                Point topRight = new Point(this.SegmentWidth, (this.SegmentWidth / 2) + segmentSpacing);
                LineSegment bc = new LineSegment();               
                
                bc.Point = bottomCenter;
                LineSegment bl = new LineSegment();
                bl.Point = bottomLeft;
                LineSegment tl = new LineSegment();
                tl.Point = topLeft;
                LineSegment tc = new LineSegment();
                tc.Point = topCenter;
                LineSegment tr = new LineSegment();
                tr.Point = topRight;
                LineSegment br = new LineSegment();
                br.Point = bottomRight;
                PathFigure figure1 = new PathFigure();
                PathSegmentCollection ps = new PathSegmentCollection();
                ps.Add(bc);
                ps.Add(bl);
                ps.Add(tl);
                ps.Add(tc);
                ps.Add(tr);
                ps.Add(br);
                figure1.StartPoint = bottomCenter;
                figure1.Segments = ps;
                Path p = new Path();
                PathGeometry pg = new PathGeometry();
                pg.Figures.Add(figure1);
                TransformGroup tg = new TransformGroup();
                ScaleTransform st = new ScaleTransform();
                RotateTransform rt = new RotateTransform();
                rt.Angle = 0;
                rt.CenterX = this.SegmentWidth/2;
                rt.CenterY = 0;
                TranslateTransform tt = new TranslateTransform();
                tt.X = 0;
                tt.Y = 0;
                tg.Children.Add(rt);
                tg.Children.Add(tt);
                tg.Children.Add(st);
                p.Data = pg;
                p.RenderTransform = tg;                
                switch (position)
                {
                    case 0: tt.X = -height;
                        break;
                    case 1: tt.X =0;
                        tt.Y = 0;
                        rt.Angle = 90;
                        break;
                    case 2: tt.X = 0;
                        break;
                    case 3: tt.X = 0;
                        tt.Y = height;
                        break;
                    case 4: rt.Angle = 90;
                        tt.X =0;
                        tt.Y = 2 * height;
                        break;
                    case 5: tt.X = -height;
                        tt.Y = height;
                        break;
                    case 6: rt.Angle = 90;
                        tt.X = -height;
                         tt.Y =height * 2; 
                         break;
                    case 7: rt.Angle = -270;
                        tt.X = 0;
                        tt.Y = 2 * height; 
                        break;
                    case 8: tt.X = -height / 2;
                         break;
                    case 9: tt.X = -height / 2;
                         tt.Y = height; 
                         break;
                    case 10: rt.Angle = -26;
                         tt.X = -height;
                        tt.Y = 0; 
                        break;
                    case 11: rt.Angle = 26;
                        tt.X = 0;
                        tt.Y = 0; 
                        break;                    
                    case 12: rt.Angle = -156;
                        tt.X = -height;
                        tt.Y = 2 * height; 
                        break;
                    case 13: rt.Angle = -206;
                        tt.X = 0;
                        tt.Y = 2 * height;                        
                        break;                    
                }
                
                return p;
            }

            return new Path();
        }
        
        #endregion 
    }
}
