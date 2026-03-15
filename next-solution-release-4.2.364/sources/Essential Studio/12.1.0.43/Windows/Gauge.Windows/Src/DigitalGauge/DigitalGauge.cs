#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Collections;
using Syncfusion.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Gauge
{
    /// <summary>
    /// Digital Gauge class
    /// </summary>
    [Docking(DockingBehavior.Ask), ToolboxItem(true),
    ToolboxBitmap(typeof(Syncfusion.Windows.Forms.Gauge.DigitalGauge), "ToolboxIcons.DigitalGauge.png")]
    [Designer(typeof(DigitalGaugeDesigner))]
    public class DigitalGauge : Control
    {
        #region Variables
        private Color _borderColor;
        private Color _backGradientStartColor;
        private Color _backGradientEndColor;
        private Color _innerFrameBorderColor;
        private Color _outerFrameGradientStartColor;
        private Color _outerFrameGradientEndColor;
        private Color _inactivetextColor;
        private string _digitText;
        private string _value;
        private CharacterType _charType;
        private int _charCount;
        private int _depth;
        private int _roundCornerRadius;
        private float _segmentspacing;
        private bool _overrideFontSize;
        private bool _showInvisibleSegments;
        private Rectangle innerrect;
        #endregion

        #region Constructor and Initialization

        /// <summary>
        /// Initializes a new instance of DigitalGauge class
        /// </summary>
        public DigitalGauge()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(DigitalGauge));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            this.Size = new Size(180, 90);
            InitializeValues();
            RecalcMinSize();
        }

        /// <summary>
        /// Initializes the values for private members used.
        /// </summary>
        private void InitializeValues()
        {
            _borderColor = Color.Gray;
            _backGradientStartColor = Color.FromArgb(240, 240, 240);
            _backGradientEndColor = Color.FromArgb(210, 210, 210);
            _innerFrameBorderColor = Color.FromArgb(180, 180, 180);
            _outerFrameGradientStartColor = Color.FromArgb(229, 229, 229);
            _outerFrameGradientEndColor = Color.FromArgb(172, 172, 172);
            _digitText = "000000";
            _inactivetextColor = Color.Transparent;
            _charCount = 0;
            _value = "000000";
            _charType = CharacterType.SevenSegment;
            _depth = 10;
            _roundCornerRadius = 25;
            _segmentspacing = 0;
            _overrideFontSize = true;
            _showInvisibleSegments = false;
            visualStyle = ThemeStyle.None;
            list = new ListView();
            listChangedHandler = new ListChangedEventHandler(dataManager_ListChanged);
            innerrect = Rectangle.Empty;
        }

        #endregion

        #region Themes
        private ThemeStyle visualStyle;

        /// <summary>
        /// Specifies the visual style for Gauge.
        /// </summary>
        [
          Category("Appearance"),
          Description("Specifies the visual style."),
          DefaultValue(ThemeStyle.None)
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
                SetVisualStyle(value);
                this.Refresh();
            }
        }

        /// <summary>
        /// Sets the Gauge controls visual styles
        /// </summary>
        private void SetVisualStyle(ThemeStyle style)
        {
            switch (style)
            {
                case ThemeStyle.Blue:
                    _backGradientStartColor = ColorTranslator.FromHtml("#ECF4FC");
                    _backGradientEndColor = ColorTranslator.FromHtml("#D8E4F2");
                    _outerFrameGradientStartColor = ColorTranslator.FromHtml("#CEDDEE");
                    _outerFrameGradientEndColor = ColorTranslator.FromHtml("#BDCAD9");
                    _innerFrameBorderColor = ColorTranslator.FromHtml("#849DBD");
                    ForeColor = ColorTranslator.FromHtml("#5F6F77");
                    break;
                case ThemeStyle.Silver:
                    _backGradientStartColor = ColorTranslator.FromHtml("#F5F5F5");
                    _backGradientEndColor = ColorTranslator.FromHtml("#E2E3E4");
                    _outerFrameGradientStartColor = Color.FromArgb(229, 229, 229);
                    _outerFrameGradientEndColor = Color.FromArgb(172, 172, 172);
                    _innerFrameBorderColor = Color.FromArgb(180, 180, 180); //ColorTranslator.FromHtml("#878787");
                    ForeColor = Color.Gray;
                    break;
                case ThemeStyle.Black:
                    _backGradientStartColor = ColorTranslator.FromHtml("#323031");
                    _backGradientEndColor = ColorTranslator.FromHtml("#232021");
                    _outerFrameGradientStartColor = ColorTranslator.FromHtml("#333132");
                    _outerFrameGradientEndColor = ColorTranslator.FromHtml("#262324");
                    _innerFrameBorderColor = ColorTranslator.FromHtml("#070707");
                    this._inactivetextColor = Color.FromArgb(35, 32, 33);
                    ForeColor = Color.White;
                    break;
                case ThemeStyle.Metro:
                    _backGradientStartColor = Color.White;
                    _backGradientEndColor = Color.White;
                    _outerFrameGradientStartColor = Color.FromArgb(17, 180, 205);
                    _outerFrameGradientEndColor = Color.FromArgb(17, 180, 205);
                    _innerFrameBorderColor = Color.FromArgb(180, 180, 180);
                    ForeColor = Color.Gray;
                    break;
                default:
                    _backGradientStartColor = Color.FromArgb(240, 240, 240);
                    _backGradientEndColor = Color.FromArgb(210, 210, 210);
                    _outerFrameGradientStartColor = Color.FromArgb(229, 229, 229);
                    _outerFrameGradientEndColor = Color.FromArgb(172, 172, 172);
                    _innerFrameBorderColor = Color.FromArgb(180, 180, 180);
                    ForeColor = Color.Black;
                    break;

            }
        }

        #endregion

        #region Overrides
        /// <summary>
        /// Overrides the base.Paint
        /// </summary>
        /// <param name="e">painteventargs</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (CharacterType == CharacterType.SevenSegment)
            {
                SevenSegmentHelper sevenSegmentHelper = new SevenSegmentHelper(e.Graphics, this);

                SizeF digitSizeF = sevenSegmentHelper.GetStringSize(_digitText, Font);
                float scaleFactor = Math.Min((innerrect.Width - _depth) / digitSizeF.Width,
                    (innerrect.Height) / digitSizeF.Height);

                Font font = default(Font);
                if (OverrideFontSize)
                    font = new Font(Font.FontFamily, scaleFactor * Font.SizeInPoints);
                else
                    font = new Font(Font.FontFamily, /*scaleFactor **/ Font.SizeInPoints);

                digitSizeF = sevenSegmentHelper.GetStringSize(_digitText, font);

                using (SolidBrush brush = new SolidBrush(ForeColor))
                {
                    using (SolidBrush lightBrush = new SolidBrush(Color.FromArgb(20, _inactivetextColor)))
                    {
                        sevenSegmentHelper.DrawDigits(
                            _digitText, font, brush, lightBrush,
                            (((innerrect.Width - (2 * _depth) - digitSizeF.Width) / 2)) + (2 * _depth),
                            (((innerrect.Height) - digitSizeF.Height) / 2) + _depth);

                    }
                }
            }
            else if (CharacterType == CharacterType.FourteenSegment)
            {
                FourteenSegmentHelper fourteenSegmentHelper = new FourteenSegmentHelper(e.Graphics, this);

                SizeF digitSizeF = fourteenSegmentHelper.GetStringSize(_digitText, Font);
                float scaleFactor = Math.Min((innerrect.Width - _depth) / digitSizeF.Width,
                   (innerrect.Height) / digitSizeF.Height);

                Font font = default(Font);
                if (OverrideFontSize)
                    font = new Font(Font.FontFamily, scaleFactor * Font.SizeInPoints);
                else
                    font = new Font(Font.FontFamily, /*scaleFactor **/ Font.SizeInPoints);

                digitSizeF = fourteenSegmentHelper.GetStringSize(_digitText, font);

                using (SolidBrush brush = new SolidBrush(ForeColor))
                {
                    using (SolidBrush lightBrush = new SolidBrush(Color.FromArgb(20, _inactivetextColor)))
                    {
                        fourteenSegmentHelper.DrawDigits(
                             _digitText, font, brush, lightBrush,
                            (((innerrect.Width - (2 * _depth) - digitSizeF.Width) / 2)) + (2 * _depth),
                            (((innerrect.Height) - digitSizeF.Height) / 2) + _depth);
                    }
                }
            }
            else if (CharacterType == CharacterType.SixteenSegment)
            {
                SixteenSegmentHelper sixteenSegmentHelper = new SixteenSegmentHelper(e.Graphics, this);

                SizeF digitSizeF = sixteenSegmentHelper.GetStringSize(_digitText, Font);
                float scaleFactor = Math.Min((innerrect.Width - _depth) / digitSizeF.Width,
                  (innerrect.Height) / digitSizeF.Height);

                Font font = default(Font);
                if (OverrideFontSize)
                    font = new Font(Font.FontFamily, scaleFactor * Font.SizeInPoints);
                else
                    font = new Font(Font.FontFamily, /*scaleFactor **/ Font.SizeInPoints);
                digitSizeF = sixteenSegmentHelper.GetStringSize(_digitText, font);

                using (SolidBrush brush = new SolidBrush(ForeColor))
                {
                    using (SolidBrush lightBrush = new SolidBrush(Color.FromArgb(20, _inactivetextColor)))
                    {
                        sixteenSegmentHelper.DrawDigits(
                           _digitText, font, brush, lightBrush,
                            (((innerrect.Width - (2 * _depth) - digitSizeF.Width) / 2)) + (2 * _depth),
                            (((innerrect.Height) - digitSizeF.Height) / 2) + _depth);
                    }
                }
            }
            else if (CharacterType == CharacterType.DotMatrixSegment)
            {
                DotMatrixHelper dotMatrixHelper = new DotMatrixHelper(e.Graphics, this);

                SizeF digitSizeF = dotMatrixHelper.GetStringSize(_digitText, Font);
                float scaleFactor = Math.Min((innerrect.Width - _depth) / digitSizeF.Width,
                  (innerrect.Height) / digitSizeF.Height);

                Font font = default(Font);
                if (OverrideFontSize)
                    font = new Font(Font.FontFamily, scaleFactor * Font.SizeInPoints);
                else
                    font = new Font(Font.FontFamily, /*scaleFactor **/ Font.SizeInPoints);
                digitSizeF = dotMatrixHelper.GetStringSize(_digitText, font);

                using (SolidBrush brush = new SolidBrush(ForeColor))
                {
                    using (SolidBrush lightBrush = new SolidBrush(Color.FromArgb(20, _inactivetextColor)))
                    {
                        dotMatrixHelper.DrawDigits(
                           _digitText, font, brush, lightBrush,
                            (((innerrect.Width - (2 * _depth) - digitSizeF.Width) / 2)) + (2 * _depth),
                            (((innerrect.Height) - digitSizeF.Height) / 2) + _depth);
                    }
                }
            }
            base.OnPaint(e);
        }

        /// <summary>
        /// Overrides base.OnPaintBackground to paint the background region of the control
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            int tmpSoundCornerRadius = Math.Min(Math.Min(_roundCornerRadius, this.Width - 2), this.Height - 2);
            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            innerrect = new Rectangle(rect.X + _depth, rect.Y + _depth, this.Width - 2 * _depth, this.Height - 2 * _depth);
            GraphicsPath graphPath = GetRoundPath(rect, tmpSoundCornerRadius);
            GraphicsPath graphInnerPath = GetRoundPath(innerrect, tmpSoundCornerRadius);

            using (LinearGradientBrush brush = new LinearGradientBrush(rect,
                _outerFrameGradientStartColor,
                _outerFrameGradientEndColor,
                    LinearGradientMode.Vertical))
            {
                e.Graphics.FillPath(brush, graphPath);
                e.Graphics.DrawPath(new Pen(_outerFrameGradientEndColor, 1), graphPath);

            }

            using (LinearGradientBrush brush2 = new LinearGradientBrush(innerrect,
                _backGradientStartColor,
                _backGradientEndColor,
                   LinearGradientMode.Vertical))
            {
                e.Graphics.FillPath(brush2, graphInnerPath);
                e.Graphics.DrawPath(new Pen(_innerFrameBorderColor, 1.6f), graphInnerPath);
            }
        }
        /// <summary>
        /// Returns the rounded rectangle for control's region.
        /// </summary>
        /// <param name="r">rectangle</param>
        /// <param name="depth">value to draw inner frame</param>
        /// <returns>graphics path</returns>
        public static GraphicsPath GetRoundPath(Rectangle r, int depth)
        {
            GraphicsPath graphPath = new GraphicsPath();

            graphPath.AddArc(r.X, r.Y, depth, depth, 180, 90);
            graphPath.AddArc(r.X + r.Width - depth, r.Y, depth, depth, 270, 90);
            graphPath.AddArc(r.X + r.Width - depth, r.Y + r.Height - depth, depth, depth, 0, 90);
            graphPath.AddArc(r.X, r.Y + r.Height - depth, depth, depth, 90, 90);
            graphPath.AddLine(r.X, r.Y + r.Height - depth, r.X, r.Y + depth / 2);

            return graphPath;
        }

        /// <summary>
        /// Specifies the background image for the control
        /// </summary>
        [Browsable(false)]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
        }

        /// <summary>
        /// Specifies the image layout for the background image of control.
        /// </summary>
        [Browsable(false)]
        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }
        }
        #endregion

        #region GaugeColor properties

        /// <summary>
        /// Gets or sets the foreground color of this component which used to display the text.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or sets the foreground color of this component which used to display the text."),
          Browsable(true)
        ]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                if (ShowInvisibleSegments)
                    _inactivetextColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the background color of this component.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or sets the background color of this component."),
          Browsable(false)
        ]
        public override Color BackColor
        {
            get
            {
                return Color.Transparent;
            }
        }

        /// <summary>
        /// Gets or sets the gradient start color for the background of Gauge.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or sets the gradient start color for the background of Gauge."),
          Browsable(true)
        ]
        public Color BackgroundGradientStartColor
        {
            get
            {
                return _backGradientStartColor;
            }
            set
            {
                _backGradientStartColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the gradient end color for the background of Gauge.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or sets the gradient end color for the background of Gauge."),
          Browsable(true)
        ]
        public Color BackgroundGradientEndColor
        {
            get
            {
                return _backGradientEndColor;
            }
            set
            {
                _backGradientEndColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the Gradient start color for the outer frame of Gauge.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or sets the Gradient start color for the outer frame of Gauge."),
          Browsable(true)
        ]
        public Color OuterFrameGradientStartColor
        {
            get
            {
                return _outerFrameGradientStartColor; 
            }
            set 
            { _outerFrameGradientStartColor = value; 
                Invalidate(); 
            }
        }

        /// <summary>
        /// Gets or sets the gradient end color for the outer frame of Gauge.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or sets the gradient end color for the outer frame of Gauge."),
          Browsable(true)
        ]
        public Color OuterFrameGradientEndColor
        {
            get
            {
                return _outerFrameGradientEndColor;
            }
            set
            {
                _outerFrameGradientEndColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or Sets the first gradient color for the Frame Border.
        /// </summary>
        [
          Category("Appearance"),
          Description("Gets or Sets the first gradient color for the Frame Border.")
        ]
        public Color FrameBorderColor
        {
            get
            {
                return _innerFrameBorderColor;
            }
            set
            {
                if (_innerFrameBorderColor != value)
                {
                    _innerFrameBorderColor = value;
                    this.Invalidate();
                }
            }
        }

        #endregion

        #region Customization Properties

        /// <summary>
        /// Gets or sets a number of characters to be displayed in the Gauge.
        /// </summary>
        [
          Category("Behavior"),
          Description("Gets or sets the number of characters to be displayed in the Gauge."),
          Browsable(true)
        ]
        public int CharacterCount
        {
            get
            {
                return _charCount; 
            }
            set
            {
                _charCount = value;
                EnsureCharacterCount();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the value to be displayed in the Gauge.
        /// </summary>
        [
          Category("Data"),
          Description("Gets or sets the value to be displayed in the Gauge."),
          Browsable(true)
        ]
        public string Value
        {
            get 
            {
                return _value; 
            }
            set
            {
                _value = value;
                EnsureCharacterCount();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets a value to the outerframe depth in Gauge.
        /// </summary>
        /// <remarks>Maximum Value : Difference between control's width & height, twice the depth value must be equal to or greater than the new input value </remarks>
        [
          Category("Layout"),
          Description("Gets or sets a value to the outerframe depth in Gauge. Refer remarks in 'code' for its maximum value"),
          Browsable(true)
        ]
        public int Depth
        {
            get 
            {
                return _depth; 
            }
            set
            {
                if (this.Width > 2 * value)
                {
                    if (((this.Width - 2 * value) > value) && ((this.Height - 2 * value)>value))
                        _depth = value;
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets a value to the corner radius for Gauge.
        /// </summary>
        [
          Category("Layout"),
          Description("Gets or sets a value to the corner radius for Gauge."),
          Browsable(true)
        ]
        public int RoundCornerRadius
        {
            get 
            { 
                return _roundCornerRadius; 
            }
            set
            {
                _roundCornerRadius = Math.Abs(value);
                RecalcMinSize();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets a value for spacing between each string being displayed in the Gauge.
        /// </summary>
        [
          Category("Behavior"),
          Description("Gets or sets a value for spacing between each string being displayed in the Gauge."),
          Browsable(true)
        ]
        public float SegmentSpacing
        {
            get
            { 
                return _segmentspacing; 
            }
            set
            {
                _segmentspacing = Math.Abs(value);
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets a value to override the control's font size.
        /// </summary>
        /// <remarks>Size mentioned in the Control's Font will be applied only when this is disabled, else the font size will be assigned based on control's size</remarks>
        [
          Category("Behavior"),
          Description("Gets or sets a value to override the control's font size."),
          Browsable(true)
        ]
        public bool OverrideFontSize
        {
            get 
            { 
                return _overrideFontSize; 
            }
            set
            {
                _overrideFontSize = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Show or hide the disable segments in the Gauge.
        /// </summary>
        [
           Category("Behavior"),
           Description("Show or hide the disable segments in the Gauge."),
           Browsable(true)
         ]
        public bool ShowInvisibleSegments
        {
            get
            { 
                return _showInvisibleSegments; 
            }
            set
            {
                _showInvisibleSegments = value;
                if (_showInvisibleSegments)
                    _inactivetextColor = ForeColor;
                else
                {
                    if (this.VisualStyle == ThemeStyle.Black)
                        _inactivetextColor = Color.FromArgb(35, 32, 33);
                    else
                        _inactivetextColor = Color.Transparent;
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets a value to choose the segment type for Gauge
        /// </summary>
        [Browsable(true), Category("Behavior"),
         Description("Gets or sets a value to choose the segment type for Gauge.")]
        public CharacterType CharacterType
        {
            get 
            {
                return _charType; 
            }
            set
            {
                _charType = value;
                Invalidate();
            }
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

        #region Helpers
        /// <summary>
        /// Recalculates the Minimum size for the control
        /// </summary>
        private void RecalcMinSize()
        {
            this.MinimumSize = new Size(RoundCornerRadius + 65, RoundCornerRadius + 65);
        }

        /// <summary>
        /// Resets the digit text value
        /// </summary>
        private void ResetDigitText()
        {
            _digitText = Value.ToUpper().Clone().ToString();
        }

        /// <summary>
        /// Ensures the character count to be displayed in the Gauge
        /// </summary>
        private void EnsureCharacterCount()
        {
            ResetDigitText();
            if (_charCount <= 0 || _charCount > Value.Length)
            {
                //Do Nothing
            }
            else
            {
                _digitText = _digitText.Substring(0, _charCount);
            }
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

        #region DataBinding
        #region Context Changed
        /// <summary>
        /// Overrides base.OnBindingContextChanged
        /// </summary>
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
        /// <summary>
        /// Gets the listview item specified to the passed index.
        /// </summary>
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
            return string.Empty;
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

        /// <summary>
        /// Populates the gauge with the value from the Datasource.
        /// </summary>
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
                        this.Value = val.ToString();
                    }
                    else
                    {
                        val = 0;
                        this.Value = val.ToString();
                    }
                }
            }
        }

        #endregion

        #endregion

        #region ShouldSerialize & Reset Property values

        protected bool ShouldSerializeBackgroundGradientStartColor()
        {
            return BackgroundGradientStartColor != Color.FromArgb(240, 240, 240);
        }

        protected bool ShouldSerializeBackgroundGradientEndColor()
        {
            return BackgroundGradientEndColor != Color.FromArgb(210, 210, 210);
        }

        protected bool ShouldSerializeOuterFrameGradientStartColor()
        {
            return OuterFrameGradientStartColor != Color.FromArgb(229, 229, 229);
        }

        protected bool ShouldSerializeOuterFrameGradientEndColor()
        {
            return OuterFrameGradientEndColor != Color.FromArgb(172, 172, 172);
        }

        protected bool ShouldSerializeFrameBorderColor()
        {
            return FrameBorderColor != Color.FromArgb(180, 180, 180);
        }

        protected bool ShouldSerializeDigitCount()
        {
            return CharacterCount != 0;
        }

        protected bool ShouldSerializeValue()
        {
            return Value != "000000";
        }

        protected bool ShouldSerializeDepth()
        {
            return Depth != 10;
        }

        protected bool ShouldSerializeRoundCornerRadius()
        {
            return RoundCornerRadius != 25;
        }

        protected bool ShouldSerializeSegmentSpacing()
        {
            return SegmentSpacing != 0;
        }

        protected bool ShouldSerializeOverrideFontSize()
        {
            return OverrideFontSize != true;
        }

        protected bool ShouldSerializeShowInvisibleSegments()
        {
            return ShowInvisibleSegments != false;
        }

        protected bool ShouldSerializeCharacterType()
        {
            return CharacterType != CharacterType.SevenSegment;
        }

        protected bool ShouldSerializeVisualStyle()
        {
            return VisualStyle != ThemeStyle.None;
        }

        protected void ResetBackgroundGradientStartColor()
        {
            BackgroundGradientStartColor = Color.FromArgb(240, 240, 240);
        }
        protected void ResetBackgroundGradientEndColor()
        {
            BackgroundGradientEndColor = Color.FromArgb(210, 210, 210);
        }
        protected void ResetOuterFrameGradientStartColor()
        {
            OuterFrameGradientStartColor = Color.FromArgb(229, 229, 229);
        }
        protected void ResetOuterFrameGradientEndColor()
        {
            OuterFrameGradientEndColor = Color.FromArgb(172, 172, 172);
        }
        protected void ResetFrameBorderColor()
        {
            FrameBorderColor = Color.FromArgb(180, 180, 180);
        }

        protected void ResetDigitCount()
        {
            CharacterCount = 0;
        }

        protected void ResetValue()
        {
            Value = "000000";
        }

        protected void ResetDepth()
        {
            Depth = 10;
        }

        protected void ResetRoundCornerRadius()
        {
            RoundCornerRadius = 25;
        }

        protected void ResetSegmentSpacing()
        {
            SegmentSpacing = 0;
        }

        protected void ResetOverrideFontSize()
        {
            OverrideFontSize = true;
        }

        protected void ResetShowInvisibleSegments()
        {
            ShowInvisibleSegments = false;
        }

        protected void ResetCharacterType()
        {
            CharacterType = CharacterType.SevenSegment;
        }

        protected void ResetVisualStyle()
        {
            VisualStyle = ThemeStyle.None;
        }

        #endregion
    }

    #region SegmentHelpers
    /// <summary>
    /// Helper class for the SevenSegment Display
    /// </summary>
    public class SevenSegmentHelper
    {
        Graphics _graphics;
        DigitalGauge Gauge;
        char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ".ToCharArray();

        static byte[,] _segmentData = {{1, 1, 1, 0, 1, 1, 1},
							 {0, 0, 1, 0, 0, 1, 0},  
							 {1, 0, 1, 1, 1, 0, 1},  
							 {1, 0, 1, 1, 0, 1, 1},  
							 {0, 1, 1, 1, 0, 1, 0},  
							 {1, 1, 0, 1, 0, 1, 1},  
							 {1, 1, 0, 1, 1, 1, 1},  
							 {1, 0, 1, 0, 0, 1, 0},  
							 {1, 1, 1, 1, 1, 1, 1},  
							 {1, 1, 1, 1, 0, 1, 1}};

        static byte[,] _segmentTextData = {{1, 1, 1, 1, 1, 1, 0},//A
							 {0, 1, 0, 1, 1, 1, 1}, //B
							 {1, 1, 0, 0, 1, 0, 1}, //C
							 {0, 0, 1, 1, 1, 1, 1}, //D 
							 {1, 1, 0, 1, 1, 0, 1}, //E 
							 {1, 1, 0, 1, 1, 0, 0}, //F 
							 {1, 1, 1, 1, 0, 1, 1}, //G 
							 {0, 1, 1, 1, 1, 1, 0}, //H 
							 {0, 1, 0, 0, 1, 0, 0}, //I 
							 {0, 0, 1, 0, 1, 1, 1}, //J
                             {0, 1, 1, 1, 1, 1, 0}, //K - N/A for seven segments - 10
                             {0, 1, 0, 0, 1, 0, 1}, //L
                             {1, 0, 0, 0, 1, 1, 0}, //M - N/A for seven segments - 12
                             {0, 0, 0, 1, 1, 1, 0}, //N
                             {1, 1, 1, 0, 1, 1, 1}, //O
                             {1, 1, 1, 1, 1, 0, 0}, //P
                             {1, 1, 1, 1, 0, 1, 0}, //Q - N/A for seven segments - 16
                             {0, 0, 0, 1, 1, 0, 0}, //R - N/A for seven segments - 17
                             {1, 1, 0, 1, 0, 1, 1}, //S
                             {0, 1, 0, 1, 1, 0, 1}, //T - N/A for seven segments - 19
                             {0, 1, 1, 0, 1, 1, 1}, //U
                             {0, 0, 0, 0, 1, 1, 1}, //V - N/A for seven segments - 21
                             {0, 1, 1, 0, 0, 0, 1}, //W - N/A for seven segments - 22
                             {0, 1, 1, 1, 1, 1, 0}, //X - N/A for seven segments - 23
                             {0, 1, 1, 1, 0, 1, 1}, //Y
                             {1, 0, 1, 1, 1, 0, 1}, //Z
                             {0, 0, 0, 0, 0, 0, 0}, //White space
                                          };

        readonly Point[][] _segmentPoints = new Point[7][];

        /// <summary>
        /// Initiates a new instance for the SevenSegmentHelper class
        /// </summary>
        /// <param name="graphics">graphics of the control</param>
        /// <param name="gauge">owner control [Gauge]</param>
        public SevenSegmentHelper(Graphics graphics, DigitalGauge gauge)
        {
            this.Gauge = gauge;
            this._graphics = graphics;
            _segmentPoints[0] = new Point[] { new Point(3, 2), new Point(39, 2), new Point(31, 10), new Point(11, 10) };
            _segmentPoints[1] = new Point[] { new Point(2, 3), new Point(10, 11), new Point(10, 31), new Point(2, 35) };
            _segmentPoints[2] = new Point[] { new Point(40, 3), new Point(40, 35), new Point(32, 31), new Point(32, 11) };
            _segmentPoints[3] = new Point[] { new Point(3, 36), new Point(11, 32), new Point(31, 32), new Point(39, 36), new Point(31, 40), new Point(11, 40) };
            _segmentPoints[4] = new Point[] { new Point(2, 37), new Point(10, 41), new Point(10, 61), new Point(2, 69) };
            _segmentPoints[5] = new Point[] { new Point(40, 37), new Point(40, 69), new Point(32, 61), new Point(32, 41) };
            _segmentPoints[6] = new Point[] { new Point(11, 62), new Point(31, 62), new Point(39, 70), new Point(3, 70) };
        }

        /// <summary>
        /// Measures and returns the size of the given text size.
        /// </summary>
        /// <param name="text">string to measure</param>
        /// <param name="font">specified font to measure the string</param>
        /// <returns>SizeF</returns>
        public SizeF GetStringSize(string text, Font font)
        {
            SizeF sizef = new SizeF(0, _graphics.DpiX * font.SizeInPoints / 72);

            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsDigit(text[i]))
                    sizef.Width += 42 * _graphics.DpiX * font.SizeInPoints / 72 / 72;
                else if (text[i] == ':' || text[i] == '.')
                    sizef.Width += 12 * _graphics.DpiX * font.SizeInPoints / 72 / 72;
                else if(char.IsLetter(text[i]))
                    sizef.Width += 42 * _graphics.DpiX * font.SizeInPoints / 72 / 72;
                else
                    sizef.Width += 42 * _graphics.DpiX * font.SizeInPoints / 72 / 72;
            }
            return sizef;
        }

        /// <summary>
        /// Used to draw the input text in the gauge.
        /// </summary>
        /// <param name="text">input text</param>
        /// <param name="font">font</param>
        /// <param name="brush">brush to paint the active segments</param>
        /// <param name="brushLight">brush to paint the inactive segments</param>
        /// <param name="x">x location of the text to draw</param>
        /// <param name="y">y location of the text to draw</param>
        public void DrawDigits(string text, Font font, Brush brush, Brush brushLight, float x, float y)
        {
            for (int cnt = 0; cnt < text.Length; cnt++)
            {
                if (Char.IsDigit(text[cnt]))
                    x = DrawDigit(text[cnt] - '0', font, brush, brushLight, x, y);
                else if (text[cnt] == ':')
                    x = DrawColon(font, brush, x, y);
                else if (text[cnt] == '.')
                    x = DrawDot(font, brush, x, y);
                else if (char.IsLetter(text[cnt]))
                    x = DrawText((char)text[cnt], font, brush, brushLight, x, y);
                else
                    x = DrawText(' ', font, brush, brushLight, x, y);
            }
        }

        private float DrawDigit(int num, Font font, Brush brush, Brush brushLight, float x, float y)
        {
            for (int cnt = 0; cnt < _segmentPoints.Length; cnt++)
            {
                if (_segmentData[num, cnt] == 1)
                {
                    FillPolygon(_segmentPoints[cnt], font, brush, x, y);
                }
                else
                {
                    FillPolygon(_segmentPoints[cnt], font, brushLight, x, y);
                }
            }
            if(Gauge.OverrideFontSize)
                x = x + (42 * _graphics.DpiX * font.SizeInPoints / 72 / 72);
            else
                x = x + (42 * _graphics.DpiX * font.SizeInPoints / 72 / 72)+Gauge.SegmentSpacing;

            return x;
        }
       
        private float DrawText(char text, Font font, Brush brush, Brush brushLight, float x, float y)
        {
            int num = Array.IndexOf(alpha, text);

            for (int cnt = 0; cnt < _segmentPoints.Length; cnt++)
            {
                if (_segmentTextData[num, cnt] == 1)
                {
                    FillPolygon(_segmentPoints[cnt], font, brush, x, y);
                }
                else
                {
                    FillPolygon(_segmentPoints[cnt], font, brushLight, x, y);
                }
            }

            if (Gauge.OverrideFontSize)
                x = x + (42 * _graphics.DpiX * font.SizeInPoints / 72 / 72);
            else
                x = x + (42 * _graphics.DpiX * font.SizeInPoints / 72 / 72) + Gauge.SegmentSpacing;

            return x;
        }

        private float DrawDot(Font font, Brush brush, float x, float y)
        {
            Point[][] dotPoints = new Point[1][];

            dotPoints[0] = new Point[] {new Point( 2, 64), new Point( 6, 61),
                                new Point(10, 64), new Point( 6, 69)};

            for (int cnt = 0; cnt < dotPoints.Length; cnt++)
            {
                FillPolygon(dotPoints[cnt], font, brush, x, y);
            }

            if (Gauge.OverrideFontSize)
                x = x + (12 * _graphics.DpiX * font.SizeInPoints / 72 / 72);
            else
                x = x + (12 * _graphics.DpiX * font.SizeInPoints / 72 / 72) + Gauge.SegmentSpacing;

            return x;
           
        }

        private float DrawColon(Font font, Brush brush, float x, float y)
        {
            Point[][] colonPoints = new Point[2][];

            colonPoints[0] = new Point[] { new Point(2, 21), new Point(6, 17), new Point(10, 21), new Point(6, 25) };
            colonPoints[1] = new Point[] { new Point(2, 51), new Point(6, 47), new Point(10, 51), new Point(6, 55) };

            for (int cnt = 0; cnt < colonPoints.Length; cnt++)
            {
                FillPolygon(colonPoints[cnt], font, brush, x, y);
            }

            if (Gauge.OverrideFontSize)
                x = x + (12 * _graphics.DpiX * font.SizeInPoints / 72 / 72);
            else
                x = x + (12 * _graphics.DpiX * font.SizeInPoints / 72 / 72) + Gauge.SegmentSpacing;

            return x;
        }

        private void FillPolygon(Point[] polygonPoints, Font font, Brush brush, float x, float y)
        {
            PointF[] polygonPointsF = new PointF[polygonPoints.Length];

            for (int cnt = 0; cnt < polygonPoints.Length; cnt++)
            {
                polygonPointsF[cnt].X = x + polygonPoints[cnt].X * _graphics.DpiX * font.SizeInPoints / 72 / 72;
                polygonPointsF[cnt].Y = y + polygonPoints[cnt].Y * _graphics.DpiY * font.SizeInPoints / 72 / 72;
            }
            _graphics.FillPolygon(brush, polygonPointsF);
        }
    }

    /// <summary>
    /// Helper class for the FourteenSegment Display
    /// </summary>
    public class FourteenSegmentHelper
    {
        Graphics _graphics;
        DigitalGauge Gauge;

        char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ".ToCharArray();

        static byte[,] _segmentData = {
                             {1,1,1,0,0,0,0,0,1,1,0,0,0,1}, //0
                             {0,0,1,0,0,0,0,0,0,1,0,0,0,0}, //1
							 {1,0,1,0,0,0,1,1,1,0,0,0,0,1}, //2 
							 {1,0,1,0,0,0,1,1,0,1,0,0,0,1}, //3 
							 {0,1,1,0,0,0,1,1,0,1,0,0,0,0}, //4 
							 {1,1,0,0,0,0,1,1,0,1,0,0,0,1}, //5 
							 {0,1,0,0,0,0,1,1,1,1,0,0,0,1}, //6 
							 {1,0,1,0,0,0,0,0,0,1,0,0,0,0}, //7 
							 {1,1,1,0,0,0,1,1,1,1,0,0,0,1}, //8 
							 {1,1,1,0,0,0,1,1,0,1,0,0,0,1}};//9

        static byte[,] _segmentTextData = {
                             {1,1,1,0,0,0,1,1,1,1,0,0,0,0},//A
							 {1,1,1,0,0,0,1,1,1,1,0,0,0,1}, //B
							 {1,1,0,0,0,0,0,0,1,0,0,0,0,1}, //C
							 {1,1,1,0,0,0,0,0,1,1,0,0,0,1}, //D 
							 {1,1,0,0,0,0,1,1,1,0,0,0,0,1}, //E 
							 {1,1,0,0,0,0,1,1,1,0,0,0,0,0}, //F 
							 {1,1,0,0,0,0,0,1,1,1,0,0,0,1}, //G 
							 {0,1,1,0,0,0,1,1,1,1,0,0,0,0}, //H 
							 {0,0,1,0,0,0,0,0,0,1,0,0,0,0}, //I 
							 {0,0,1,0,0,0,0,0,0,1,0,0,0,1}, //J
                             {0,0,0,0,1,1,0,0,0,0,0,1,1,0}, //K 
                             {0,1,0,0,0,0,0,0,1,0,0,0,0,1}, //L
                             {1,1,1,0,1,0,0,0,1,1,0,1,0,0}, //M 
                             //{1,1,1,0,0,0,0,0,1,1,0,0,0,0}, //N as n
                             {0,1,1,1,0,0,0,0,1,1,0,0,1,0}, //N as N 
                             {1,1,1,0,0,0,0,0,1,1,0,0,0,1}, //O
                             {1,1,1,0,0,0,1,1,1,0,0,0,0,0}, //P
                             {0,0,0,0,0,0,0,0,0,0,0,0,0,0}, //Q - only in 16 segments
                             {1,1,1,0,0,0,1,1,1,0,0,0,1,0}, //R 
                             {1,1,0,0,0,0,1,1,0,1,0,0,0,1}, //S
                             {1,0,0,0,1,0,0,0,0,0,0,1,0,0}, //T 
                             {0,1,1,0,0,0,0,0,1,1,0,0,0,1}, //U
                             {0,1,0,0,0,1,0,0,1,0,1,0,0,0}, //V 
                             //{0,0,1,1,0,0,0,0,0,1,0,0,1,0}, //V 
                             {0,1,1,0,1,0,0,0,1,1,0,1,0,1}, //W 
                             {0,0,0,1,0,1,0,0,0,0,1,0,1,0}, //X 
                             {0,1,1,0,0,0,1,1,0,1,0,0,0,1}, //Y
                             {1,0,1,0,0,0,1,1,1,0,0,0,0,1}, //Z
                             {0,0,0,0,0,0,0,0,0,0,0,0,0,0}, //white space
                                          };

        readonly Point[][] _segmentPoints = new Point[14][];

        /// <summary>
        /// Initiates a new instance for the FourteenSegmentHelper class
        /// </summary>
        /// <param name="graphics">graphics of the control</param>
        /// <param name="gauge">owner control [Gauge]</param>
        public FourteenSegmentHelper(Graphics graphics, DigitalGauge gauge)
        {
            this._graphics = graphics;
            this.Gauge = gauge;

            // Top line
            _segmentPoints[0] = new Point[] { new Point(3, 2), new Point(49, 2), new Point(43, 8), new Point(9, 8) };

            // Top left line
            _segmentPoints[1] = new Point[] { new Point(2, 3), new Point(8,9), new Point(8,32), new Point(2, 35) };

            // Top right line
            _segmentPoints[2] = new Point[] { new Point(50, 3), new Point(44, 9), new Point(44, 32), new Point(50, 35) };

            //Top left cross
            _segmentPoints[3] = new Point[] { new Point(9,9), new Point(13,9), new Point(22,28), 
                                              new Point(22,32), new Point(18,32),new Point(9,13)};

            ////center vertical line - top - flat
            //_segmentPoints[4] = new Point[] { new Point(23, 9), new Point(29, 9), new Point(29, 32), new Point(26, 35), new Point(23, 32) };

            //center vertical line - top - cross edged
            _segmentPoints[4] = new Point[] { new Point(26, 9), new Point(29, 12), new Point(29, 32), new Point(26, 35), new Point(23, 32), new Point(23, 12) };

            //Top right cross
            _segmentPoints[5] = new Point[] { new Point(43,9), new Point(43, 13), new Point(34, 32), 
                                              new Point(30, 32), new Point(30, 28),new Point(39,9)};

            //center horiz lines
            _segmentPoints[6] = new Point[] { new Point(3, 36), new Point(9, 33), new Point(21, 33), 
                                new Point(26, 36), new Point(21, 39), new Point(9, 39) };

            _segmentPoints[7] = new Point[] { new Point(26,36), new Point(31, 33), new Point(43, 33),
                                new Point(49, 36), new Point(43, 39), new Point(31, 39) };

            //bottom left line
            _segmentPoints[8] = new Point[] { new Point(2, 37), new Point(8, 40), new Point(8, 63), new Point(2, 69) };

            //bottom right line
            _segmentPoints[9] = new Point[] { new Point(50, 37), new Point(44, 40), new Point(44, 63), new Point(50, 69) };

           // Bottom left cross
            _segmentPoints[10] = new Point[] { new Point(22,40), new Point(22,44), new Point(13,63),
                                               new Point(9,63),new Point(9,59) ,new Point(18, 40)  };

            ////center vertical line - bottom - flat
            //_segmentPoints[11] = new Point[] { new Point(26, 37), new Point(29, 40), new Point(29, 63), new Point(23, 63), new Point(23, 40) };

            //center vertical line - bottom - cross edged
            _segmentPoints[11] = new Point[] { new Point(26, 37), new Point(29, 40), new Point(29, 60), 
                                               new Point(26, 63), new Point(23, 60), new Point(23, 40) };

            // Bottom right cross
            _segmentPoints[12] = new Point[] { new Point(30,40), new Point(34,40), new Point(43,59), 
                                               new Point(43,63),new Point(39,63),new Point(30,44) };

            //Bottom line
            _segmentPoints[13] = new Point[] { new Point(9, 64), new Point(43, 64), new Point(49, 70), new Point(3, 70) };
        }

        /// <summary>
        /// Measures and returns the size of the given text size.
        /// </summary>
        /// <param name="text">string to measure</param>
        /// <param name="font">specified font to measure the string</param>
        /// <returns>SizeF</returns>
        public SizeF GetStringSize(string text, Font font)
        {
            SizeF sizef = new SizeF(0, _graphics.DpiX * font.SizeInPoints / 72);

            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsDigit(text[i]))
                    sizef.Width += 52 * _graphics.DpiX * font.SizeInPoints / 72 / 72;
                else if (text[i] == ':' || text[i] == '.')
                    sizef.Width += 12 * _graphics.DpiX * font.SizeInPoints / 72 / 72;
                else if (char.IsLetter(text[i]))
                    sizef.Width += 52 * _graphics.DpiX * font.SizeInPoints / 72 / 72;
                else
                    sizef.Width += 52 * _graphics.DpiX * font.SizeInPoints / 72 / 72;
            }
            return sizef;
        }

        /// <summary>
        /// Used to draw the input text in the gauge.
        /// </summary>
        /// <param name="text">input text</param>
        /// <param name="font">font</param>
        /// <param name="brush">brush to paint the active segments</param>
        /// <param name="brushLight">brush to paint the inactive segments</param>
        /// <param name="x">x location of the text to draw</param>
        /// <param name="y">y location of the text to draw</param>
        public void DrawDigits(string text, Font font, Brush brush, Brush brushLight, float x, float y)
        {
            for (int cnt = 0; cnt < text.Length; cnt++)
            {
                if (Char.IsDigit(text[cnt]))
                    x = DrawDigit(text[cnt] - '0', font, brush, brushLight, x, y);
                else if (text[cnt] == ':')
                    x = DrawColon(font, brush, x, y);
                else if (text[cnt] == '.')
                    x = DrawDot(font, brush, x, y);
                else if (char.IsLetter(text[cnt]))
                    x = DrawText((char)text[cnt], font, brush, brushLight, x, y);
                else
                    x = DrawText(' ', font, brush, brushLight, x, y);
            }
        }

        private float DrawDigit(int num, Font font, Brush brush, Brush brushLight, float x, float y)
        {
            for (int cnt = 0; cnt < _segmentPoints.Length; cnt++)
            {
                if (_segmentData[num, cnt] == 1)
                {
                    FillPolygon(_segmentPoints[cnt], font, brush, x, y);
                }
                else
                {
                    FillPolygon(_segmentPoints[cnt], font, brushLight, x, y);
                }
            }
            if (Gauge.OverrideFontSize)
                x = x + (52 * _graphics.DpiX * font.SizeInPoints / 72 / 72);
            else
                x = x + (52 * _graphics.DpiX * font.SizeInPoints / 72 / 72) + Gauge.SegmentSpacing;

            return x;
        }

        private float DrawText(char text, Font font, Brush brush, Brush brushLight, float x, float y)
        {
            int num = Array.IndexOf(alpha, text);

            for (int cnt = 0; cnt < _segmentPoints.Length; cnt++)
            {
                if (_segmentTextData[num, cnt] == 1)
                {
                    FillPolygon(_segmentPoints[cnt], font, brush, x, y);
                }
                else
                {
                    FillPolygon(_segmentPoints[cnt], font, brushLight, x, y);
                }
            }
            if (Gauge.OverrideFontSize)
                x = x + (52 * _graphics.DpiX * font.SizeInPoints / 72 / 72);
            else
                x = x + (52 * _graphics.DpiX * font.SizeInPoints / 72 / 72) + Gauge.SegmentSpacing;

            return x;
            
        }

        private float DrawDot(Font font, Brush brush, float x, float y)
        {
            Point[][] dotPoints = new Point[1][];

            dotPoints[0] = new Point[] {new Point( 2, 64), new Point( 6, 61),
                                new Point(10, 64), new Point( 6, 69)};

            for (int cnt = 0; cnt < dotPoints.Length; cnt++)
            {
                FillPolygon(dotPoints[cnt], font, brush, x, y);
            }
            if (Gauge.OverrideFontSize)
                x = x + (12 * _graphics.DpiX * font.SizeInPoints / 72 / 72);
            else
                x = x + (12 * _graphics.DpiX * font.SizeInPoints / 72 / 72) + Gauge.SegmentSpacing;

            return x;
        }

        private float DrawColon(Font font, Brush brush, float x, float y)
        {
            Point[][] colonPoints = new Point[2][];

            colonPoints[0] = new Point[] { new Point(2, 21), new Point(6, 17), new Point(10, 21), new Point(6, 25) };
            colonPoints[1] = new Point[] { new Point(2, 51), new Point(6, 47), new Point(10, 51), new Point(6, 55) };

            for (int cnt = 0; cnt < colonPoints.Length; cnt++)
            {
                FillPolygon(colonPoints[cnt], font, brush, x, y);
            }

            if (Gauge.OverrideFontSize)
                x = x + (12 * _graphics.DpiX * font.SizeInPoints / 72 / 72);
            else
                x = x + (12 * _graphics.DpiX * font.SizeInPoints / 72 / 72) + Gauge.SegmentSpacing;

            return x;
        }

        private void FillPolygon(Point[] polygonPoints, Font font, Brush brush, float x, float y)
        {
            PointF[] polygonPointsF = new PointF[polygonPoints.Length];

            for (int cnt = 0; cnt < polygonPoints.Length; cnt++)
            {
                polygonPointsF[cnt].X = x + polygonPoints[cnt].X * _graphics.DpiX * font.SizeInPoints / 72 / 72;
                polygonPointsF[cnt].Y = y + polygonPoints[cnt].Y * _graphics.DpiY * font.SizeInPoints / 72 / 72;
            }
            _graphics.FillPolygon(brush, polygonPointsF);
        }
    }

    /// <summary>
    /// Helper class for the SixteenSegment Display
    /// </summary>
    public class SixteenSegmentHelper
    {
        Graphics _graphics;
        DigitalGauge Gauge;
        char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ".ToCharArray();

        static byte[,] _segmentData = {
                             {1,1,1,1,0,0,0,0,0,1,1,0,0,0,1,1}, //0
                             {0,0,0,1,0,0,0,0,0,0,1,0,0,0,0,0}, //1
							 {1,1,0,1,0,0,0,1,1,1,0,0,0,0,1,1}, //2 
							 {1,1,0,1,0,0,0,1,1,0,1,0,0,0,1,1}, //3 
							 {0,0,1,1,0,0,0,1,1,0,1,0,0,0,0,0}, //4 
							 {1,1,1,0,0,0,0,1,1,0,1,0,0,0,1,1}, //5 
							 {0,0,1,0,0,0,0,1,1,1,1,0,0,0,1,1}, //6 
							 {1,1,0,1,0,0,0,0,0,0,1,0,0,0,0,0}, //7 
							 {1,1,1,1,0,0,0,1,1,1,1,0,0,0,1,1}, //8 
							 {1,1,1,1,0,0,0,1,1,0,1,0,0,0,1,1}};//9

        static byte[,] _segmentTextData = {
                             {1,1,1,1,0,0,0,1,1,1,1,0,0,0,0,0},//A
							 {1,1,1,1,0,0,0,1,1,1,1,0,0,0,1,1}, //B
							 {1,1,1,0,0,0,0,0,0,1,0,0,0,0,1,1}, //C
							 {1,1,1,1,0,0,0,0,0,1,1,0,0,0,1,1}, //D 
							 {1,1,1,0,0,0,0,1,1,1,0,0,0,0,1,1}, //E 
							 {1,1,1,0,0,0,0,1,1,1,0,0,0,0,0,0}, //F 
							 {1,1,1,0,0,0,0,0,1,1,1,0,0,0,1,1}, //G 
							 {0,0,1,1,0,0,0,1,1,1,1,0,0,0,0,0}, //H 
							 {0,0,0,1,0,0,0,0,0,0,1,0,0,0,0,0}, //I 
							 {0,0,0,1,0,0,0,0,0,0,1,0,0,0,0,1}, //J
                             {0,0,0,0,0,1,1,0,0,0,0,0,1,1,0,0}, //K 
                             {0,0,1,0,0,0,0,0,0,1,0,0,0,0,1,1}, //L
                             {1,1,1,1,0,1,0,0,0,1,1,0,0,0,0,0}, //M 
                             {0,0,1,1,1,0,0,0,0,1,1,0,0,1,0,0}, //N 
                             {1,1,1,1,0,0,0,0,0,1,1,0,0,0,1,1}, //O
                             {1,1,1,1,0,0,0,1,1,1,0,0,0,0,0,0}, //P
                             {1,1,1,1,0,0,0,0,0,1,1,0,0,1,1,1}, //Q 
                             {1,1,1,1,0,0,0,1,1,1,0,0,0,1,0,0}, //R 
                             {1,1,1,0,0,0,0,1,1,0,1,0,0,0,1,1}, //S
                             {1,1,0,0,0,1,0,0,0,0,0,0,1,0,0,0}, //T 
                             {0,0,1,1,0,0,0,0,0,1,1,0,0,0,1,1}, //U
                             {0,0,1,0,0,0,1,0,0,1,0,1,0,0,0,0}, //V 
                             {0,0,1,1,0,1,0,0,0,1,1,0,1,0,1,1}, //W 
                             {0,0,0,0,1,0,1,0,0,0,0,1,0,1,0,0}, //X 
                             {0,0,1,1,0,0,0,1,1,0,1,0,0,0,1,1}, //Y
                             {1,1,0,1,0,0,0,1,1,1,0,0,0,0,1,1}, //Z
                             {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0} //white space
                                          };

        readonly Point[][] _segmentPoints = new Point[16][];

        /// <summary>
        /// Initiates a new instance for the SixteenSegmentHelper class
        /// </summary>
        /// <param name="graphics">graphics of the control</param>
        /// <param name="gauge">owner control [Gauge]</param>
        public SixteenSegmentHelper(Graphics graphics, DigitalGauge gauge)
        {
            this._graphics = graphics;
            this.Gauge = gauge;
            // Top lines
            _segmentPoints[0] = new Point[] { new Point(3, 2), new Point(25, 2), new Point(21, 8), new Point(9, 8) };
            _segmentPoints[1] = new Point[] { new Point(27,2), new Point(49,2), new Point(43,8), new Point(31,8) };

            // Top left line
            _segmentPoints[2] = new Point[] { new Point(2, 3), new Point(8, 9), new Point(8, 32), new Point(2, 35) };

            // Top right line
            _segmentPoints[3] = new Point[] { new Point(50, 3), new Point(44, 9), new Point(44, 32), new Point(50, 35) };

            //Top left cross
            _segmentPoints[4] = new Point[] { new Point(9,9), new Point(13,9), new Point(22,28), 
                                              new Point(22,32), new Point(18,32),new Point(9,13)};

            //center vertical line - top - cross edged
            _segmentPoints[5] = new Point[] { new Point(26, 3), new Point(29, 8), new Point(29, 32),
                                              new Point(26, 35), new Point(23, 32), new Point(23, 8) };

            //Top right cross
            _segmentPoints[6] = new Point[] { new Point(43,9), new Point(43, 13), new Point(34, 32), 
                                              new Point(30, 32), new Point(30, 28),new Point(39,9)};

            //center horiz lines
            _segmentPoints[7] = new Point[] { new Point(3, 36), new Point(9, 33), new Point(21, 33), 
                                new Point(26, 36), new Point(21, 39), new Point(9, 39) };

            _segmentPoints[8] = new Point[] { new Point(26,36), new Point(31, 33), new Point(43, 33),
                                new Point(49, 36), new Point(43, 39), new Point(31, 39) };

            //bottom left line
            _segmentPoints[9] = new Point[] { new Point(2, 37), new Point(8, 40), new Point(8, 63), new Point(2, 69) };

            //bottom right line
            _segmentPoints[10] = new Point[] { new Point(50, 37), new Point(44, 40), new Point(44, 63), new Point(50, 69) };

            // Bottom left cross
            _segmentPoints[11] = new Point[] { new Point(22,40), new Point(22,44), new Point(13,63),
                                               new Point(9,63),new Point(9,59) ,new Point(18, 40)  };

            //center vertical line - bottom - cross edged
            _segmentPoints[12] = new Point[] { new Point(26, 37), new Point(29, 40), new Point(29, 64), 
                                               new Point(26, 69), new Point(23, 64), new Point(23, 40) };

            // Bottom right cross
            _segmentPoints[13] = new Point[] { new Point(30,40), new Point(34,40), new Point(43,59), 
                                               new Point(43,63),new Point(39,63),new Point(30,44) };

            //Bottom lines
            _segmentPoints[14] = new Point[] { new Point(9, 64), new Point(21, 64), new Point(25, 70), new Point(3, 70) };
            _segmentPoints[15] = new Point[] { new Point(31, 64), new Point(43, 64), new Point(49, 70), new Point(27, 70) };
        }

        /// <summary>
        /// Measures and returns the size of the given text size.
        /// </summary>
        /// <param name="text">string to measure</param>
        /// <param name="font">specified font to measure the string</param>
        /// <returns>SizeF</returns>
        public SizeF GetStringSize(string text, Font font)
        {
            SizeF sizef = new SizeF(0, _graphics.DpiX * font.SizeInPoints / 72);

            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsDigit(text[i]))
                    sizef.Width += 52 * _graphics.DpiX * font.SizeInPoints / 72 / 72;
                else if (text[i] == ':' || text[i] == '.')
                    sizef.Width += 12 * _graphics.DpiX * font.SizeInPoints / 72 / 72;
                else if (char.IsLetter(text[i]))
                    sizef.Width += 52 * _graphics.DpiX * font.SizeInPoints / 72 / 72;
                else
                    sizef.Width += 52 * _graphics.DpiX * font.SizeInPoints / 72 / 72;
            }
            return sizef;
        }

        public void DrawDigits(string text, Font font, Brush brush, Brush brushLight, float x, float y)
        {
            for (int cnt = 0; cnt < text.Length; cnt++)
            {
                if (Char.IsDigit(text[cnt]))
                    x = DrawDigit(text[cnt] - '0', font, brush, brushLight, x, y);
                else if (text[cnt] == ':')
                    x = DrawColon(font, brush, x, y);
                else if (text[cnt] == '.')
                    x = DrawDot(font, brush, x, y);
                else if (char.IsLetter(text[cnt]))
                    x = DrawText((char)text[cnt], font, brush, brushLight, x, y);
                else
                    x = DrawText(' ', font, brush, brushLight, x, y);
            }
        }

        private float DrawDigit(int num, Font font, Brush brush, Brush brushLight, float x, float y)
        {
            for (int cnt = 0; cnt < _segmentPoints.Length; cnt++)
            {
                if (_segmentData[num, cnt] == 1)
                {
                    FillPolygon(_segmentPoints[cnt], font, brush, x, y);
                }
                else
                {
                    FillPolygon(_segmentPoints[cnt], font, brushLight, x, y);
                }
            }
            if (Gauge.OverrideFontSize)
                x = x + (52 * _graphics.DpiX * font.SizeInPoints / 72 / 72);
            else
                x = x + (52 * _graphics.DpiX * font.SizeInPoints / 72 / 72) + Gauge.SegmentSpacing;

            return x;
            
        }

        private float DrawText(char text, Font font, Brush brush, Brush brushLight, float x, float y)
        {
            int num = Array.IndexOf(alpha, text);

            for (int cnt = 0; cnt < _segmentPoints.Length; cnt++)
            {
                if (_segmentTextData[num, cnt] == 1)
                {
                    FillPolygon(_segmentPoints[cnt], font, brush, x, y);
                }
                else
                {
                    FillPolygon(_segmentPoints[cnt], font, brushLight, x, y);
                }
            }
            if (Gauge.OverrideFontSize)
                x = x + (52 * _graphics.DpiX * font.SizeInPoints / 72 / 72);
            else
                x = x + (52 * _graphics.DpiX * font.SizeInPoints / 72 / 72) + Gauge.SegmentSpacing;

            return x;
        }

        private float DrawDot(Font font, Brush brush, float x, float y)
        {
            Point[][] dotPoints = new Point[1][];

            dotPoints[0] = new Point[] {new Point( 2, 64), new Point( 6, 61),
                                new Point(10, 64), new Point( 6, 69)};

            for (int cnt = 0; cnt < dotPoints.Length; cnt++)
            {
                FillPolygon(dotPoints[cnt], font, brush, x, y);
            }
            if (Gauge.OverrideFontSize)
                x = x + (12 * _graphics.DpiX * font.SizeInPoints / 72 / 72);
            else
                x = x + (12 * _graphics.DpiX * font.SizeInPoints / 72 / 72) + Gauge.SegmentSpacing;

            return x;
        }

        private float DrawColon(Font font, Brush brush, float x, float y)
        {
            Point[][] colonPoints = new Point[2][];

            colonPoints[0] = new Point[] { new Point(2, 21), new Point(6, 17), new Point(10, 21), new Point(6, 25) };
            colonPoints[1] = new Point[] { new Point(2, 51), new Point(6, 47), new Point(10, 51), new Point(6, 55) };

            for (int cnt = 0; cnt < colonPoints.Length; cnt++)
            {
                FillPolygon(colonPoints[cnt], font, brush, x, y);
            }
            if (Gauge.OverrideFontSize)
                x = x + (12 * _graphics.DpiX * font.SizeInPoints / 72 / 72);
            else
                x = x + (12 * _graphics.DpiX * font.SizeInPoints / 72 / 72) + Gauge.SegmentSpacing;

            return x;
        }

        private void FillPolygon(Point[] polygonPoints, Font font, Brush brush, float x, float y)
        {
            PointF[] polygonPointsF = new PointF[polygonPoints.Length];

            for (int cnt = 0; cnt < polygonPoints.Length; cnt++)
            {
                polygonPointsF[cnt].X = x + polygonPoints[cnt].X * _graphics.DpiX * font.SizeInPoints / 72 / 72;
                polygonPointsF[cnt].Y = y + polygonPoints[cnt].Y * _graphics.DpiY * font.SizeInPoints / 72 / 72;
            }
            _graphics.FillPolygon(brush, polygonPointsF);
        }
    }

    /// <summary>
    /// Helper class for the DotMatrixSegment Display
    /// </summary>
    public class DotMatrixHelper
    {
        Graphics _graphics;
        DigitalGauge Gauge;
        char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ".ToCharArray();
        char[] chars = ":.!".ToCharArray();

        #region Segment Data
        static byte[,] _segmentData = {
                              //0
                             {0,0,0,0,0,0,0,0,
                              0,0,1,1,1,1,0,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,0,1,1,1,1,0,0,
                              0,0,0,0,0,0,0,0},
                              //1
                             {0,0,0,0,0,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,1,1,0,0,0,
                              0,0,1,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,1,1,1,1,1,0,
                              0,0,0,0,0,0,0,0},
                              //2
                             {0,0,0,0,0,0,0,0,
                              0,0,1,1,1,1,0,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,1,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,1,0,0,0,0,
                              0,0,1,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,1,1,1,1,1,0,
                              0,0,0,0,0,0,0,0},
                              //3
                             {0,0,0,0,0,0,0,0,
                              0,1,1,1,1,1,0,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,1,1,1,1,0,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,1,1,1,1,1,0,0,
                              0,0,0,0,0,0,0,0},
                              //4
                             {0,0,0,0,0,0,0,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,1,1,0,
                              0,0,0,0,1,0,1,0,
                              0,0,0,1,0,0,1,0,
                              0,0,1,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,1,1,1,1,1,1,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,0,0},
                              //5
                             {0,0,0,0,0,0,0,0,
                              0,1,1,1,1,1,1,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,1,1,1,1,0,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,1,1,1,1,1,0,0,
                              0,0,0,0,0,0,0,0},
                              //6
                             {0,0,0,0,0,0,0,0,
                              0,0,0,1,0,0,0,0,
                              0,0,1,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,1,1,1,1,0,0,//
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,0,1,1,1,1,0,0,
                              0,0,0,0,0,0,0,0},
                              //7
                             {0,0,0,0,0,0,0,0,
                              0,1,1,1,1,1,1,0,
                              0,1,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,0,0},
                              //8
                             {0,0,0,0,0,0,0,0,
                              0,0,1,1,1,1,0,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,0,1,1,1,1,0,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,0,1,1,1,1,0,0,
                              0,0,0,0,0,0,0,0},
                              //9
                             {0,0,0,0,0,0,0,0,
                              0,0,1,1,1,1,0,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,0,1,1,1,1,0,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,1,1,1,1,0,0,
                              0,0,0,0,0,0,0,0}
                                      }; //0
        #endregion 

        #region Segment TextData

        static byte[,] _segmentTextData = {
                              //A
                             {0,0,0,0,0,0,0,0,
                              0,0,1,1,1,1,0,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,1,1,1,1,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,0,0,0,0,0,0,0},
                              //B
                             {0,0,0,0,0,0,0,0,
                              0,1,1,1,1,1,0,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,1,1,1,1,0,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,1,1,1,1,0,0,
                              0,0,0,0,0,0,0,0},
                              //C
                             {0,0,0,0,0,0,0,0,
                              0,0,1,1,1,1,1,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,0,1,1,1,1,1,0,
                              0,0,0,0,0,0,0,0},
                              //D
                             {0,0,0,0,0,0,0,0,
                              0,1,1,1,1,1,0,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,0,1,
                              0,1,0,0,0,0,0,1,
                              0,1,0,0,0,0,0,1,
                              0,1,0,0,0,0,0,1,
                              0,1,0,0,0,0,0,1,
                              0,1,0,0,0,0,0,1,
                              0,1,0,0,0,0,0,1,
                              0,1,0,0,0,0,0,1,
                              0,1,0,0,0,0,1,0,
                              0,1,1,1,1,1,0,0,
                              0,0,0,0,0,0,0,0},
                              //E
                             {0,0,0,0,0,0,0,0,
                              0,1,1,1,1,1,1,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,1,1,1,1,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,1,1,1,1,1,0,
                              0,0,0,0,0,0,0,0},
                              //F
                             {0,0,0,0,0,0,0,0,
                              0,1,1,1,1,1,1,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,1,1,1,1,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,0,0,0,0,0,0,0},
                              //G
                             {0,0,0,0,0,0,0,0,
                              0,0,1,1,1,1,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,1,1,1,1,0,//
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,0,1,1,1,1,0,0,
                              0,0,0,0,0,0,0,0},
                              //H
                             {0,0,0,0,0,0,0,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,1,1,1,1,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,0,0,0,0,0,0,0},
                              //I
                             {0,0,0,0,0,0,0,0,
                              0,0,1,1,1,1,1,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,1,1,1,1,1,0,
                              0,0,0,0,0,0,0,0},
                              //j
                             {0,0,0,0,0,0,0,0,
                              0,0,1,1,1,1,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,1,0,0,0,1,0,
                              0,0,1,0,0,0,1,0,
                              0,0,0,1,1,1,0,0,
                              0,0,0,0,0,0,0,0},
                              //k
                             {0,0,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,1,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,1,0,0,
                              0,1,0,0,1,0,0,0,
                              0,1,0,1,0,0,0,0,
                              0,1,1,0,0,0,0,0,
                              0,1,1,0,0,0,0,0,
                              0,1,0,1,0,0,0,0,
                              0,1,0,0,1,0,0,0,
                              0,1,0,0,0,1,0,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,0,1,
                              0,0,0,0,0,0,0,0},
                              //L
                             {0,0,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,1,1,1,1,1,0,
                              0,0,0,0,0,0,0,0},
                              //M
                             {0,0,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,1,
                              0,1,1,0,0,0,1,1,
                              0,1,0,1,0,1,0,1,
                              0,1,0,0,1,0,0,1,
                              0,1,0,0,0,0,0,1,
                              0,1,0,0,0,0,0,1,
                              0,1,0,0,0,0,0,1,
                              0,1,0,0,0,0,0,1,
                              0,1,0,0,0,0,0,1,
                              0,1,0,0,0,0,0,1,
                              0,1,0,0,0,0,0,1,
                              0,1,0,0,0,0,0,1,
                              0,0,0,0,0,0,0,0},
                              //N
                             {0,0,0,0,0,0,0,0,
                              0,1,1,1,1,1,0,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,0,0,0,0,0,0,0},
                              //O
                             {0,0,0,0,0,0,0,0,
                              0,0,1,1,1,1,0,0,
                              0,1,0,0,0,0,1,0,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              0,1,0,0,0,0,1,0,
                              0,0,1,1,1,1,0,0,
                              0,0,0,0,0,0,0,0},
                              //P
                             {0,0,0,0,0,0,0,0,
                              0,1,1,1,1,1,0,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,1,1,1,1,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,0,0,0,0,0,0,0},
                              //Q
                             {0,0,0,0,0,0,0,0,
                              0,0,1,1,1,1,0,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,1,1,0,
                              0,0,1,1,1,1,1,0,
                              0,0,0,0,0,0,0,1,
                              0,0,0,0,0,0,0,0},
                              //R
                             {0,0,0,0,0,0,0,0,
                               0,1,1,1,1,1,0,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,1,1,1,1,0,0,
                              0,1,1,0,0,0,0,0,
                              0,1,0,1,0,0,0,0,
                              0,1,0,0,1,0,0,0,
                              0,1,0,0,0,1,0,0,
                              0,1,0,0,0,0,1,0,
                              0,0,0,0,0,0,0,0},
                              //S
                             {0,0,0,0,0,0,0,0,
                              0,0,1,1,1,1,1,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,0,1,1,1,1,0,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,1,1,1,1,1,0,0,
                              0,0,0,0,0,0,0,0},
                              //T
                             {0,0,0,0,0,0,0,0,
                             0,1,1,1,1,1,1,1,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,0,0,0,0},
                              //U
                             {0,0,0,0,0,0,0,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,0,1,1,1,1,0,0,
                              0,0,0,0,0,0,0,0},
                              //V
                             {0,0,0,0,0,0,0,0,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              0,1,0,0,0,0,1,0,
                              0,0,1,0,0,1,0,0,
                              0,0,0,1,1,0,0,0,
                              0,0,0,0,0,0,0,0},
                              //W
                             {0,0,0,0,0,0,0,0,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              1,0,0,0,0,0,0,1,
                              1,0,0,1,1,0,0,1,
                              1,0,1,0,0,1,0,1,
                              1,1,0,0,0,0,1,1,
                              1,0,0,0,0,0,0,1,
                              0,0,0,0,0,0,0,0},
                              //X
                             {0,0,0,0,0,0,0,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,0,1,0,0,1,0,0,
                              0,0,0,1,1,0,0,0,
                              0,0,0,1,1,0,0,0,
                              0,0,1,0,0,1,0,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,1,0,0,0,0,1,0,
                              0,0,0,0,0,0,0,0},
                              //Y
                             {0,0,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,1,
                              0,1,0,0,0,0,0,1,
                              0,1,0,0,0,0,0,1,
                              0,1,0,0,0,0,0,1,
                              0,1,0,0,0,0,0,1,
                              0,1,0,0,0,0,0,1,
                              0,0,1,1,1,1,1,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,0,0,0,0,0},
                              //Z
                             {0,0,0,0,0,0,0,0,
                              0,1,1,1,1,1,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,0,1,0,
                              0,0,0,0,0,1,0,0,
                              0,0,0,0,1,0,0,0,
                              0,0,0,1,0,0,0,0,
                              0,0,1,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,0,0,0,0,0,0,
                              0,1,1,1,1,1,1,0,
                              0,0,0,0,0,0,0,0},
                              //space
                             {0,0,0,0,0,0,0,0,
                              0,0,0,0,0,0,0,0,
                              0,0,0,0,0,0,0,0,
                              0,0,0,0,0,0,0,0,
                              0,0,0,0,0,0,0,0,
                              0,0,0,0,0,0,0,0,
                              0,0,0,0,0,0,0,0,
                              0,0,0,0,0,0,0,0,
                              0,0,0,0,0,0,0,0,
                              0,0,0,0,0,0,0,0,
                              0,0,0,0,0,0,0,0,
                              0,0,0,0,0,0,0,0,
                              0,0,0,0,0,0,0,0,
                              0,0,0,0,0,0,0,0}
                                      }; //0
        #endregion

        #region CharData
        static byte[,] _charData = {
                              //:
                             {0,
                              0,
                              0,
                              0,
                              1,
                              0,
                              0,
                              0,
                              0,
                              1,
                              0,
                              0,
                              0,
                              0,},
                              //.
                             {0,
                              0,
                              0,
                              0,
                              0,
                              0,
                              0,
                              0,
                              0,
                              0,
                              0,
                              0,
                              1,
                              0,},
                              //!
                               //.
                             {0,
                              1,
                              1,
                              1,
                              1,
                              1,
                              1,
                              1,
                              1,
                              1,
                              0,
                              0,
                              1,
                              0,}
                               };
        #endregion

        readonly Point[][] _segmentPoints = new Point[112][];
        readonly Point[][] _charPoints = new Point[14][];

        /// <summary>
        /// Initiates a new instance for the DotMatrixHelper class
        /// </summary>
        /// <param name="graphics">graphics of the control</param>
        /// <param name="gauge">owner control [Gauge]</param>
        public DotMatrixHelper(Graphics graphics, DigitalGauge gauge)
        {
            this._graphics = graphics;
            this.Gauge = gauge;
            int xi=2,yi=2,lineno=0;
            for (int i = 0; i < 112; i++)
            {
                if ((i / 8) == lineno)
                {
                    _segmentPoints[i] = new Point[] { new Point(xi,yi), new Point(xi+4,yi), new Point(xi+4, yi+4), new Point(xi,yi+4) };
                    xi += 5;
                }
                else
                {
                    lineno = (i / 8);
                    xi = 2;
                    yi += 5;
                    _segmentPoints[i] = new Point[] { new Point(xi, yi), new Point(xi + 4, yi), new Point(xi + 4, yi + 4), new Point(xi, yi + 4) };
                    xi += 5;
                }
            }

            int xd = 2, yd = 2;
            for (int d = 0; d < 14; d++)
            {
                _charPoints[d] = new Point[] { new Point(xd, yd), new Point(xd + 4, yd), new Point(xd + 4, yd + 4), new Point(xd, yd + 4) };
                yd += 5;
            }
        }

        /// <summary>
        /// Measures and returns the size of the given text size.
        /// </summary>
        /// <param name="text">string to measure</param>
        /// <param name="font">specified font to measure the string</param>
        /// <returns>SizeF</returns>
        public SizeF GetStringSize(string text, Font font)
        {
            SizeF sizef = new SizeF(0, _graphics.DpiX * font.SizeInPoints / 72);

            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsDigit(text[i]))
                    sizef.Width += 40 * _graphics.DpiX * font.SizeInPoints / 72 / 72;
                else if (text[i] == ':' || text[i] == '.')
                    sizef.Width += 12 * _graphics.DpiX * font.SizeInPoints / 72 / 72;
                else if (char.IsLetter(text[i]))
                    sizef.Width += 40 * _graphics.DpiX * font.SizeInPoints / 72 / 72;
                else
                    sizef.Width += 40 * _graphics.DpiX * font.SizeInPoints / 72 / 72;
            }
            return sizef;
        }

        public void DrawDigits(string text, Font font, Brush brush, Brush brushLight, float x, float y)
        {
            for (int cnt = 0; cnt < text.Length; cnt++)
            {
                if (Char.IsDigit(text[cnt]))
                    x = DrawDigit(text[cnt] - '0', font, brush, brushLight, x, y);
                else if (text[cnt] == ':' || text[cnt] == '.' || text[cnt] == '!')
                    x = DrawChar(text[cnt],font, brush, brushLight, x, y);
                else if (char.IsLetter(text[cnt]))
                    x = DrawText((char)text[cnt], font, brush, brushLight, x, y);
                else
                    x = DrawText(' ', font, brush, brushLight, x, y);
            }
        }

        private float DrawDigit(int num, Font font, Brush brush, Brush brushLight, float x, float y)
        {
            for (int cnt = 0; cnt < _segmentPoints.Length; cnt++)
            {
                if (_segmentData[num, cnt] == 1)
                {
                    FillPolygon(_segmentPoints[cnt], font, brush, x, y);
                }
                else
                {
                    FillPolygon(_segmentPoints[cnt], font, brushLight, x, y);
                }
            }
            if (Gauge.OverrideFontSize)
                x = x + (40 * _graphics.DpiX * font.SizeInPoints / 72 / 72);
            else
                x = x + (40 * _graphics.DpiX * font.SizeInPoints / 72 / 72) + Gauge.SegmentSpacing;

            return x;

        }

        private float DrawText(char text, Font font, Brush brush, Brush brushLight, float x, float y)
        {
            int num = Array.IndexOf(alpha, text);

            for (int cnt = 0; cnt < _segmentPoints.Length; cnt++)
            {
                if (_segmentTextData[num, cnt] == 1)
                {
                    FillPolygon(_segmentPoints[cnt], font, brush, x, y);
                }
                else
                {
                    FillPolygon(_segmentPoints[cnt], font, brushLight, x, y);
                }
            }
            if (Gauge.OverrideFontSize)
                x = x + (40 * _graphics.DpiX * font.SizeInPoints / 72 / 72);
            else
                x = x + (40 * _graphics.DpiX * font.SizeInPoints / 72 / 72) + Gauge.SegmentSpacing;

            return x;
        }

        private float DrawChar(char c,Font font, Brush brush, Brush brushLight, float x, float y)
        {
            int num = Array.IndexOf(chars, c);
            
            for (int cnt = 0; cnt < _charPoints.Length; cnt++)
            {
                if (_charData[num, cnt] == 1)
                {
                    FillPolygon(_charPoints[cnt], font, brush, x, y);
                }
                else
                {
                    FillPolygon(_charPoints[cnt], font, brushLight, x, y);
                }
            }

            if (Gauge.OverrideFontSize)
                x = x + (12 * _graphics.DpiX * font.SizeInPoints / 72 / 72);
            else
                x = x + (12 * _graphics.DpiX * font.SizeInPoints / 72 / 72) + Gauge.SegmentSpacing;

            return x;
        }

        private void FillPolygon(Point[] polygonPoints, Font font, Brush brush, float x, float y)
        {
            PointF[] polygonPointsF = new PointF[polygonPoints.Length];

            for (int cnt = 0; cnt < polygonPoints.Length; cnt++)
            {
                polygonPointsF[cnt].X = x + polygonPoints[cnt].X * _graphics.DpiX * font.SizeInPoints / 72 / 72;
                polygonPointsF[cnt].Y = y + polygonPoints[cnt].Y * _graphics.DpiY * font.SizeInPoints / 72 / 72;
            }
            _graphics.FillPolygon(brush, polygonPointsF);
        }
    }

    #endregion

    #region Designer class
    /// <summary>
    /// Desginer class for DigitalGauge
    /// </summary>
    public class DigitalGaugeDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        /// <summary>
        /// Designer ActionList collection
        /// </summary>
        private System.ComponentModel.Design.DesignerActionListCollection actionLists;

        /// <summary>
        ///  Initializes a new instance of the CheckBoxAdvDesigner class
        /// </summary>
        public DigitalGaugeDesigner()
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
                    this.actionLists.Add(new DigitalGaugeActionList(this.Component));
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
    /// Designer action list of DigitalGauge
    /// </summary>
    public class DigitalGaugeActionList : SyncActionListBase<DigitalGauge>
    {
        /// <summary>
        /// Initializes a new instance of the DigitalGauge class.
        /// </summary>
        /// <param name="component"> Represents component</param>
        public DigitalGaugeActionList(IComponent component)
            : base(component)
        {
        }


        /// <summary>
        /// Gets or sets a value to choose the segment type for Gauge
        /// </summary>
        public CharacterType CharacterType
        {
            get
            {
                CharacterType _characterType = CharacterType.SevenSegment;
                if (this.Control != null)
                {
                    DigitalGauge control = this.Control as DigitalGauge;
                    _characterType = control.CharacterType;
                }
                return _characterType;
            }
            set
            {
                SetValue("CharacterType", value);
            }
        }

        /// <summary>
        /// Gets or sets a number of characters to be displayed in the Gauge.
        /// </summary>
        public int CharacterCount
        {
            get
            {
                int _characterCount = 0;
                if (this.Control != null)
                {
                    DigitalGauge control = this.Control as DigitalGauge;
                    _characterCount = control.CharacterCount;
                }
                return _characterCount;
            }
            set
            {
                SetValue("CharacterCount", value);
            }
        }
        /// <summary>
        /// Gets or sets a value to be displayed in the Gauge.
        /// </summary>
        public string Value
        {
            get
            {
                string _value = "0000";
                if (this.Control != null)
                {
                    DigitalGauge control = this.Control as DigitalGauge;
                    _value = control.Value;
                }
                return _value;
            }

            set
            {
                SetValue("Value", value);
            }
        }
        /// <summary>
        /// Specifies the visual style for Gauge.
        /// </summary>
        public ThemeStyle VisualStyle
        {
            get
            {
                ThemeStyle _visualStyle = ThemeStyle.None;
                if (this.Control != null)
                {
                    DigitalGauge control = this.Control as DigitalGauge;
                    _visualStyle = control.VisualStyle;
                }
                return _visualStyle;
            }

            set
            {
                SetValue("VisualStyle", value);
            }
        }

        /// <summary>
        /// Gets or sets a value to show / hide the disabled segments in the Gauge.
        /// </summary>
        public bool ShowInvisibleSegments
        {
            get
            {
                bool _showInvisibleSegments = false;
                if (this.Control != null)
                {
                    DigitalGauge control = this.Control as DigitalGauge;
                    _showInvisibleSegments = control.ShowInvisibleSegments;
                }
                return _showInvisibleSegments;
            }

            set
            {
                SetValue("ShowInvisibleSegments", value);
            }
        }
        /// <summary>
        /// Overrridden InitializeActionList.
        /// </summary>
        protected override void InitializeActionList()
        {
            // Customization Category
            this.AddDesignerActionHeaderItem("Behavior");
            this.AddDesignerActionPropertyItem("CharacterType", "CharacterType", "Behavior", "Gets or sets a value to choose the segment type for Gauge.");
            this.AddDesignerActionPropertyItem("CharacterCount", "CharacterCount", "Behavior", "Gets or sets a number of digits to be displayed in the Gauge.");
            this.AddDesignerActionHeaderItem("Data");
            this.AddDesignerActionPropertyItem("Value", "Value", "Data", "Gets or sets a value to be displayed in the Gauge.");
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("VisualStyle", "VisualStyle", "Appearance", "Specifies the visual style for Gauge.");
            this.AddDesignerActionPropertyItem("ShowInvisibleSegments", "ShowInvisibleSegments", "Appearance", "Gets or sets a value to show / hide the disabled segments in the Gauge.");
        }
    }
    #endregion
}
