#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Tools.Design;
using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// TrackBar control extended by decrease and increase buttons.
    /// </summary>
    [Designer(typeof(TrackBarExDesigner))]
    [TypeConverter(typeof(TrackBarExConverter))]
    [Description("TrackBar control extended with increase and decrease buttons.")]
    [ToolboxBitmap(typeof(TrackBarEx), "ToolboxIcons.TrackBarEx.bmp")]
    public class TrackBarEx : Control
    {
        #region Internal Classes
        /// <summary>
        /// Different areas of the control.
        /// </summary>
        protected enum TrackBarExArea
        {
            /// <summary>
            /// Out of control.
            /// </summary>
            None,

            /// <summary>
            /// Decrease button.
            /// </summary>
            DecreaseButton,

            /// <summary>
            /// Increase button.
            /// </summary>
            IncreaseButton,

            /// <summary>
            /// Channel on the left of the slider.
            /// </summary>
            LargeDecrease,

            /// <summary>
            /// Channel on the right of the slider.
            /// </summary>
            LargeIncrease,

            /// <summary>
            /// Represents Slider.
            /// </summary>
            Slider
        }

        /// <summary>
        /// Different states of control items (buttons and slader).
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

        public enum Theme
        {
            Default,
            Metro
        }
        #endregion

        #region Constants
        /// <summary>
        /// Height of channel.
        /// </summary>
        private const int CHANNEL_HEIGHT = 4;
		/// <summary>
		///Backcolor value
		/// </summary>
        private Color backColor = Color.Transparent;
        /// <summary>
        /// Size of decrease button.
        /// </summary>
        private static readonly Size DECR_BUTTON_SIZE = new Size(18, 18);

        /// <summary>
        /// Size of increase button.
        /// </summary>
        private static readonly Size INCR_BUTTON_SIZE = new Size(18, 18);

        /// <summary>
        /// Minimal height of TrackBar.
        /// </summary>
        private const int MIN_HEIGHT = 20;

        /// <summary>
        /// Initial width of TrackBar.
        /// </summary>
        private const int INIT_WIDTH = 250;

        /// <summary>
        /// Large change of TrackBar value.
        /// </summary>
        private const int LARGE_CHANGE = 5;

        /// <summary>
        /// Maximum value of TrackBar.
        /// </summary>
        private const int MAXIMUM = 10;

        /// <summary>
        /// Minimum value of TrackBar.
        /// </summary>
        private const int MINIMUM = 0;

        /// <summary>
        /// Small change of TrackBar value.
        /// </summary>
        private const int SMALL_CHANGE = 1;

        /// <summary>
        /// Size of slider.
        /// </summary>
        private static readonly Size SLIDER_SIZE = new Size(11, 14);

        /// <summary>
        /// Interval for timer.
        /// </summary>
        private const int TIMER_INT = 100;

        /// <summary>
        /// Default start color of trackbar gradient.
        /// </summary>
        private static readonly Color TRACK_BAR_GRADIENT_START = Color.FromArgb(198, 222, 254);

        /// <summary>
        /// Default end color of trackbar gradient.
        /// </summary>
        private static readonly Color TRACK_BAR_GRADIENT_END = Color.FromArgb(115, 150, 198);

        /// <summary>
        /// Default color of buttons.
        /// </summary>
        private static readonly Color BUTTON_COLOR = Color.FromArgb(159, 191, 239);

        /// <summary>
        /// foreColorvalue
        /// </summary>
        private Color forecolor = ColorTranslator.FromHtml("#16A5DC");
        /// <summary>
        /// Default color of highlighted buttons.
        /// </summary>
        private static readonly Color HIGHLIGHTED_BUTTON_COLOR = Color.FromArgb(247, 199, 82);

        /// <summary>
        /// Default color of pushed buttons.
        /// </summary>
        private static readonly Color PUSHED_BUTTON_COLOR = Color.FromArgb(255, 138, 24);
      
        private static readonly ControlStyles PAINT_STYLES =
            ControlStyles.SupportsTransparentBackColor |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

        /// <summary>

        /// Default Slider size
        /// </summary>
        private static Size SLIDERSIZE = default(Size);
        /// <summary>
        /// Default channel height
        /// </summary>
        private static int CHANNELHEIGHT = default(int);
        /// <summary>
        /// Default button size
        /// </summary>
        private static Size BUTTONSIZE = default(Size);
        #endregion

        #region Fields
        /// <summary>
        /// Indicates whether buttons should be shown.
        /// </summary>
        private bool m_bShowButtons = true;

        /// <summary>
        /// Height of channel.
        /// </summary>
        private int m_channelHeight = CHANNEL_HEIGHT;

        /// <summary>
        /// Size of decrease button.
        /// </summary>
        private Size m_decrButtonSize = DECR_BUTTON_SIZE;

        /// <summary>
        /// Size of increase button.
        /// </summary>
        private Size m_incrButtonSize = INCR_BUTTON_SIZE;

        /// <summary>
        /// Stores custom height when AutoSize set to true.
        /// </summary>
        private int m_setHeight;

        /// <summary>
        /// Large change of TrackBar value.
        /// </summary>
        private int m_largeChange = LARGE_CHANGE;

        /// <summary>
        /// Maximum value of TrackBar.
        /// </summary>
        private int m_maximum = MAXIMUM;

        /// <summary>
        /// Minimum value of TrackBar.
        /// </summary>
        private int m_minimum = MINIMUM;

        /// <summary>
        /// Small change of TrackBar value.
        /// </summary>
        private int m_smallChange = SMALL_CHANGE;

        /// <summary>
        /// Value of TrackBar position.
        /// </summary>
        private int m_value;

        /// <summary>
        /// Size of slider.
        /// </summary>
        private Size m_sliderSize = SLIDER_SIZE;

        /// <summary>
        /// Timer for for handling mouse keeping pushed.
        /// </summary>
        private Timer m_timer;

        /// <summary>
        /// Indicates whether slider is being moved with mouse.
        /// </summary>
        private bool m_bSliderMoving;

        /// <summary>
        /// Area where mouse pointer is currently situated.
        /// </summary>
        private TrackBarExArea m_curArea;

        /// <summary>
        /// Currently pushed item.
        /// </summary>
        private TrackBarExArea m_pushedItem;

        /// <summary>
        /// Indicates whether control background should be transparent.
        /// </summary>
        private bool m_bTransparent;

        /// <summary>
        /// Interval for timer.
        /// </summary>
        private int m_timerInt = TIMER_INT;

        /// <summary>
        /// Indicates whether focus rect should be shown.
        /// </summary>
        private bool m_bShowFocusRect = true;

        /// <summary>
        /// Start color of trackbar gradient.
        /// </summary>
        private Color m_clrTrackBarGradientStart = TRACK_BAR_GRADIENT_START;

        /// <summary>
        /// End color of trackbar gradient.
        /// </summary>
        private Color m_clrTrackBarGradientEnd = TRACK_BAR_GRADIENT_END;

        /// <summary>
        /// Color of button.
        /// </summary>
        private Color m_clrButton = BUTTON_COLOR;

        /// <summary>
        /// Color of highlighted button.
        /// </summary>
        private Color m_clrHighlightedButton = HIGHLIGHTED_BUTTON_COLOR;

        /// <summary>
        /// Color of pushed button.
        /// </summary>
        private Color m_clrPushedButton = PUSHED_BUTTON_COLOR;

        /// <summary>
        /// Specify the orientation of TrackBarEx control.
        /// </summary>
        private Orientation m_orientation = Orientation.Horizontal;
        /// <summary>
        ///Scaling value
        /// </summary>
        private bool isScaling = false;
        /// <summary>
        ///Scalefactor value
        /// </summary>
        private float _scalefactor = 1f;
        #endregion

        #region Static Fields
        /// <summary>
        /// Blend for slider.
        /// </summary>
        private static Blend _sliderBlend;

        /// <summary>
        /// Blend for buttons.
        /// </summary>
        private static Blend _buttonsBlend;
        #endregion

        #region Properties
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
            this.ChannelHeight = (int)(CHANNELHEIGHT + (scaleFactor * 2));
            this.IncreaseButtonSize = this.DecreaseButtonSize = new Size((int)(BUTTONSIZE.Width + (scaleFactor * 2)), (int)(BUTTONSIZE.Height + (scaleFactor * 2)));
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }

        /// <summary>
        /// Gets or sets a value indicating whether buttons should be shown.
        /// </summary>
        [Category("Appearance")]
        [Description("Indicates whether buttons should be shown.")]
        [DefaultValue(true)]
        public bool ShowButtons
        {
            get
            {
                return m_bShowButtons;
            }
            set
            {
                if (m_bShowButtons != value)
                {
                    m_bShowButtons = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets height of channel.
        /// </summary>
        [Category("Appearance")]
        [Description("Height of channel.")]
        [DefaultValue(4)]
        public int ChannelHeight
        {
            get
            {
                return m_channelHeight;
            }
            set
            {
                if (m_channelHeight != value)
                {
                    m_channelHeight = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets size of decrease button.
        /// </summary>
        [Category("Appearance")]
        [Description("Size of decrease button.")]
        public Size DecreaseButtonSize
        {
            get
            {
                Size value = this.DecreaseButtonSizeInternal;

                if (this.Orientation == Orientation.Vertical)
                {
                    value = new Size(value.Height, value.Width);
                }
                return value;
            }
            set
            {
                if (this.Orientation == Orientation.Vertical)
                {
                    value = new Size(value.Height, value.Width);
                }
                this.DecreaseButtonSizeInternal = value;
            }
        }

        /// <summary>
        /// Gets or sets size of increase button.
        /// </summary>
        [Category("Appearance")]
        [Description("Size of increase button.")]
        public Size IncreaseButtonSize
        {
            get
            {
                Size value = this.IncreaseButtonSizeInternal;

                if (this.Orientation == Orientation.Vertical)
                {
                    value = new Size(value.Height, value.Width);
                }
                return value;
            }
            set
            {
                if (this.Orientation == Orientation.Vertical)
                {
                    value = new Size(value.Height, value.Width);
                }
                this.IncreaseButtonSizeInternal = value;
            }
        }

        /// <summary>
        /// Gets or sets large change of TrackBar value.
        /// </summary>
        [Category("Behavior")]
        [Description("Large change of TrackBar value.")]
        [DefaultValue(5)]
        public int LargeChange
        {
            get
            {
                return m_largeChange;
            }
            set
            {
                m_largeChange = value;
            }
        }

        /// <summary>
        /// Gets or sets small change of TrackBar value.
        /// </summary>
        [Category("Behavior")]
        [Description("Small change of TrackBar value.")]
        [DefaultValue(1)]
        public int SmallChange
        {
            get
            {
                return m_smallChange;
            }
            set
            {
                m_smallChange = value;
            }
        }

        /// <summary>
        /// Gets or sets minimum value of TrackBar.
        /// </summary>
        [Category("Behavior"), Description("Minimum value of TrackBar.")]
        [DefaultValue(0), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Minimum
        {
            get
            {
                return m_minimum;
            }
            set
            {
                if (m_minimum != value)
                {
                    if (value < m_maximum)
                    {
                        m_minimum = value;
                    }
                    else
                    {
                        m_minimum = m_maximum - 1;
                    }

                    if (m_value < m_minimum)
                    {
                        this.Value = m_minimum;
                    }

                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets maximum value of TrackBar.
        /// </summary>
        [Category("Behavior"), Description("Maximum value of TrackBar.")]
        [DefaultValue(10), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Maximum
        {
            get
            {
                return m_maximum;
            }
            set
            {
                if (m_maximum != value)
                {
                    if (value > m_minimum)
                    {
                        m_maximum = value;
                    }
                    else
                    {
                        m_maximum = m_minimum + 1;
                    }

                    if (m_value > m_maximum)
                    {
                        this.Value = m_maximum;
                    }

                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets value of TrackBar position.
        /// </summary>
        [Category("Behavior")]
        [Description("Value of TrackBar position.")]
        public int Value
        {
            get
            {
                return m_value;
            }
            set
            {
                value = GetValidValue(value);

                if (m_value != value)
                {
                    InvalidateSlider();

                    m_value = value;

                    InvalidateSlider();

                    Update();

                    OnValueChanged();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets size of slider.
        /// </summary>
        [Category("Appearance")]
        [Description("Size of slider.")]
        public Size SliderSize
        {
            get
            {
                return m_sliderSize;
            }
            set
            {
                if (m_sliderSize != value)
                {
                    m_sliderSize = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether control background should be transparent.
        /// </summary>
        [Category("Appearance")]
        [Description("Indicates whether control background should be transparent.")]
        [DefaultValue(false)]
        public bool Transparent
        {
            get
            {
                return m_bTransparent;
            }
            set
            {
                if (m_bTransparent != value)
                {
                    m_bTransparent = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets interval for timer.
        /// </summary>
        [Category("Behavior")]
        [Description("Interval for timer.")]
        [DefaultValue(200)]
        public int TimerInterval
        {
            get
            {
                return m_timerInt;
            }
            set
            {
                m_timerInt = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether focus rect should be shown.
        /// </summary>
        [Category("Appearance")]
        [Description("Indicates whether focus rect should be shown.")]
        [DefaultValue(true)]
        public bool ShowFocusRect
        {
            get
            {
                return m_bShowFocusRect;
            }
            set
            {
                if (m_bShowFocusRect != value)
                {
                    m_bShowFocusRect = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets start color of trackbar gradient.
        /// </summary>
        [Category("Appearance")]
        [Description("Start color of trackbar gradient.")]
        public Color TrackBarGradientStart
        {
            get
            {
                return m_clrTrackBarGradientStart;
            }
            set
            {
                if (m_clrTrackBarGradientStart != value)
                {
                    m_clrTrackBarGradientStart = value;

                    if (!m_bTransparent)
                    {
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets end color of trackbar gradient.
        /// </summary>
        [Category("Appearance")]
        [Description("End color of trackbar gradient.")]
        public Color TrackBarGradientEnd
        {
            get
            {
                return m_clrTrackBarGradientEnd;
            }
            set
            {
                if (m_clrTrackBarGradientEnd != value)
                {
                    m_clrTrackBarGradientEnd = value;

                    if (!m_bTransparent)
                    {
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets color of the buttons.
        /// </summary>
        [Category("Appearance")]
        [Description("Color of the buttons.")]
        public Color ButtonColor
        {
            get
            {
                return m_clrButton;
            }
            set
            {
                if (m_clrButton != value)
                {
                    m_clrButton = value;

                    InvalidateArea(TrackBarExArea.DecreaseButton);
                    InvalidateArea(TrackBarExArea.IncreaseButton);
                    InvalidateArea(TrackBarExArea.Slider);
                }
            }
        }

        /// <summary>
        /// Gets or sets color of highlighted buttons.
        /// </summary>
        [Category("Appearance")]
        [Description("Color of highlighted buttons.")]
        public Color HighlightedButtonColor
        {
            get
            {
                return m_clrHighlightedButton;
            }
            set
            {
                if (m_clrHighlightedButton != value)
                {
                    m_clrHighlightedButton = value;

                    InvalidateArea(TrackBarExArea.DecreaseButton);
                    InvalidateArea(TrackBarExArea.IncreaseButton);
                    InvalidateArea(TrackBarExArea.Slider);
                }
            }
        }

        /// <summary>
        /// Gets or sets color of pushed buttons.
        /// </summary>
        [Category("Appearance")]
        [Description("Color of pushed buttons.")]
        public Color PushedButtonEndColor
        {
            get
            {
                return m_clrPushedButton;
            }
            set
            {
                if (m_clrPushedButton != value)
                {
                    m_clrPushedButton = value;

                    InvalidateArea(TrackBarExArea.DecreaseButton);
                    InvalidateArea(TrackBarExArea.IncreaseButton);
                    InvalidateArea(TrackBarExArea.Slider);
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color for the control. (overridden property)
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor
        {
            get { return backColor; }
            set { base.BackColor = value; }
        }
		/// <summary>
		///Overrides ForeColor.
		/// </summary>
        public override Color ForeColor
        {
            get
            {
                return forecolor;
            }
            set
            {
                forecolor = value;
            }
        }
        /// <summary>
        /// Gets or sets the orientation of TrackBarEx control.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(typeof(Orientation), "Horizontal")]
        [Description("Specify the orientation of TrackBarEx control.")]
        public Orientation Orientation
        {
            get
            {
                return m_orientation;
            }
            set
            {
                if (m_orientation != value)
                {
                    m_orientation = value;

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
        /// Gets display rectangle of TrackBarEx control. If orientation is vertical,
        /// rectangle will be the same as when orientation is horizontal.
        /// </summary>
        internal Rectangle DisplayRectangleInternal
        {
            get
            {
                Rectangle rcDisplay = this.DisplayRectangle;

                if (m_orientation == Orientation.Vertical)
                {
                    int iTemp = rcDisplay.Width;
                    rcDisplay.Width = rcDisplay.Height;
                    rcDisplay.Height = iTemp;
                }

                return rcDisplay;
            }
        }
      
        internal Size DecreaseButtonSizeInternal
        {
            get
            {
                return m_decrButtonSize;
            }
            set
            {
                if (m_decrButtonSize != value)
                {
                    m_decrButtonSize = value;
                    Invalidate();
                }
            }
        }
    
        internal Size IncreaseButtonSizeInternal
        {
            get
            {
                return m_incrButtonSize;
            }
            set
            {
                if (m_incrButtonSize != value)
                {
                    m_incrButtonSize = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Protected Properties
        /// <summary>
        /// Gets color for painting buttons border.
        /// </summary>
        protected virtual Color ButtonBorderColor
        {
            get
            {
                return Color.FromArgb(66, 109, 165);
            }
        }

        /// <summary>
        /// Gets start color for buttons gradient painting.
        /// </summary>
        protected virtual Color ButtonStartColor
        {
            get
            {
                if (Enabled)
                    return Color.FromArgb(248, 248, 255);
                else
                    return Color.FromArgb(150, Color.FromArgb(248, 248, 255));

            }
        }

        /// <summary>
        /// Gets start color for highlighted buttons gradient painting.
        /// </summary>
        protected virtual Color ButtonHighlightedStartColor
        {
            get
            {
                if (Enabled)
                    return Color.FromArgb(248, 248, 255);
                else
                    return Color.FromArgb(100, Color.FromArgb(248, 248, 255));
            }
        }

        /// <summary>
        /// Gets color of buttons signs.
        /// </summary>
        protected virtual Color ButtonSignColor
        {
            get
            {
                if (Enabled)
                    return Color.FromArgb(78, 97, 115);
                else
                    return Color.FromArgb(100, Color.FromArgb(78, 97, 115));
            }
        }

        /// <summary>
        /// Gets bounds of decrease button.
        /// </summary>
        protected virtual Rectangle DecreaseButtonBounds
        {
            get
            {
                Rectangle rcDisplay = this.DisplayRectangleInternal;

                int width = m_decrButtonSize.Width;
                int height = m_decrButtonSize.Height;

                int x = this.RightToLeft != RightToLeft.Yes ? rcDisplay.X : rcDisplay.Right - width;
                int y = rcDisplay.Y + (rcDisplay.Height - height) / 2;

                return new Rectangle(x, y, width, height);
            }
        }

        /// <summary>
        /// Gets bounds of increase button.
        /// </summary>
        protected virtual Rectangle IncreaseButtonBounds
        {
            get
            {
                Rectangle rcDisplay = this.DisplayRectangleInternal;

                int width = m_incrButtonSize.Width;
                int height = m_incrButtonSize.Height;

                int x = this.RightToLeft != RightToLeft.Yes ? rcDisplay.Right - width : rcDisplay.X;
                int y = rcDisplay.Y + (rcDisplay.Height - height) / 2;

                return new Rectangle(x, y, width, height);
            }
        }

        /// <summary>
        /// Gets bounds of channel.
        /// </summary>
        protected virtual Rectangle ChannelBounds
        {
            get
            {
                Rectangle rcDisplay = this.DisplayRectangleInternal;

                int x = rcDisplay.X + m_sliderSize.Width / 2 + (this.RightToLeft != RightToLeft.Yes ? m_decrButtonSize.Width : m_incrButtonSize.Width);
                int y = rcDisplay.Y + (rcDisplay.Height - m_channelHeight) / 2;
                int w = rcDisplay.Width - (m_decrButtonSize.Width + m_incrButtonSize.Width + m_sliderSize.Width);

                return new Rectangle(x, y, w, m_channelHeight);
            }
        }

        /// <summary>
        /// Gets bounds of slider.
        /// </summary>
        protected virtual Rectangle SliderBounds
        {
            get
            {
                Rectangle rcDisplay = this.DisplayRectangleInternal;
                Rectangle rcChannel = this.ChannelBounds;

                int pos = rcChannel.Width * (m_value - m_minimum) / (m_maximum - m_minimum);

                int x = rcChannel.X - m_sliderSize.Width / 2 + (this.RightToLeft != RightToLeft.Yes ? pos : rcChannel.Width - pos);
                int y = rcDisplay.Y + (rcDisplay.Height - m_sliderSize.Height) / 2;

                return new Rectangle(new Point(x, y), m_sliderSize);
            }
        }

        /// <summary>
        /// Gets inflated slider bounds.
        /// </summary>
        protected virtual Rectangle SliderActionBounds
        {
            get
            {
                Rectangle result = this.SliderBounds;
                result.Inflate(2, 1);
                return result;
            }
        }

        /// <summary>
        /// Gets or sets area where mouse pointer is currently situated.
        /// </summary>
        protected TrackBarExArea CurrentArea
        {
            get
            {
                return m_curArea;
            }
            set
            {
                if (m_curArea != value)
                {
                    TrackBarExArea oldArea = m_curArea;
                    m_curArea = value;

                    InvalidateArea(oldArea);
                    InvalidateArea(m_curArea);
                }
            }
        }

        /// <summary>
        /// Gets or sets currently pushed item.
        /// </summary>
        protected TrackBarExArea PushedItem
        {
            get
            {
                return m_pushedItem;
            }
            set
            {
                if (m_pushedItem != value)
                {
                    TrackBarExArea oldArea = m_pushedItem;
                    m_pushedItem = value;

                    InvalidateArea(oldArea);
                    InvalidateArea(m_pushedItem);
                }
            }
        }
        #endregion

        #region Initialization
      
        static TrackBarEx()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(TrackBarEx));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            _sliderBlend = new Blend();
            _sliderBlend.Factors = new float[] { 0.0F, 0.0F, 1.0F, 0.0F, 0.0F };
            _sliderBlend.Positions = new float[] { 0.0F, 0.15F, 0.35F, 0.8F, 1.0F };

            _buttonsBlend = new Blend();
            _buttonsBlend.Factors = new float[] { 0.0F, 0.0F, 1.0F, 0.0F, 0.0F };
            _buttonsBlend.Positions = new float[] { 0.0F, 0.2F, 0.5F, 0.8F, 1.0F };
        }

        /// <summary>
        /// Initializes a new instance of the TrackBarEx class.
        /// </summary>
        public TrackBarEx()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(TrackBarEx));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            this.Width = INIT_WIDTH;
            this.AutoSize = true;
            m_setHeight = (m_orientation == Orientation.Horizontal) ? this.Height : this.Width;

            m_value = (m_minimum + m_maximum) / 2;

            m_timer = new Timer();
            m_timer.Tick += new EventHandler(OnTimerTick);

            this.SetStyle(PAINT_STYLES, true);
            this.BackColor = Color.Transparent;
            CTRLSIZE = this.Size;
            SLIDERSIZE = this.SliderSize;
            CHANNELHEIGHT = this.ChannelHeight;
            BUTTONSIZE = this.IncreaseButtonSize;
        }

        public TrackBarEx(int min, int max)
            : this()
        {
            m_minimum = min;
            m_maximum = max;
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the Value property of a track bar changes, either by movement of the scroll box or by manipulation in code.
        /// </summary>
        [Description("Occurs when the Value property of a track bar changes, either by movement of the scroll box or by manipulation in code.")]
        public event EventHandler ValueChanged;

        /// <summary>
        /// Occurs when either a mouse or keyboard action moves the scroll box. 
        /// </summary>
        [Description("Occurs when either a mouse or keyboard action moves the scroll box. ")]
        public event EventHandler Scroll;
        #endregion

        #region Public Methods
        /// <summary>
        /// Decreases value by small change.
        /// </summary>
        public void SmallDecrease()
        {
            ScrollTo(m_value - m_smallChange);
        }

        /// <summary>
        /// Decreases value by large change.
        /// </summary>
        public void LargeDecrease()
        {
            ScrollTo(m_value - m_largeChange);
        }

        /// <summary>
        /// Increases value by small change.
        /// </summary>
        public void SmallIncrease()
        {
            ScrollTo(m_value + m_smallChange);
        }

        /// <summary>
        /// Increases value by large change.
        /// </summary>
        public void LargeIncrease()
        {
            ScrollTo(m_value + m_largeChange);
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Draws buttons.
        /// </summary>
        /// <param name="graphics">Graphics object to draw to.</param>
        protected virtual void DrawButtons(Graphics graphics)
        {
            Rectangle decrButtonRect = this.DecreaseButtonBounds;
            Rectangle incrButtonRect = this.IncreaseButtonBounds;

            ItemState decrButtonState = ItemState.Default;
            if (this.CurrentArea == TrackBarExArea.DecreaseButton)
            {
                if (this.PushedItem == TrackBarExArea.DecreaseButton)
                {
                    decrButtonState = ItemState.Pushed;
                }
                else if (this.PushedItem == TrackBarExArea.None)
                {
                    decrButtonState = ItemState.Highlighted;
                }
            }

            ItemState incrButtonState = ItemState.Default;
            if (this.CurrentArea == TrackBarExArea.IncreaseButton)
            {
                if (this.PushedItem == TrackBarExArea.IncreaseButton)
                {
                    incrButtonState = ItemState.Pushed;
                }
                else if (this.PushedItem == TrackBarExArea.None)
                {
                    incrButtonState = ItemState.Highlighted;
                }
            }

            DrawButtonBackground(graphics, decrButtonRect, decrButtonState);
            DrawButtonBackground(graphics, incrButtonRect, incrButtonState);

            using (Pen signPen = new Pen(this.ButtonSignColor))
            {
                using (Pen trSignPen = new Pen(Color.FromArgb(110, this.ButtonSignColor), 3))
                {
                    using (Pen lightPen = new Pen(this.ButtonStartColor, 3))
                    {
                        int signPortion = 4;

                        int width = decrButtonRect.Width * signPortion / 10;
                        int marg = (decrButtonRect.Width - width) / 2;
                        int loc = decrButtonRect.Top + decrButtonRect.Height / 2;

                        // Newly Added
                        if (m_orientation == Orientation.Horizontal)
                        {
                            graphics.DrawLine(lightPen, decrButtonRect.Left + marg - 1, loc, decrButtonRect.Right - marg + 1, loc);
                            graphics.DrawLine(trSignPen, decrButtonRect.Left + marg, loc, decrButtonRect.Right - marg, loc);
                            graphics.DrawLine(signPen, decrButtonRect.Left + marg + 1, loc, decrButtonRect.Right - marg - 1, loc);
                        }
                        else if (m_orientation == Orientation.Vertical)
                        {
                            graphics.DrawLine(lightPen, loc, decrButtonRect.Top + marg, loc, decrButtonRect.Bottom - marg);
                            graphics.DrawLine(trSignPen, loc, decrButtonRect.Top + marg, loc, decrButtonRect.Bottom - marg);
                            graphics.DrawLine(signPen, loc, decrButtonRect.Top + marg, loc, decrButtonRect.Bottom - marg);
                        }
                        width = incrButtonRect.Width * signPortion / 10;
                        marg = (incrButtonRect.Width - width) / 2;
                        loc = incrButtonRect.Top + incrButtonRect.Height / 2;
                        graphics.DrawLine(lightPen, incrButtonRect.Left + marg - 1, loc, incrButtonRect.Right - marg + 1, loc);
                        graphics.DrawLine(trSignPen, incrButtonRect.Left + marg, loc, incrButtonRect.Right - marg, loc);
                        graphics.DrawLine(signPen, incrButtonRect.Left + marg, loc, incrButtonRect.Right - marg, loc);

                        int height = incrButtonRect.Height * signPortion / 10;
                        marg = (incrButtonRect.Height - height) / 2;
                        loc = incrButtonRect.Left + incrButtonRect.Width / 2;
                        graphics.DrawLine(trSignPen, loc, incrButtonRect.Top + marg, loc, incrButtonRect.Bottom - marg);
                        graphics.DrawLine(signPen, loc, incrButtonRect.Top + marg, loc, incrButtonRect.Bottom - marg);
                    }
                }
            }
        }
        protected void DrawMetroSlider(Graphics graphics)
        {
            Rectangle rect = this.SliderBounds;

            GraphicsState grState = graphics.Save();
            graphics.SetClip(rect);
            rect.Y = rect.Y - 2;
            rect.Height = rect.Height +4;
            rect.X = rect.X + 2;
            rect.Width = rect.Width -5;
            SolidBrush brush = new SolidBrush (this.ForeColor );
            graphics.FillRectangle(brush, rect);
            Pen pen = new Pen(ControlPaint.Light(this.ForeColor),1);
            graphics.DrawRectangle(pen, rect.X +0.01F, rect.Y + 2, rect.Width - 1, rect.Height - 5);
            rect.X = rect.X + rect.Width;
            rect.Width = 2;
            brush = new SolidBrush(Color.White );
            graphics.FillRectangle(brush, rect);
            brush.Dispose();
            pen.Dispose();
        }

        /// <summary>
        /// Draws slider.
        /// </summary>
        /// <param name="graphics">Graphics object to draw to.</param>
        protected virtual void DrawSlider(Graphics graphics)
        {
            Rectangle rect = this.SliderBounds;

            GraphicsState grState = graphics.Save();
            graphics.SetClip(rect);

            if (rect.Width > 0 && rect.Height > 0)
            {
                using (GraphicsPath sliderPath = GetSliderPath(rect))
                {
                    ItemState sliderState = ItemState.Default;
                    if (this.PushedItem == TrackBarExArea.Slider)
                    {
                        sliderState = ItemState.Pushed;
                    }
                    else if (this.CurrentArea == TrackBarExArea.Slider)
                    {
                        sliderState = ItemState.Highlighted;
                    }

                    Color sliderEndColor = GetButtonEndColor(sliderState);

                    using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(rect.Location, new Size(1, rect.Height)), this.ButtonStartColor, sliderEndColor, LinearGradientMode.Vertical))                        
                    {
                        brush.Blend = _sliderBlend;
                        sliderPath.FillMode = FillMode.Winding;
                        graphics.FillPath(brush, sliderPath);
                    }
                }

                using (GraphicsPath borderPath =
                    GetSliderPath(new Rectangle(new Point(rect.Left + 1, rect.Top + 1), new Size(rect.Width - 3, rect.Height - 3))))
                {
                    using (Pen pen = new Pen(this.ButtonBorderColor))
                    {
                        graphics.DrawPath(pen, borderPath);
                    }
                }

                int middleLineTop = rect.Top + rect.Height * 3 / 10;
                int middleLineHeight = rect.Width * 5 / 10;
                int middle = rect.Left + rect.Width / 2;

                using (Pen pen = new Pen(Color.FromArgb(150, this.ButtonBorderColor)))
                {
                    graphics.DrawLine(pen, new Point(middle, middleLineTop), new Point(middle, middleLineTop + middleLineHeight));
                }

                using (Pen pen = new Pen(Color.FromArgb(150, this.ButtonStartColor)))
                {
                    graphics.DrawLine(
                        pen, new Point(middle + 1, middleLineTop + 1), new Point(middle + 1, middleLineTop + middleLineHeight));
                }
            }

            graphics.Restore(grState);
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Disallows background painting.
        /// </summary>
        /// <param name="pevent">PaintEventArgs that contains the event data.</param>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (this.Style == Theme.Default)
            {
                if (!this.Transparent)
                {
                    Matrix matrix = pevent.Graphics.Transform;
                    Size szClient = this.ClientSize;

                    if (m_orientation == Orientation.Vertical)
                    {
                        pevent.Graphics.TranslateTransform(0, this.Bounds.Height);
                        pevent.Graphics.RotateTransform(-90.0f);

                        int iTemp = szClient.Height;
                        szClient.Height = szClient.Width;
                        szClient.Width = iTemp;
                    }

                    if (szClient.Width > 0 && szClient.Height > 0)
                    {
                        Rectangle rcBrush = new Rectangle(Point.Empty, new Size(1, szClient.Height));
                        using (Brush brush = new LinearGradientBrush(rcBrush, this.TrackBarGradientStart, this.TrackBarGradientEnd, LinearGradientMode.Vertical))
                        {
                            pevent.Graphics.FillRectangle(brush, this.DisplayRectangleInternal);
                        }
                    }

                    pevent.Graphics.Transform = matrix;
                }
                else base.OnPaintBackground(pevent);
            }
            else
            {
                SolidBrush brush = new SolidBrush(Color.White);
                pevent.Graphics.FillRectangle(brush, pevent.ClipRectangle);
                brush.Dispose();
            }
        }

        private Theme style = Theme.Default;
        /// <summary>
        /// Gets or sets a value for style.
        /// </summary>
        [Category("Appearance")]
        [Description("Value for style.")]
        [DefaultValue(true)]
        public Theme Style
        {
            get { return style; }
            set { style = value; this.Invalidate(); }
        }

        /// <summary>
        /// Paints TrackBarEx.
        /// </summary>
        /// <param name="e">EventArgs that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Matrix matrix = e.Graphics.Transform;

            if (m_orientation == Orientation.Vertical)
            {
                e.Graphics.TranslateTransform(0, this.Bounds.Height);
                e.Graphics.RotateTransform(-90.0f);
            }

            if (this.Style == Theme.Default) 
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (m_bShowButtons && Style != Theme.Metro)
            {
                DrawButtons(e.Graphics);
            }

            Rectangle channelBounds = this.ChannelBounds;

            if (channelBounds.Height > 0 && channelBounds.Width > 0)
            {
                if (this.style == Theme.Default)
                {
                    using (LinearGradientBrush channelBrush = new LinearGradientBrush(new Rectangle(channelBounds.Location, new Size(1, channelBounds.Height)), Color.FromArgb(32, Color.Black), Color.FromArgb(160, Color.White), LinearGradientMode.Vertical))
                    {
                        channelBrush.WrapMode = WrapMode.TileFlipY;
                        e.Graphics.FillRectangle(channelBrush, channelBounds);
                    }
                }
                else
                {
                    SolidBrush brush = new SolidBrush(this.ForeColor );
                    Rectangle rect = new Rectangle();
                    rect = channelBounds;
                    ////rect.X = rect.X - 15;
                    ////rect.Width = rect.Width + 30;
                    rect.Y = rect.Y + 1;
                    rect.Height = rect.Height - 1;
                    e.Graphics.FillRectangle(brush, rect);
                    rect.X = this.SliderBounds.Left + this.SliderBounds.Width -1;
                    rect.Width = rect.Width -this.SliderBounds.Left +15;
                    brush = new SolidBrush(Color.White);
                    e.Graphics.FillRectangle(brush, rect);
                    rect.Height = rect.Height - 1;
                    brush = new SolidBrush(ColorTranslator.FromHtml("#D1D3D4"));
                    e.Graphics.FillRectangle(brush, rect);
                    brush.Dispose();
                }
            }
            if (this.Style == Theme.Default)
                DrawSlider(e.Graphics);
            else
                DrawMetroSlider(e.Graphics);

            if (m_bShowFocusRect && this.Focused)
            {
                ControlPaint.DrawFocusRectangle(e.Graphics, this.DisplayRectangleInternal);
            }

            e.Graphics.Transform = matrix;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
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
        /// Draws focus rectangle if needed.
        /// </summary>
        /// <param name="e">EventArgs that contains the event data.</param>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            if (m_bShowFocusRect)
            {
                InvalidateFocusRect();
            }
        }

        /// <summary>
        /// Hides focus rectangle if needed.
        /// </summary>
        /// <param name="e">EventArgs that contains the event data.</param>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            if (m_bShowFocusRect)
            {
                InvalidateFocusRect();
            }
        }

        /// <summary>
        /// Invalidates control.
        /// </summary>
        /// <param name="e">EventArgs that contains the event data.</param>
        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            Invalidate();
        }

        /// <summary>
        /// Makes AutoSize property visible in property grid.
        /// </summary>
        [Category("Behavior")]
        [EditorBrowsable(EditorBrowsableState.Always),Browsable(true),DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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
                        m_setHeight = (m_orientation == Orientation.Horizontal) ? this.Height : this.Width;

                        if (m_orientation == Orientation.Horizontal)
                            this.Height = GetPreferredHeight();
                        else
                            this.Width = GetPreferredHeight();
                    }
                    else
                    {
                        if (m_orientation == Orientation.Horizontal)
                            this.Height = m_setHeight;
                        else
                            this.Width = m_setHeight;
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
                if (m_orientation == Orientation.Horizontal)
                    height = GetPreferredHeight();
                else
                    width = GetPreferredHeight();
            }

            base.SetBoundsCore(x, y, width, height, specified);
        }

        /// <summary>
        /// Processes mouse click.
        /// </summary>
        /// <param name="e">EventArgs that contains the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Point ptMouseLocation = TranslateClientLocation(e.Location);

            if (!this.Focused)
            {
                this.Focus();
            }

            this.Capture = true;

            if (e.Button == MouseButtons.Left)
            {
                Rectangle rcSlider = this.SliderActionBounds;

                if (this.DecreaseButtonBounds.Contains(ptMouseLocation))
                {
                    this.PushedItem = TrackBarExArea.DecreaseButton;
                    StartTimer(TrackBarExArea.DecreaseButton);
                    SmallDecrease();
                }
                else if (this.IncreaseButtonBounds.Contains(ptMouseLocation))
                {
                    this.PushedItem = TrackBarExArea.IncreaseButton;
                    StartTimer(TrackBarExArea.IncreaseButton);
                    SmallIncrease();
                }
                else if (rcSlider.Contains(ptMouseLocation))
                {
                    m_bSliderMoving = true;
                    this.PushedItem = TrackBarExArea.Slider;
                }
                else
                {
                    Rectangle rcChannel = this.ChannelBounds;

                    if (ptMouseLocation.X >= rcChannel.X && ptMouseLocation.X <= rcChannel.Right)
                    {
                        bool bRtl = this.RightToLeft == RightToLeft.Yes;

                        if ((ptMouseLocation.X < rcSlider.X && !bRtl) || (ptMouseLocation.X > rcSlider.Right && bRtl))
                        {
                            StartTimer(TrackBarExArea.LargeDecrease);
                            LargeDecrease();
                        }
                        if ((ptMouseLocation.X > rcSlider.Right && !bRtl) || (ptMouseLocation.X < rcSlider.X && bRtl))
                        {
                            StartTimer(TrackBarExArea.LargeIncrease);
                            LargeIncrease();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resets timers and releases mouse capture.
        /// </summary>
        /// <param name="e">EventArgs that contains the event data.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            this.Capture = false;
        }

        /// <summary>
        /// Handles release of mouse capture.
        /// </summary>
        /// <param name="e">EventArgs that contains the event data.</param>
        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            m_timer.Stop();
            m_bSliderMoving = false;
            this.PushedItem = TrackBarExArea.None;
        }

        /// <summary>
        /// Handles mouse moving.
        /// </summary>
        /// <param name="e">EventArgs that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Point ptMouseLocation = TranslateClientLocation(e.Location);

            if (m_bSliderMoving)
            {
                if(this.Style == Theme.Metro)
                this.Invalidate();
                Rectangle slider = this.SliderBounds;

                float smallChangeInPix = (float)this.ChannelBounds.Width * m_smallChange / (m_maximum - m_minimum);

                int offset = ptMouseLocation.X - (slider.Left + slider.Width / 2);
                int shift = (int)Math.Round(offset / smallChangeInPix) * m_smallChange;

                if (this.RightToLeft == RightToLeft.Yes)
                {
                    shift = -shift;
                }

                ScrollTo(m_value + shift);
            }
            else
            {
                if (this.DecreaseButtonBounds.Contains(ptMouseLocation))
                {
                    this.CurrentArea = TrackBarExArea.DecreaseButton;
                }
                else if (this.IncreaseButtonBounds.Contains(ptMouseLocation))
                {
                    this.CurrentArea = TrackBarExArea.IncreaseButton;
                }
                else if (this.SliderActionBounds.Contains(ptMouseLocation))
                {
                    this.CurrentArea = TrackBarExArea.Slider;
                }
                else
                {
                    this.CurrentArea = TrackBarExArea.None;
                }
            }
        }

        /// <summary>
        /// Resets current area.
        /// </summary>
        /// <param name="e">EventArgs that contains the event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            this.CurrentArea = TrackBarExArea.None;
        }
      
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            HandledMouseEventArgs hArgs = e as HandledMouseEventArgs;
            if (hArgs == null || !hArgs.Handled)
            {
                if (((Control.ModifierKeys & (Keys.Alt | Keys.Shift)) == Keys.None) && (Control.MouseButtons == MouseButtons.None))
                {
                    int mouseWheelScrollLines = Math.Max(SystemInformation.MouseWheelScrollLines, m_smallChange);

                    ScrollTo(m_value + mouseWheelScrollLines * e.Delta / SystemInformation.MouseWheelScrollDelta);

                    if (hArgs != null)
                    {
                        hArgs.Handled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Handles keyboard.
        /// </summary>
        /// <param name="e">EventArgs that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!e.Handled)
            {
                switch (e.KeyData)
                {
                    case Keys.Left:
                        {
                            int shift = this.RightToLeft == RightToLeft.Yes ? m_smallChange : -m_smallChange;
                            ScrollTo(m_value + shift);
                        }
                        break;

                    case Keys.Right:
                        {
                            int shift = this.RightToLeft == RightToLeft.Yes ? -m_smallChange : m_smallChange;
                            ScrollTo(m_value + shift);
                        }
                        break;

                    case Keys.Down:
                        SmallDecrease();
                        break;

                    case Keys.Up:
                        SmallIncrease();
                        break;

                    case Keys.PageDown:
                        LargeDecrease();
                        break;

                    case Keys.PageUp:
                        LargeIncrease();
                        break;

                    case Keys.Home:
                        ScrollTo(this.RightToLeft == RightToLeft.Yes ? m_maximum : m_minimum);
                        break;

                    case Keys.End:
                        ScrollTo(this.RightToLeft == RightToLeft.Yes ? m_minimum : m_maximum);
                        break;
                }
            }
        }

        /// <summary>
        /// Accepts needed keys.
        /// </summary>
        /// <param name="keyData">Key Data</param>
        /// <returns>Return True if Inputkey</returns>
        protected override bool IsInputKey(Keys keyData)
        {
            bool result = base.IsInputKey(keyData);

            result |= (keyData >= Keys.PageUp && keyData <= Keys.Down ) || keyData == Keys.Home || keyData == Keys.End;

            return result;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Gets preferred height.
        /// </summary>
        /// <returns>Returns Preferred Height</returns>
        private int GetPreferredHeight()
        {
            return Math.Max(MIN_HEIGHT, Math.Max(this.DecreaseButtonBounds.Height + this.Padding.Vertical, this.IncreaseButtonBounds.Height + this.Padding.Vertical));             
        }

        /// <summary>
        /// Creates path for slider from rectangle.
        /// </summary>
        /// <param name="rect">Rectangle to create path from.</param>
        /// <returns>Created slider path.</returns>
        private GraphicsPath GetSliderPath(Rectangle rect)
        {
            GraphicsPath sliderPath = new GraphicsPath();
            int topHeight = rect.Height * 7 / 10;
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
        /// Draws background of buttons.
        /// </summary>
        /// <param name="graphics">Graphics object to draw to.</param>
        /// <param name="rect">Rectangle of button.</param>
        /// <param name="state">State of the button.</param>
        private void DrawButtonBackground(Graphics graphics, Rectangle rect, ItemState state)
        {
            if (rect.Width > 0 && rect.Height > 0)
            {
                Color sliderEndColor = GetButtonEndColor(state);

                using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(rect.Location, new Size(1, rect.Height)), this.ButtonStartColor, sliderEndColor, LinearGradientMode.Vertical))               
                {
                    brush.Blend = _buttonsBlend;
                    graphics.FillEllipse(brush, rect);
                }

                using (Pen pen = new Pen(this.ButtonBorderColor))
                {
                    rect.Inflate(-1, -1);
                    graphics.DrawEllipse(pen, rect);
                }
            }
        }

        /// <summary>
        /// Gets slider end color according to the given state.
        /// </summary>
        /// <param name="state">State to retrieve color for.</param>
        /// <returns>Retrieved color.</returns>
        private Color GetButtonEndColor(ItemState state)
        {
            Color result = Color.Empty;

            switch (state)
            {
                case ItemState.Default:
                    result = this.ButtonColor;
                    break;

                case ItemState.Highlighted:
                    result = this.HighlightedButtonColor;
                    break;

                case ItemState.Pushed:
                    result = this.PushedButtonEndColor;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Initializes and starts timer.
        /// </summary>
        /// <param name="mousePushedArea">Area where mouse was pushed and caused timer to start.</param>
        private void StartTimer(TrackBarExArea mousePushedArea)
        {
            m_timer.Interval = m_timerInt * 4;
            m_timer.Tag = mousePushedArea;
            m_timer.Start();
        }
 
        private void InvalidateSlider()
        {
            if (this.IsHandleCreated)
            {
                Invalidate(TranslateClientRect(this.SliderBounds));
            }
        }

        /// <summary>
        /// Invalidates given area of control.
        /// </summary>
        /// <param name="area">Area to invalidate.</param>
        private void InvalidateArea(TrackBarExArea area)
        {
            switch (area)
            {
                case TrackBarExArea.DecreaseButton:
                    Invalidate(TranslateClientRect(this.DecreaseButtonBounds));
                    break;

                case TrackBarExArea.IncreaseButton:
                    Invalidate(TranslateClientRect(this.IncreaseButtonBounds));
                    break;

                case TrackBarExArea.Slider:
                    Invalidate(TranslateClientRect(this.SliderBounds));
                    break;
            }
        }

        /// <summary>
        /// Translates bounds that are the bounds of buttons of TrackBarEx.
        /// </summary>
        /// <param name="pBounds"> Bounds of buttons of TrackBarEx that are being modified. </param>
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
        /// Modifies given mouse location according to TrackBarEx orientation.
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
        /// Invalidates focus rectangle.
        /// </summary>
        private void InvalidateFocusRect()
        {
            Rectangle focusRect = this.Parent.RectangleToClient(this.RectangleToScreen(this.ClientRectangle));

            using (Region rgn = new Region(focusRect))
            {
                focusRect.Inflate(-1, -1);
                rgn.Exclude(focusRect);
                this.Parent.Invalidate(rgn, true);
            }
        }

        private void ScrollTo(int pos)
        {
            int value = GetValidValue(pos);

            if (m_value != value)
            {
                InvalidateSlider();

                m_value = value;

                InvalidateSlider();

                Update();

                OnScroll();
                OnValueChanged();
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
    
        private int GetValidValue(int value)
        {
            if (value < m_minimum)
            {
                value = m_minimum;
            }
            else if (value > m_maximum)
            {
                value = m_maximum;
            }
            return value;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles mouse keeping pushed..
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs that contains the event data.</param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            TrackBarExArea area = (TrackBarExArea)((Timer)sender).Tag;
            Point p = TranslateClientLocation(PointToClient(Control.MousePosition));

            Rectangle rcSlider = this.SliderActionBounds;
            Rectangle rcChannel = this.ChannelBounds;

            switch (area)
            {
                case TrackBarExArea.DecreaseButton:
                    if (this.DecreaseButtonBounds.Contains(p))
                    {
                        SmallDecrease();
                    }
                    break;
                case TrackBarExArea.IncreaseButton:
                    if (this.IncreaseButtonBounds.Contains(p))
                    {
                        SmallIncrease();
                    }
                    break;
                case TrackBarExArea.LargeDecrease:
                    if (p.X >= rcChannel.X && p.X <= rcChannel.Right)
                    {
                        bool bRtl = this.RightToLeft == RightToLeft.Yes;

                        if ((p.X < rcSlider.X && !bRtl) || (p.X > rcSlider.X && bRtl))
                        {
                            LargeDecrease();
                        }
                    }
                    break;
                case TrackBarExArea.LargeIncrease:
                    if (p.X >= rcChannel.X && p.X <= rcChannel.Right)
                    {
                        bool bRtl = this.RightToLeft == RightToLeft.Yes;

                        if ((p.X < rcSlider.Right && !bRtl) || (p.X < rcSlider.X && bRtl))
                        {
                            LargeIncrease();
                        }
                    }
                    break;
            }

            m_timer.Interval = m_timerInt;
        }
        #endregion

        #region ShouldSerialize & Reset Methods
  
        protected bool ShouldSerializeDecreaseButtonSize()
        {
            return m_decrButtonSize != DECR_BUTTON_SIZE;
        }
   
        protected void ResetDecreaseButtonSize()
        {
            this.DecreaseButtonSizeInternal = DECR_BUTTON_SIZE;
        }
      
        protected bool ShouldSerializeIncreaseButtonSize()
        {
            return m_incrButtonSize != INCR_BUTTON_SIZE;
        }
   
        protected void ResetIncreaseButtonSize()
        {
            this.IncreaseButtonSizeInternal = INCR_BUTTON_SIZE;
        }
    
        protected bool ShouldSerializeSliderSize()
        {
            return m_sliderSize != SLIDER_SIZE;
        }

        protected bool ShouldSerializeAutoSize()
        {
            return base.AutoSize != true;
        }

        protected bool ShouldSerializeSize()
        {
            if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
                return this.Width != INIT_WIDTH || this.Height != MIN_HEIGHT;
            else
                return this.Width != MIN_HEIGHT || this.Height != INIT_WIDTH;
        }

        protected void ResetSliderSize()
        {
            this.SliderSize = SLIDER_SIZE;
        }

        protected bool ShouldSerializeTrackBarGradientStart()
        {
            return m_clrTrackBarGradientStart != TRACK_BAR_GRADIENT_START;
        }

        protected void ResetTrackBarGradientStart()
        {
            this.TrackBarGradientStart = TRACK_BAR_GRADIENT_START;
        }
    
        protected bool ShouldSerializeTrackBarGradientEnd()
        {
            return m_clrTrackBarGradientEnd != TRACK_BAR_GRADIENT_END;
        }

        protected bool ShouldSerializeStyle()
        {
            return Style != Theme.Default;
        }
   
        protected void ResetTrackBarGradientEnd()
        {
            this.TrackBarGradientEnd = TRACK_BAR_GRADIENT_END;
        }
   
        protected bool ShouldSerializeButtonColor()
        {
            return m_clrButton != BUTTON_COLOR;
        }
  
        protected void ResetButtonColor()
        {
            this.ButtonColor = BUTTON_COLOR;
        }
    
        protected bool ShouldSerializeHighlightedButtonColor()
        {
            return m_clrHighlightedButton != HIGHLIGHTED_BUTTON_COLOR;
        }
   
        protected void ResetHighlightedButtonColor()
        {
            this.HighlightedButtonColor = HIGHLIGHTED_BUTTON_COLOR;
        }
  
        protected bool ShouldSerializePushedButtonEndColor()
        {
            return m_clrPushedButton != PUSHED_BUTTON_COLOR;
        }
 
        protected void ResetPushedButtonEndColor()
        {
            this.PushedButtonEndColor = PUSHED_BUTTON_COLOR;
        }
        #endregion
    }
}
#endif
