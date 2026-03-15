#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Syncfusion.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Tools
{
    [ToolboxItem(true), ToolboxBitmap(typeof(RadialSlider), "ToolboxIcons.RadialSlider.png"), Docking(DockingBehavior.Ask)]
    [Designer(typeof(RadialSliderDesigner))]
    public partial class RadialSlider : Control
    {
        #region properties
        /// <summary>
        /// Gets/Sets Slider needle type
        /// </summary>
        private SliderNeedleType needleType;
        /// <summary>
        /// Gets/Sets Slider needle color
        /// </summary>
        private Color sliderNeedleColor;
        /// <summary>
        ///Gets/Sets Slider circle color
        /// </summary>
        private Color sliderCircleColor;
        /// <summary>
        ///Gets/Sets the Slider backcolor
        /// </summary>
        private Color sliderBackColor;
        /// <summary>
        ///Gets/Sets the Slider inner circle color
        /// </summary>
        private Color innerCircleColor;
        /// <summary>
        ///Gets/Sets the Slider background color
        /// </summary>
        private Color backgroundColor;
        /// <summary>
        ///Gets/Sets the Slider circle width
        /// </summary>
        private int outerCircleWidth;
        /// <summary>
        /// Gets/Sets the value for shoeoutercircle
        /// </summary>
        private bool showOuterCircle;
        /// <summary>
        /// Gets/Sets the value for innercircle gap
        /// </summary>
        private int innerCircleGap;
        /// <summary>
        /// Gets/Sets the sliderneedlewidth
        /// </summary>
        private int needleWidth;
        /// <summary>
        ///Gets/Sets the Slider lines color
        /// </summary>
        private Color linesColor;
        /// <summary>
        ///Gets/Sets the Slider lines width
        /// </summary>
        private int linesWidth;
        /// <summary>
        ///Gets/Sets thevalue for slider divisions
        /// </summary>
        private int sliderDivision;
        /// <summary>
        /// Gets/Sets the values for maximum
        /// </summary>
        private double maximumValue;
        /// <summary>
        /// Gets/Sets the values for maximum
        /// </summary>
        private double minimumValue;
        /// <summary>
        ///Gets/Sets the values for Range style
        /// </summary>
        private RangeStyles rangeStyle;
        /// <summary>
        ///Gets/Sets the values for Range style
        /// </summary>
        private bool showRangeBorder;
        /// <summary>
        /// Gets/Sets the value for Slider style
        /// </summary>
        private SliderStyles sliderStyle;
        /// <summary>
        ///Gets/Sets the Slider dummy angle
        /// </summary>
        private double dummyAngle;
        private string stringFormat;
        /// <summary>
        ///Gets/Sets the Slider angle difference
        /// </summary>
        double FindAngleDiffence;
        /// <summary>
        /// Slider frame image
        /// </summary>
        Bitmap image1;
        /// <summary>
        /// Slider angle
        /// </summary>
        private double angle;
        /// <summary>
        /// Slider draw region
        /// </summary>
        private Rectangle drawRegion;
        /// <summary>
        /// Slider orgin
        /// </summary>
        private Point origin;
        /// <summary>
        /// Slider angle point
        /// </summary>
        PointF anglePoint;
        /// <summary>
        /// Gets/Sets the Slider New Value
        /// </summary>
        Double currentValue;
        /// <summary>
        /// Gets/Sets the Slider Old Value
        /// </summary>
        Double oldValue;
        /// <summary>
        /// Inner circle width
        /// </summary>
        private int innerCircleWidth;
        /// <summary>
        /// Innercircle border thickness
        /// </summary>
        private int innerCircleBorderThickness;
        /// <summary>
        /// InnerCircleWidth
        /// </summary>
        private int InnerCircleWidth;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

        /// <summary>
        /// is scaling value
        /// </summary>
        private bool isScaling = false;
    #endregion
        #region Initialization
        public RadialSlider()
        {
            InitializeComponent();
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(RadialSlider));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer , true);
            setDrawRegion();
            image1 = new Bitmap(typeof(RadialSlider).Assembly.GetManifestResourceStream( "Syncfusion.Windows.Forms.Tools.Controls.RadialSlider.RadialFrame.png"));
            needleType = SliderNeedleType.StraightLine;
            sliderNeedleColor = ColorTranslator.FromHtml("#FF599737");
            sliderCircleColor = ColorTranslator.FromHtml("#FF599737");
            sliderBackColor = SystemColors.Control;
            innerCircleColor = ColorTranslator.FromHtml("#FF599737");
            outerCircleWidth = 2;
            showOuterCircle = false;
            innerCircleGap = 60;
            needleWidth = 2;
            linesColor = Color.Black;
            linesWidth = 2;
            sliderDivision = 10;
            maximumValue = 10;
            minimumValue = 0;
            rangeStyle = RangeStyles.Solid;
            sliderStyle = SliderStyles.Default;
            dummyAngle = 360;
            FindAngleDiffence = 0.0;
            currentValue = 0;
            oldValue = 0;
            innerCircleBorderThickness = 2;
            backgroundColor = SystemColors.Control;
            showRangeBorder = true;
            stringFormat = "N00";
            InnerCircleWidth = 50;
            this.ForeColor = ColorTranslator.FromHtml("#FF599737");
            this.MinimumSize = new Size(75,75);
            CTRLSIZE = new Size(150,150);
        }
        #endregion
        #region Gets/Sets properties
        /// <summary>
        /// Gets/Sets Control size before touch enabled
        /// </summary>
        [Browsable(false)]
        public Size BeforeTouchSize
        {
            get
            {
                return CTRLSIZE;
            }
            set
            {
                CTRLSIZE = value;
            }
        }
        bool _touchMode = false;
        /// <summary>
        /// Gets or sets value to enable or disable the Touchmode to the controls.
        /// </summary>
        /// <remarks>Scale factor will be updated automatically if scalefactor is equal to 1</remarks>
        [Browsable(true),DefaultValue(false),
        Category("Layout"), Description("Gets or sets value to enable or disable the Touchmode to the controls.")]
        public bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                    {
                        ApplyScaleToControl(1.5f);
                    }
                    else
                    {
                        ApplyScaleToControl(1.0f);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }
        /// <summary>
        /// 
        /// </summary>
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }

        /// <summary>
        /// Scale the control based on the scale factor passed in the argument.
        /// </summary>
        /// <param name="scaleFactor">value to scale the factor based upon.</param>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }

        /// <summary>
        /// Gets/Sets the valur for Slider needle Type
        /// </summary>
        [Category("Customization")]
        public SliderNeedleType NeedleType
        {
            get
            {
                return needleType;
            }
            set
            {
                needleType = value;
                this.Refresh();
            }
        }
        /// <summary>
        /// Gets/Sets the values for Slider needle color
        /// </summary>
        [Category("Customization")]
        public Color SliderNeedleColor
        {
            get
            {
                return sliderNeedleColor;
            }
            set
            {
                sliderNeedleColor = value;
                this.Refresh();
            }
        }
        /// <summary>
        /// Gets/Sets the values for Slider outer circle color
        /// </summary>
        [Category("Customization")]
        public Color OuterCircleColor
        {
            get
            {
                return sliderCircleColor;
            }
            set
            {
                sliderCircleColor = value;
                this.Refresh();
            }
        }
        /// <summary>
        /// Hide Controls BackColor 
        /// </summary>
        [Category("Customization")]
        [Browsable (false)]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
        }
        /// <summary>
        /// Gets/Sets the Control backcolor
        /// </summary>
        [Category("Appearance")]
        [Browsable(true)]
        public Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
                this.Refresh();
            }
        }
        /// <summary>
        /// Gets/Sets the values for Slider circle color
        /// </summary>
        [Category("Customization")]
        public Color InnerCircleColor
        {
            get
            {
                return innerCircleColor;
            }
            set
            {
                innerCircleColor = value;
                this.Refresh();
            }
        }
        /// <summary>
        /// Gets/Sets the values for Slider circle width
        /// </summary>
        [Category("Customization")]
        public bool ShowOuterCircle
        {
            get
            {
                return showOuterCircle;
            }
            set
            {
                showOuterCircle = value;
                this.Refresh();
            }
        }
        /// <summary>
        /// Slider Needle width
        /// </summary>
        [Category("Customization")]
        public int NeedleWidth
        {
            get
            {
                return needleWidth;
            }
            set
            {
                needleWidth = value;
                this.Refresh();
            }
        }
        /// <summary>
        /// Gets/Sets the values for slider lines color
        /// </summary>
        [Category("Customization")]
        public Color LinesColor
        {
            get
            {
                return linesColor;
            }
            set
            {
                linesColor = value;
                this.Refresh();
            }
        }
        #region AngleChanged EventHandlers


        public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs args);

        public class ValueChangedEventArgs : EventArgs
        {
            public ValueChangedEventArgs(Double CurrentVlaue , Double OldValue)
            {
                this.Value = CurrentVlaue;
                this.OldValue = OldValue;
            }

            private Double m_value;
            public Double Value
            {
                get
                {
                    return m_value;
                }
                private set
                {
                    m_value = value;
                }
            }

            private Double oldValue;
            public Double OldValue
            {
                get
                {
                    return oldValue;
                }
                private set
                {
                    oldValue = value;
                }
            }
        }

        /// <summary>
        /// Values for angle chaged
        /// </summary>
        public event ValueChangedEventHandler ValueChanged;

        protected void OnValueChanged(Double DoubleValue, Double SliderOldValue)
        {
            if (this.ValueChanged != null)
                ValueChanged(this, new ValueChangedEventArgs(DoubleValue , SliderOldValue));
        }
        #endregion
        /// <summary>
        /// Gets/Sets the value for Angle
        /// </summary>
        internal double Angle
        {
            get { return angle; }
            set
            {
                angle = value;
                this.Refresh();
            }
        }
        /// <summary>
        /// Gets/Sets value for Slider
        /// </summary>
        [Category("Behavior")]
        public double Value
        {
            get
            {
                return currentValue;
            }
            set
            {
                if (value <= MaximumValue)
                {
                    currentValue = value;
                    ReCalculateValue();
                }
            }
        }
        private void ReCalculateValue()
        {
            if (SliderStyle == SliderStyles.Default)
            {
                if (MinimumValue == 0)
                    this.Angle =this.DummyAngle= (360 - (((FindAngleDiffence * 100) / (MaximumValue)) * Value));
                else
                    this.Angle = this.DummyAngle = (360 - ((FindAngleDiffence * 100 / (MaximumValue - MinimumValue)) * (Value - MinimumValue)));
            }
            else
            {
                if (MinimumValue == 0)
                    this.Angle = this.DummyAngle = (360 - ((300 / (MaximumValue)) * Value));
                else
                    this.Angle = this.DummyAngle = (360 - ((300 / (MaximumValue - MinimumValue)) * (Value - MinimumValue)));
            }
            this.Refresh();
        }
        /// <summary>
        /// Gets/Sets the values for Slider Division
        /// </summary>
        [Category("Customization")]
        public int SliderDivision
        {
            get
            {
                return sliderDivision;
            }
            set
            {
                if (value > 1)
                {
                    sliderDivision = value;
                }
                this.Refresh();
            }
        }
        /// <summary>
        /// Gets/Sets the values for Maximum value
        /// </summary>
        [Category("Customization")]
        public double MaximumValue
        {
            get
            {
                return maximumValue;
            }
            set
            {
                if (value > MinimumValue)
                {
                    maximumValue = value;
                    ReCalculateValue();
                }
                this.Refresh();
            }
        }
        /// <summary>
        /// Gets/Sets the values for Minimum value
        /// </summary>
        [Category("Customization")]
        public double MinimumValue
        {
            get
            {
                return minimumValue;
            }
            set
            {
                if (value < MaximumValue)
                {
                    minimumValue = value;
                    ReCalculateValue();
                    if (Value < MinimumValue)
                        Value = MinimumValue;
                }
                this.Refresh();
            }
        }
        /// <summary>
        /// Avoid flickering
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
        /// <summary>
        /// Gets/Sets the values for Range style
        /// </summary>
        [Category("Customization")]
        public RangeStyles RangeStyle
        {
            get
            {
                return rangeStyle;
            }
            set
            {
                rangeStyle = value;
            }
        }
        /// <summary>
        /// Gets/Sets a value to show or hide the range border for framestyle
        /// </summary>
        [Category("Customization"),Browsable(false)]
        public bool ShowRangeBorder
        {
            get
            {
                return showRangeBorder;
            }
            set
            {
                showRangeBorder = value;
                this.Refresh();
            }
        }
        Color exColor = default(Color);
        /// <summary>
        /// Gets/Sets the values for Slider style
        /// </summary>
        [Category("Customization")]
        public SliderStyles SliderStyle
        {
            get
            {
                return sliderStyle;
            }
            set
            {
                sliderStyle = value;
                ReCalculateValue();
                this.Refresh();
            }
        }
        /// <summary>
        /// Gets/Sets the values for Font
        /// </summary>
        [Category("Customization")]
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                if (value.Size < 20)
                    base.Font = value;
            }
        }
        /// <summary>
        /// Gets/Sets the values for SliderSringFormat
        /// </summary>
        public String SliderStringFormat
        {
            get
            {
                return stringFormat;
            }
            set
            {
                stringFormat = value;
            }
        }
        /// <summary>
        /// Gets/Sets the values for DummyAngle
        /// </summary>
        internal double DummyAngle
        {
            get
            {
                return dummyAngle;
            }
            set
            {
                if (value != -1)
                    dummyAngle = value;
            }
        }
        #endregion
        #region Slider Calculation
        /// <summary>
        /// Return points deponds on degrees and radius
        /// </summary>
        /// <param name="degrees"> angle</param>
        /// <param name="radius"> length</param>
        /// <param name="origin"> Center point</param>
        /// <returns></returns>
        private PointF DegreesToXY(double degrees, float radius, Point origin)
        {
            PointF xy = new PointF();
            double radians = degrees * Math.PI / 180.0;

            xy.X = (float)Math.Cos(radians) * radius + origin.X;
            xy.Y = (float)Math.Sin(-radians) * radius + origin.Y;

            return xy;
        }
        /// <summary>
        /// Region to Radial Slider
        /// </summary>
        private void setDrawRegion()
        {
            drawRegion = new Rectangle(0, 0, this.Width, this.Height);
            drawRegion.X += 2;
            drawRegion.Y += 2;
            drawRegion.Width -= 4;
            drawRegion.Height -= 4;
            int offset = 2;
            origin = new Point(drawRegion.Width / 2 + offset, drawRegion.Height / 2 + offset);
            this.Refresh();
        }
        /// <summary>
        /// Size to Radial Slider
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            this.Height = this.Width; //Keep it a square
            setDrawRegion();
            base.OnResize(e);
        }
        /// <summary>
        /// Return points deponds on degrees and radius
        /// </summary>
        /// <param name="degrees"> angle</param>
        /// <param name="radius"> length</param>
        /// <param name="origin"> Center point</param>
        /// <returns></returns>
        private PointF DegreesToXY1(double degrees, float radius, Point origin)
        {
            PointF xy = new PointF();
            double radians = degrees * Math.PI / 180.0;

            xy.X = (float)Math.Cos(radians) * radius + origin.X;
            xy.Y = (float)Math.Sin(-radians) * radius + origin.Y;

            return xy;
        }
        /// <summary>
        /// Return Angle
        /// </summary>
        /// <param name="xy"> Current point</param>
        /// <param name="origin">Orgin point</param>
        /// <returns></returns>
        private float XYToDegrees(Point xy, Point origin)
        {
            double angle = 0.0;

            if (xy.Y < origin.Y)
            {
                if (xy.X > origin.X)
                {
                    angle = (double)(xy.X - origin.X) / (double)(origin.Y - xy.Y);
                    angle = Math.Atan(angle);
                    angle = 90.0 - angle * 180.0 / Math.PI;
                }
                else if (xy.X < origin.X)
                {
                    angle = (double)(origin.X - xy.X) / (double)(origin.Y - xy.Y);
                    angle = Math.Atan(-angle);
                    angle = 90.0 - angle * 180.0 / Math.PI;
                }
            }
            else if (xy.Y > origin.Y)
            {
                if (xy.X > origin.X)
                {
                    angle = (double)(xy.X - origin.X) / (double)(xy.Y - origin.Y);
                    angle = Math.Atan(-angle);
                    angle = 270.0 - angle * 180.0 / Math.PI;
                }
                else if (xy.X < origin.X)
                {
                    angle = (double)(origin.X - xy.X) / (double)(xy.Y - origin.Y);
                    angle = Math.Atan(angle);
                    angle = 270.0 - angle * 180.0 / Math.PI;
                }
            }
            return (float)angle;
        }
        #endregion
        #region Slider Painting
        /// <summary>
        /// drawing the control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            anglePoint = DegreesToXY(angle, origin.X - this.Width / 5 , origin);
            Rectangle originSquare = new Rectangle(origin.X - 1, origin.Y - 1, 3, 3);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            if (SliderStyle == SliderStyles.Default)
            {
                DrawDefaultSlider(e);
            }
            else if (SliderStyle == SliderStyles.Frame)
            {
                DrawFrameSlider(e);
            }
            base.OnPaint(e);
        }
        /// <summary>
        /// Drawing Frame Slider
        /// </summary>
        /// <param name="e">e used for drawing the slider</param>
        private void DrawFrameSlider(PaintEventArgs e)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(new Rectangle(0, 0, this.Width, this.Height));
                this.Region = new Region(path);

                Color color = this.Parent != null ? Parent.BackColor : Color.White;
                using (Pen pen = new Pen(color , 2))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }
            e.Graphics.DrawImage(image1, new Rectangle(0, 0, this.Width, this.Height));
            using (GraphicsPath pat = new GraphicsPath())
            {
                pat.AddEllipse(new Rectangle(0, 0, this.Width, this.Height));
                Color color = this.Parent != null ? this.Parent.BackColor : Color.White;
                e.Graphics.DrawPath(new Pen(color, 3), pat);
            }
            DrawRange(e);
            using (Brush stringbrush = new SolidBrush(this.ForeColor))
            {
                SizeF sz1 = e.Graphics.MeasureString(Convert.ToInt32((((360 - Angle) * (MaximumValue / 100)) / FindAngleDiffence)).ToString(SliderStringFormat), new Font("Times New Roman", 40));
                if (MaximumValue <= 20)
                {
                    if (Angle == 0)
                    {
                        sz1 = e.Graphics.MeasureString(MinimumValue.ToString(), new Font("Times New Roman", this.Width / 9));
                        e.Graphics.DrawString(MinimumValue.ToString(), new Font("Times New Roman", this.Width / 9), stringbrush, new PointF(this.Width / 2 - sz1.Width / 2, this.Width / 2 - sz1.Height / 2));
                    }
                    else
                    {
                        sz1 = e.Graphics.MeasureString(Convert.ToDouble(MinimumValue + (((360 - Angle) * ((MaximumValue - MinimumValue) / 100)) / FindAngleDiffence)).ToString(SliderStringFormat), new Font("Times New Roman", this.Width / 9));
                        e.Graphics.DrawString(Convert.ToDouble(MinimumValue + (((360 - Angle) * ((MaximumValue - MinimumValue) / 100)) / FindAngleDiffence)).ToString(SliderStringFormat), new Font("Times New Roman", this.Width / 9), stringbrush, new PointF(this.Width / 2 - sz1.Width / 2, this.Width / 2 - sz1.Height / 2));
                    }
                }
                else
                {
                    if (MaximumValue > 999)
                    {
                        if (Angle == 0)
                        {
                            sz1 = e.Graphics.MeasureString(MinimumValue.ToString(SliderStringFormat), new Font("Times New Roman", this.Width / 10));
                            e.Graphics.DrawString(MinimumValue.ToString(SliderStringFormat), new Font("Times New Roman", this.Width / 12), stringbrush, new PointF(this.Width / 2 - sz1.Width / 2, this.Width / 2 - sz1.Height / 2));
                        }
                        else
                        {
                            sz1 = e.Graphics.MeasureString(Convert.ToDouble(MinimumValue + (((360 - Angle) * ((MaximumValue - MinimumValue) / 100)) / FindAngleDiffence)).ToString(SliderStringFormat), new Font("Times New Roman", this.Width / (Convert.ToDouble((((360 - Angle) * (MaximumValue / 100)) / FindAngleDiffence)).ToString("0").Length * 3)));
                            e.Graphics.DrawString(Convert.ToDouble(MinimumValue + (((360 - Angle) * ((MaximumValue - MinimumValue) / 100)) / FindAngleDiffence)).ToString(SliderStringFormat), new Font("Times New Roman", this.Width / (Convert.ToDouble((((360 - Angle) * (MaximumValue / 100)) / FindAngleDiffence)).ToString("0").Length * 3)), stringbrush, new PointF(this.Width / 2 - sz1.Width / 2, this.Width / 2 - sz1.Height / 2));
                        }
                    }
                    else
                    {
                        if (Angle == 0)
                        {
                            sz1 = e.Graphics.MeasureString(MinimumValue.ToString(SliderStringFormat), new Font("Times New Roman", this.Width / 9));
                            e.Graphics.DrawString(MinimumValue.ToString(SliderStringFormat), new Font("Times New Roman", this.Width / 9), stringbrush, new PointF(this.Width / 2 - sz1.Width / 2, this.Width / 2 - sz1.Height / 2));
                        }
                        else
                        {
                            sz1 = e.Graphics.MeasureString(Convert.ToDouble(MinimumValue + (((360 - Angle) * ((MaximumValue - MinimumValue) / 100)) / FindAngleDiffence)).ToString(SliderStringFormat), new Font("Times New Roman", this.Width / 9));
                            e.Graphics.DrawString(Convert.ToDouble(MinimumValue + (((360 - Angle) * ((MaximumValue - MinimumValue) / 100)) / FindAngleDiffence)).ToString(SliderStringFormat), new Font("Times New Roman", this.Width / 9), stringbrush, new PointF(this.Width / 2 - sz1.Width / 2, this.Width / 2 - sz1.Height / 2));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Sets the values to InnerCircleWidth
        /// </summary>
        /// <param name="InnerCircleWidth"></param>
        public void  SetInnerCircleWidth(int CircleWidth)
        {
            InnerCircleWidth = CircleWidth;
        }
        /// <summary>
        /// Drawing  Default Slider
        /// </summary>
        /// <param name="e">Ee.Graphics used for drwing the slider</param>
        private void DrawDefaultSlider(PaintEventArgs e)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(new Rectangle(0, 0, this.Width, this.Height));
                this.Region = new Region(path);

                Color color = this.Parent != null ? Parent.BackColor : Color.White;

                using (Brush brush = new SolidBrush(this.BackgroundColor))
                {
                    e.Graphics.FillPath(brush, path);
                }
                if (ShowOuterCircle)
                {
                    using (Pen SliderCirclePen = new Pen(OuterCircleColor, outerCircleWidth))
                    {
                        e.Graphics.DrawEllipse(SliderCirclePen, new Rectangle(outerCircleWidth / 2, outerCircleWidth / 2, this.Width - outerCircleWidth, this.Height - outerCircleWidth));
                    }
                }

                using (Pen pen = new Pen(color, 1))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }
            using (Pen SliderCirclePen = new Pen(OuterCircleColor, outerCircleWidth))
            {
                e.Graphics.DrawEllipse(SliderCirclePen, new Rectangle(this.Width / 4 - this.Width / 20, this.Width / 4 - this.Width / 20, this.Width / 2 + this.Width / 10, this.Width / 2 + this.Width / 10));
            }
            DrawLines(e);
            DrawCurrentPointLine(e);
            float[] dashValues = { 1, 2, 1, 2 };

            using (Pen pen = new Pen(Color.FromArgb(80, SliderNeedleColor), NeedleWidth))
            {
                if (DummyAngle != Angle)
                    e.Graphics.DrawLine(pen, origin, DegreesToXY(DummyAngle, origin.X - this.Width / 5, origin));
            }
            using (Pen SliderPen = new Pen(SliderNeedleColor, NeedleWidth))
            {
                if (NeedleType == SliderNeedleType.DottedLine)
                    SliderPen.DashPattern = dashValues;
                e.Graphics.DrawLine(SliderPen, origin, anglePoint);

            }

            using (Brush brush = new SolidBrush(Color.White))
            {
                e.Graphics.FillEllipse(brush, new Rectangle(this.Width / 2 - InnerCircleWidth/2, this.Width / 2 - InnerCircleWidth / 2, InnerCircleWidth, InnerCircleWidth));
                using (Brush stringbrush = new SolidBrush(this.ForeColor))
                {
                    if (Angle == 0)
                        Angle = 360;
                    SizeF sz1 = e.Graphics.MeasureString(Convert.ToInt32((((360 - Angle) * (MaximumValue / 100)) / FindAngleDiffence)).ToString(), this.Font);
                    if (MaximumValue >= 100)
                    {
                        sz1 = e.Graphics.MeasureString(Convert.ToDouble(MinimumValue + (((360 - Angle) * ((MaximumValue - MinimumValue) / 100)) / FindAngleDiffence)).ToString(SliderStringFormat), this.Font);
                        e.Graphics.DrawString(Convert.ToDouble(MinimumValue + (((360 - Angle) * ((MaximumValue - MinimumValue) / 100)) / FindAngleDiffence)).ToString(SliderStringFormat), this.Font, stringbrush, new PointF(this.Width / 2 - sz1.Width / 2, this.Width / 2 - sz1.Height / 2));
                    }
                    else
                    {
                        sz1 = e.Graphics.MeasureString(Convert.ToDouble(((360 - Angle) / FindAngleDiffence) / (100 / MaximumValue)).ToString("0.#"), this.Font);
                        if (MaximumValue < 2)
                        {
                            sz1 = e.Graphics.MeasureString(Convert.ToDouble(MinimumValue + ((360 - Angle) / FindAngleDiffence) / (100 / (MaximumValue - MinimumValue))).ToString(SliderStringFormat), this.Font);
                            e.Graphics.DrawString(Convert.ToDouble(MinimumValue + ((360 - Angle) / FindAngleDiffence) / (100 / (MaximumValue - MinimumValue))).ToString(SliderStringFormat), this.Font, stringbrush, new PointF(this.Width / 2 - sz1.Width / 2, this.Width / 2 - sz1.Height / 2));
                        }
                        else if (maximumValue < 13)
                        {
                            sz1 = e.Graphics.MeasureString(Convert.ToDouble(MinimumValue + ((360 - Angle) / FindAngleDiffence) / (100 / (MaximumValue - MinimumValue))).ToString(SliderStringFormat), this.Font);
                            e.Graphics.DrawString(Convert.ToDouble(MinimumValue + ((360 - Angle) / FindAngleDiffence) / (100 / (MaximumValue - MinimumValue))).ToString(SliderStringFormat), this.Font, stringbrush, new PointF(this.Width / 2 - sz1.Width / 2, this.Width / 2 - sz1.Height / 2));
                        }
                        else if (MaximumValue < 101)
                        {
                            sz1 = e.Graphics.MeasureString(Convert.ToDouble(MinimumValue + ((360 - Angle) / FindAngleDiffence) / (100 / (MaximumValue - MinimumValue))).ToString(SliderStringFormat), this.Font);
                            e.Graphics.DrawString(Convert.ToDouble(MinimumValue + ((360 - Angle) / FindAngleDiffence) / (100 / (MaximumValue - MinimumValue))).ToString(SliderStringFormat), this.Font, stringbrush, new PointF(this.Width / 2 - sz1.Width / 2, this.Width / 2 - sz1.Height / 2));
                        }
                    }
                }
            }
            anglePoint = DegreesToXY(angle, origin.X - this.Width / 10, origin);
            if (MaximumValue >= 100)
            {
                SizeF sz = e.Graphics.MeasureString(Convert.ToDouble(MinimumValue + ((360 - Angle) * ((MaximumValue - MinimumValue) / 100) / FindAngleDiffence)).ToString(SliderStringFormat), this.Font);
                anglePoint.X -= sz.Width / 2;
                anglePoint.Y -= sz.Height / 2;
                using (Brush brush = new SolidBrush(this.ForeColor))
                {
                    if ((360 - Angle) % (320 / SliderDivision) != 0)
                    {
                        e.Graphics.DrawString(Convert.ToDouble(MinimumValue + ((360 - Angle) * ((MaximumValue - MinimumValue) / 100) / FindAngleDiffence)).ToString(SliderStringFormat), this.Font, brush, anglePoint);
                    }
                }
            }
            else
            {
                SizeF sz = e.Graphics.MeasureString(Convert.ToDouble(((360 - Angle) / FindAngleDiffence) / (100 / MaximumValue)).ToString(SliderStringFormat), this.Font);
                anglePoint.X -= sz.Width / 2;
                anglePoint.Y -= sz.Height / 2;
                using (Brush brush = new SolidBrush(this.ForeColor))
                {
                    if ((360 - Angle) % (320 / SliderDivision) != 0)
                    {
                        if (MaximumValue < 2)
                        {
                            sz = e.Graphics.MeasureString(Convert.ToDouble(MinimumValue + ((360 - Angle) / FindAngleDiffence) / (100 / (MaximumValue - MinimumValue))).ToString(SliderStringFormat), this.Font);
                            e.Graphics.DrawString(Convert.ToDouble(MinimumValue + ((360 - Angle) / FindAngleDiffence) / (100 / (MaximumValue - MinimumValue))).ToString(SliderStringFormat), this.Font, brush, anglePoint);
                        }
                        else if (MaximumValue < 13)
                        {
                            sz = e.Graphics.MeasureString(Convert.ToDouble(MinimumValue + ((360 - Angle) / FindAngleDiffence) / (100 / (MaximumValue - MinimumValue))).ToString(SliderStringFormat), this.Font);
                            e.Graphics.DrawString(Convert.ToDouble(MinimumValue + ((360 - Angle) / FindAngleDiffence) / (100 / (MaximumValue - MinimumValue))).ToString(SliderStringFormat), this.Font, brush, anglePoint);
                        }
                        else if (MaximumValue < 101)
                        {
                            e.Graphics.DrawString(Convert.ToDouble(MinimumValue + ((360 - Angle) / FindAngleDiffence) / (100 / (MaximumValue - MinimumValue))).ToString(SliderStringFormat), this.Font, brush, anglePoint);
                        }
                    }
                }
            }
            using (Pen InnerCirclePen = new Pen(InnerCircleColor, innerCircleBorderThickness))
            {
                e.Graphics.DrawEllipse(InnerCirclePen, new Rectangle(this.Width / 2 - InnerCircleWidth / 2,this.Width / 2- InnerCircleWidth / 2 , InnerCircleWidth, InnerCircleWidth));
            }
        }
        /// <summary>
        /// Drawing current point
        /// </summary>
        /// <param name="e">e.Graphics used for drawing current line</param>
        private void DrawCurrentPointLine(PaintEventArgs e)
        {
            PointF pointf = DegreesToXY(Angle, origin.X - this.Width / 5 , origin);
            PointF pointf1 = DegreesToXY(Angle, origin.X - this.Width / 5 + 5 , origin);
            using (Pen LinesPen = new Pen(LinesColor))
            {
                e.Graphics.DrawLine(LinesPen, pointf1, pointf);
            }
        }
        /// <summary>
        ///Drawing division lines
        /// </summary>
        /// <param name="e">e.Graphics used for drawing range line</param>
        private void DrawLines(PaintEventArgs e)
        {
            int Radius = this.Width / 2 - 2;
            PointF pointf;
            PointF pointf1;
            int count = 0;
            
            double cacl = MinimumValue;
            for (double i = 0; i < 321; i =i+((double)320/SliderDivision))
            {
             
                pointf = DegreesToXY(360 - i, origin.X -this.Width/5, origin);
                pointf1 = DegreesToXY(360 - i, origin.X - this.Width / 5  + 5, origin);
                if (count <= SliderDivision)
                {
                    using (Pen LinesPen = new Pen(LinesColor))
                    {
                        e.Graphics.DrawLine(LinesPen, pointf1, pointf);
                    }
                    PointF anglePoint = DegreesToXY(360 - i, origin.X - this.Width / 7, origin);
                    {
                        SizeF sz = e.Graphics.MeasureString(((count) * (MaximumValue / SliderDivision)).ToString("0.#"), this.Font);
                        using (Brush brush = new SolidBrush(this.ForeColor))
                        {
                            if (count == 0)
                            {
                                sz = e.Graphics.MeasureString((cacl).ToString("0.#"), this.Font);
                                anglePoint.X -= sz.Width / 2;
                                anglePoint.Y -= sz.Height / 2;
                                e.Graphics.DrawString((cacl).ToString("0.#"), this.Font, brush, anglePoint);
                            }
                            else
                            {
                                sz = e.Graphics.MeasureString(((cacl) + ((MaximumValue - MinimumValue) / SliderDivision)).ToString("0.#"), this.Font);
                                anglePoint.X -= sz.Width / 2;
                                anglePoint.Y -= sz.Height / 2;
                                e.Graphics.DrawString(((cacl) + ((MaximumValue - MinimumValue) / SliderDivision)).ToString("0.#"), this.Font, brush, anglePoint);
                                cacl = (cacl) + ((MaximumValue - MinimumValue) / SliderDivision);
                            }
                        }
                       
                        if (count == SliderDivision)
                        {
                            FindAngleDiffence = (360 - (360 - i)) / 100;
                        }
                        count++;
                    }
                }
            }
        }
        /// <summary>
        ///Drawing Slider range
        /// </summary>
        /// <param name="e">e.Graphics used for drawing range line</param>
        private void DrawRange(PaintEventArgs e)
        {
            int Radius = this.Width / 2 - 2;
            PointF pointf;
            PointF pointf1;
            int penWidth;
            if (RangeStyle == RangeStyles.Solid)
                penWidth = (int)(this.Width/100 * (e.Graphics.DpiX / 96) + 1);
            else
                penWidth = (int)(2 * (e.Graphics.DpiX / 96));
            //Drawing completerange for Frame
            if (ShowRangeBorder)
            {
                for (double i = 0; i <= 300; i = i + 1)
                {
                    if (i == 0 || i == 300)
                    {
                        pointf = DegreesToXY1(360 - i, origin.X - this.Width / 4, origin);
                        pointf1 = DegreesToXY1(360 - i, origin.X - this.Width / 4 + this.Width / 6 + 1, origin);
                        e.Graphics.DrawLine(new Pen(this.ForeColor, (1 * e.Graphics.DpiX / 96)), pointf, pointf1);
                    }
                    else
                    {
                        pointf = DegreesToXY1(360 - i, origin.X - this.Width / 4, origin);
                        pointf1 = DegreesToXY1(360 - i, origin.X - this.Width / 4 + 1, origin);
                        e.Graphics.DrawLine(new Pen(this.ForeColor, penWidth), pointf, pointf1);
                        pointf = DegreesToXY1(360 - i, origin.X - this.Width / 4 + this.Width / 6, origin);
                        pointf1 = DegreesToXY1(360 - i, origin.X - this.Width / 4 + this.Width / 6 + 1, origin);
                        e.Graphics.DrawLine(new Pen(this.ForeColor, penWidth), pointf, pointf1);
                    }
                }
            }
            //Drawing highlighted range for Frame
            if (Angle != 0)
            {
                for (double i = 0; i < 360 - Angle; i = i + 1)
                {
                    pointf = DegreesToXY1(360 - i, origin.X - this.Width / 4, origin);
                    pointf1 = DegreesToXY1(360 - i, origin.X - this.Width / 4 + this.Width/6, origin);
                    e.Graphics.DrawLine(new Pen(this.ForeColor, penWidth), pointf, pointf1);
                }
            }
            FindAngleDiffence = (360 - (360 - 320)) / 100;
        }
        /// <summary>
        /// return points from angle
        /// </summary>
        /// <param name="theAngle">theAngle used for calculating the points </param>
        /// <returns></returns>
        private PointF FindXAndY(int theAngle)
        {
            float x2 = this.Width / 2 - 75 * (float)Math.Sin(Math.PI / 180 * theAngle);  //Converting the angle to radians and finding the X position
            float y2 = this.Width / 2 - 75 * (float)Math.Cos(Math.PI / 180 * theAngle);  //Converting the angle to radians and finding the Y position
            PointF aPoint = new PointF(x2, y2);
            return aPoint;
        }
        /// <summary>
        /// Gets the current point
        /// </summary>
        /// <param name="e"> e.X and e.Y used for find the angle</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            bool outSideToCirCle = !CheckInSide(new Point(e.X, e.Y));
            bool maximum = CheckMaximumSide(new Point(e.X, e.Y));
            if (outSideToCirCle && maximum)
            {
                int thisAngle = findNearestAngle(new Point(e.X, e.Y));
                if (thisAngle != -1)
                {
                    if (thisAngle > 0 && thisAngle < 360 - (FindAngleDiffence * 100))
                        thisAngle = 0;
                    this.Angle = thisAngle;
                    this.Refresh();
                }
            }
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (!EnableTouchMode && this.DesignMode)
                CTRLSIZE = this.Size;
        }
        /// <summary>
        /// Checking the points inside the circle or not
        /// </summary>
        /// <param name="location"> location used for checking the points are inside the circle or not</param>
        /// <returns></returns>
        private bool CheckInSide(Point location)
        {
            Point normalized = new Point(location.X - origin.X,
                                         location.Y - origin.Y);
            if(SliderStyle == SliderStyles.Default)
            return (normalized.X * normalized.X +
                   normalized.Y * normalized.Y)
                   <= (26 * 26);
            else
                return (normalized.X * normalized.X +
                 normalized.Y * normalized.Y)
                 <= (this.Width/4* this.Width/4);
        }
        /// <summary>
        /// Checking the points outside the circle or not
        /// </summary>
        /// <param name="location"> location used for checking the points are outside the circle or not</param>
        /// <returns></returns>
        private bool CheckMaximumSide(Point location)
        {
            Point normalized = new Point(location.X - origin.X,
                                         location.Y - origin.Y);
            if (SliderStyle == SliderStyles.Default)
                return (normalized.X * normalized.X +
                       normalized.Y * normalized.Y)
                       <= (this.Width / 2 * this.Width / 2);
            else
                return (normalized.X * normalized.X +
                  normalized.Y * normalized.Y)
                  <= ((this.Width / 4 + this.Width / 6) * (this.Width / 6 + this.Width / 4));
        }
        /// <summary>
        /// Sets the value for dummy angle
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.DummyAngle = this.Angle;
            this.Refresh();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Value = (MinimumValue + ((360 - Angle) * ((MaximumValue - MinimumValue) / 100)) / FindAngleDiffence);
            if (!this.DesignMode && ValueChanged != null)
                OnValueChanged(Value, oldValue);
            oldValue = currentValue;
            this.Refresh();
        }
        /// <summary>
        /// Gets the value for dummy angle
        /// </summary>
        /// <param name="e">e.X and e.Y used for calculating the dummy angle</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            bool outSideToCirCle = !CheckInSide(new Point(e.X, e.Y));
            bool maximum = CheckMaximumSide(new Point(e.X, e.Y));
            if (outSideToCirCle && maximum)
            {
                DummyAngle = findNearestAngle(new Point(e.X, e.Y));
                //if (DummyAngle > 0 && DummyAngle < 40)
                //    DummyAngle = 0;
                this.Refresh();
                if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
                {
                    int thisAngle = findNearestAngle(new Point(e.X, e.Y));

                    if (thisAngle != -1)
                    {
                        if (thisAngle > 0 && thisAngle < 360 - (FindAngleDiffence * 100))
                            thisAngle = 0;
                        this.Angle = thisAngle;
                        this.Refresh();
                    }
                }
            }
            else
            {
                this.DummyAngle = this.Angle;
                this.Refresh();
            }
        }
        /// <summary>
        /// Finding the nearest angle
        /// </summary>
        /// <param name="mouseXY">mouseXY used for finding nearest angle </param>
        /// <returns></returns>
        private int findNearestAngle(Point mouseXY)
        {
            int thisAngle = (int)XYToDegrees(mouseXY, origin);
            if (thisAngle != 0)
                return thisAngle;
            else
                return -1;
        }
        #endregion
    }
        #region Slider Needle Types
    /// <summary>
    /// Slider needle types
    /// </summary>
    public enum SliderNeedleType
    {
        StraightLine,

        DottedLine
    }
    #endregion
        #region Slider Styles
    /// <summary>
    /// Slider styles
    /// </summary>
    public enum SliderStyles
    {
        Default,

        Frame
    }
    #endregion
        #region Slider Range styles
    /// <summary>
    /// Slider range styles
    /// </summary>
    public enum RangeStyles
    {
        Solid,

        Line
    }
