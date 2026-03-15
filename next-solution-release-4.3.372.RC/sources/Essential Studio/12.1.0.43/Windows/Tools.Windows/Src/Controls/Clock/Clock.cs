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

namespace Syncfusion.Windows.Forms.Tools
{
    # region enum

    public enum Align
    {
        /// <summary>
        /// Aligns at left
        /// </summary>
        Left,

        /// <summary>
        ///  Aligns at top
        /// </summary>
        Top,

        /// <summary>
        /// Aligns at right
        /// </summary>
        Right,

        /// <summary>
        ///  Aligns at bottom
        /// </summary>
        Bottom
    }


    public enum ClockVisualStyle
    {
        /// <summary>
        /// Office Blue color
        /// </summary>
        OfficeBlue,

        /// <summary>
        /// Office silver color
        /// </summary>
        OfficeSilver,

        /// <summary>
        /// Office black color
        /// </summary>
        OfficeBlack,

        /// <summary>
        ///  None
        /// </summary>
        None
    }

    # endregion

    # region Clock
    [Designer(typeof(ClockDesigner), typeof(System.ComponentModel.Design.IDesigner))]
    [
        ToolboxItem(true),
        ToolboxBitmap(typeof(Clock), "ToolboxIcons.Clock.bmp"),
        Description("Clock.")
    ]

    public partial class Clock : Control
    {

        # region Members
        /// <summary>
        ///  Renderer of the clock
        /// </summary>
        internal ClockRenderer render = new ClockRenderer();

        /// <summary>
        ///  Renderer of the clock
        /// </summary>
        internal DigitalClockRenderer DigitalRender;
        /// <summary>
        /// Timer
        /// </summary>
        public Timer Ticks = new Timer();

        /// <summary>
        ///  Indicates current time
        /// </summary>
        private DateTime now = new DateTime();

        /// <summary>
        /// Indicates hour hand color
        /// </summary>
        private Color hourHandColor = ColorTranslator.FromHtml("#5F6F77");

        /// <summary>
        /// Indicates minute hand color
        /// </summary>
        private Color minuteHandColor = ColorTranslator.FromHtml("#5F6F77");

        /// <summary>
        /// Indicates second hand color
        /// </summary>
        private Color secondHandColor = ColorTranslator.FromHtml("#5F6F77");

        /// <summary>
        /// Indicates start gradient back color
        /// </summary>
        private Color startGradientBackColor = ColorTranslator.FromHtml("#FFFFFF");

        /// <summary>
        /// Indicates end gradient back color
        /// </summary>
        private Color endGradientBackColor = ColorTranslator.FromHtml("#CCD6D9");

        /// <summary>
        /// Indicates minutes bacl color
        /// </summary>
        private Color minuteColor = ColorTranslator.FromHtml("#597A84");

        /// <summary>
        /// Indicates alignments
        /// </summary>
        private Align align = Align.Right;

        /// <summary>
        ///  Indicates remainder
        /// </summary>
        private DateTime remainder = DateTime.Now;

        /// <summary>
        /// Indicates remainder enabled or disabled
        /// </summary>
        private bool enableRemainder = false;

        /// <summary>
        /// Show or Hide AMorPM
        /// </summary>
        private bool showHourDesignator = false;

        /// <summary>
        /// Show or Hide border
        /// </summary>
        private bool showborder = true;

        /// <summary>
        /// Indicates color of the border
        /// </summary>
        private Color borderColor = ColorTranslator.FromHtml("#848489");

        /// <summary>
        /// Indicates image
        /// </summary>
        private Image image;

        /// <summary>
        /// Indicates clock visual style
        /// </summary>
        private ClockVisualStyle visualStyle = ClockVisualStyle.None;

        /// <summary>
        /// Indicates thickness of the hour hand
        /// </summary>
        private float hourHandThickness = 2;

        /// <summary>
        /// Indicates thickness of the minute hand
        /// </summary>
        private float minuteHandThickness = 2;

        /// <summary>
        /// Indicates thickness of the minute
        /// </summary>
        private bool isTransparent = false;

        /// <summary>
        /// Indicates thickness of the minute
        /// </summary>
        private float minuteThickness = 0.1f;

        /// <summary>
        /// Indicates thickness of the second hand
        /// </summary>
        private float secondHandThickness = 1;

        /// <summary>
        /// Remainder event handler
        /// </summary>
        public EventHandler RemainderTime;

        /// <summary>
        /// Tick event handler
        /// </summary>
        public EventHandler Tick;

        /// <summary>
        /// Indicate renderer
        /// </summary>
        private IClockRenderer renderer;

        /// <summary>
        /// Indicate renderer
        /// </summary>
        private IDigitalClockRenderer digitalRenderer;
        /// <summary>
        /// Show or hide the minutes
        /// </summary>
        private bool showMinute = true;

        /// <summary>
        /// Show or hide the minutes
        /// </summary>
        private bool showSecondHand= true;

        /// <summary>
        /// Indicate path region
        /// </summary>
        private GraphicsPath path1 = new GraphicsPath();

        /// <summary>
        /// Indicate path region
        /// </summary>
        private GraphicsPath path2 = new GraphicsPath();
        /// <summary>
        /// image for clock frame
        /// </summary>
        Image newImage = null;

        /// <summary>
        /// image  collection for clock frame
        /// </summary>
        ImageListAdv imagelist;

        /// <summary>
        /// Clock type
        /// </summary>
        private ClockTypes clockType;

        /// <summary>
        /// Show / Hide the custom time
        /// </summary>
        private bool showCustomTimeClock;

        /// <summary>
        /// custom time value
        /// </summary>
        private DateTime customTime ;

