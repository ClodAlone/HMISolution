#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Tools.Design;

namespace Syncfusion.Windows.Forms.Tools
{
    [
    Designer(typeof(RangeSliderDesigner)), Description("Represents a Track Bar extended with range setting"),
    ToolboxBitmap(typeof(RangeSlider), "ToolboxIcons.Slider.bmp"), DefaultEvent("ValueChanged"),
    ]
    public class RangeSlider : Control
    {
        /// <summary>
        /// Different areas of the control
        /// </summary>
        protected enum RangeSliderArea
        {
            /// <summary>
            /// Out of Control
            /// </summary>
            None,

            /// <summary>
            /// Over Left thumb
            /// </summary>
            LeftThumb,

            /// <summary>
            /// Over Right Thumb
            /// </summary>
            RightThumb
        }

        /// <summary>
        /// Different states of an item
        /// </summary>
        private enum ItemState
        {
            /// <summary>
            /// Default state of the item.
            /// </summary>
            Default,

            /// <summary>
            /// Item should be drawn highlighted.
            /// </summary>
            Highlighted,

            /// <summary>
            /// Item is pushed.
            /// </summary>
            Pushed
        }

        /// <summary>
        /// Different focus of Thumb
        /// </summary>
        public enum Thumb
        {          
            /// <summary>
            /// Focus of Left Thumb.
            /// </summary>
            Left,

            /// <summary>
            /// Focus of Right Thumb.
            /// </summary>
            Right
        }
        /// <summary>
        /// RangeSlider Style
        /// </summary>
        public enum RangeSliderStyle
        {
            /// <summary>
            /// Default appearance.
            /// </summary>
            Default,
            /// <summary>
            /// Metro-like appearance.
            /// </summary>
            Metro
        }
        #region Constants

        /// <summary>
        /// Maximum value of TrackBar.
        /// </summary>
        private const int MAXIMUM = 10;

        /// <summary>
        /// Minimum value of TrackBar.
        /// </summary>
        private const int MINIMUM = 0;

        /// <summary>
        /// Position of Left thumb
        /// </summary>
        private const int SLIDERMIN = 0;

        /// <summary>
        /// position of right thumb
        /// </summary>
        private const int SLIDERMAX = 10;

        /// <summary>
        /// Initial Position of Left thumb
        /// </summary>
        private const int LEFTSLIDERPOSITION = 0;

        /// <summary>
        /// Initial Position of right thumb
        /// </summary>
        private const int RIGHTSLIDERPOSITION = 10;

        /// <summary>
        /// Initial Height of the channel
        /// </summary>
        private const int CHANNELHEIGHT = 4;

        /// <summary>
        /// Initial frquency of ticks
        /// </summary>
        private const int TICKFREQUENCY = 1;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

        /// <summary>

        /// Default channel height
        /// </summary>
        private static int CHNHEIGHT = default(int);
        /// <summary>
        /// Default slider size
        /// </summary>
        private static Size SLIDERSIZE = default(Size);
        #endregion

        #region Fields

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Blend for slider.
        /// </summary>
        private static Blend _sliderBlend;

        /// <summary>
        /// Value of the SliderMin
        /// </summary>
        private int sliderMin = SLIDERMIN;

        /// <summary>
        /// Value of the SliderMax
        /// </summary>
        private int sliderMax = SLIDERMAX;

        /// <summary>
        /// Value of the Left Slider Position
        /// </summary>
        private int leftSliderPosition = LEFTSLIDERPOSITION;

        /// <summary>
        /// Value of the Right Slider Position
        /// </summary>
        private int rightSliderPosition = RIGHTSLIDERPOSITION;

        /// <summary>
        /// Indicates whether left slider is moved by mouse
        /// </summary>
        private bool leftSliderMoving;

        /// <summary>
        /// Indicates whether right slider is moved by mouse
        /// </summary>
        private bool rightSliderMoving;

        /// <summary>
        /// color of the range
        /// </summary>
        private Color rangeColor = Color.FromArgb(242, 144, 36);
		/// <summary>
		/// Previous range color.
		/// </summary>
        protected Color prev_rangeColor = ColorTranslator.FromHtml("#16A5DC");
        /// <summary>
        /// Area where mouse pointer is currently situated
        /// </summary>
        private RangeSliderArea currentArea;

        /// <summary>
        /// Currently pushed Item
        /// </summary>
        private RangeSliderArea pushedItem;

        /// <summary>
        /// color of the thumb
        /// </summary>
        private Color thumbColor = Color.FromArgb(159, 191, 239);
		/// <summary>
		/// Previous thumb color.
		/// </summary>
        protected Color prev_thumbColor = ColorTranslator.FromHtml("#16A5DC");
        /// <summary>
        /// color of the Highlighted Thumb
        /// </summary>
        private Color highlightedThumbColor = Color.FromArgb(247, 199, 82);

        /// <summary>
        /// Color of the Pushed Thumb
        /// </summary>
        private Color pushedThumbColor = Color.FromArgb(255, 138, 24);

        /// <summary>
        /// Color of the Channel
        /// </summary>
        private Color channelColor = Color.FromArgb(51, 66, 121);
		/// <summary>
		/// Previous channel color.
		/// </summary>
        protected Color prev_channelColor = ColorTranslator.FromHtml("#D1D3D4");
        /// <summary>
        /// Orientation of the Control
        /// </summary>
        private Orientation orientation = Orientation.Horizontal;

        /// <summary>
        /// Height of the Channel
        /// </summary>
        private int channelHeight = CHANNELHEIGHT;

        /// <summary>
        /// size of the slider
        /// </summary>
        private Size sliderSize = new Size(11, 14);

        /// <summary>
        /// Minimum value of the RangeSlider
        /// </summary>
        private int maximum = MAXIMUM;

        /// <summary>
        /// Maximum value of the RangeSlider
        /// </summary>
        private int minimum = MINIMUM;