#endregion

    /// <summary>
    /// Desginer class for Carousel
    /// </summary>
    public class RadialSliderDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        /// <summary>
        /// Designer ActionList collection
        /// </summary>
        private System.ComponentModel.Design.DesignerActionListCollection actionLists;

        /// <summary>
        ///  Initializes a new instance of the CheckBoxAdvDesigner class
        /// </summary>
        public RadialSliderDesigner()
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
                    this.actionLists.Add(new RadialSliderActionList(this.Component));
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
    /// Designer action list of carousel
    /// </summary>
    public class RadialSliderActionList : SyncActionListBase<RadialSlider>
    {
        /// <summary>
        /// Initializes a new instance of the CheckBoxAdvActionList class.
        /// </summary>
        /// <param name="component"> Represents component</param>
        public RadialSliderActionList(IComponent component)
            : base(component)
        {
        }


        /// <summary>
        /// Gets/Sets the value for Maximum value
        /// </summary>
        public double MaximumValue
        {
            get
            {
                double maximumValue = 10;
                if (this.Control != null)
                {
                    RadialSlider control = this.Control as RadialSlider;
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
        public double MinimumValue
        {
            get
            {
                double minimumValue = 0;
                if (this.Control != null)
                {
                    RadialSlider control = this.Control as RadialSlider;
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
        /// Gets/Sets the value for Slider style
        /// </summary>
        public SliderStyles SliderStyle
        {
            get
            {
                SliderStyles sliderStyle = SliderStyles.Default;
                if (this.Control != null)
                {
                    RadialSlider control = this.Control as RadialSlider;
                    sliderStyle = control.SliderStyle;
                }

                return sliderStyle;
            }

            set
            {
                SetValue("SliderStyle", value);
            }
        }
        /// <summary>
        /// Gets/Sets the value for Slider Division
        /// </summary>
        public int SliderDivision
        {
            get
            {
                int sliderDivision = 10;
                if (this.Control != null)
                {
                    RadialSlider control = this.Control as RadialSlider;
                    sliderDivision = control.SliderDivision ;
                }

                return sliderDivision;
            }

            set
            {
                SetValue("SliderDivision", value);
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
            this.AddDesignerActionPropertyItem("SliderStyle", "SliderStyle", "Customization", " Gets/Sets the value for Slider Style.");
          
            if (this.SliderStyle == SliderStyles.Default)
            {
                this.AddDesignerActionPropertyItem("SliderDivision", "SliderDivision", "Customization", "Gets/Sets the value for Slider Division.");
            }
        }
    }
}