        /// <summary>
        /// Clock Format
        /// </summary>
        private string clockFormat;

        /// <summary>
        ///  Digital clock border color
        /// </summary>
        internal Color digitalOuterColor;

        /// <summary>
        ///  clock back color
        /// </summary>
        private Color backgroundColor;
        /// <summary>
        /// Handling the digital text
        /// </summary>
        private string digitText;

        /// <summary>
        /// Clock frame
        /// </summary>
        private ClockFrames clockFrame;

        /// <summary>
        /// Digital clock shapes
        /// </summary>
        private ClockShapes clockshape;

        /// <summary>
        /// Show/hide the Digital clock frame
        /// </summary>
        private bool showClockFrame;

        /// <summary>
        /// get/set the digital text color
        /// </summary>
        internal Color digitColor;

        /// <summary>
        /// Value for custom clock hours
        /// </summary>
        int hours;

        /// <summary>
        ///  Value for custom clock minutes
        /// </summary>
        int minutes;

        /// <summary>
        ///  Value for custom clock seconds
        /// </summary>
        int seconds;

        /// <summary>
        /// Finding AM/PM
        /// </summary>
        internal bool isAM;
        /// <summary>
        /// Gets/Sets the Clock time
        /// </summary>
        private DateTime currentDateTime;
        /// <summary>
        /// Parent BackColor
        /// </summary>
        internal Color color;
        /// <summary>
        /// Digital text size
        /// </summary>
        internal SizeF digitSizeF;
        /// <summary>
        /// Digital text font
        /// </summary>
        internal Font font;
        /// <summary>
        /// Display digital clock dates
        /// </summary>
        private bool displayDates;

        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

        # endregion

        # region Constructor