        /// <summary>
        /// Frequency of the Ticks
        /// </summary>
        private int tickFrequency = TICKFREQUENCY;

        /// <summary>
        /// Indicates whether to show ticks
        /// </summary>
        private bool showTicks = true;

        /// <summary>
        /// Height of the RangeSlider
        /// </summary>
        private int height;

        /// <summary>
        /// Current Focus of the Thumb
        /// </summary>       
        private Thumb focussedThumb;

        /// <summary>
        /// Indicates whether to Enable Thumb
        /// </summary>
        private bool showThumbFocus = false;
        /// <summary>
        /// Specifies an advanced appearance this control.
        /// </summary>
        private RangeSliderStyle style = RangeSliderStyle.Default;
        ///// <summary>
        ///// Indicate the Thumb movement
        ///// </summary>       
        //private Thumb moveState;

        private static readonly ControlStyles PAINT_STYLES =
        ControlStyles.SupportsTransparentBackColor |
        ControlStyles.OptimizedDoubleBuffer |
        ControlStyles.AllPaintingInWmPaint |
        ControlStyles.UserPaint;
        /// <summary>
        ///scaling value
        /// </summary>
        private bool isScaling = false;
        /// <summary>
        ///scale factorvalue
        /// </summary>
        private float _scalefactor = 1f;
        #endregion

        #region Initialization

        static RangeSlider()
        {
            _sliderBlend = new Blend();
            _sliderBlend.Factors = new float[] { 0.0F, 0.0F, 1.0F, 0.0F, 0.0F };
            _sliderBlend.Positions = new float[] { 0.0F, 0.15F, 0.35F, 0.8F, 1.0F };
        }

        /// <summary>
        /// Initializes a new instance of the RangeSlider
        /// </summary>
        public RangeSlider()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(RangeSlider));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

