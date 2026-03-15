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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Collections;
using Syncfusion.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Gauge
{

    /// <summary>
    /// Radial gauges represents values of a given specific range.
    /// It comes with sophisticated support to provide endless possibilities for customization.
    /// With Essential Gauge, users can display several data ranges in a concise and compact area. 
    /// Data in the control can be easily depicted and quickly understood by users of any level.
    /// </summary>
    [Docking(DockingBehavior.Ask),ToolboxItem(true),
    ToolboxBitmap(typeof(Syncfusion.Windows.Forms.Gauge.RadialGauge), "ToolboxIcons.RadialGauge.png")]
    [Designer(typeof(RadialGaugeDesigner))]
    public class RadialGauge : Control
    {
        #region Variables
        private Point m_Center;
        Single m_value;
        Single m_MinValue;
        Single m_MaxValue;
        private Int32 m_GaugeArcRadius;
        private Int32 m_GaugeArcStart;
        private Int32 m_GaugeArcEnd;
        private Int32 m_NeedleWidth;
        private int m_NeedleRadius;
        private Int32 m_MajorTickMarkWidth;
        private Int32 m_MajorTickMarkHeight;
        private Int32 m_MinorTickMarkHeight;
        private Int32 m_MinorInnerLinesHeight;
        private Single m_MajorLinesDifference;
        private Int32 m_NumbersRadius;
        private Single fontBoundY1;
        private Single fontBoundY2;
        private Int32 m_MinorDifference;
        private ThemeStyle visualStyle;
        #region GaugeColor properties
        private Color m_NeedleColor = Color.Gray;
        private Color m_ScaleLabelColor = Color.Gray;
        private Color m_InterLineColor = Color.Gray;
        private Color m_MajorLinesColor = Color.Gray;
        private Color m_MinorTickMarkColor = Color.Gray;
        private Color m_GaugeArcColor = Color.Gray;
        private Color m_BackgroundGradientStartColor = Color.FromArgb(240, 240, 240);
        private Color m_BackgroundGradientEndColor = Color.FromArgb(210, 210, 210);
        private Color m_OuterFrameGradientStartColor = Color.FromArgb(229, 229, 229);
        private Color m_OuterFrameGradientEndColor = Color.FromArgb(172, 172, 172);
        private Color m_InnerFrameGradientStartColor = Color.FromArgb(180, 180, 180);
        private Color m_InnerFrameGradientEndColor = Color.FromArgb(194, 194, 194);
        private NeedleStyle needleStyle;
        private Font m_GaugeLableFont = null;
        private Font m_GaugeValueFont = null;
        private Color m_GaugeLabelColor = Color.Gray;
        private Color m_GaugeValueColor = Color.Gray;
        #endregion

        #endregion

        #region EventHandler

        [Description("This event is raised when gauge value changed.")]
        public event EventHandler ValueChanged;
        private void OnValueChanged()
        {
            EventHandler e = ValueChanged;
            if (e != null) e(this, null);
        }

        [Description("This event is raised if the value is entering or leaving defined range.")]
        public event EventHandler<ThresholdValueChangedEventArgs> ThresholdValueChanged;
        private void OnThresholdValueChanged(Range range, Single value)
        {
            EventHandler<ThresholdValueChangedEventArgs> e = ThresholdValueChanged;
            if (e != null) e(this, new ThresholdValueChangedEventArgs(range, value, range.InRange));
        }

        #endregion

        #region DataVariables
        private ListChangedEventHandler listChangedHandler;
        private EventHandler positionChangedHandler;
        private object dataSource;
        private string dataMember;
        private CurrencyManager dataManager;
        private ListView list;
        #endregion

        #region Constructor and Initialization
        /// <summary>
        /// Constructor for Gauge
        /// </summary>
        public RadialGauge()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(RadialGauge));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            this.SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            InitSettings();
            _GaugeRanges = new RangeCollection(this);
            this.MinimumSize = new Size(230, 230);
            m_Center = new Point(this.Width / 2, this.Height / 2);
            list = new ListView();
            listChangedHandler = new ListChangedEventHandler(dataManager_ListChanged);
        }

        private void InitSettings()
        {
            m_Center = new Point(100, 100);
            m_value = 0;
            m_MinValue = 0;
            m_MaxValue = 120;
            m_GaugeArcRadius = 70;
            m_GaugeArcStart = 135;
            m_GaugeArcEnd = 270;
            m_NeedleWidth = 2;
            m_NeedleRadius = 60;
            m_MajorTickMarkWidth = 1;
            m_MajorTickMarkHeight = 10;
            m_MinorTickMarkHeight = 5;
            m_MinorInnerLinesHeight = 5;
            m_MajorLinesDifference = 20.0f;
            m_NumbersRadius = 95;
            m_MinorDifference = 1;
            needleStyle = NeedleStyle.Default;
            visualStyle = ThemeStyle.None;
            m_GaugeLableFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            m_GaugeValueFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Ovverrides Paint event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            FindFontBounds(e.Graphics);

            #region Outer Arc

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            GraphicsPath pth = new GraphicsPath();
            Color c = this.Parent != null ? this.Parent.BackColor : Color.Empty;
            Rectangle r = new Rectangle(0, 0, this.Width, this.Height);
            System.Drawing.Drawing2D.GraphicsPath basePath = new System.Drawing.Drawing2D.GraphicsPath();

            int x = this.Width;
            int y = this.Height;

            //Define rectangles inside which we will draw circles.

            Rectangle rect = new Rectangle(0 + 10, 0 + 10, (int)x - 20, (int)y - 20);
            Rectangle rectrim = new Rectangle(0 + 23, 0 + 23, (int)x - 46, (int)y - 46);
            Rectangle rectinner = new Rectangle(0 + 40, 0 + 40, (int)x - 80, (int)y - 80);            

            if (this.FrameType == FrameType.FullCircle)
            {
                //OUTER
                LinearGradientBrush gb = new LinearGradientBrush(rect, m_OuterFrameGradientStartColor,m_OuterFrameGradientEndColor, LinearGradientMode.Vertical);

                pth.AddEllipse(rect);
                if (TransparentBackGround)
                    this.Region = new Region(pth);
                PathGradientBrush pgb = new PathGradientBrush(pth);

                pgb.CenterColor = m_BackgroundGradientStartColor;

                pgb.SurroundColors = new Color[] { m_BackgroundGradientEndColor };

                pgb.FocusScales = new PointF(0.1f, 0.1f);
                
                Blend bevelBlend = new Blend();
                bevelBlend.Positions = new float[] { 0.0f, .2f, .4f, .6f, .8f, 1.0f };
                bevelBlend.Factors = new float[] { .2f, .4f, .6f, .6f, 1f, 1f };
                Rectangle lgbRect = rect;
                lgbRect.Inflate(1, 1);
                LinearGradientBrush innerBevelBrush = new LinearGradientBrush(lgbRect,
                                                    m_InnerFrameGradientStartColor,
                                                    m_InnerFrameGradientEndColor,
                                                    LinearGradientMode.Vertical);

                innerBevelBrush.Blend = bevelBlend;
                if (this.ShowBackgroundFrame)
                {
                    e.Graphics.FillEllipse(gb, rect);
                    e.Graphics.FillEllipse(innerBevelBrush, rectrim);
                    rectrim.Inflate(-3, -3);

                    e.Graphics.FillEllipse(pgb, rectrim);
                }
            }
            else
            {
                x = this.Width;
                y = this.Width;
                rect = new Rectangle(0 + 10, 0 + 10, (int)x - 20, (int)y - 20);
                rectrim = new Rectangle(0 + 23, 0 + 23, (int)x - 46, (int)y - 46);

                //OUTER
                LinearGradientBrush gb = new LinearGradientBrush(rect, m_OuterFrameGradientStartColor, m_OuterFrameGradientEndColor, LinearGradientMode.Vertical);

                pth.AddEllipse(rect);
                if (TransparentBackGround)
                    this.Region = new Region(pth);
                PathGradientBrush pgb = new PathGradientBrush(pth);

                pgb.CenterColor = m_BackgroundGradientStartColor;

                pgb.SurroundColors = new Color[] { m_BackgroundGradientEndColor };

                pgb.FocusScales = new PointF(0.1f, 0.1f);

                Blend bevelBlend = new Blend();
                bevelBlend.Positions = new float[] { 0.0f, .2f, .4f, .6f, .8f, 1.0f };
                bevelBlend.Factors = new float[] { .2f, .4f, .6f, .6f, 1f, 1f };
                Rectangle lgbRect = rect;
                lgbRect.Inflate(1, 1);
                LinearGradientBrush innerBevelBrush = new LinearGradientBrush(lgbRect,
                                                    m_InnerFrameGradientStartColor,
                                                    m_InnerFrameGradientEndColor,
                                                    LinearGradientMode.BackwardDiagonal);

                innerBevelBrush.Blend = bevelBlend;
                if (this.ShowBackgroundFrame)
                {
                    e.Graphics.FillEllipse(gb, rect);
                    //Inner background
                    e.Graphics.FillEllipse(innerBevelBrush, rectrim);
                    rectrim.Inflate(-3, -3);
                    //Center circle
                    e.Graphics.FillEllipse(pgb, rectrim);
                    Rectangle rc = new Rectangle(27, this.Height - 12, this.Width - 54, 12);
                    //Bottom rect
                    e.Graphics.FillRectangle(gb, rc);
                }
            }

            GraphicsPath gp = new GraphicsPath();

            if (m_GaugeArcRadius > 0)
            {
                e.Graphics.DrawArc(new Pen(this.GaugeArcColor,2), new Rectangle(m_Center.X - GaugeRadius, m_Center.Y - GaugeRadius,
                   2 * GaugeRadius, 2 * GaugeRadius), m_GaugeArcStart, m_GaugeArcEnd);
            }


            #endregion

            //Draw Gauge needle
            DrawNeedle(e.Graphics);

            // Draw Label in center of Gauge
            DrawLabel(e.Graphics);

            //Draw Gauge Range selected value
            DrawRanges(gp, e.Graphics);

            //Draw Gauge Major,inner lines and numbers
            DrawLines(gp, e.Graphics);            

        }
        /// <summary>
        /// Overrides resize method
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            ChangeFrame();            
            m_NeedleRadius = m_GaugeArcRadius;
            DrawPointer();
        }

        #endregion

        # region HelperClass

        #region Ranges
        /// <summary>
        /// method used to draw the range line
        /// </summary>
        /// <param name="gp"></param>
        /// <param name="gr"></param>
        public void DrawRanges(GraphicsPath gp, Graphics gr)
        {
            Single rangeStartAngle;
            Single rangeSweepAngle;

            foreach (Range ptrRange in _GaugeRanges)
            {
                if (ptrRange.EndValue > ptrRange.StartValue)
                {
                    rangeStartAngle = m_GaugeArcStart + (ptrRange.StartValue - m_MinValue) * m_GaugeArcEnd / (m_MaxValue - m_MinValue);
                    rangeSweepAngle = (ptrRange.EndValue - ptrRange.StartValue) * m_GaugeArcEnd / (m_MaxValue - m_MinValue);

                    if (ptrRange.RangePlacement == TickPlacement.Inside)
                    {
                        gr.SmoothingMode = SmoothingMode.AntiAlias;
                        gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        int m_GaugeArcRadius1 = m_GaugeArcRadius - m_MajorTickMarkHeight - ptrRange.Height;
                        gr.DrawArc(new Pen(ptrRange.Color, ptrRange.Height), new Rectangle(m_Center.X - m_GaugeArcRadius1, m_Center.Y - m_GaugeArcRadius1, 2 * m_GaugeArcRadius1, 2 * m_GaugeArcRadius1), rangeStartAngle, rangeSweepAngle);
                    }
                    else
                    {
                        Int32 linevalue = m_GaugeArcRadius + m_MajorTickMarkHeight;                        
                        gr.SmoothingMode = SmoothingMode.AntiAlias;
                        gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        int m_GaugeArcRadius1 = m_GaugeArcRadius + m_MajorTickMarkHeight;
                        gr.DrawArc(new Pen(ptrRange.Color, ptrRange.Height), new Rectangle(m_Center.X - m_GaugeArcRadius1, m_Center.Y - m_GaugeArcRadius1, 2 * m_GaugeArcRadius1, 2 * m_GaugeArcRadius1), rangeStartAngle, rangeSweepAngle);                        
                    }

                }
            }

        }
        #endregion       

        #region Needle
        /// <summary>
        /// Gets or sets a value indicating the Needle Style.
        /// </summary>
        [
          Category("Appearance"),
          Description("Specifies the gauge Needle Style.")
        ]
        public NeedleStyle NeedleStyle
        {
            get
            {
                return needleStyle;
            }
            set
            {
                needleStyle = value;
                Refresh();
            }
        }
        /// <summary>
        /// method used to Draw the needle
        /// </summary>
        /// <param name="graphics"></param>
        public void DrawNeedle(Graphics graphics)
        {
            Single brushAngle = (Int32)(m_GaugeArcStart + (m_value - m_MinValue) * m_GaugeArcEnd / (m_MaxValue - m_MinValue)) % 360;
            Double needleAngle = brushAngle * Math.PI / 180;
            if (ShowNeedle)
            {
                switch (NeedleStyle)
                {
                    case NeedleStyle.Advanced:
                        PointF[] points = new PointF[3];
                        Brush brush1 = Brushes.White;
                        Brush brush2 = Brushes.White;
                        Brush brush3 = Brushes.White;
                        Brush brush4 = Brushes.White;

                        Brush brushBucket = Brushes.White;
                        Int32 subcol = (Int32)(((brushAngle + 225) % 180) * 100 / 180);
                        Int32 subcol2 = (Int32)(((brushAngle + 135) % 180) * 100 / 180);

                        graphics.FillEllipse(new SolidBrush(this.NeedleColor), m_Center.X - m_NeedleWidth * 3, m_Center.Y - m_NeedleWidth * 3, m_NeedleWidth * 6, m_NeedleWidth * 6);
                        brush1 = new SolidBrush(ControlPaint.Light(this.NeedleColor));
                        brush2 = new SolidBrush(ControlPaint.LightLight(this.NeedleColor));
                        brush3 = new SolidBrush(ControlPaint.Light(this.NeedleColor));
                        brush4 = new SolidBrush(ControlPaint.Light(this.NeedleColor));
                        graphics.DrawEllipse(new Pen(this.NeedleColor), m_Center.X - m_NeedleWidth * 3, m_Center.Y - m_NeedleWidth * 3, m_NeedleWidth * 6, m_NeedleWidth * 6);

                        if (Math.Floor((Single)(((brushAngle + 225) % 360) / 180.0)) == 0)
                        {
                            brushBucket = brush1;
                            brush1 = brush2;
                            brush2 = brushBucket;
                        }

                        if (Math.Floor((Single)(((brushAngle + 135) % 360) / 180.0)) == 0)
                        {
                            brush4 = brush3;
                        }

                        points[0].X = (Single)(m_Center.X + m_NeedleRadius * Math.Cos(needleAngle));
                        points[0].Y = (Single)(m_Center.Y + m_NeedleRadius * Math.Sin(needleAngle));
                        points[1].X = (Single)(m_Center.X - m_NeedleRadius / 20 * Math.Cos(needleAngle));
                        points[1].Y = (Single)(m_Center.Y - m_NeedleRadius / 20 * Math.Sin(needleAngle));
                        points[2].X = (Single)(m_Center.X - m_NeedleRadius / 5 * Math.Cos(needleAngle) + m_NeedleWidth * 2 * Math.Cos(needleAngle + Math.PI / 2));
                        points[2].Y = (Single)(m_Center.Y - m_NeedleRadius / 5 * Math.Sin(needleAngle) + m_NeedleWidth * 2 * Math.Sin(needleAngle + Math.PI / 2));
                        graphics.FillPolygon(brush1, points);

                        points[2].X = (Single)(m_Center.X - m_NeedleRadius / 5 * Math.Cos(needleAngle) + m_NeedleWidth * 2 * Math.Cos(needleAngle - Math.PI / 2));
                        points[2].Y = (Single)(m_Center.Y - m_NeedleRadius / 5 * Math.Sin(needleAngle) + m_NeedleWidth * 2 * Math.Sin(needleAngle - Math.PI / 2));
                        graphics.FillPolygon(brush2, points);

                        points[0].X = (Single)(m_Center.X - (m_NeedleRadius / 20 - 1) * Math.Cos(needleAngle));
                        points[0].Y = (Single)(m_Center.Y - (m_NeedleRadius / 20 - 1) * Math.Sin(needleAngle));
                        points[1].X = (Single)(m_Center.X - m_NeedleRadius / 5 * Math.Cos(needleAngle) + m_NeedleWidth * 2 * Math.Cos(needleAngle + Math.PI / 2));
                        points[1].Y = (Single)(m_Center.Y - m_NeedleRadius / 5 * Math.Sin(needleAngle) + m_NeedleWidth * 2 * Math.Sin(needleAngle + Math.PI / 2));
                        points[2].X = (Single)(m_Center.X - m_NeedleRadius / 5 * Math.Cos(needleAngle) + m_NeedleWidth * 2 * Math.Cos(needleAngle - Math.PI / 2));
                        points[2].Y = (Single)(m_Center.Y - m_NeedleRadius / 5 * Math.Sin(needleAngle) + m_NeedleWidth * 2 * Math.Sin(needleAngle - Math.PI / 2));
                        graphics.FillPolygon(brush4, points);

                        points[0].X = (Single)(m_Center.X - m_NeedleRadius / 20 * Math.Cos(needleAngle));
                        points[0].Y = (Single)(m_Center.Y - m_NeedleRadius / 20 * Math.Sin(needleAngle));
                        points[1].X = (Single)(m_Center.X + m_NeedleRadius * Math.Cos(needleAngle));
                        points[1].Y = (Single)(m_Center.Y + m_NeedleRadius * Math.Sin(needleAngle));

                        graphics.DrawLine(new Pen(this.NeedleColor), m_Center.X, m_Center.Y, points[0].X, points[0].Y);
                        graphics.DrawLine(new Pen(this.NeedleColor), m_Center.X, m_Center.Y, points[1].X, points[1].Y);
                        break;
                    case NeedleStyle.Default:
                        Point startPoint = new Point((Int32)(m_Center.X - m_NeedleRadius / 8 * Math.Cos(needleAngle)),
                                                    (Int32)(m_Center.Y - m_NeedleRadius / 8 * Math.Sin(needleAngle)));
                        Point endPoint = new Point((Int32)(m_Center.X + m_NeedleRadius * Math.Cos(needleAngle)),
                                                 (Int32)(m_Center.Y + m_NeedleRadius * Math.Sin(needleAngle)));

                        graphics.FillEllipse(new SolidBrush(this.NeedleColor), m_Center.X - m_NeedleWidth * 3, m_Center.Y - m_NeedleWidth * 3, m_NeedleWidth * 6, m_NeedleWidth * 6);
                        graphics.DrawLine(new Pen(this.NeedleColor, m_NeedleWidth), m_Center.X, m_Center.Y, endPoint.X, endPoint.Y);
                        //graphics.DrawLine(new Pen(this.NeedleColor, m_NeedleWidth), m_Center.X, m_Center.Y, startPoint.X, startPoint.Y);
                        break;
                    case NeedleStyle.Pointer:
                        startPoint = new Point((Int32)(m_Center.X - m_NeedleRadius / 8 * Math.Cos(needleAngle)),
                                                  (Int32)(m_Center.Y - m_NeedleRadius / 8 * Math.Sin(needleAngle)));
                        endPoint = new Point((Int32)(m_Center.X + m_NeedleRadius * Math.Cos(needleAngle)),
                                                (Int32)(m_Center.Y + m_NeedleRadius * Math.Sin(needleAngle)));
                        PointF[] pointerPoints = new PointF[3];
                        brush4 = new SolidBrush(ControlPaint.Light(this.NeedleColor));

                        pointerPoints[0].X = (Single)(m_Center.X - (m_NeedleRadius / 20 - 1) * Math.Cos(needleAngle));
                        pointerPoints[0].Y = (Single)(m_Center.Y - (m_NeedleRadius / 20 - 1) * Math.Sin(needleAngle));
                        pointerPoints[1].X = (Single)(m_Center.X - m_NeedleRadius / 5 * Math.Cos(needleAngle) + m_NeedleWidth * 2 * Math.Cos(needleAngle + Math.PI / 2));
                        pointerPoints[1].Y = (Single)(m_Center.Y - m_NeedleRadius / 5 * Math.Sin(needleAngle) + m_NeedleWidth * 2 * Math.Sin(needleAngle + Math.PI / 2));
                        pointerPoints[2].X = (Single)(m_Center.X - m_NeedleRadius / 5 * Math.Cos(needleAngle) + m_NeedleWidth * 2 * Math.Cos(needleAngle - Math.PI / 2));
                        pointerPoints[2].Y = (Single)(m_Center.Y - m_NeedleRadius / 5 * Math.Sin(needleAngle) + m_NeedleWidth * 2 * Math.Sin(needleAngle - Math.PI / 2));
                        graphics.FillPolygon(brush4, pointerPoints);

                        pointerPoints[0].X = (Single)(m_Center.X - m_NeedleRadius / 20 * Math.Cos(needleAngle));
                        pointerPoints[0].Y = (Single)(m_Center.Y - m_NeedleRadius / 20 * Math.Sin(needleAngle));
                        pointerPoints[1].X = (Single)(m_Center.X + m_NeedleRadius * Math.Cos(needleAngle));
                        pointerPoints[1].Y = (Single)(m_Center.Y + m_NeedleRadius * Math.Sin(needleAngle));

                        graphics.DrawLine(new Pen(this.NeedleColor), m_Center.X, m_Center.Y, pointerPoints[0].X, pointerPoints[0].Y);
                        graphics.DrawLine(new Pen(this.NeedleColor), m_Center.X, m_Center.Y, pointerPoints[1].X, pointerPoints[1].Y);
                        graphics.FillEllipse(new SolidBrush(this.NeedleColor), m_Center.X - m_NeedleWidth * 3, m_Center.Y - m_NeedleWidth * 3, m_NeedleWidth * 6, m_NeedleWidth * 6);
                        graphics.DrawEllipse(new Pen(this.NeedleColor), m_Center.X - m_NeedleWidth * 3, m_Center.Y - m_NeedleWidth * 3, m_NeedleWidth * 6, m_NeedleWidth * 6);
                        break;
                }
            }
        }
        #endregion

        #region Slider lines
        /// <summary>
        /// Used to draw the Tick lines 
        /// </summary>
        /// <param name="gp"></param>
        /// <param name="graphics"></param>
        public void DrawLines(GraphicsPath gp, Graphics graphics)
        {
            String valueText = "";
            SizeF boundingBox;
            Single countValue = 0;
            int TempGaugeArcRadius = m_GaugeArcRadius;
            if (this.TickPlacement == TickPlacement.Inside)
            {
                TempGaugeArcRadius = m_GaugeArcRadius - 4;
            }
            else
            {
                TempGaugeArcRadius = m_GaugeArcRadius + 2;
            }
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            while (countValue <= (m_MaxValue - m_MinValue))
            {
                valueText = (m_MinValue + countValue).ToString();
                graphics.ResetTransform();
                boundingBox = graphics.MeasureString(valueText, Font, -1, StringFormat.GenericTypographic);

                Int32 linevalue = TempGaugeArcRadius - m_MajorTickMarkHeight;
                Int32 lineminervalue = TempGaugeArcRadius - m_MinorTickMarkHeight;
                Int32 lineminerintervalue = TempGaugeArcRadius - m_MinorInnerLinesHeight;
                gp.Reset();
                gp.AddEllipse(new Rectangle(m_Center.X - TempGaugeArcRadius, m_Center.Y - TempGaugeArcRadius, 2 * TempGaugeArcRadius, 2 * TempGaugeArcRadius));
                gp.Reverse();
                gp.AddEllipse(new Rectangle(m_Center.X - linevalue, m_Center.Y - linevalue, 2 * linevalue, 2 * linevalue));
                gp.Reverse();
                graphics.SetClip(gp);

                graphics.DrawLine(new Pen(this.MajorTickMarkColor, m_MajorTickMarkWidth),
                               (Single)(m_Center.X),
                               (Single)(m_Center.Y),
                               (Single)(m_Center.X + 2 * lineminervalue * Math.Cos((m_GaugeArcStart + countValue * m_GaugeArcEnd / (m_MaxValue - m_MinValue)) * Math.PI / 180.0)),
                               (Single)(m_Center.Y + 2 * lineminervalue * Math.Sin((m_GaugeArcStart + countValue * m_GaugeArcEnd / (m_MaxValue - m_MinValue)) * Math.PI / 180.0)));

                gp.Reset();
                if (this.TickPlacement == TickPlacement.Inside)
                {
                    gp.AddEllipse(new Rectangle(m_Center.X - TempGaugeArcRadius, m_Center.Y - TempGaugeArcRadius, 2 * TempGaugeArcRadius, 2 * TempGaugeArcRadius));
                    gp.Reverse();
                    gp.AddEllipse(new Rectangle(m_Center.X - lineminervalue, m_Center.Y - lineminervalue, 2 * lineminervalue, 2 * lineminervalue));                    
                }
                else
                {
                    gp.AddEllipse(new Rectangle(m_Center.X - (linevalue + m_MinorTickMarkHeight), m_Center.Y - (linevalue + m_MinorTickMarkHeight), 2 * (linevalue + m_MinorTickMarkHeight), 2 * (linevalue + m_MinorTickMarkHeight)));
                    gp.Reverse();
                    gp.AddEllipse(new Rectangle(m_Center.X - linevalue, m_Center.Y - linevalue, 2 * linevalue, 2 * linevalue));
                }
                gp.Reverse();
                graphics.SetClip(gp);
                if (countValue < (m_MaxValue - m_MinValue))
                {
                    for (Int32 counter2 = 1; counter2 <= m_MinorDifference; counter2++)
                    {
                        if (((m_MinorDifference % 2) == 1) && ((Int32)(m_MinorDifference / 2) + 1 == counter2))
                        {
                            gp.Reset();
                            if (this.TickPlacement == TickPlacement.Inside)
                            {
                                gp.AddEllipse(new Rectangle(m_Center.X - lineminerintervalue, m_Center.Y - lineminerintervalue, 2 * lineminerintervalue, 2 * lineminerintervalue));
                                gp.Reverse();
                                gp.AddEllipse(new Rectangle(m_Center.X - TempGaugeArcRadius, m_Center.Y - TempGaugeArcRadius, 2 * TempGaugeArcRadius, 2 * TempGaugeArcRadius));
                            }
                            else
                            {
                                gp.AddEllipse(new Rectangle(m_Center.X - (linevalue + m_MinorInnerLinesHeight), m_Center.Y - (linevalue + m_MinorInnerLinesHeight), 2 * (linevalue + m_MinorInnerLinesHeight), 2 * (linevalue + m_MinorInnerLinesHeight)));
                                gp.Reverse();
                                gp.AddEllipse(new Rectangle(m_Center.X - linevalue, m_Center.Y - linevalue, 2 * linevalue, 2 * linevalue));
                            }
                            gp.Reverse();
                            graphics.SetClip(gp);
                            
                            graphics.DrawLine(new Pen(this.InterLinesColor),
                            (Single)(m_Center.X),
                            (Single)(m_Center.Y),
                            (Single)(m_Center.X + 2 * lineminerintervalue * Math.Cos((m_GaugeArcStart + countValue * m_GaugeArcEnd / (m_MaxValue - m_MinValue) + counter2 * m_GaugeArcEnd / (((Single)((m_MaxValue - m_MinValue) / m_MajorLinesDifference)) * (m_MinorDifference + 1))) * Math.PI / 180.0)),
                            (Single)(m_Center.Y + 2 * lineminerintervalue * Math.Sin((m_GaugeArcStart + countValue * m_GaugeArcEnd / (m_MaxValue - m_MinValue) + counter2 * m_GaugeArcEnd / (((Single)((m_MaxValue - m_MinValue) / m_MajorLinesDifference)) * (m_MinorDifference + 1))) * Math.PI / 180.0)));
                            gp.Reset();
                            if (this.TickPlacement == TickPlacement.Inside)
                            {
                                gp.AddEllipse(new Rectangle(m_Center.X - TempGaugeArcRadius, m_Center.Y - TempGaugeArcRadius, 2 * TempGaugeArcRadius, 2 * TempGaugeArcRadius));
                                gp.Reverse();
                                gp.AddEllipse(new Rectangle(m_Center.X - lineminervalue, m_Center.Y - lineminervalue, 2 * lineminervalue, 2 * lineminervalue));                                
                            }
                            else
                            {                              
                                gp.AddEllipse(new Rectangle(m_Center.X - (linevalue + m_MinorTickMarkHeight), m_Center.Y - (linevalue + m_MinorTickMarkHeight), 2 * (linevalue + m_MinorTickMarkHeight), 2 * (linevalue + m_MinorTickMarkHeight)));
                                gp.Reverse();
                                gp.AddEllipse(new Rectangle(m_Center.X - linevalue, m_Center.Y - linevalue, 2 * linevalue, 2 * linevalue));
                            }
                            gp.Reverse();
                            graphics.SetClip(gp);
                        }
                        else
                        {
                            graphics.DrawLine(new Pen(this.MinorTickMarkColor),
                            (Single)(m_Center.X),
                            (Single)(m_Center.Y),
                            (Single)(m_Center.X + 2 * lineminervalue * Math.Cos((m_GaugeArcStart + countValue * m_GaugeArcEnd / (m_MaxValue - m_MinValue) + counter2 * m_GaugeArcEnd / (((Single)((m_MaxValue - m_MinValue) / m_MajorLinesDifference)) * (m_MinorDifference + 1))) * Math.PI / 180.0)),
                            (Single)(m_Center.Y + 2 * lineminervalue * Math.Sin((m_GaugeArcStart + countValue * m_GaugeArcEnd / (m_MaxValue - m_MinValue) + counter2 * m_GaugeArcEnd / (((Single)((m_MaxValue - m_MinValue) / m_MajorLinesDifference)) * (m_MinorDifference + 1))) * Math.PI / 180.0)));
                        }
                    }
                }
                graphics.SetClip(ClientRectangle);

                if (ShowScaleLabel)
                {
                    if (this.TextOrientation != TextOrientation.Horizontal)
                    {
                        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                        graphics.RotateTransform(90.0F + m_GaugeArcStart + countValue * m_GaugeArcEnd / (m_MaxValue - m_MinValue));
                    }

                    graphics.TranslateTransform((Single)(m_Center.X + ScaleNumbersRadius * Math.Cos((m_GaugeArcStart + countValue * m_GaugeArcEnd / (m_MaxValue - m_MinValue)) * Math.PI / 180.0f)),
                                           (Single)(m_Center.Y + ScaleNumbersRadius * Math.Sin((m_GaugeArcStart + countValue * m_GaugeArcEnd / (m_MaxValue - m_MinValue)) * Math.PI / 180.0f)),
                                           System.Drawing.Drawing2D.MatrixOrder.Append);



                    graphics.DrawString(valueText, Font, new SolidBrush(this.ScaleLabelColor), -boundingBox.Width / 2, -fontBoundY1 - (fontBoundY2 - fontBoundY1 + 1) / 2, StringFormat.GenericTypographic);
                }

                countValue += m_MajorLinesDifference;
            }
        }
        /// <summary>
        /// Repaints the control
        /// </summary>
        public void RepaintControl()
        {
            Refresh();
        }

        #endregion
        private bool m_SetTransparent = false;
        [
         Category("Behavior"),
         Description("Gets or sets a value to make control background as transparent.")
        ]
        /// <summary>
        /// Gets or sets a value to make control background as transparent.
        /// </summary>
        public bool TransparentBackGround
        {
            get
            {
                return m_SetTransparent;
            }
            set
            {
                m_SetTransparent = value;

                UpdateStyles();
            }
        }
        private bool m_ShowBackgroundFrame = true;
        [
         Category("Behavior"),
         Description("Gets or sets a value to show or hide the Gauge's background frame.")
        ]
        /// <summary>
        /// Gets or sets a value to show or hide the Gauge's background frame.
        /// </summary>
        public bool ShowBackgroundFrame
        {
            get
            {
                return m_ShowBackgroundFrame;
            }
            set
            {
                m_ShowBackgroundFrame = value;

                this.Refresh();
            }
        }
        private bool m_ShowNeedle = true;
        [
         Category("Behavior"),
         Description("Gets or sets a value to show or hide gauge needle.")
        ]
        /// <summary>
        /// Gets or sets a value to show or hide gauge needle.
        /// </summary>
        public bool ShowNeedle
        {
            get
            {
                return m_ShowNeedle;
            }
            set
            {
                m_ShowNeedle = value;

                this.Refresh();
            }
        }

        private bool m_ShowScaleLabel = true;
        [
         Category("Behavior"),
         Description("Gets or sets a value to show or hide gauge label.")
        ]
        /// <summary>
        /// Gets or sets a value to show or hide gauge label.
        /// </summary>
        public bool ShowScaleLabel
        {
            get
            {
                return m_ShowScaleLabel;
            }
            set
            {
                m_ShowScaleLabel = value;

                this.Refresh();
            }
        }
        
        /// <summary>
        /// Specifies font for GaugeValue.
        /// </summary>
        [
            Category("Appearance"),
            Description("Specifies font for GaugeValue.")
        ]
        public Font GaugeValueFont
        {
            get
            {
                return m_GaugeValueFont;
            }
            set
            {
                m_GaugeValueFont = value;
                Refresh();
            }
        }
        
        /// <summary>
        /// Specifies font for GaugeLabel.
        /// </summary>
        [
            Category("Appearance"),
            Description("Specifies font for GaugeLabel")
        ]
        public Font GaugeLableFont
        {
            get
            {
                return m_GaugeLableFont;
            }
            set
            {
                m_GaugeLableFont = value;
                Refresh();
            }
        }
        
        /// <summary>
        /// Gets or sets a color for GaugeValue.
        /// </summary>
        [
           Category("Appearance"),
           Description("Gets or sets a color for GaugeValue.")
        ]
        public Color GaugeValueColor
        {
            get
            {
                return m_GaugeValueColor;
            }
            set
            {
                m_GaugeValueColor = value;
                Refresh();
            }
        }        
        /// <summary>
        /// Gets or sets a color for GaugeLabel
        /// </summary>
        [
           Category("Appearance"),
           Description("Gets or sets a color for GaugeLabel.")
        ]
        public Color GaugeLableColor
        {
            get
            {
                return m_GaugeLabelColor;
            }
            set
            {
                m_GaugeLabelColor = value;
                Refresh();
            }
        }

        #region Label
        private bool showGaugeValue = false;
        [
         Category("Behavior"),
         Description("Gets or sets a value to show or hide GaugeValue.")
        ]
        /// <summary>
        /// Gets or sets a value to show or hide GaugeValue.
        /// </summary>
        public bool ShowGaugeValue
        {
            get
            {
                return showGaugeValue;
            }
            set
            {
                showGaugeValue = value;
                Refresh();
            }
        }

        private string gaugeLabel = "Gauge";
        /// <summary>
        /// Gets or Sets a value to gauge label
        /// </summary>
        [
          Category("Customization"),
          Description("Gets or Sets a value to gauge label.")
        ]
        public string GaugeLabel
        {
            get
            {
                return gaugeLabel;
            }
            set
            {
                gaugeLabel = value;
                Refresh();
            }
        }
        /// <summary>
        /// Used to draw the label 
        /// </summary>
        /// <param name="graphics"></param>
        private void DrawLabel(Graphics graphics)
        {
            SolidBrush br = new SolidBrush(this.GaugeLableColor);
            SizeF s = graphics.MeasureString(GaugeLabel, GaugeLableFont);
            if (this.FrameType == FrameType.FullCircle)
            {
                graphics.DrawString(GaugeLabel, GaugeLableFont, br,
                    new Point((int)((m_Center.X) - (s.Width / 2)), (int)(m_Center.Y + GaugeRadius / 2) + 15));

                if (ShowGaugeValue)
                {
                    using (Font f = new Font(GaugeValueFont.Name, GaugeValueFont.Size + 4))
                    {
                        s = graphics.MeasureString(Value.ToString(), GaugeValueFont);
                        Point p = new Point((int)((m_Center.X) - (s.Width / 2)), (int)(m_Center.Y + GaugeRadius / 3));
                        // Create rectangle for region.
                        Rectangle excludeRect = new Rectangle(p.X-3,(p.Y- 4)+30, 32, 30);

                        // Create region for exclusion.
                        Region excludeRegion = new Region(excludeRect);

                        // Set clipping region to exclude region.
                        graphics.ExcludeClip(excludeRegion);

                        // Fill large rectangle to show clipping region.
                        br = new SolidBrush(this.GaugeValueColor);
                        graphics.DrawString(Value.ToString(), f,br ,p);

                    }
                }
            }
            else if (FrameType == FrameType.HalfCircle)
            {
                graphics.DrawString(GaugeLabel, GaugeLableFont, br,
                    new Point((int)((m_Center.X) - (s.Width / 2)), (int)(m_Center.Y + GaugeRadius / 6) ));
                if (ShowGaugeValue)
                {
                    using (Font f = new Font(GaugeValueFont.Name, GaugeValueFont.Size + 4))
                    {
                        br = new SolidBrush(this.GaugeValueColor);
                        s = graphics.MeasureString(Value.ToString(), GaugeValueFont);                        
                            graphics.DrawString(Value.ToString(), f, br,
                           new Point((int)((m_Center.X) - (s.Width / 2)), (int)(m_Center.Y - GaugeRadius / 3)));                        
                    }
                }
            }
            br.Dispose();
        }
        /// <summary>
        /// Override text
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
        }
        #endregion

        # endregion
        #region ShouldSerialize

        protected bool ShouldSerializeBackgroundGradientEndColor()
        {
            return BackgroundGradientEndColor != Color.FromArgb(210, 210, 210); 
        }

        protected bool ShouldSerializeBackgroundGradientStartColor()
        {
            return BackgroundGradientStartColor != Color.FromArgb(240, 240, 240);
        }

        protected bool ShouldSerializeOuterFrameGradientStartColor()
        {
            return OuterFrameGradientStartColor != Color.FromArgb(229, 229, 229);
        }

        protected bool ShouldSerializeOuterFrameGradientEndColor()
        {
            return OuterFrameGradientEndColor != Color.FromArgb(172, 172, 172);
        }

        protected bool ShouldSerializeInnerFrameGradientStartColor()
        {
            return InnerFrameGradientStartColor != Color.FromArgb(180, 180, 180);
        }

        protected bool ShouldSerializeInnerFrameGradientEndColor()
        {
            return InnerFrameGradientEndColor != Color.FromArgb(194, 194, 194);
        }        

        protected bool ShouldSerializeVisualStyle()
        {
            return VisualStyle != ThemeStyle.None;
        }

        protected bool ShouldSerializeGaugeArcColor()
        {
            return GaugeArcColor != Color.Gray;
        }

        protected bool ShouldSerializeNeedleColor()
        {
            return NeedleColor != Color.Gray;
        }

        protected bool ShouldSerializeMajorTickMarkColor()
        {
            return MajorTickMarkColor != Color.Gray;
        }

        protected bool ShouldSerializeMinorTickMarkColor()
        {
            return MinorTickMarkColor != Color.Gray;
        }

        protected bool ShouldSerializeInterLinesColor()
        {
            return InterLinesColor != Color.Gray;
        }

        protected bool ShouldSerializeScaleLabelColor()
        {
            return ScaleLabelColor != Color.Gray;
        }

        protected bool ShouldSerializeTextOrientation()
        {
            return TextOrientation != TextOrientation.Horizontal;
        }

        protected bool ShouldSerializeMajorTickMarkHeight()
        {
            return MajorTickMarkHeight != 10;
        }

        protected bool ShouldSerializeMinorTickMarkHeight()
        {
            return MinorTickMarkHeight != 5;
        }

        protected bool ShouldSerializeMinorInnerLinesHeight()
        {
            return MinorInnerLinesHeight != 5;
        }

        protected bool ShouldSerializeMajorDifference()
        {
            return MajorDifference != 20;
        }

        protected bool ShouldSerializeMinorDifference()
        {
            return MinorDifference != 1;
        }

        protected bool ShouldSerializeValue()
        {
            return Value != 0;
        }

        protected bool ShouldSerializeMaximumValue()
        {
            return MaximumValue != 120;
        }

        protected bool ShouldSerializeMinimumValue()
        {
            return MinimumValue != 0;
        }

        protected bool ShouldSerializeShowGaugeValue()
        {
            return ShowGaugeValue != false;
        }

        protected bool ShouldSerializeNeedleStyle()
        {
            return NeedleStyle != NeedleStyle.Default;
        }

        protected bool ShouldSerializeFrameType()
        {
            return FrameType != FrameType.FullCircle;
        }

        protected bool ShouldSerializeTickPlacement()
        {
            return TickPlacement != TickPlacement.Inside;
        }

        protected bool ShouldSerializeLabelPlacement()
        {
            return LabelPlacement != LabelPlacement.Inside;
        }

        protected bool ShouldSerializeDisplayRecordIndex()
        {
            return DisplayRecordIndex != 0;
        }

        protected bool ShouldSerializeGaugeLableColor()
        {
            return GaugeLableColor != Color.Gray;
        }

        protected bool ShouldSerializeGaugeValueColor()
        {
            return GaugeValueColor != Color.Gray;
        }

        protected bool ShouldSerializeShowBackgroundFrame()
        {
            return ShowBackgroundFrame != true;
        }

        protected bool ShouldSerializeShowNeedle()
        {
            return ShowNeedle != true;
        }

        protected bool ShouldSerializeShowScaleLabel()
        {
            return ShowScaleLabel != true;
        }

        protected bool ShouldSerializeTransparentBackGround()
        {
            return TransparentBackGround != false;
        }

        protected void ResetBackgroundGradientEndColor()
        {
            BackgroundGradientEndColor = Color.FromArgb(210, 210, 210);
        }

        protected void ResetBackgroundGradientStartColor()
        {
            BackgroundGradientStartColor = Color.FromArgb(240, 240, 240);
        }

        protected void ResetOuterFrameGradientStartColor()
        {
            OuterFrameGradientStartColor = Color.FromArgb(229, 229, 229);
        }

        protected void ResetOuterFrameGradientEndColor()
        {
            OuterFrameGradientEndColor = Color.FromArgb(172, 172, 172);
        }

        protected void ResetInnerFrameGradientStartColor()
        {
            InnerFrameGradientStartColor = Color.FromArgb(180, 180, 180);
        }

        protected void ResetInnerFrameGradientEndColor()
        {
            InnerFrameGradientEndColor = Color.FromArgb(194, 194, 194);
        }

        protected void ResetVisualStyle()
        {
            VisualStyle = ThemeStyle.None;
        }

        protected void ResetGaugeArcColor()
        {
            GaugeArcColor = Color.Gray;
        }

        protected void ResetNeedleColor()
        {
            NeedleColor = Color.Gray;
        }

        protected void ResetMajorTickMarkColor()
        {
            MajorTickMarkColor = Color.Gray;
        }

        protected void ResetMinorTickMarkColor()
        {
            MinorTickMarkColor = Color.Gray;
        }

        protected void ResetInterLinesColor()
        {
            InterLinesColor = Color.Gray;
        }

        protected void ResetScaleLabelColor()
        {
            ScaleLabelColor = Color.Gray;
        }

        protected void ResetTextOrientation()
        {
            TextOrientation = TextOrientation.Horizontal;
        }

        protected void ResetMajorTickMarkHeight()
        {
            MajorTickMarkHeight = 10;
        }

        protected void ResetMinorTickMarkHeight()
        {
            MinorTickMarkHeight = 5;
        }

        protected void ResetMinorInnerLinesHeight()
        {
            MinorInnerLinesHeight = 5;
        }

        protected void ResetMajorDifference()
        {
            MajorDifference = 20;
        }

        protected void ResetMinorDifference()
        {
            MinorDifference = 1;
        }

        protected void ResetValue()
        {
            Value = 0;
        }

        protected void ResetMaximumValue()
        {
            MaximumValue = 120;
        }

        protected void ResetMinimumValue()
        {
            MinimumValue = 0;
        }

        protected void ResetShowGaugeValue()
        {
            ShowGaugeValue = false;
        }

        protected void ResetNeedleStyle()
        {
            NeedleStyle = NeedleStyle.Default;
        }

        protected void ResetFrameType()
        {
            FrameType = FrameType.FullCircle;
        }

        protected void ResetTickPlacement()
        {
            TickPlacement = TickPlacement.Inside;
        }

        protected void ResetLabelPlacement()
        {
            LabelPlacement = LabelPlacement.Inside;
        }

        protected void ResetDisplayRecordIndex()
        {
            DisplayRecordIndex = 0;
        }

        protected void ResetGaugeLableColor()
        {
            GaugeLableColor = Color.Gray;
        }

        protected void ResetGaugeValueColor()
        {
            GaugeValueColor = Color.Gray;
        }

        protected void ResetShowBackgroundFrame()
        {
            ShowBackgroundFrame = true;
        }

        protected void ResetShowNeedle()
        {
            ShowNeedle = true;
        }

        protected void ResetShowScaleLabel()
        {
            ShowScaleLabel = true;
        }

        protected void ResetTransparentBackGround()
        {
            TransparentBackGround = false;
        }
        # endregion

        #region Themes
        /// <summary>
        /// Specifies the visual style.
        /// </summary>
        [
          Category("Appearance"),
          Description("Specifies the visual style.")
        ]
        public ThemeStyle VisualStyle
        {
            get
            {
                return visualStyle;
            }
            set
            {
                visualStyle = value;
                SetVisualStyle();
                this.Refresh();

            }
        }
        /// <summary>
        /// Sets the Gauge controls visual styles
        /// </summary>
        private void SetVisualStyle()
        {

            if (VisualStyle == ThemeStyle.Blue)
            {
                m_BackgroundGradientStartColor = ColorTranslator.FromHtml("#ECF4FC");
                m_BackgroundGradientEndColor = ColorTranslator.FromHtml("#D8E4F2");
                m_OuterFrameGradientStartColor = ColorTranslator.FromHtml("#CEDDEE");
                m_OuterFrameGradientEndColor = ColorTranslator.FromHtml("#BDCAD9");
                m_InnerFrameGradientStartColor = ColorTranslator.FromHtml("#849DBD");
                m_InnerFrameGradientEndColor = ColorTranslator.FromHtml("#D8E4F2");
                this.NeedleColor = this.ScaleLabelColor = this.GaugeArcColor = this.MajorTickMarkColor = this.MinorTickMarkColor = this.InterLinesColor = ColorTranslator.FromHtml("#5F6F77");
            }
            else if (VisualStyle == ThemeStyle.Silver)
            {
                m_BackgroundGradientStartColor = ColorTranslator.FromHtml("#F5F5F5");
                m_BackgroundGradientEndColor = ColorTranslator.FromHtml("#E2E3E4");
                m_OuterFrameGradientStartColor = Color.FromArgb(229, 229, 229);
                m_OuterFrameGradientEndColor = Color.FromArgb(172, 172, 172);
                m_InnerFrameGradientStartColor = ColorTranslator.FromHtml("#878787");
                m_InnerFrameGradientEndColor = ColorTranslator.FromHtml("#F5F5F5");
                this.NeedleColor = this.ScaleLabelColor = this.GaugeArcColor = this.MajorTickMarkColor = this.MinorTickMarkColor = this.InterLinesColor = Color.Gray;
            }
            else if (VisualStyle == ThemeStyle.Black)
            {
                m_BackgroundGradientStartColor = ColorTranslator.FromHtml("#323031");
                m_BackgroundGradientEndColor = ColorTranslator.FromHtml("#232021");
                m_OuterFrameGradientStartColor = ColorTranslator.FromHtml("#333132");
                m_OuterFrameGradientEndColor = ColorTranslator.FromHtml("#262324");
                m_InnerFrameGradientEndColor = ColorTranslator.FromHtml("#232021");
                m_InnerFrameGradientStartColor  = ColorTranslator.FromHtml("#070707");
                this.NeedleColor = this.ScaleLabelColor = this.GaugeArcColor = this.MajorTickMarkColor = this.MinorTickMarkColor = this.InterLinesColor = Color.White;
            }
            else if (VisualStyle == ThemeStyle.Metro)
            {
                this.BackgroundGradientStartColor = Color.White;
                this.BackgroundGradientEndColor = Color.White;
                m_OuterFrameGradientStartColor = Color.FromArgb(17, 180, 205);
                m_OuterFrameGradientEndColor = Color.FromArgb(17, 180, 205);
                m_InnerFrameGradientStartColor = Color.FromArgb(180, 180, 180);
                m_InnerFrameGradientEndColor = Color.FromArgb(194, 194, 194);
                 this.GaugeArcColor = this.MajorTickMarkColor = this.MinorTickMarkColor = this.InterLinesColor = Color.Gray;
                 this.NeedleColor = Color.FromArgb(17, 180, 205);
                 this.ScaleLabelColor = Color.Gray;
            }
            else if (VisualStyle == ThemeStyle.None)
            {
                m_BackgroundGradientStartColor = Color.FromArgb(240, 240, 240);
                m_BackgroundGradientEndColor = Color.FromArgb(210, 210, 210);
                m_OuterFrameGradientStartColor = Color.FromArgb(229, 229, 229);
                m_OuterFrameGradientEndColor = Color.FromArgb(172, 172, 172);
                m_InnerFrameGradientStartColor = Color.FromArgb(180, 180, 180);
                m_InnerFrameGradientEndColor = Color.FromArgb(194, 194, 194);
                this.NeedleColor = this.ScaleLabelColor = this.GaugeArcColor = this.MajorTickMarkColor = this.MinorTickMarkColor = this.InterLinesColor = Color.Black;
            }

        }

        #endregion

        #region Frames
        FrameType frameType = FrameType.FullCircle;
        /// <summary>
        /// Gets or sets a value indicating the Frame Type of the gauge.
        /// </summary>
        [
          Category("Appearance"),
          Description("Specifies the gauge Frame Style.")
        ]
        public FrameType FrameType
        {
            get
            {
                return frameType;
            }
             set
            {
                frameType = value;
                ChangeFrame();
            }
        }
        /// <summary>
        /// Used to change the frame
        /// </summary>
        public void ChangeFrame()
        {
            if (this.FrameType == FrameType.HalfCircle)
            {
                this.MinimumSize = new Size(230, 152);
                m_GaugeArcStart = 180;
                m_GaugeArcEnd = 180;
                this.Size = new Size(this.Width, (this.Width / 3) * 2);
                m_Center = new Point(this.Width / 2, this.Width / 2);
                m_NeedleRadius = m_GaugeArcRadius-30;
            }
            else
            {
                m_Center = new Point(this.Width / 2, this.Height/2);
                m_GaugeArcStart = 135;
                m_GaugeArcEnd = 270;
                m_NeedleRadius = m_GaugeArcRadius-40;
                this.MinimumSize = new Size(230, 230);
                this.Size = new Size(this.Width, this.Width);
            }
            DrawPointer();
            Invalidate();
        }

        #endregion

        # region Gauge Colors 
        /// <summary>
        /// Gets or Sets the first color of the gradient inner background.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the first color of the gradient inner background.")
        ]
        public Color BackgroundGradientStartColor
        {
            get
            {
                return m_BackgroundGradientStartColor;
            }
            set
            {
                if (m_BackgroundGradientStartColor != value)
                {
                    m_BackgroundGradientStartColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or Sets the second color of the gradient inner background.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the second color of the gradient inner background.")
        ]
        public Color BackgroundGradientEndColor
        {
            get
            {
                return m_BackgroundGradientEndColor;
            }
            set
            {
                if (m_BackgroundGradientEndColor != value)
                {
                    m_BackgroundGradientEndColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or Sets the first gradient color for the inner frame.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the first gradient color for the inner frame.")
        ]
        public Color OuterFrameGradientStartColor
        {
            get
            {
                return m_OuterFrameGradientStartColor;
            }
            set
            {
                if (m_OuterFrameGradientStartColor != value)
                {
                    m_OuterFrameGradientStartColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or Sets the second gradient color for the outer frame.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the second gradient color for the outer frame.")
        ]
        public Color OuterFrameGradientEndColor
        {
            get
            {
                return m_OuterFrameGradientEndColor;
            }
            set
            {
                if (m_OuterFrameGradientEndColor != value)
                {
                    m_OuterFrameGradientEndColor = value;
                    this.Invalidate();
                }
            }
        }
        

        /// <summary>
        /// Gets or Sets the first gradient color for the Frame Border.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the first gradient color for the Frame Border.")
        ]
        public Color InnerFrameGradientStartColor
        {
            get
            {
                return m_InnerFrameGradientStartColor;
            }
            set
            {
                if (m_InnerFrameGradientStartColor != value)
                {
                    m_InnerFrameGradientStartColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or Sets the second gradient color for the Frame Border.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the second gradient color for the Frame Border.")
        ]
        public Color InnerFrameGradientEndColor
        {
            get
            {
                return m_InnerFrameGradientEndColor;
            }
            set
            {
                if (m_InnerFrameGradientEndColor != value)
                {
                    m_InnerFrameGradientEndColor = value;
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating the Major Lines color of the gauge.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the Major Lines Color.")
        ]
        public Color MajorTickMarkColor
        {
            get { return m_MajorLinesColor; }
            set
            {
                if (m_MajorLinesColor != value)
                {
                    m_MajorLinesColor = value;
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the Minor Lines color of the gauge.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the Minor Lines Color.")
        ]
        public Color MinorTickMarkColor
        {
            get { return m_MinorTickMarkColor; }
            set
            {
                if (m_MinorTickMarkColor != value)
                {
                    m_MinorTickMarkColor = value;
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the Inter Lines color of the gauge.
        /// </summary>
        [
           Category("Appearance"),
           Description("Gets or Sets the Inter Lines Color.")
         ]
        public Color InterLinesColor
        {
            get { return m_InterLineColor; }
            set
            {
                if (m_InterLineColor != value)
                {
                    m_InterLineColor = value;
                    Refresh();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating the Gauge Arc Color of the gauge.
        /// </summary>
        [
           Category("Appearance"),
           Description("Gets or Sets the Gauge Arc Color.")
        ]
        public Color GaugeArcColor
        {
            get { return m_GaugeArcColor; }
            set
            {
                if (m_GaugeArcColor != value)
                {
                    m_GaugeArcColor = value;
                    Refresh();
                }
            }
        }


        /// <summary>
        /// Gets or sets a value indicating the Needle Color of the gauge.
        /// </summary>
        [
           Category("Appearance"),
           Description("Gets or Sets the Gauge Needle Color.")
        ]
        public Color NeedleColor
        {
            get { return m_NeedleColor; }
            set
            {
                if (m_NeedleColor != value)
                {
                    m_NeedleColor = value;

                    Refresh();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating the Numbers color of the gauge.
        /// </summary>
        [
           Category("Appearance"),
           Description("Gets or Sets the color for numeric labels in radial scale.")
        ]
        public Color ScaleLabelColor
        {
            get { return m_ScaleLabelColor; }
            set
            {
                if (m_ScaleLabelColor != value)
                {
                    m_ScaleLabelColor = value;
                    Refresh();
                }
            }
        }
        #endregion

        #region Line Heights
        /// <summary>
        /// Gets or Sets a value to gauge Major line Heights
        /// </summary>
        [
          Category("Customization"),
          Description("Gets or Sets a value to gauge Major line Heights.")
        ]
        public int MajorTickMarkHeight
        {
            get
            {
                return m_MajorTickMarkHeight;
            }
            set
            {
                m_MajorTickMarkHeight = value;
                DrawPointer();                
                Invalidate();
            }
        }
        /// <summary>
        /// Gets or Sets a value to gauge minor line Height
        /// </summary>
        [
          Category("Customization"),
          Description("Gets or Sets a value to gauge minor line Height.")
        ]
        public int MinorTickMarkHeight
        {
            get
            {
                return m_MinorTickMarkHeight;
            }
            set
            {
                m_MinorTickMarkHeight = value;
                Refresh();
            }
        }
        /// <summary>
        /// Gets or Sets a value to gauge minor inner line Height
        /// </summary>
        [
          Category("Customization"),
          Description("Gets or Sets a value to gauge minor inner line Height.")
        ]
        public int MinorInnerLinesHeight
        {
            get
            {
                return m_MinorInnerLinesHeight;
            }
            set
            {
                m_MinorInnerLinesHeight = value;
                Refresh();
            }
        }
        /// <summary>
        /// Gets or Sets a value to gauge Major line Difference
        /// </summary>
        [
          Category("Customization"),
          Description("Gets or Sets a value to gauge Major line Difference.")
        ]
        public Single MajorDifference
        {
            get { return m_MajorLinesDifference; }
            set
            {
                if ((m_MajorLinesDifference != value) && (value > 0))
                {
                    m_MajorLinesDifference = Math.Max(Math.Min(value, m_MaxValue), m_MinValue);

                    Refresh();
                }
            }
        }
        /// <summary>
        /// Gets or Sets a value to gauge minor line Difference
        /// </summary>
        [
          Category("Customization"),
          Description("Gets or Sets a value to gauge minor line Difference.")
        ]
        public Int32 MinorDifference
        {
            get { return m_MinorDifference; }
            set
            {
                if (m_MinorDifference != value)
                {
                    m_MinorDifference = value;

                    Refresh();
                }
            }
        }

        #endregion

        #region Backcolor
        /// <summary>
        /// Gets the Gauge backcolor
        /// </summary>
        [Browsable(false)]
        public override Color BackColor
        {
            get
            {
                return Color.Transparent;
            }
        }
        /// <summary>
        /// Gets or Sets the minimum size
        /// </summary>
        public override Size MinimumSize
        {
            get
            {
                return base.MinimumSize;
            }
            set
            {
                base.MinimumSize = value;
            }
        }

        private Color bgColor = Color.Red;
        /// <summary>
        /// Gets or Sets the background color
        /// </summary>
        private Color BackgroundColor
        {
            get
            {
                return bgColor;
            }
            set
            {
                bgColor = value;
                Invalidate();
            }
        }
        #endregion

        #region Radius
        private Int32 m_ScaleNumbersRadius = 115;
        private Int32 ScaleNumbersRadius
        {
            get
            {
                if (LabelPlacement == LabelPlacement.Inside)
                    return (m_GaugeArcRadius - ((2 * 10) + 5));
                else
                {
                    if (this.TickPlacement == TickPlacement.Inside)
                        return (m_GaugeArcRadius + ((2 * 10) - 5));
                    else
                    {
                        if (MajorTickMarkHeight > 10)
                            return (m_GaugeArcRadius + ((1 * MajorTickMarkHeight)));
                        else
                            return (m_GaugeArcRadius + ((1 * 10)));
                    }
                }
            }

        }

        private int GaugeRadius
        {
            get
            {
                if (LabelPlacement == LabelPlacement.Inside)
                {
                    m_NeedleRadius = ((this.Width / 2) - Width / 6) - 30;
                    return ((this.Width / 2) - Width / 7);
                }
                else
                {
                    m_NeedleRadius = ((this.Width / 2) - Width / 6) - 30;
                    return ((this.Width / 2) - Width / 7)-10;
                }
            }
        }
        #endregion

        #region Orientation and Placements

        #region Text Orientation
        private TextOrientation textOrientation = TextOrientation.Horizontal;
        /// <summary>
        /// Gets or sets a value indicating the Text Orientation of gauge values.
        /// </summary>
        [
            Category("Appearance"),
            Description("Gets or Sets the Gauge Text Orientation.")
        ]
        public TextOrientation TextOrientation
        {
            get
            {
                return textOrientation;
            }
            set
            {
                textOrientation = value;
                Invalidate();
            }
        }
        #endregion

        #region TickPlacement
        TickPlacement m_tickposition = TickPlacement.Inside;
        /// <summary>
        /// Gets or sets a value indicating the tick line placement of gauge values.
        /// </summary>
        [
            Category("Appearance"),
            Description("Gets or Sets the Gauge TickPlacement.")
        ]
        public TickPlacement TickPlacement
        {
            get
            {
                return m_tickposition;
            }
            set
            {
                if (m_tickposition != value)
                {
                    m_tickposition = value;
                    DrawPointer();
                }
                Invalidate();
            }
        }
        /// <summary>
        /// Used to draw the lines 
        /// </summary>
        private void DrawPointer()
        {
            if (TickPlacement == TickPlacement.Inside)
            {
                m_GaugeArcRadius = this.GaugeRadius;
            }
            else if (TickPlacement == TickPlacement.OutSide)
            {
                m_GaugeArcRadius = this.GaugeRadius + this.m_MajorTickMarkHeight;
            }
            else
            {
                m_GaugeArcRadius = this.GaugeRadius + (this.m_MajorTickMarkHeight / 2);
            }
        }
        #endregion

        #region NumberPlacement
        LabelPlacement m_NumberPlacement = LabelPlacement.Inside;
        /// <summary>
        /// Gets or sets a value indicating the value display position in gauge.
        /// </summary>
        [
            Category("Appearance"),
            Description("Gets or sets the placement type for labels in radial scale.")
        ]
        public LabelPlacement LabelPlacement 
        {
            get
            {
                return m_NumberPlacement;
            }
            set
            {
                if (m_NumberPlacement != value)
                {
                    m_NumberPlacement = value;
                    SetNumberPosition();
                    DrawPointer();
                }
                Invalidate();
            }
        }
        /// <summary>
        /// Used to set the label position
        /// </summary>
        private void SetNumberPosition()
        {
            if (LabelPlacement == LabelPlacement.Outside)
            {
                m_GaugeArcRadius = this.GaugeRadius + this.m_MajorTickMarkHeight;
                m_NumbersRadius = this.m_ScaleNumbersRadius;
            }
            else if (LabelPlacement == LabelPlacement.Inside)
            {
                m_NumbersRadius = this.m_ScaleNumbersRadius - 40;
            }

        }
        /// <summary>
        /// Used to calculate the text bounds 
        /// </summary>
        /// <param name="g"></param>
        private void FindFontBounds(Graphics g)
        {
            //find upper and lower bounds for numeric characters
            Int32 c1;
            Int32 c2;
            Boolean boundFound;
            Bitmap b;
            SolidBrush backBrush = new SolidBrush(Color.White);
            SolidBrush foreBrush = new SolidBrush(Color.Black);
            SizeF boundingBox;

            boundingBox = g.MeasureString("0123456789", Font, -1, StringFormat.GenericTypographic);
            b = new Bitmap((Int32)(boundingBox.Width), (Int32)(boundingBox.Height));
            g = Graphics.FromImage(b);
            g.FillRectangle(backBrush, 0.0F, 0.0F, boundingBox.Width, boundingBox.Height);
            g.DrawString("0123456789", Font, foreBrush, 0.0F, 0.0F, StringFormat.GenericTypographic);

            fontBoundY1 = 0;
            fontBoundY2 = 0;
            c1 = 0;
            boundFound = false;
            while ((c1 < b.Height) && (!boundFound))
            {
                c2 = 0;
                while ((c2 < b.Width) && (!boundFound))
                {
                    if (b.GetPixel(c2, c1) != backBrush.Color)
                    {
                        fontBoundY1 = c1;
                        boundFound = true;
                    }
                    c2++;
                }
                c1++;
            }

            c1 = b.Height - 1;
            boundFound = false;
            while ((0 < c1) && (!boundFound))
            {
                c2 = 0;
                while ((c2 < b.Width) && (!boundFound))
                {
                    if (b.GetPixel(c2, c1) != backBrush.Color)
                    {
                        fontBoundY2 = c1;
                        boundFound = true;
                    }
                    c2++;
                }
                c1--;
            }
        }
        #endregion

        #endregion

        #region Values
        /// <summary>
        /// Gets or Sets the minimum value to display on the RadialGauge
        /// </summary>
        [
         Category("Data"),
        Description("The minimum value to display on the RadialGauge.")
        ]
        public Single MinimumValue
        {
            get { return m_MinValue; }
            set
            {
                if ((m_MinValue != value) && (value < m_MaxValue))
                {
                    m_MinValue = value;
                    Refresh();
                }
            }
        }
        /// <summary>
        /// Gets or Sets the maximum value to display on the RadialGauge
        /// </summary>
        [
        Category("Data"),
        Description("The maximum value to display on the RadialGauge.")
        ]
        public Single MaximumValue
        {
            get { return m_MaxValue; }
            set
            {
                if ((m_MaxValue != value) && (value > m_MinValue))
                {
                    m_MaxValue = value;
                    Refresh();
                }
            }
        }
        /// <summary>
        /// Gets or Sets the value
        /// </summary>
         [
         Category("Data"),
         Description("Gets or Sets the value.")
        ]
        public Single Value
        {
            get { return m_value; }
            set
            {
                value = Math.Min(Math.Max(value, m_MinValue), m_MaxValue);
                if (m_value != value)
                {
                    m_value = value;
                     OnValueChanged();

                    foreach (Range ptrRange in _GaugeRanges)
                    {
                        if ((m_value >= ptrRange.StartValue)
                            && (m_value <= ptrRange.EndValue))
                        {
                            //Entering Range
                            if (!ptrRange.InRange)
                            {
                                ptrRange.InRange = true;
                                OnThresholdValueChanged(ptrRange, m_value);
                            }
                        }
                        else
                        {
                            //Leaving Range
                            if (ptrRange.InRange)
                            {
                                ptrRange.InRange = false;
                                OnThresholdValueChanged(ptrRange, m_value);
                            }
                        }
                    }
                    Refresh();
                }
            }
        }
        #endregion

        #region Ranges
        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RangeCollection Ranges { get { return _GaugeRanges; } }
        private RangeCollection _GaugeRanges;
        #endregion

        #region DataBinding
        #region Context Changed
        protected override void OnBindingContextChanged(EventArgs e)
        {
            this.EnsureDataBinding();
            base.OnBindingContextChanged(e);
        }
        #endregion

        #region EnsureDataBinding
        /// <summary>
        /// Tries to get a new CurrencyManager for new DataBinding
        /// </summary>
        private void EnsureDataBinding()
        {
            if (this.DataSource == null ||
                base.BindingContext == null)
                return;

            CurrencyManager cm;
            try
            {
                cm = (CurrencyManager)base.BindingContext[this.DataSource, this.DataMember];
            }
            catch (System.ArgumentException)
            {
                // If no CurrencyManager was found
                return;
            }
            if (this.dataManager != cm)
            {
                // Unwire the old CurrencyManager
                if (this.dataManager != null)
                {
                    this.dataManager.ListChanged -= listChangedHandler;
                    this.dataManager.PositionChanged -= positionChangedHandler;
                }
                this.dataManager = cm;
                // Wire the new CurrencyManager
                if (this.dataManager != null)
                {
                    this.dataManager.ListChanged += listChangedHandler;
                    this.dataManager.PositionChanged += positionChangedHandler;
                }

                // Update metadata and data
                CalculateColumns();
                UpdateAllData();
            }
        }
        #endregion

        #region Item(s) changed from DataSource
        /// <summary>
        /// Datasources get updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataManager_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.Reset ||
                e.ListChangedType == ListChangedType.ItemMoved)
            {
                // Update all data
                UpdateAllData();
            }
            else if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                // Add new Item
                AddItem(e.NewIndex);
            }
            else if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                // Change Item
                UpdateItem(e.NewIndex);
            }
            else if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                // Delete Item
                DeleteItem(e.NewIndex);
            }
            else
            {
                // Update metadata and all data
                CalculateColumns();
                UpdateAllData();
            }
            PopulateGauge();
        }
        #endregion

        #region Item Methods
        /// <summary>
        /// Updates all Items.
        /// </summary>
        private void UpdateAllData()
        {
            list.Items.Clear();
            for (int i = 0; i < DataManager.Count; i++)
            {
                AddItem(i);
            }
        }

        /// <summary>
        /// Adds a new item.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        private void AddItem(int index)
        {
            ListViewItem item = GetListViewItem(index);
            this.list.Items.Insert(index, item);
        }

        /// <summary>
        /// Updates the data of the item with the DataSource.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        private void UpdateItem(int index)
        {
            if (index >= 0 &&
                index < list.Items.Count)
            {
                ListViewItem item = GetListViewItem(index);
                list.Items[index] = item;
            }
        }

        /// <summary>
        /// Returns a <see cref="ListViewItem"/> which contains the row-data at given index.
        /// </summary>
        /// <param name="index">The index of the row.</param>
        /// <returns>A item which contains the data.</returns>
        private ListViewItem GetListViewItem(int index)
        {
            object row = DataManager.List[index];
            PropertyDescriptorCollection propColl = DataManager.GetItemProperties();
            ArrayList items = new ArrayList();
            PropertyDescriptor prop = null;
            // Fill value for each column
            foreach (ColumnHeader column in list.Columns)
            {                
                prop = propColl.Find(column.Text, false);
                if (prop != null)
                {
                    items.Add(prop.GetValue(row).ToString());
                }
            }
            return new ListViewItem((string[])items.ToArray(typeof(string)));
        }
        private string CollectListViewItem(int index)
        {

            object row = DataManager.List[index];
            PropertyDescriptorCollection propColl = DataManager.GetItemProperties();            

            // Fill value for each column
            foreach (ColumnHeader column in list.Columns)
            {
                PropertyDescriptor prop = null;
                prop = propColl.Find(column.Text, false);
                if (prop != null)
                {                   
                    return prop.GetValue(row).ToString();                    
                }
                else
                    return "error";
            }
            return "";            
        }
        /// <summary>
        /// Delete the item at the given index.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        private void DeleteItem(int index)
        {
            if (index >= 0 &&
                index < list.Items.Count)
                list.Items.RemoveAt(index);
        }

        /// <summary>
        /// Calculates the Colums of the <see cref="BoundListView"/>.
        /// </summary>
        private void CalculateColumns()
        {
            list.Columns.Clear();
            
            if (dataManager == null)
                return;
            ColumnHeader column;
            foreach (PropertyDescriptor prop in DataManager.GetItemProperties())
            {
                column = new ColumnHeader();
                column.Text = prop.Name;
                list.Columns.Add(column);
            }
        }


        #endregion

        #region Data Properties
        #region DataSource
        /// <summary>
        /// Gets or sets the data source that you want to display the data.
        /// </summary>
        [TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design")]
        [Category("Data")]
        [Description("Specifies the data source for the control.")]
        [DefaultValue(null)]
        public object DataSource
        {
            get
            {
                return this.dataSource;
            }
            set
            {
                if (this.dataSource != value)
                {
                    this.dataSource = value;
                    EnsureDataBinding();
                }
            }
        }

        #endregion

        #region DataMember
        /// <summary>
        /// Specifies a secondary list of Datasource, to display it
        /// </summary>
        [Category("Data")]
        [Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design",
             "System.Drawing.Design.UITypeEditor, System.Drawing")]
        [Description("Specifies a secondary list of Datasource, to display it")]
        [DefaultValue(null)]
        public string DataMember
        {
            get
            {
                return this.dataMember;
            }
            set
            {
                if (this.dataMember != value)
                {
                    this.dataMember = value;
                    EnsureDataBinding();
                }
            }
        }
        #endregion

        #region CurrencyManager
        /// <summary>
        /// Gets the CurrencyManager of the bound list.
        /// </summary>
        protected CurrencyManager DataManager
        {
            get
            {
                return this.dataManager;
            }
        }
        #endregion

        #endregion

        #region DisplayMember and GaugePopulation

        private BindingMemberInfo displayMember;
        [
        Category("Data"),
       Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design",
           "System.Drawing.Design.UITypeEditor, System.Drawing"),
       DefaultValue(@""),
       TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design"),
       Description(@"Indicates the property to display for the items in this control.")
       ]
        public string DisplayMember
        {
            get
            {
                return this.displayMember.BindingMember;
            }
            set
            {
                BindingMemberInfo displayMember0;

                displayMember0 = this.displayMember;
                try
                {
                    this.SetDataConnection(this.dataSource, new BindingMemberInfo(value), false);
                }
                catch (Exception)
                {
                    this.displayMember = displayMember0;
                }
            }
        }
        /// <summary>
        /// Sets the data connection
        /// </summary>
        /// <param name="newDataSource"></param>
        /// <param name="newDisplayMember"></param>
        /// <param name="force"></param>
        private void SetDataConnection(object newDataSource, BindingMemberInfo newDisplayMember, bool force)
        {
            CurrencyManager dataManager;

            bool isNewDataSource = !(this.dataSource == newDataSource);
            bool isNewDisplayMember = !this.displayMember.Equals(newDisplayMember);

            if (force || isNewDataSource || isNewDisplayMember)
            {

                if (this.dataSource as IComponent != null)
                    ((IComponent)this.dataSource).Disposed -= new EventHandler(this.DataSourceDisposed);
                this.dataSource = newDataSource;
                this.displayMember = newDisplayMember;

                dataManager = null;
                if (newDataSource != null && this.BindingContext != null && newDataSource != Convert.DBNull)
                    dataManager = (CurrencyManager)this.BindingContext[newDataSource, newDisplayMember.BindingPath];
            }
        }
        /// <summary>
        /// Disposes the data source
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataSourceDisposed(object sender, EventArgs e)
        {
            this.SetDataConnection(null, new BindingMemberInfo(string.Empty), true);
        }

        private int displayRecordIndex = 0;
        /// <summary>
        /// Gets or Sets the display record index
        /// </summary>
        [
        Category("Data"),
        Description("Gets or Sets the display record index.")
       ]
        public int DisplayRecordIndex
        {
            get
            {
                return displayRecordIndex;
            }
            set
            {
                displayRecordIndex = value;
                PopulateGauge();
            }
        }

        private void PopulateGauge()
        {
            int index = 0;

            foreach (ColumnHeader c in this.list.Columns)
            {
                if (c.Text == this.displayMember.BindingField)
                {
                    index = list.Columns.IndexOf(c);
                    break;
                }
            }

            if (DataManager != null && DisplayRecordIndex < DataManager.List.Count)
            {
                object row = DataManager.List[DisplayRecordIndex];
                PropertyDescriptorCollection propColl = DataManager.GetItemProperties();
                ColumnHeader column = list.Columns[index];
                PropertyDescriptor prop = propColl.Find(column.Text, false);
                if (prop != null)
                {
                    float val = 0f;
                    if (float.TryParse(prop.GetValue(row).ToString(), out val))
                    {
                        this.Value = val;
                    }
                    else
                    {
                        val = 0;
                        this.Value = val;
                    }
                }
            }
        }

        #endregion

        #endregion
    }

    #region[ Gauge Range ]
    
    /// <summary>
    /// Event argument for <see cref="ValueInRangeChanged"/> event.
    /// </summary>
    public class ThresholdValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Affected GaugeRange
        /// </summary>
        public Range Range { get; private set; }
        /// <summary>
        /// Gauge Value
        /// </summary>
        public Single Value { get; private set; }
        /// <summary>
        /// True if value is within current range.
        /// </summary>
        public bool InRange { get; private set; }
        public ThresholdValueChangedEventArgs(Range range, Single value, bool inRange)
        {
            this.Range = range;
            this.Value = value;
            this.InRange = inRange;
        }
        public ThresholdValueChangedEventArgs(Single value)
        {
            this.Value = value;
        }
    }
   
    #endregion

    /// <summary>
    /// Desginer class for RadialGauge
    /// </summary>
    public class RadialGaugeDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        /// <summary>
        /// Designer ActionList collection
        /// </summary>
        private System.ComponentModel.Design.DesignerActionListCollection actionLists;

        /// <summary>
        ///  Initializes a new instance of the CheckBoxAdvDesigner class
        /// </summary>
        public RadialGaugeDesigner()
            : base()
        {
        }


#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

        /// <summary>
        /// Gets a value indication the designer action
        /// </summary>
        public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == this.actionLists)
                {
                    this.actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
                    this.actionLists.Add(new RadialGaugeActionList(this.Component));
                }
                return this.actionLists;
            }
        }

#endif
        /// <summary>
        /// Overridden Initialize method.
        /// </summary>
        /// <param name="component">Componnent object</param>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
        }
    }


    /// <summary>
    /// Designer action list of RadialGauge
    /// </summary>
    public class RadialGaugeActionList : SyncActionListBase<RadialGauge>
    {
        /// <summary>
        /// Initializes a new instance of the RadialGauge class.
        /// </summary>
        /// <param name="component"> Represents component</param>
        public RadialGaugeActionList(IComponent component)
            : base(component)
        {
        }


        /// <summary>
        /// Gets/Sets the value for Maximum value
        /// </summary>
        public Single MaximumValue
        {
            get
            {
                Single maximumValue = 10;
                if (this.Control != null)
                {
                    RadialGauge control = this.Control as RadialGauge;
                    maximumValue = control.MaximumValue;
                }
                return maximumValue;
            }
            set
            {
                SetValue("MaximumValue", value);
            }
        }

        /// <summary>
        /// Gets/Sets the value for Maximum value
        /// </summary>
        public Single MinimumValue
        {
            get
            {
                Single minimumValue = 0;
                if (this.Control != null)
                {
                    RadialGauge control = this.Control as RadialGauge;
                    minimumValue = control.MinimumValue;
                }
                return minimumValue;
            }
            set
            {
                SetValue("MinimumValue", value);
            }
        }
        /// <summary>
        /// Gets/Sets the value for Frame style
        /// </summary>
        public FrameType FrameType
        {
            get
            {
                FrameType frameType = FrameType.FullCircle;
                if (this.Control != null)
                {
                    RadialGauge control = this.Control as RadialGauge;
                    frameType = control.FrameType;
                }
                return frameType;
            }

            set
            {
                SetValue("FrameType", value);
            }
        }
        /// <summary>
        /// Gets/Sets the value for Frame style
        /// </summary>
        public NeedleStyle NeedleStyle
        {
            get
            {
                NeedleStyle needleStyle = NeedleStyle.Default;
                if (this.Control != null)
                {
                    RadialGauge control = this.Control as RadialGauge;
                    needleStyle = control.NeedleStyle;
                }
                return needleStyle;
            }

            set
            {
                SetValue("NeedleStyle", value);
            }
        }
        /// <summary>
        /// Gets or Sets a value to gauge Major line Difference
        /// </summary>
        public Single MajorDifference
        {
            get
            {
                Single majorDifference = 10;
                if (this.Control != null)
                {
                    RadialGauge control = this.Control as RadialGauge;
                    majorDifference = control.MajorDifference;
                }
                return majorDifference;
            }
            set
            {
                SetValue("MajorDifference", value);
            }
        }

        /// <summary>
        /// Overrridden InitializeActionList.
        /// </summary>
        protected override void InitializeActionList()
        {
            // Customization Category
            this.AddDesignerActionHeaderItem("Customization");
            this.AddDesignerActionPropertyItem("MaximumValue", "MaximumValue", "Customization", "Gets/Sets the values for Maximum value.");
            this.AddDesignerActionPropertyItem("MinimumValue", "MinimumValue", "Customization", "Gets/Sets the values for Minimum value.");
            this.AddDesignerActionPropertyItem("MajorDifference", "MajorDifference", "Customization", "Gets/Sets the value for Major Difference.");
            this.AddDesignerActionPropertyItem("FrameType", "FrameType", "Customization", " Gets/Sets the value for Frame Type.");            
            this.AddDesignerActionPropertyItem("NeedleStyle", "NeedleStyle", "Customization", "Gets/Sets the value for Needle style.");
        }
    }
}