        public Clock()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(Clock));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            this.SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            Renderer = new ClockRenderer();
            DigitalRenderer = new DigitalClockRenderer();
            DigitalRender = new DigitalClockRenderer();
            Ticks.Interval = 1000;
            Ticks.Start();
            Ticks.Tick += new EventHandler(Tick_Tick);
            clockType = ClockTypes.Analog;
            showCustomTimeClock = false;
            digitalOuterColor = SystemColors.Control;
            clockFormat = "HH:mm:ss";
            digitText = "00.00.00";
            customTime = DateTime.Now;
            clockFrame = ClockFrames.RectangularFrame;
            clockshape = ClockShapes.Rectangle;
            showClockFrame = false;
            digitColor = Color.Black;
            hours = 0;
            minutes = 0;
            seconds = 0;
            isAM = true;
            displayDates = true;
            currentDateTime = DateTime.Now;
            backgroundColor = SystemColors.Control;
            imagelist = new ImageListAdv();
            imagelist.Images.Add(new Bitmap(typeof(Clock).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.Clock.DigitalClock.png")));
            imagelist.Images.Add(new Bitmap(typeof(Clock).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.Clock.DigitalClockCircle.png")));
            imagelist.Images.Add(new Bitmap(typeof(Clock).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.Clock.DigitalClockSquare.png")));
            this.MinimumSize = new Size(75, 75);
            CTRLSIZE = this.MinimumSize;
        }

        # endregion

        # region Properties

        /// <summary>
        /// Specifies the thickness of the hour hand.
        /// </summary>
        [
          Category("Appearance"),
          Description("Specifies the thickness of the hour hand.")
        ]
        public float HourHandThickness
        {
            get
            {
                return hourHandThickness;
            }
            set
            {
                hourHandThickness = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Specifies the thickness of the minutee hand.
        /// </summary>
        [
          Category("Appearance"),
          Description("Specifies the thickness of the minute hand.")
        ]
        public float MinuteHandThickness
        {
            get
            {
                return minuteHandThickness;
            }
            set
            {
                minuteHandThickness = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Specifies the thickness of the second hand.
        /// </summary>
        [
          Category("Appearance"),
          Description("Specifies the thickness of the second hand.")
        ]
        public float SecondHandThickness
        {
            get
            {
                return secondHandThickness;
            }
            set
            {
                secondHandThickness = value;
                this.Invalidate();
            }
            
        }

        /// <summary>
        /// Specifies the thickness of the minutes.
        /// </summary>
        [
          Category("Appearance"),
          Description("Specifies the thickness of the minutes.")
        ]
        public float MinuteThickness
        {
            get
            {
                return minuteThickness;
            }
            set
            {
                minuteThickness = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets/sets the value for CustomTime
        /// Supports for both analog and digital clock
        /// </summary>
        [
          Category("Appearance"),
          Description("get/sets the value for CustomTime.")
        ]
        public DateTime CustomTime
        {
            get
            {
                return customTime;
            }
            set
            {
                    customTime = value;
                    SetTime();
            }
        }
        /// <summary>
        /// Set default font
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the renderer.")
        ]
        [Browsable(false)]
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
            }
        }
        /// <summary>
        /// Gets/ sets the value to CurrentDateTime
        /// Supports for both analog and digital clock
        /// </summary>
        [
          Category("Appearance"),
          Description("Get/Sets the value for CurrentDateTime")
        ]
        public DateTime CurrentDateTime
        {
            get
            {
                return currentDateTime;
            }
            set
            {
                currentDateTime = value;
            }
        }
        /// <summary>
        /// Gets/ sets the value to Show/Hide the custom time clock
        /// Supports for both analog and digital clock
        /// </summary>
        [
          Category("Appearance"),
          Description("Show / Hide the custom time clock.")
        ]
        public bool ShowCustomTimeClock
        {
            get
            {
                return showCustomTimeClock;
            }
            set
            {
                showCustomTimeClock = value;
                if (value)
                    SetTime();
            }
        }
        /// <summary>
        /// Gets /sets the clock type
        /// </summary>
        [
          Category("Appearance"),
          Description("Get/sets the value for clock type.")
        ]
        public ClockTypes ClockType
        {
            get
            {
                return clockType;
            }
            set
            {
                clockType = value;
                this.Refresh();
            }
        }
        /// <summary>
        /// Specifies the thickness of the minutes.
        /// </summary>
        [
          Category("Appearance"),
          Description("Specifies the thickness of the minutes.")
        ]
        public bool IsTransparent
        {
            get
            {
                return isTransparent;
            }
            set
            {
                isTransparent = value;
                setColor();
                this.Invalidate();
            }
        }



        /// <summary>
        /// Specifies the visual style.
        /// </summary>
        [
          Category("Appearance"),
          Description("Specifies the visual style.")
        ]
        public ClockVisualStyle  VisualStyle
        {

            get
            {
                return visualStyle;
            }
            set
            {
                visualStyle = value;
                setVisualStyle();

            }
        }

        /// <summary>
        /// Gets or Sets the renderer.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the renderer.")
        ]
        public IClockRenderer Renderer
        {
            get
            {
                return renderer;
            }
            set
            {
                renderer = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or Sets the digital clock renderer.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the renderer.")
        ]
        [Browsable(false)]
        public IDigitalClockRenderer DigitalRenderer
        {
            get
            {
                return digitalRenderer;
            }
            set
            {
                digitalRenderer = value;
                this.Invalidate();
            }
        }
        /// <summary>
        /// Gets or Sets the current time.
        /// </summary>
        [
          Description("Gets or Sets the current time.")
        ]
        public DateTime Now
        {
            get
            {
                return now;
            }
            set
            {
                now = value;
                if (StopTimer)
                {
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or Sets the color of the hour hand.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the color of the hour hand.")
        ]
        public Color HourHandColor
        {
            get
            {
                return hourHandColor;
            }
            set
            {
                if (hourHandColor != value)
                {
                    hourHandColor = value;
                    setColor();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or Sets the color of the minute hand.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the color of the minute hand.")
        ]
        public Color MinuteHandColor
        {
            get
            {
                return minuteHandColor;
            }
            set
            {
                if (minuteHandColor != value)
                {
                    minuteHandColor = value;
                    setColor();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or Sets the Image.
        /// </summary>
        [
          Description("Gets or Sets Image.")
        ]
        public Image Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or Sets the color of the second hand.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the color of the second hand.")
        ]
        public Color SecondHandColor
        {
            get
            {
                return secondHandColor;
            }
            set
            {
                if (secondHandColor != value)
                {
                    secondHandColor = value;
                    setColor();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or Sets the first color of the gradient background.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the first color of the gradient background.")
        ]
        public Color StartGradientBackColor
        {
            get
            {
                return startGradientBackColor;
            }
            set
            {
                if (startGradientBackColor != value)
                {
                    startGradientBackColor = value;
                    setColor();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or Sets the second color of the gradient background.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the second color of the gradient background.")
        ]
        public Color EndGradientBackColor
        {
            get
            {
                return endGradientBackColor;
            }
            set
            {
                if (endGradientBackColor != value)
                {
                    endGradientBackColor = value;
                    setColor();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or Sets the border color.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the border color.")
        ]
        public Color BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                borderColor = value;
                digitalOuterColor = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Hide Controls back color
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
        /// Gets or Sets control back color
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets control back color.")
        ]
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
        /// Gets or Sets the alignment of AMorPM.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the alignment of AMorPM.")
        ]
        public Align AlignAMorPM
        {
            get
            {
                return align;
            }
            set
            {
                align = value;
                this.Invalidate();
            }
        
        }

        /// <summary>
        /// Enable of Disable remainder.
        /// </summary>
        [
          Category("Appearance"),
          Description("Enable of Disable remainder.")
        ]
        public bool EnableRemainder
        {
            get
            {
                return enableRemainder;
            }
            set
            {
                enableRemainder = value;
            }
        }

        /// <summary>
        /// Gets or Sets the date time for remainder.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the date time for remainder.")
        ]
        public DateTime Remainder
        {
            get
            {
                return remainder;
            }
            set
            {
                remainder = value;
            }
        }

        /// <summary>
        /// Show or Hide the minutes.
        /// </summary>
        [
          Category("Appearance"),
          Description("Show or Hide the minutes.")
        ]
        public bool ShowMinute
        {
            get
            {
                return showMinute;
            }
            set
            {
                showMinute = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Show or Hide the second hand.
        /// </summary>
        [
          Category("Appearance"),
          Description("Show or Hide the second hand.")
        ]
        public bool ShowSecondHand
        {
            get
            {
                return showSecondHand ;
            }
            set
            {
                showSecondHand = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or Sets the color of the minutes.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the color of the minutes.")
        ]
        public Color MinuteColor
        {
            get
            {
                return minuteColor;
            }
            set
            {
                if (minuteColor != value)
                {
                    minuteColor = value;
                    setColor();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Show or Hide the border.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets/Sets value for Show or Hide the border.")
        ]
        public bool ShowBorder
        {
            get
            {
                return showborder ;
            }
            set
            {
                showborder = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Marked as Obsolete. Please use ShowHourDesignator instead.
        /// </summary>
        [
          Category("Appearance"),
          Description("Show or Hide the AMorPM.[Marked as Obsolete,Please use ShowHourDesignator instead]"),
          ObsoleteAttribute("Please use ShowHourDesignator instead.")
        ]
        public bool ShowAMorPM
        {
            get
            {
                return showHourDesignator;
            }
            set
            {
                showHourDesignator = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets a value to show/hide the HourDesignator.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or sets a value to show/hide the HourDesignator."),
        ]
        public bool ShowHourDesignator
        {
            get
            {
                return showHourDesignator;
            }
            set
            {
                showHourDesignator = value;
                this.Invalidate();
            }
        }


        private bool stoptimer = false;
        public bool StopTimer
        {
            get
            {
                return stoptimer;
            }
            set
            {
                if (stoptimer != value)
                {
                    stoptimer = value;
                    if (stoptimer)
                        this.Ticks.Stop();
                    else
                        this.Ticks.Start();
                    this.Invalidate();
                }
                
            }
        }
        
        # endregion

        # region Overrides

        /// <summary>
        /// Gets / sets the value for digital clock text color
        /// Support only for digital clock
        /// </summary>
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                digitColor = value;
            }
        }
        /// <summary>
        /// Show/Hide the Digital Clock Frame
        /// Support only for digital clock
        /// </summary>
        [
          Category("Appearance"),
          Description("Show/Hide the Digital Clock Frame")
        ]
        public bool ShowClockFrame
        {
            get
            {
                return showClockFrame;
            }
            set
            {
                showClockFrame = value;
                this.Refresh();
                this.Invalidate();
            }
        }
        /// <summary>
        /// Gets /sets the value for digital clock shape
        /// Support only for digital clock
        /// </summary>
        [
          Category("Appearance"),
          Description("Get/Sets the value for digital clock shape")
        ]
        public ClockShapes ClockShape
        {
            get
            {
                return clockshape;
            }
            set
            {
                clockshape = value;
                this.Refresh();
            }
        }
        /// <summary>
        /// Gets /sets the value for digital clock frame
        /// Support only for digital clock
        /// </summary>
        [
          Category("Appearance"),
          Description("Get/Sets the value for digital clock frame")
        ]
        public ClockFrames ClockFrame
        {
            get
            {
                return clockFrame;
            }
            set
            {
                clockFrame = value;
                this.Refresh();
            }
        }
        /// <summary>
        /// Gets/Sets the value for DigitalClock Date
        /// </summary>
        [
          Category("Appearance"),
          Description("Get/Sets the value for CurrentDateTime")
        ]
        public bool DisplayDates
        {
            get
            {
                return displayDates;
            }
            set
            {
                displayDates = value;
                this.Refresh();
            }
        }
        /// <summary>
        /// Gets /sets the value for Digital text
        /// Support only for digital clock
        /// </summary>
        [Browsable(true), DefaultValue("00.00.00")]
        internal string DigitText
        {
            get
            {
                return digitText;
            }
            set
            {
                digitText = value;
                Invalidate();
            }
        }
        /// <summary>
        /// Gets / sets the value for Clock format
        /// Support only for digital clock
        /// </summary>
        [
          Category("Appearance"),
          Description("Get/sets the value for Digital Clock Format")
        ]
        public string ClockFormat
        {
            get
            {
                return clockFormat;
            }
            set
            {
                clockFormat = value;
            }
        }
        /// <summary>
        ///  OnPaint
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (ClockType == ClockTypes.Digital)
             {
                Rectangle rect1 = new Rectangle(2, 2 + this.Width / 4, this.Width - 5, this.Width / 2 - 5);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                string s = "00:00:000";
                digitSizeF = DigitalRender.GetStringSize(e.Graphics, s, Font);
                float scaleFactor = Math.Min(ClientSize.Width / digitSizeF.Width, ClientSize.Height / digitSizeF.Height);
                font = new Font(Font.FontFamily, scaleFactor * Font.SizeInPoints);
                digitSizeF = new SizeF(this.Width, this.Width / 4);
              color = this.Parent != null ? this.Parent.BackColor : Color.White;
                if (ShowClockFrame)
                {
                    SetControlSize(ClockFrame, e.Graphics, color);
                    if (newImage != null && this != null)
                        digitalRenderer.DrawDigitalClockFrame(e.Graphics, newImage,this);  
                }
                else
                {
                    SetControlSize(ClockShape, e.Graphics, color);
                    digitalRenderer.DrawDigitalClockBorder(e.Graphics, this);
                    using (SolidBrush brush = new SolidBrush(digitColor))
                    {
                        using (SolidBrush lightBrush = new SolidBrush(Color.FromArgb(20, digitColor)))
                        {
                            digitalRenderer.DrawDigits(e.Graphics, DigitText, CustomTime, font, brush, lightBrush,new PointF( (2 + ClientSize.Width - digitSizeF.Width) / 2, (5 + ClientSize.Height - digitSizeF.Height) / 2), this.ShowHourDesignator, isAM);
                        }
                    }
                }
             }
            else
        {
                this.Height = this.Width;
                Rectangle rect1 = new Rectangle(-1, -1, this.Width + 1, this.Width + 1);
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(rect1);
                    this.Region = new Region(path);
                }
                base.OnPaint(e);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Width - 1);
                DrawBackGround(e.Graphics, rect);
                if (showMinute)
                    renderer.DrawMinutesLine(e.Graphics, this.Width / 2, new Point(this.Width / 2, this.Width / 2), MinuteColor, this.Font, this.MinuteThickness);
                renderer.DrawHourHand(e.Graphics, this.Width / 4, this.HourHandThickness, new Point(this.Width / 2, this.Width / 2), this.HourHandColor, now);
                renderer.DrawMinuteHand(e.Graphics, this.Width / 2 - this.Width / 8, this.MinuteHandThickness, new Point(this.Width / 2, this.Width / 2), this.minuteHandColor, now);
                if (ShowSecondHand)
                    renderer.DrawSecondHand(e.Graphics, this.Width / 2 - this.Width / 8, this.SecondHandThickness, new Point(this.Width / 2, this.Width / 2), this.SecondHandColor, now);
                using (Pen pen = new Pen(this.BackgroundColor))
                {
                    e.Graphics.DrawEllipse(pen, rect1);
                }
            }
        }
        /// <summary>
        /// Set the digital clock size
        /// </summary>
        /// <param name="clockShapes">Used for calculate digital clock size</param>
        /// <param name="g">used for drawing edged region</param>
        /// <param name="c">Used for drawing edged region</param>
        private void SetControlSize(ClockShapes clockShapes, Graphics g, Color c)
        {
            GraphicsPath pat = new GraphicsPath();
            switch (clockShapes)
             {
                case ClockShapes.Circle:
                    this.Height = this.Width;
                    pat.AddEllipse(new Rectangle(0, 0, this.Width, this.Width));
                    this.Region = new Region(pat);
                    g.DrawPath(new Pen(c), pat);
                    break;
                case ClockShapes.Square:
                    this.Height = this.Width;
                    this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                    pat.AddRectangle(new Rectangle(0, 0, this.Width - 1, this.Height - 1));
                    g.DrawPath(new Pen(c), pat);
                    break;
                case ClockShapes.RoundedSquare:
                    this.Height = this.Width;
                    this.Region = new Region(GetRoundedregion(0, 0, this.Width, this.Height, this.Width / 6));
                    g.DrawPath(new Pen(c), GetRoundedregion(0, 0, this.Width, this.Height, this.Width / 6));
                    break;
                case ClockShapes.Rectangle:
                    this.Height = this.Width / 2;
                    this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                    pat.AddRectangle(new Rectangle(0, 0, this.Width, this.Height));
                    g.DrawPath(new Pen(c), pat);
                    break;
                case ClockShapes.RoundedRectangle:
                    this.Height = this.Width / 2;
                    this.Region = new Region(GetRoundedregion(0, 0, this.Width, this.Height, this.Width / 7));
                    g.DrawPath(new Pen(c, 3), GetRoundedregion(0, 0, this.Width, this.Height, this.Width / 7));
                    break;
            }
            pat.Dispose();
        }
        /// <summary>
        /// Get rounded region for the digital clock
        /// </summary>
        /// <param name="x">begining x-co-ordinate points for digital clock</param>
        /// <param name="y">begining y-co-ordinate points for digital clock</param>
        /// <param name="width">Used for measuring the region</param>
        /// <param name="height">Used for measuring the region</param>
        /// <param name="radius">Used for measuring the region</param>
        /// <returns> returns rounded region</returns>
        public GraphicsPath GetRoundedregion(float x, float y, float width, float height, float radius)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(x + radius, y, x + width - (radius * 2), y); // Line
            gp.AddArc(x + width - (radius * 2), y, radius * 2, radius * 2, 270, 90); // Corner
            gp.AddLine(x + width, y + radius, x + width, y + height - (radius * 2)); // Line
            gp.AddArc(x + width - (radius * 2), y + height - (radius * 2), radius * 2, radius * 2, 0, 90); // Corner
            gp.AddLine(x + width - (radius * 2), y + height, x + radius, y + height); // Line
            gp.AddArc(x, y + height - (radius * 2), radius * 2, radius * 2, 90, 90); // Corner
            gp.AddLine(x, y + height - (radius * 2), x, y + radius); // Line
            gp.AddArc(x, y, radius * 2, radius * 2, 180, 90); // Corner
            gp.CloseFigure();
            return gp;
        }
        /// <summary>
        /// Set Digital clock size 
        /// </summary>
        /// <param name="clockFrame"> Used for drawing the clock frame</param>
        /// <param name="g">used for drawing edged region</param>
        /// <param name="c">Used for drawing edged region</param>
        private void SetControlSize(ClockFrames clockFrame, Graphics g , Color c)
        {
            using (GraphicsPath pat = new GraphicsPath())
            {
                switch (clockFrame)
                {
                    case ClockFrames.RectangularFrame:
                        this.Height = this.Width/2;
                        newImage = imagelist.Images[0];
                        this.Region =new Region( new Rectangle( 0,0,this.Width , this.Height));
                        break;
                    case ClockFrames.CircularFrame:
                        this.Height = this.Width;
                        newImage = imagelist.Images[1];
                        this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                        pat.AddEllipse(new Rectangle(0, 0, this.Width, this.Height));
                        this.Region = new Region(pat);
                        g.DrawPath(new Pen(c), pat);
                        break;
                    case ClockFrames.SquareFrame:
                        this.Height = this.Width;
                        newImage = imagelist.Images[2];
                        this.Region = new Region(GetRoundedregion(0, 0, this.Width, this.Height, this.Width / 18));
                        g.DrawPath(new Pen(c), GetRoundedregion(0, 0, this.Width, this.Height, this.Width / 18));
                        break;
                }
            }
        }
        protected override void OnRegionChanged(EventArgs e)
        {
            base.OnRegionChanged(e);
        }
      
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            path1.Dispose();
            path2.Dispose();
        }

        /// <summary>
        ///Size changed
        /// </summary>
        protected override void OnSizeChanged(EventArgs e)
        {
            if (!EnableTouchMode && this.DesignMode)
                CTRLSIZE = this.Size;
            base.OnSizeChanged(e);
        }
        /// <summary>
        /// OnResize
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        # endregion

        # region ShouldSerialization

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeVisualStyle()
        {
            return VisualStyle != ClockVisualStyle.None;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeHourHandColor()
        {
            return HourHandColor != ColorTranslator.FromHtml("#5F6F77");
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeMinuteHandColor()
        {
            return MinuteHandColor != ColorTranslator.FromHtml("#5F6F77");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeSecondHandColor()
        {
            return SecondHandColor != ColorTranslator.FromHtml("#5F6F77");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeHourHandThickness()
        {
            return HourHandThickness != 2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeMinuteHandThickness()
        {
            return MinuteHandThickness != 2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeSecondHandThickness()
        {
            return SecondHandThickness != 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeMinuteColor()
        {
            return MinuteColor != ColorTranslator.FromHtml("#597A84");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeShowAMorPM()
        {
            return ShowAMorPM != false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeStartGradientBackColor()
        {
            return StartGradientBackColor != ColorTranslator.FromHtml("#FFFFFF");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeEndGradientBackColor()
        {
            return EndGradientBackColor != ColorTranslator.FromHtml("#CCD6D9");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeBorderColor()
        {
            return BorderColor != ColorTranslator.FromHtml("#848489");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeIsTransparent()
        {
            return IsTransparent != false ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeImage()
        {
            return Image != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeAlignAMorPM()
        {
            return this.AlignAMorPM != Align.Right;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeEnableRemainder()
        {
            return this.EnableRemainder != false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeMinuteThickness()
        {
            return this.MinuteThickness != 0.1F;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeShowMinute()
        {
            return this.ShowMinute != true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeShowBorder()
        {
            return this.ShowBorder   != true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeShowSecondHand()
        {
            return this.ShowSecondHand  != true;
        }


        # endregion

        # region Methods

        /// <summary>
        /// Sets VisualStyle
        /// </summary>
        private void setVisualStyle()
        {

            if (VisualStyle == ClockVisualStyle.OfficeBlue)
            {
                this.StartGradientBackColor = ColorTranslator.FromHtml("#E4F1FF");
                this.EndGradientBackColor = ColorTranslator.FromHtml("#B4D2FC");
                this.borderColor = this.minuteColor = this.hourHandColor = this.minuteHandColor = this.secondHandColor = ColorTranslator.FromHtml("#5F6F77");
            }
            else if (VisualStyle == ClockVisualStyle.OfficeSilver)
            {
                this.StartGradientBackColor = ColorTranslator.FromHtml("#DDE1E6");
                this.EndGradientBackColor = ColorTranslator.FromHtml("#CFD3DD");
                this.borderColor = this.minuteColor = this.hourHandColor = this.minuteHandColor = this.secondHandColor = Color.DarkGray;
            }
            else if (VisualStyle == ClockVisualStyle.OfficeBlack)
            {
                this.StartGradientBackColor = ColorTranslator.FromHtml("#8F8F8F");
                this.EndGradientBackColor = ColorTranslator.FromHtml("#505050");
                this.borderColor = this.minuteColor = this.hourHandColor = this.minuteHandColor = this.secondHandColor = Color.White;
            }
            else if (VisualStyle == ClockVisualStyle.None)
            {
                this.StartGradientBackColor = ColorTranslator.FromHtml("#FFFFFF");
                EndGradientBackColor = ColorTranslator.FromHtml("#CCD6D9");
                this.borderColor = this.minuteColor = this.hourHandColor = this.minuteHandColor = this.secondHandColor = ColorTranslator.FromHtml("#5F6F77");
            }

        }
        private string CustomDateTime()
        {
            string hour, minute, second;
            minutes = (minutes + (seconds / 60));
            hours = (hours + minutes / 60);
            hours = hours % 24;
            minutes = minutes % 60;
            seconds = seconds % 60;
            if (hours < 10)
                hour = "0" + hours.ToString();
            else
                hour = hours.ToString();
            if (minutes < 10)
                minute = "0" + minutes.ToString();
            else
                minute = minutes.ToString();
            if (seconds < 10)
                second = "0" + seconds.ToString();
            else
                second = seconds.ToString();
            if (ShowHourDesignator)
            {
                if (Convert.ToInt32(hour) > 11)
                {
                    isAM = false;
                }
                else
                {
                    isAM = true;
                }
                if (Convert.ToInt32(hour) > 12)
                {
                    hour = (Convert.ToInt32(hour) % 12).ToString();
                    
                }
                else
                {
                    if (hours < 10)
                        hour = "0" + hours.ToString();
                    else
                        hour = hours.ToString();
                }
            }
            CurrentDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, CustomTime.Date.Day, Convert.ToInt32(hour), Convert.ToInt32(minute), Convert.ToInt32(second));
            OnValueChanged(new DateTime(DateTime.Now.Year, DateTime.Now.Month, CustomTime.Date.Day, Convert.ToInt32(hour), Convert.ToInt32(minute), Convert.ToInt32(second)));
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, CustomTime.Date.Day ,Convert.ToInt32(hour), Convert.ToInt32(minute), Convert.ToInt32(second)).ToString(ClockFormat);  
        }
        /// <summary>
        /// Set the time
        /// </summary>
        private void SetTime()
        {
            hours = customTime.Hour;
            minutes = customTime.Minute;
            seconds = customTime.Second;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Tick_Tick(object sender, EventArgs e)
        {
            if (ClockType == ClockTypes.Digital)
            {
                if (!this.DesignMode)
                {
                    if (ShowCustomTimeClock)
                    {
                        ++seconds;
                        this.DigitText = CustomDateTime();
                    }
                    else
                    {
                        if (ShowHourDesignator)
                        {
                            int AMHour = DateTime.Now.Hour;
                            isAM = true;
                            if (AMHour > 11)
                            {
                                isAM = false;
                            }
                            if (AMHour > 12)
                            {
                                AMHour = DateTime.Now.Hour % 12;
                                
                            }
                            CurrentDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, CustomTime.Date.Day, AMHour, DateTime.Now.Minute, DateTime.Now.Second);
                            OnValueChanged(new DateTime(DateTime.Now.Year, DateTime.Now.Month, CustomTime.Date.Day, AMHour, DateTime.Now.Minute, DateTime.Now.Second)); 
                            this.DigitText = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, AMHour, DateTime.Now.Minute, DateTime.Now.Second).ToString(ClockFormat);
                        }
                        else
                        {
                            CurrentDateTime = DateTime.Now;
                            OnValueChanged(DateTime.Now);
                            this.DigitText = DateTime.Now.ToString(ClockFormat);
                        }
                    }
                }
                else
                {
                    this.DigitText = "00:00:00";
                }
            }
            else
            {
                if (!this.DesignMode)
                {
                    if (ShowCustomTimeClock)
                    {
                        ++seconds;
                        this.DigitText = CustomDateTime();
                        OnValueChanged(new DateTime(now.Year, now.Month, now.Day, hours, minutes, seconds));
                        now = new DateTime(now.Year, now.Month, now.Day, hours, minutes, seconds);
                    }
                    else
                    {
                        OnValueChanged(DateTime.Now);
                        now = DateTime.Now;
                    }
                    string t1 = Convert.ToString(now.TimeOfDay);
                    string t2 = Convert.ToString(Remainder.TimeOfDay);
                    if (now.Hour == Remainder.Hour && now.Minute == Remainder.Minute && now.Second == Remainder.Second && enableRemainder)
                    {
                        showSplashPanel();
                        if (RemainderTime != null)
                            RemainderTime(this, new EventArgs());
                    }

                    if (Tick != null && !this.DesignMode)
                        Tick(this, new EventArgs());

                    Invalidate();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void showSplashPanel()
        {
            AutoLabel label = new AutoLabel();
            label.Text = this.Text;
            label.BackColor = Color.Transparent;
            SplashPanel splashPanel = new SplashPanel();
            splashPanel.BackgroundColor = new Syncfusion.Drawing.BrushInfo(Syncfusion.Drawing.GradientStyle.Vertical, EndGradientBackColor, StartGradientBackColor);
            splashPanel.ShowAnimation = true;
            splashPanel.Controls.Add(label);
            splashPanel.DesktopAlignment = SplashAlignment.RightBottom;
            splashPanel.Size = new Size(200, 100);
            label.Size = new Size(splashPanel.Width - 5, splashPanel.Height - 50);
            label.Location = new Point(splashPanel.Width / 2 - label.Width / 2, label.Height);
            splashPanel.ShowSplash();
            ButtonAdv button = new ButtonAdv();
            button.Appearance = ButtonAppearance.Office2007;
            button.UseVisualStyle = true;
            button.Size = new Size(40, 20);
            button.Location = new Point(splashPanel.Width / 2 - button.Width / 2, splashPanel.Height - 2 * button.Height);
            button.Office2007ColorScheme = Office2007Theme.Managed;
            splashPanel.Controls.Add(button);
            button.Text = "OK";
            button.Click += new EventHandler(button_Click);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void button_Click(object sender, EventArgs e)
        {
            ((sender as ButtonAdv).Parent as SplashPanel).HideSplash();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        private void drawBorder(Graphics g, Rectangle rect)
        {

            path1.AddEllipse(rect);
            for (int i = 0; i < (this.Width / 40); i++)
            {
                rect = itreateRect(rect);
            }
            path2.AddEllipse(rect);
            using (Region region = new Region(path1))
            {
                region.Exclude(path2);
                using (Brush linearGradientBrush = new LinearGradientBrush(rect, EndGradientBackColor, startGradientBackColor, 45))
                {
                    g.FillRegion(linearGradientBrush, region);
                }
            }


        }

        private void  setColor()
        {
                if (IsTransparent)
                {
                    this.StartGradientBackColor = Color.FromArgb(100, StartGradientBackColor);
                    this.EndGradientBackColor = Color.FromArgb(100, EndGradientBackColor);
                    this.HourHandColor = Color.FromArgb(200, HourHandColor);
                    this.MinuteHandColor = Color.FromArgb(200, MinuteHandColor);
                    this.SecondHandColor = Color.FromArgb(200, SecondHandColor);
                    this.MinuteColor = Color.FromArgb(200, MinuteColor);
                    this.BackColor = Color.Transparent;
                }
                else
                {
                    this.StartGradientBackColor = Color.FromArgb(255, StartGradientBackColor);
                    this.EndGradientBackColor = Color.FromArgb(255, EndGradientBackColor);
                    this.HourHandColor = Color.FromArgb(255, HourHandColor);
                    this.MinuteHandColor = Color.FromArgb(255, MinuteHandColor);
                    this.SecondHandColor = Color.FromArgb(255, SecondHandColor);
                    this.MinuteColor = Color.FromArgb(255, MinuteColor);
                }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        private void DrawBackGround(Graphics g, Rectangle rect)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Color OuterBorder = BorderColor;
            if (Image == null)
            {
                using (Brush linearGradientBrush = new LinearGradientBrush(rect, startGradientBackColor, EndGradientBackColor, 45))
                {
                    g.FillEllipse(linearGradientBrush, rect);
                }
                Rectangle innerRect = new Rectangle(rect.X + rect.Width / 4, rect.Y + rect.Height / 4, rect.Width + rect.Width / 2, rect.Height + rect.Height / 2);
                using (Brush linearGradientBrush = new LinearGradientBrush(innerRect, EndGradientBackColor, startGradientBackColor, 45))
                {
                    g.FillPie(linearGradientBrush, innerRect, 180, 90);
                }
            }
            else
                render.DrawBackGroundImage(g, rect, Image);

            drawAMOrPM(g, rect);

            if (ShowBorder)
            {
                using (Pen p = new Pen(OuterBorder))
                {
                    drawBorder(g, rect);
                    drawBorder(g, rect);
                    g.DrawEllipse(p, rect);

                    for (int i = 0; i < (this.Width / 40); i++)
                    {
                        rect = itreateRect(rect);
                    }
                    g.DrawEllipse(p, rect);
                    itreateRect(rect);
                    g.DrawEllipse(p, rect);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        private void renderAMorPM(Graphics g, Rectangle rect)
        {
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, rect);
            }

            using (Pen p = new Pen(Color.Black))
            {
                g.DrawRectangle(p, rect);
            }
            string text = "AM";
            if (now.ToString().EndsWith("PM"))
                text = "PM";
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            using (SolidBrush brush = new SolidBrush(Color.Black))
            {
                g.DrawString(text, this.Font, brush, rect, format);
            }
            format.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        private void drawAMOrPM(Graphics g, Rectangle rect)
        {
            if (ShowHourDesignator)
            {
                if (this.AlignAMorPM == Align.Right)
                {
                    Rectangle r = new Rectangle(rect.Width / 2 + rect.Width / 6, rect.Height / 2 - rect.Height / 25, rect.Width / 7, rect.Height / 14);
                    renderAMorPM(g, r);
                }
                else if (this.AlignAMorPM == Align.Top)
                {
                    Rectangle r = new Rectangle(rect.Width / 2 - rect.Width / 10, rect.Height / 4, rect.Width / 7, rect.Height / 14);
                    renderAMorPM(g, r);
                }
                else if (this.AlignAMorPM == Align.Left)
                {
                    Rectangle r = new Rectangle(rect.Width / 6, rect.Height / 2 - rect.Height / 25, rect.Width / 7, rect.Height / 14);
                    renderAMorPM(g, r);
                }
                else if (this.AlignAMorPM == Align.Bottom)
                {
                    Rectangle r = new Rectangle(rect.Width / 2 - rect.Width / 10, rect.Height / 2 + rect.Height / 6, rect.Width / 7, rect.Height / 14);
                    renderAMorPM(g, r);
                }
                else
                { }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        private Rectangle itreateRect(Rectangle rect)
        {
            rect.X = rect.X + 1;
            rect.Y = rect.Y + 1;
            rect.Width = rect.Width - 2;
            rect.Height = rect.Height - 2;
            return rect;
        }


        # endregion

        #region ClockTimeEvent
        public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs e);
        public class ValueChangedEventArgs : EventArgs
        {
            public ValueChangedEventArgs(DateTime ClockTime)
            {
                currentDateTime = ClockTime;
            }
            private DateTime currentDateTime;
            public DateTime CurrentDateTime
            {
                get
                {
                    return currentDateTime;
                }
                set
                {
                    currentDateTime = value;
                }
            }
        }
        /// <summary>
        /// Values for angle chaged
        /// </summary>
        public event ValueChangedEventHandler ValueChanged;

        protected void OnValueChanged(DateTime ClockDateTime)
        {
            if (this.ValueChanged != null)
                ValueChanged(this, new ValueChangedEventArgs(ClockDateTime));
        }
        #endregion

        #region For Touch

        bool isScaling = false;
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
        ///Gets or Sets the touch mode
        /// </summary>
		[DefaultValue(false)]
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
        ///Applies the scaling
        /// </summary>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            isScaling = false;
            this.ResumeLayout();
            this.Refresh();
        }
        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }
        #endregion

    }

    /// <summary>
    /// Clock type
    /// </summary>
    public enum ClockTypes
    {
        /// <summary>
        /// Analog clock
        /// </summary>
        Analog,
        /// <summary>
        /// digital clock
        /// </summary>
        Digital
    }
    /// <summary>
    /// Clock Shapes
    /// </summary>
    public enum ClockShapes
    {
        /// <summary>
        /// Rectangle shape
        /// </summary>
        Rectangle,
        /// <summary>
        /// Rounded rectangle shape
        /// </summary>
        RoundedRectangle,
        /// <summary>
        /// Circle shape
        /// </summary>
        Circle,
        /// <summary>
        /// Square shape
        /// </summary>
        Square,
        /// <summary>
        /// Rounded square
        /// </summary>
        RoundedSquare        
    }
    //public enum Sounds : uint
    //{
    //    MB_OK = 0,
    //    MB_ICONHAND = 0x00000010,
    //    MB_ICONQUESTION = 0x00000020,
    //    MB_ICONEXCLAMATION = 0x00000030,
    //    MB_ICONASTERISK = 0x00000040
    //}
    /// <summary>
    /// Clock frames
    /// </summary>
    public enum ClockFrames
    {
        /// <summary>
        /// Default digital clock frame1
        /// </summary>
        RectangularFrame,
        /// <summary>
        /// default digital clock frame 2
        /// </summary>
        CircularFrame,
        /// <summary>
        /// default digital clock frame 3
        /// </summary>
        SquareFrame
    }
    # endregion
    /// <summary>
    /// CheckBoxAdv Designer
    /// </summary>
    public class ClockDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        /// <summary>
        /// Designer ActionList collection
        /// </summary>
        private System.ComponentModel.Design.DesignerActionListCollection actionLists;

        /// <summary>
        ///  Initializes a new instance of the CheckBoxAdvDesigner class
        /// </summary>
        public ClockDesigner()
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
                    this.actionLists.Add(new ClockActionList(this.Component));
                }

                return this.actionLists;
            }
        }

#endif
    }
}