            this.Width = 250;
            this.AutoSize = true;
            height = (Orientation == Orientation.Horizontal) ? this.Height : this.Width;
            SliderMin = minimum;
            SliderMax = maximum;
            this.SetStyle(PAINT_STYLES, true);
            this.BackColor = Color.Transparent;
            CTRLSIZE = this.Size;
            SLIDERSIZE = this.SliderSize;
            CHNHEIGHT = this.ChannelHeight;
        }

        /// <summary>
        /// Initializes a new instance of the RangeSlider
        /// </summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public RangeSlider(int min, int max)
            : this()
        {
            minimum = min;
            maximum = max;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets an advanced appearance for the RangeSliderAdv.
        /// </summary>
        [Description("Gets or sets an advanced appearance for the RangeSliderAdv.")]
        [Category("Appearance")]
        [DefaultValue(RangeSliderStyle.Default)]
        public RangeSliderStyle VisualStyle
        {
            get
            {
                return this.style;
            }
            set
            {
                if (this.style != value)
                {
                    this.style = value;
                    if (this.VisualStyle == RangeSliderStyle.Metro)
                    {
                        if (this.DesignMode)
                        {
                            RangeColor = Color.FromArgb(22,165,220);
                            ThumbColor = Color.FromArgb(22, 165, 220);
                            ChannelColor = Color.FromArgb(209, 211, 212);
                            SliderSize = new Size(7, 17);
                        }
                    }
                    else if (this.VisualStyle == RangeSliderStyle.Default)
                    {
                        if (SliderSize == new Size(7, 17))
                            this.SliderSize = new Size(11, 14);
                        RangeColor = Color.FromArgb(242, 144, 36);
                        ChannelColor = Color.FromArgb(51, 66, 121);
                        ThumbColor = Color.FromArgb(159, 191, 239);
                    }
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets the color of the thumb.
        /// </summary>
        /// <value>The color of the thumb.</value>
        [Browsable(true), Category("Appearance"), Description("Color of the Thumbs.")]
        public Color ThumbColor
        {
            get
            {
                return thumbColor;
            }
            set
            {
                if (thumbColor != value)
                {
                    thumbColor = value;
                    prev_thumbColor = value;
                    InvalidateArea(RangeSliderArea.LeftThumb);
                    InvalidateArea(RangeSliderArea.RightThumb);
                }
            }
        }
		/// <summary>
		/// Serializes thumb color.
		/// </summary>
        protected bool ShouldSerializeThumbColor()
        {
            if (this.prev_thumbColor != ColorTranslator.FromHtml("#16A5DC"))
                this.prev_thumbColor = ColorTranslator.FromHtml("#16A5DC");
            return true;
        }

        /// <summary>
        ///resets the thumb color
        /// </summary>
        public void ResetThumbColor()
        {
            this.prev_thumbColor = ColorTranslator.FromHtml("#16A5DC");
        }
        /// <summary>
        /// Gets or sets the color of the highlighted thumb.
        /// </summary>
        /// <value>The color of the highlighted thumb.</value>
        [Browsable(true), Category("Appearance"), Description("Color of the Highlighted Thumb.")]
        public Color HighlightedThumbColor
        {
            get
            {
                return highlightedThumbColor;
            }
            set
            {
                if (highlightedThumbColor != value)
                {
                    highlightedThumbColor = value;

                    InvalidateArea(RangeSliderArea.LeftThumb);
                    InvalidateArea(RangeSliderArea.RightThumb);
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the pushed thumb.
        /// </summary>
        /// <value>The color of the pushed thumb.</value>
        [Browsable(true), Category("Appearance"), Description("Color of the Pushed Thumb.")]
        public Color PushedThumbColor
        {
            get
            {
                return pushedThumbColor;
            }
            set
            {
                if (pushedThumbColor != value)
                {
                    pushedThumbColor = value;
                    InvalidateArea(RangeSliderArea.LeftThumb);
                    InvalidateArea(RangeSliderArea.RightThumb);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Position of the Minimum slider.
        /// </summary>
        /// <value> Minimum Slider value.</value>
        [Browsable(true), Category("Behavior"), Description("Position of the Left Slider"), DefaultValue(0)]
        public int SliderMin
        {
            get { return sliderMin; }
            set
            {
                if (value >= Minimum && value <= Maximum)
                {
                    if (value <= SliderMax)
                    {
                        sliderMin = value;
                        Invalidate();
                        Update();
                        OnValueChanged();
                    }
                    else
                    {
                        throw new ArgumentException(" SliderMin is greater than SliderMax");
                    }
                }
                else
                {
                    sliderMin = Minimum;
                    Invalidate();
                    Update();
                    OnValueChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Position of the Maximum  slider.
        /// </summary>
        /// <value>Maximum Slider value.</value>
        [Browsable(true), Category("Behavior"), Description("Position of the Right Slider"), DefaultValue(10)]
        public int SliderMax
        {
            get { return sliderMax; }
            set
            {
                if (value <= Maximum && value >= Minimum)
                {
                    if (value >= SliderMin)
                    {
                        sliderMax = value;
                        Invalidate();
                        Update();
                        OnValueChanged();
                    }
                    else
                    {
                        throw new ArgumentException(" SliderMax is less than SliderMin");
                    }
                }
                else
                {
                    sliderMax = Maximum;
                    Invalidate();
                    Update();
                    OnValueChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the range.
        /// </summary>
        /// <value>The color of the range.</value>
        [Browsable(true), Category("Appearance"), Description("Color of Range")]
        public Color RangeColor
        {
            get { return rangeColor; }
            set
            {
                rangeColor = value;
                prev_rangeColor = value;
                this.Invalidate();
            }
        }
		/// <summary>
		/// Serializes range color.
		/// </summary>
        protected bool ShouldSerializeRangeColor()
        {
            if (this.prev_rangeColor != ColorTranslator.FromHtml("#16A5DC"))
            {
                this.prev_rangeColor = ColorTranslator.FromHtml("#16A5DC");
            }
            return true;
        }

        /// <summary>
        /// resets range color
        /// </summary>
        public void ResetRangeColor()
        {
            this.prev_rangeColor = ColorTranslator.FromHtml("#16A5DC");
        }
        /// <summary>
        /// Gets or sets the color of the channel.
        /// </summary>
        /// <value>The color of the channel.</value>
        [Browsable(true), Category("Appearance"), Description("Color of Channel")]
        public Color ChannelColor
        {
            get { return channelColor; }
            set
            {
                channelColor = value;
                prev_channelColor = value;
                this.Invalidate();
            }
        }
		/// <summary>
		/// Serializes channel color.
		/// </summary>
        protected bool ShouldSerializeChannelColor()
        {
            if (this.prev_channelColor != ColorTranslator.FromHtml("#D1D3D4"))
                this.prev_channelColor = ColorTranslator.FromHtml("#D1D3D4");
            return true;
        }

        /// <summary>
        ///Resets channel color
        /// </summary>
        public void ResetChannelColor()
        {
            this.prev_channelColor = ColorTranslator.FromHtml("#D1D3D4");
        }
        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        [Category("Behavior"), DefaultValue(typeof(Orientation), "Horizontal"), Description("Specify the orientation of RangeSlider control.")]
        public Orientation Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                if (orientation != value)
                {
                    orientation = value;
                    Size size = this.Size;
                    int iTemp = size.Height;
                    size.Height = size.Width;
                    size.Width = iTemp;
                    this.Size = size;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the channel.
        /// </summary>
        /// <value>The height of the channel.</value>
        [Category("Appearance"), Description("Height of channel."), DefaultValue(4)]
        public int ChannelHeight
        {
            get
            {
                return channelHeight;
            }
            set
            {
                if (channelHeight != value)
                {
                    channelHeight = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the slider.
        /// </summary>
        /// <value>The size of the slider.</value>
        [Category("Appearance"), Description("Size of Thumb."), DefaultValue(typeof(Size), "11, 14")]
        public Size SliderSize
        {
            get
            {
                return sliderSize;
            }
            set
            {
                if (sliderSize != value)
                {
                    sliderSize = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>The minimum.</value>
        [Category("Behavior"), Description("Minimum value of RangeSlider."), DefaultValue(0)]
        public int Minimum
        {
            get
            {
                return minimum;
            }
            set
            {
                if (minimum != value)
                {
                    if (value < maximum)
                    {
                        minimum = value;
                    }
                    else
                    {
                        minimum = maximum - 1;
                    }

                    if (SliderMin < minimum)
                    {
                        this.SliderMin = minimum;
                    }

                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>The maximum.</value>
        [Category("Behavior"), Description("Maximum value of RangeSlider."), DefaultValue(10)]
        public int Maximum
        {
            get
            {
                return maximum;
            }
            set
            {
                if (maximum != value)
                {
                    if (value > minimum)
                    {
                        maximum = value;
                    }
                    else
                    {
                        maximum = minimum + 1;
                    }

                    if (SliderMax > maximum)
                    {
                        this.SliderMax = maximum;
                    }

                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the tick frequency.
        /// </summary>
        /// <value>The tick frequency.</value>
        [Category("Behavior"), Description("Frequency of Ticks"), DefaultValue(1)]
        public int TickFrequency
        {
            get { return tickFrequency; }
            set
            {
                if (tickFrequency != value)
                {
                    if (value <= (Maximum - Minimum))
                    {
                        tickFrequency = value;
                        Invalidate();
                    }
                    else
                    {
                        throw new ArgumentException("Invalid value");
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show ticks.
        /// </summary>
        /// <value><c>true</c> if ticks are shown; otherwise, <c>false</c>.</value>
        [Category("Behavior"), Description("Shows Ticks if set to True"), DefaultValue(true)]
        public bool ShowTicks
        {
            get
            {
                return showTicks;
            }
            set
            {
                showTicks = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets the background color for the control. (overridden property)
        /// </summary>
        [Browsable(false), DefaultValue(typeof(Color), "Transparent")]
        public override Color BackColor
        {
            get { return Color.Transparent; }
        }

        /// <summary>
        /// Gets the range.
        /// </summary>
        /// <value>The range.</value>
        [Browsable(false)]
        public int Range
        {
            get
            {
                return sliderMax - sliderMin;
            }
        }

        /// <summary>
        /// Gets or sets the Focus of the Thumb.
        /// </summary>
        /// <value>The focus of the Thumb.</value>
        [Browsable(true), Category("Appearance"), Description("Thumb Focus"), DefaultValue(Thumb.Left)]
        public Thumb FocussedThumb
        {
            get { return focussedThumb; }
            set
            {
                focussedThumb = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to Focus Thumb.
        /// </summary>
        /// <value><c>true</c> if Focus Thumb enabled; otherwise, <c>false</c>.</value>
        [Category("Behavior"), Description("Set the Focus on Thumb"), DefaultValue(false)]
        public bool ShowThumbFocused
        {
            get
            {
                return showThumbFocus;
            }
            set
            {
                showThumbFocus = value;
            }
        }      

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the Value property of a RangerSlider changes, either by movement of the thumb or by manipulation in code.
        /// </summary>
        [Description("Occurs when the SliderMin and SliderMax property of a RangerSlider changes, either by movement of the thumb or by manipulation in code.")]
        public event EventHandler ValueChanged;

        /// <summary>
        /// Occurs when  a mouse  action moves the thumb. 
        /// </summary>
        [Description("Occurs when  a mouse action moves the thumb. ")]
        public event EventHandler Scroll;

        #endregion

        #region Methods
        
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
                        ApplyScaleToControl(1.5F);
                    else
                        ApplyScaleToControl(1);
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
            this.Height = (int)(CTRLSIZE.Height * scaleFactor);
            this.SliderSize = new Size((int)(SLIDERSIZE.Width * scaleFactor), (int)(SLIDERSIZE.Height * scaleFactor));
            this.ChannelHeight = (int)(CHNHEIGHT + (scaleFactor * 2));
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }

        /// <summary>
        /// Gets or sets the left slider position.
        /// </summary>
        /// <value>The left slider position.</value>
        protected int LeftSliderPosition
        {
            get { return leftSliderPosition; }
            set
            {
                leftSliderPosition = value;
            }
        }

        /// <summary>
        /// Gets or sets the right slider position.
        /// </summary>
        /// <value>The right slider position.</value>
        protected int RightSliderPosition
        {
            get { return rightSliderPosition; }
            set
            {
                rightSliderPosition = value;
            }
        }

        /// <summary>
        /// Gets the left slider bounds.
        /// </summary>
        /// <value>The left slider bounds.</value>
        protected virtual Rectangle LeftSliderBounds
        {
            get
            {
                Rectangle rcDisplay = DisplayRectangleInternal;
                Rectangle rcChannel = ChannelBounds;
                LeftSliderPosition = rcChannel.Width * (SliderMin - Minimum) / (Maximum - Minimum);
                int x = rcChannel.X - (SliderSize.Width) / 2 + (this.RightToLeft != RightToLeft.Yes ? LeftSliderPosition : rcChannel.Width - LeftSliderPosition);
                int y = rcDisplay.Y + (rcDisplay.Height - SliderSize.Height) / 2;
                return new Rectangle(new Point(x, y), SliderSize);
            }
        }

        /// <summary>
        /// Gets the right slider bounds.
        /// </summary>
        /// <value>The right slider bounds.</value>
        protected virtual Rectangle RightSliderBounds
        {
            get
            {
                Rectangle rcDisplay = DisplayRectangleInternal;
                Rectangle rcChannel = ChannelBounds;
                RightSliderPosition = rcChannel.Width * (SliderMax - Minimum) / (Maximum - Minimum);
                int x = rcChannel.X - SliderSize.Width / 2 + (this.RightToLeft != RightToLeft.Yes ? RightSliderPosition : rcChannel.Width - RightSliderPosition);
                int y = rcDisplay.Y + (rcDisplay.Height - SliderSize.Height) / 2;
                return new Rectangle(new Point(x, y), SliderSize);
            }
        }

        /// <summary>
        /// Gets the channel bounds.
        /// </summary>
        /// <value>The channel bounds.</value>
        protected virtual Rectangle ChannelBounds
        {
            get
            {
                Rectangle rcDisplay = this.DisplayRectangleInternal;
                int x = rcDisplay.X + sliderSize.Width / 2 + 18;
                int y = rcDisplay.Y + (rcDisplay.Height - channelHeight) / 2;
                int w = rcDisplay.Width - (36 + sliderSize.Width);
                return new Rectangle(x, y, w, channelHeight);
            }
        }

        /// <summary>
        /// Gets display rectangle of RangeSlider control. If orientation is vertical,
        /// rectangle will be the same as when orientation is horizontal.
        /// </summary>
        internal Rectangle DisplayRectangleInternal
        {
            get
            {
                Rectangle rcDisplay = this.DisplayRectangle;
                if (orientation == Orientation.Vertical)
                {
                    int iTemp = rcDisplay.Width;
                    rcDisplay.Width = rcDisplay.Height;
                    rcDisplay.Height = iTemp;
                }
                return rcDisplay;
            }
        }

        /// <summary>
        /// Gets the color of the thumb border.
        /// </summary>
        /// <value>The color of the thumb border.</value>
        protected virtual Color ThumbBorderColor
        {
            get
            {
                return Color.FromArgb(66, 109, 165);
            }
        }

        /// <summary>
        /// Gets the start color of the thumb.
        /// </summary>
        /// <value>The start color of the thumb.</value>
        protected virtual Color ThumbStartColor
        {
            get
            {
                return Color.FromArgb(248, 248, 255);
            }
        }

        /// <summary>
        /// Gets or sets the current area where Mouse pointer is.
        /// </summary>
        /// <value>The curent area.</value>
        protected RangeSliderArea CurrentArea
        {
            get
            {
                return currentArea;
            }
            set
            {
                if (currentArea != value)
                {
                    RangeSliderArea oldArea = currentArea;
                    currentArea = value;
                    InvalidateArea(oldArea);
                    InvalidateArea(currentArea);
                }
            }
        }

        /// <summary>
        /// Gets or sets the currently pushed item.
        /// </summary>
        /// <value>The pushed item.</value>
        protected RangeSliderArea PushedItem
        {
            get
            {
                return pushedItem;
            }
            set
            {
                if (pushedItem != value)
                {
                    RangeSliderArea oldArea = pushedItem;
                    pushedItem = value;
                    InvalidateArea(oldArea);
                    InvalidateArea(pushedItem);
                }
            }
        }

        /// <summary>
        /// Gets the inner channel.
        /// </summary>
        /// <value>The inner channel.</value>
        protected virtual Rectangle InnerChannel
        {
            get
            {
                int x = 0;
                int width = 0;
                if (this.RightToLeft == RightToLeft.No)
                {
                    x = LeftSliderBounds.Right;
                    width = RightSliderBounds.X - LeftSliderBounds.Right;
                }
                else if (this.RightToLeft == RightToLeft.Yes)
                {
                    x = RightSliderBounds.Right;
                    width = LeftSliderBounds.X - RightSliderBounds.Right;
                }
                return new Rectangle(x, ChannelBounds.Y, width, ChannelBounds.Height);
            }
        }

        /// <summary>
        /// Gets the right outer channel.
        /// </summary>
        /// <value>The right outer channel.</value>
        protected virtual Rectangle RightOuterChannel
        {
            get
            {
                int x = 0;
                int width = 0;
                if (this.RightToLeft == RightToLeft.No)
                {
                    x = RightSliderBounds.Right;
                    width = ChannelBounds.Right - RightSliderBounds.Right;
                }
                else if (this.RightToLeft == RightToLeft.Yes)
                {
                    x = LeftSliderBounds.Right;
                    width = ChannelBounds.Right - LeftSliderBounds.Right;
                }
                if (this.VisualStyle == RangeSliderStyle.Metro)
                    return new Rectangle(x, ChannelBounds.Y + 1, width, ChannelBounds.Height - 1);
                return new Rectangle(x, ChannelBounds.Y, width, ChannelBounds.Height);
            }
        }

        /// <summary>
        /// Gets the left outer channel.
        /// </summary>
        /// <value>The left outer channel.</value>
        protected virtual Rectangle LeftOuterChannel
        {
            get
            {
                int x = 0;
                int width = 0;
                if (this.RightToLeft == RightToLeft.No)
                {
                    x = ChannelBounds.X;
                    width = LeftSliderBounds.X - ChannelBounds.X;
                }
                else if (this.RightToLeft == RightToLeft.Yes)
                {
                    x = ChannelBounds.X;
                    width = RightSliderBounds.X - ChannelBounds.X;
                }
                 if (this.VisualStyle == RangeSliderStyle.Metro)
                    return new Rectangle(x, ChannelBounds.Y + 1, width, ChannelBounds.Height - 1);
                return new Rectangle(x, ChannelBounds.Y, width, ChannelBounds.Height);
            }
        }

        /// <summary>
        /// Draws the sliders.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        protected void DrawSlider(Graphics graphics)
        {
            Rectangle leftRect = this.LeftSliderBounds;
            Rectangle rightRect = this.RightSliderBounds;
            ItemState leftSliderState = ItemState.Default;

            bool leftFocus = false;
            bool rightFocus = false;

            if (this.currentArea == RangeSliderArea.LeftThumb)
            {
                if (this.PushedItem == RangeSliderArea.LeftThumb)
                {
                    leftSliderState = ItemState.Pushed;
                }
                else if (this.CurrentArea == RangeSliderArea.LeftThumb)
                {
                    leftSliderState = ItemState.Highlighted;
                }
            }

            ItemState rightSliderState = ItemState.Default;

            if (this.currentArea == RangeSliderArea.RightThumb)
            {
                if (this.PushedItem == RangeSliderArea.RightThumb)
                {
                    rightSliderState = ItemState.Pushed;
                }
                else if (this.CurrentArea == RangeSliderArea.RightThumb)
                {
                    rightSliderState = ItemState.Highlighted;
                }
            }

            if (ShowThumbFocused == true)
            {
                if (FocussedThumb == Thumb.Left)
                {
                    leftFocus = true;
                }
                else if (FocussedThumb == Thumb.Right)
                {
                    rightFocus = true;
                }
            }
            if (VisualStyle == RangeSliderStyle.Metro)
            {
                DrawMetroSliders(leftRect, graphics, leftFocus, true);
                DrawMetroSliders(rightRect, graphics, rightFocus, false);
            }
            else
            {
                DrawSliders(leftRect, graphics, leftSliderState, leftFocus);
                DrawSliders(rightRect, graphics, rightSliderState, rightFocus);
            }
        }

        /// <summary>
        /// Draw the metro slider.
        /// </summary>
        /// <param name="rect">Rectangle.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="leftFocus">LeftThumb.</param>
        protected void DrawMetroSliders(Rectangle rect, Graphics graphics, bool focus, bool leftThumb)
        {
            GraphicsState grState = graphics.Save();
            graphics.SetClip(rect);
            rect.Y = rect.Top + 1;
            if (leftThumb == true)
            {
                rect.X = rect.Left + 2;
                rect.Width = rect.Width + 2;
            }
            else
            {
                rect.X = rect.Left;
                rect.Width = rect.Width - 2;
            }
            SolidBrush brush = new SolidBrush(this.ThumbColor);
            graphics.FillRectangle(brush, rect);
            if (leftThumb == true)
            {
                brush.Dispose();

                graphics.SetClip(rect);
                rect.X = rect.Left + 2;
                rect.Width = 3;
                rect.Height = rect.Height - 2;
                brush = new SolidBrush(this.ThumbColor);
                graphics.FillRectangle(brush, rect);
            }
            brush.Dispose();
            graphics.Restore(grState);
        }
        /// <summary>
        /// Draw the slider.
        /// </summary>
        /// <param name="rect">Rectangle.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="sliderState">State of the slider.</param>
        private void DrawSliders(Rectangle rect, Graphics graphics, ItemState sliderState, bool focus)
        {
            GraphicsState grState = graphics.Save();
            graphics.SetClip(rect);

            if (rect.Width > 0 && rect.Height > 0)
            {
                using (GraphicsPath sliderPath = GetSliderPath(new Rectangle(new Point(rect.Left + 1, rect.Top + 1), new Size(rect.Width - 2, rect.Height - 3))))
                {
                    Color sliderEndColor = GetThumbEndColor(sliderState);

                    using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(rect.Location, new Size(1, rect.Height)), this.ThumbStartColor, sliderEndColor, LinearGradientMode.Vertical))
                    {
                        brush.Blend = _sliderBlend;
                        sliderPath.FillMode = FillMode.Winding;
                        graphics.FillPath(brush, sliderPath);
                    }
                }

                using (GraphicsPath borderPath =
                    GetSliderPath(new Rectangle(new Point(rect.Left + 1, rect.Top + 1), new Size(rect.Width - 2, rect.Height - 3))))
                {
                    using (Pen pen = new Pen(this.ThumbBorderColor))
                    {
                        graphics.DrawPath(pen, borderPath);

                        if (focus == true)
                        {
                            pen.Color = Color.Black;
                            pen.DashStyle = DashStyle.Dot;
                            graphics.DrawPath(pen, borderPath);
                        }
                    }
                }

                int middleLineTop = rect.Top + rect.Height * 3 / 10;
                int middleLineHeight = rect.Width * 5 / 10;
                int middle = rect.Left + rect.Width / 2;

                using (Pen pen = new Pen(Color.FromArgb(150, this.ThumbBorderColor)))
                {
                    graphics.DrawLine(pen, new Point(middle, middleLineTop), new Point(middle, middleLineTop + middleLineHeight));
                }

                using (Pen pen = new Pen(Color.FromArgb(150, this.ThumbStartColor)))
                {
                    graphics.DrawLine(
                        pen, new Point(middle + 1, middleLineTop + 1), new Point(middle + 1, middleLineTop + middleLineHeight));
                }
            }

            graphics.Restore(grState);
        }

        /// <summary>
        /// Draws the ticks.
        /// </summary>
        /// <param name="g">The graphics.</param>
        private void DrawTicks(Graphics g)
        {
            int noOfTicks = 2;
            int diff = maximum - minimum;
            if (tickFrequency != 0)
            {
                if (diff % tickFrequency == 0)
                {
                    noOfTicks = diff / tickFrequency + 1;
                }
                else
                {
                    noOfTicks = diff / tickFrequency + 2;
                }
            }

            int upX = ChannelBounds.Left;
            int upY = ChannelBounds.Bottom + 6;
            int downX = ChannelBounds.Left;
            int downY = upY + 3;
            int j = 1;
            int k = 1;
            if (tickFrequency == 1)
            {
                for (int i = 1; i <= noOfTicks; i++)
                {
                    using (Pen pen = new Pen(Color.Black))
                    {
                        g.DrawLine(pen, new Point(upX, upY), new Point(downX, downY));
                    }
                    upX = ChannelBounds.Left + ChannelBounds.Width * j / diff;
                    downX = upX;
                    j++;
                }
            }
            else
            {
                for (int i = 0; i < Maximum; i++)
                {
                    if (j == k && k <= Maximum)
                    {
                        using (Pen pen = new Pen(Color.Black))
                        {
                            g.DrawLine(pen, new Point(upX, upY), new Point(downX, downY));
                        }
                        k = k + tickFrequency;
                    }
                    upX = (this.RightToLeft == RightToLeft.No ? (ChannelBounds.Left + ChannelBounds.Width * j / diff) : (ChannelBounds.Left + (ChannelBounds.Width - (ChannelBounds.Width * j / diff))));
                    downX = upX;
                    j++;
                }
                using (Pen pen = new Pen(Color.Black))
                {
                    g.DrawLine(pen, new Point(ChannelBounds.Right, upY), new Point(ChannelBounds.Right, downY));
                }
            }
        }

        /// <summary>
        /// Gets the slider path.
        /// </summary>
        /// <param name="rect">Rectangle.</param>
        /// <returns>Path of the slider in the given rectangle</returns>
        private GraphicsPath GetSliderPath(Rectangle rect)
        {
            GraphicsPath sliderPath = new GraphicsPath();
            int topHeight = (rect.Height) * 7 / 10;
            Point leftMiddlePoint = new Point(rect.Left, rect.Top + topHeight);
            Point rightMiddlePoint = new Point(rect.Right, rect.Top + topHeight);
            Point bottomMiddlePoint = new Point(rect.Left + rect.Width / 2, rect.Bottom);
            sliderPath.AddLine(rect.Location, leftMiddlePoint);
            sliderPath.AddLine(leftMiddlePoint, bottomMiddlePoint);
            sliderPath.AddLine(bottomMiddlePoint, rightMiddlePoint);
            sliderPath.AddLine(rightMiddlePoint, new Point(rect.Right, rect.Top));
            sliderPath.CloseFigure();
            return sliderPath;
        }

        /// <summary>
        /// Gets the end color of the thumb.
        /// </summary>
        /// <param name="state"> state of the thumb.</param>
        /// <returns>end color of the thumb.</returns>
        private Color GetThumbEndColor(ItemState state)
        {
            Color result = Color.Empty;
            switch (state)
            {
                case ItemState.Default:
                    result = this.ThumbColor;
                    break;
                case ItemState.Highlighted:
                    result = this.HighlightedThumbColor;
                    break;
                case ItemState.Pushed:
                    result = this.PushedThumbColor;
                    break;
            }
            return result;
        }

        /// <summary>
        /// Paints the RangeSlider
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data.</param>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            Matrix matrix = e.Graphics.Transform;
            if (orientation == Orientation.Vertical)
            {
                e.Graphics.TranslateTransform(0, this.Bounds.Height);
                e.Graphics.RotateTransform(-90.0f);
            }
            if (this.VisualStyle != RangeSliderStyle.Metro)
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle channelBounds = this.ChannelBounds;
            if (channelBounds.Height > 0 && channelBounds.Width > 0)
            {
                using (SolidBrush outerChannelBrush = new SolidBrush(ChannelColor))
                {
                    e.Graphics.FillRectangle(outerChannelBrush, LeftOuterChannel);
                }
                using (SolidBrush innerChannelBrush = new SolidBrush(RangeColor))
                {
                    e.Graphics.FillRectangle(innerChannelBrush, InnerChannel);
                }
                using (SolidBrush outerChannelBrush = new SolidBrush(ChannelColor))
                {
                    e.Graphics.FillRectangle(outerChannelBrush, RightOuterChannel);
                }
            }

            DrawSlider(e.Graphics);

            if (ShowTicks)
            {
                DrawTicks(e.Graphics);
            }

            e.Graphics.Transform = matrix;
        }

        /// <summary>
        /// Processes Mouse Click 
        /// </summary>
        /// <param name="e">A MouseEventArgs that contains the event data.</param>
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Point ptMouseLocation = TranslateClientLocation(e.Location);
            this.Capture = true;

            if (e.Button == MouseButtons.Left)
            {
                Rectangle leftRect = this.LeftSliderBounds;
                Rectangle rightRect = this.RightSliderBounds;
                if (leftRect.Contains(ptMouseLocation))
                {
                    leftSliderMoving = true;
                    focussedThumb = Thumb.Left;
                    this.PushedItem = RangeSliderArea.LeftThumb;
                }
                else if (rightRect.Contains(ptMouseLocation))
                {
                    this.PushedItem = RangeSliderArea.RightThumb;
                    focussedThumb = Thumb.Right;
                    rightSliderMoving = true;
                }
                else
                {
                    this.PushedItem = RangeSliderArea.None;
                }
            }
        }

        /// <summary>
        /// Handles Mouse Moving
        /// </summary>
        /// <param name="e">A MouseEventArgs that contains the event data.</param>
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Point ptMouseLocation = TranslateClientLocation(e.Location);
            if (SliderMax == Minimum)
            {
                rightSliderMoving = true;
                leftSliderMoving = false;
            }
            if (leftSliderMoving)
            {
                Rectangle slider = this.LeftSliderBounds;
                float smallChangeInPix = (float)this.ChannelBounds.Width * 1 / (Maximum - Minimum);
                int offset = ptMouseLocation.X - (slider.Left + slider.Width / 2);
                int shift = (int)Math.Round(offset / smallChangeInPix) * 1;
                if (this.RightToLeft == RightToLeft.Yes)
                {
                    shift = -shift;
                }
                ScrollLeftSliderTo(SliderMin + shift);
                this.focussedThumb = Thumb.Left;
            }
            else if (rightSliderMoving)
            {
                Rectangle slider = this.RightSliderBounds;
                float smallChangeInPix = (float)this.ChannelBounds.Width * 1 / (Maximum - Minimum);
                int offset = ptMouseLocation.X - (slider.Left + slider.Width / 2);
                int shift = (int)Math.Round(offset / smallChangeInPix) * 1;
                if (this.RightToLeft == RightToLeft.Yes)
                {
                    shift = -shift;
                }
                ScrollRightSliderTo(SliderMax + shift);
                this.focussedThumb = Thumb.Right;
            }
            else
            {
                if (this.LeftSliderBounds.Contains(ptMouseLocation))
                {
                    this.CurrentArea = RangeSliderArea.LeftThumb;
                }
                else if (this.RightSliderBounds.Contains(ptMouseLocation))
                {
                    this.CurrentArea = RangeSliderArea.RightThumb;
                }
                else
                {
                    this.CurrentArea = RangeSliderArea.None;
                }
            }
        }
        /// <summary>
        ///Font changed
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }
        /// <summary></summary>
        /// <param name="e"/>
        protected override void OnSizeChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnSizeChanged(e);
            if (!EnableTouchMode && this.DesignMode)
            {
                CTRLSIZE = this.Size;
            }
        }
        /// <summary>
        /// Handles When Mouse is released
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            base.OnMouseCaptureChanged(e);
            leftSliderMoving = false;
            rightSliderMoving = false;
            this.PushedItem = RangeSliderArea.None;
            if (ShowThumbFocused == true)
                Invalidate();
        }

        /// <summary>
        /// Resets current Area
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.CurrentArea = RangeSliderArea.None;
        }

        /// <summary>
        /// Sets the SliderMin value and invalidate
        /// </summary>
        /// <param name="pos">position.</param>
        private void ScrollLeftSliderTo(int pos)
        {
            int value = GetValidValue(pos);
            if (SliderMin != value && value <= SliderMax)
            {
                InvalidateLeftSlider();
                SliderMin = value;
                InvalidateLeftSlider();
                Update();
                OnScroll();
                OnValueChanged();
            }
        }

        /// <summary>
        /// Sets the SliderMax value and invalidate
        /// </summary>
        /// <param name="pos">position.</param>
        private void ScrollRightSliderTo(int pos)
        {
            int value = GetValidValue(pos);
            if (SliderMax != value && value >= SliderMin)
            {
                InvalidateRightSlider();
                SliderMax = value;
                InvalidateRightSlider();
                Update();
                OnScroll();
                OnValueChanged();
            }
        }

        /// <summary>
        /// Sets the SliderMoveLeft and invalidate
        /// </summary>
        /// <param name="pos">position.</param>
        public void MoveThumb(Thumb thumb, int pos)
        {
            int value = pos;
            if (value <= SliderMax)
            {
                InvalidateLeftSlider();
                if (thumb == Thumb.Left)
                    SliderMin += value;
                else if (thumb == Thumb.Right)
                    SliderMax += value;
                InvalidateLeftSlider();
                Update();
                OnScroll();
                OnValueChanged();
            }
        }
       
        /// <summary>
        /// Invalidates the left slider.
        /// </summary>
        private void InvalidateLeftSlider()
        {
            if (this.IsHandleCreated)
            {
                Invalidate(TranslateClientRect(this.LeftSliderBounds));
                Invalidate(this.LeftOuterChannel);
                Invalidate(this.InnerChannel);
            }
        }

        /// <summary>
        /// Invalidates the right slider.
        /// </summary>
        private void InvalidateRightSlider()
        {
            if (this.IsHandleCreated)
            {
                Invalidate(TranslateClientRect(this.RightSliderBounds));
                Invalidate(this.RightOuterChannel);
                Invalidate(this.InnerChannel);
            }
        }

        private int GetValidValue(int value)
        {
            if (value < Minimum)
            {
                value = Minimum;
            }
            else if (value > Maximum)
            {
                value = Maximum;
            }
            return value;
        }

        /// <summary>
        /// Translates bounds of RangeSlider
        /// </summary>
        /// <param name="pBounds"> Bounds of Rectangle that are being modified. </param>
        /// <returns> Non-modified rectangle if orientation is horizontal,
        /// modified rectangle - otherwise. </returns>
        private Rectangle TranslateClientRect(Rectangle pBounds)
        {
            Rectangle rcTranslatedArea = pBounds;
            if (this.Orientation == Orientation.Vertical)
            {
                Rectangle rcTemp = rcTranslatedArea;
                Size szClient = this.ClientRectangle.Size;
                rcTranslatedArea.X = rcTemp.Y;
                rcTranslatedArea.Y = szClient.Height - rcTemp.Right;
                rcTranslatedArea.Width = rcTemp.Height;
                rcTranslatedArea.Height = rcTemp.Width;
            }
            return rcTranslatedArea;
        }

        /// <summary>
        /// Modifies given mouse location according to RangeSlider orientation.
        /// </summary>
        /// <param name="pMouseLocation"> Mouse location which is to be modified. </param>
        /// <returns> Non-modified mouse location if orientation is horizontal,
        /// modified mouse location - otherwise. </returns>
        private Point TranslateClientLocation(Point pMouseLocation)
        {
            Point ptMouseLocation = pMouseLocation;

            if (this.Orientation == Orientation.Vertical)
            {
                Point ptTemp = ptMouseLocation;
                Size szDisplay = this.ClientRectangle.Size;
                ptMouseLocation.X = szDisplay.Height - ptTemp.Y;
                ptMouseLocation.Y = ptTemp.X;
            }

            return ptMouseLocation;
        }

        /// <summary>
        /// Invalidates given area.
        /// </summary>
        /// <param name="area">The area to be invalidated.</param>
        private void InvalidateArea(RangeSliderArea area)
        {
            switch (area)
            {
                case RangeSliderArea.LeftThumb:
                    Invalidate(TranslateClientRect(this.LeftSliderBounds));
                    break;
                case RangeSliderArea.RightThumb:
                    Invalidate(TranslateClientRect(this.RightSliderBounds));
                    break;
            }
        }

        private void OnScroll()
        {
            if (this.Scroll != null)
            {
                this.Scroll(this, EventArgs.Empty);
            }
        }

        private void OnValueChanged()
        {
            if (ValueChanged != null)
            {
                this.ValueChanged(this, EventArgs.Empty);
            }
        }

        #region ShouldSerialize & Reset methods

        protected bool ShouldSerializePushedThumbColor()
        {
            return this.pushedThumbColor != Color.FromArgb(255, 138, 24);
        }

        protected void ResetPushedThumbColor()
        {
            this.PushedThumbColor = Color.FromArgb(255, 138, 24);
        }

        protected bool ShouldSerializeHighlightedThumbColor()
        {
            return this.highlightedThumbColor != Color.FromArgb(247, 199, 82);
        }

        protected void ResetHighlightedThumbColor()
        {
            this.HighlightedThumbColor = Color.FromArgb(247, 199, 82);
        }

        #endregion

        /// <summary>
        /// Makes AutoSize property visible in property grid.
        /// </summary>
        [Category("Behavior")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DefaultValue(true)]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                if (base.AutoSize != value)
                {
                    base.AutoSize = value;

                    if (value)
                    {
                        height = (Orientation == Orientation.Horizontal) ? this.Height : this.Width;

                        if (Orientation == Orientation.Horizontal)
                            this.Height = 22;
                        else
                            this.Width = 22;
                    }
                    else
                    {
                        if (Orientation == Orientation.Horizontal)
                            this.Height = height;
                        else
                            this.Width = height;
                    }
                }
            }
        }

        /// <summary>
        /// Sets height on AutoSize.
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y Position</param>
        /// <param name="width">Bounds  Width</param>
        /// <param name="height">Bounds Height</param>
        /// <param name="specified"> Bounds Speified</param>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (this.AutoSize)
            {
                if (Orientation == Orientation.Horizontal)
                    height = 22;
                else
                    width = 22;
            }

            base.SetBoundsCore(x, y, width, height, specified);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        ///<param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }   
}

