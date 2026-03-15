#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
# region file using directive

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using System.Drawing.Design;
using System.Drawing.Drawing2D;

#endregion

namespace Syncfusion.Windows.Forms.Tools
{
    
    # region Enum
    /// <summary>
    /// Holds option for ButtonMode
    /// </summary>
    public enum ButtonMode
    {
        Normal,
        Toggle
    }
    /// <summary>
    /// Holds option for Style
    /// </summary>
    public enum SplitButtonVisualStyle
    {
        Default,
        Metro
    }
    public enum Position
    {       /// <summary>
        /// Specifies  the DropDown position on Top 
        /// </summary>
        Top,
        /// <summary>
        /// Specifies  the DropDown position on Bottom
        /// </summary>
        Bottom,
        /// <summary>
        ///Specifies  the DropDown position on Left 
        /// </summary>
        Left,
        /// <summary>
        ///Specifies  the DropDown position on Right 
        /// </summary>
        Right
    }
    # endregion

    # region Icon
 
    [
    ToolboxItem(true),
    ToolboxBitmap(typeof(SplitButton), "ToolboxIcons.SplitButton.bmp"),
    Description("Button With DropDown.")
    ]

    #endregion

    # region SplitButton

    public class SplitButton : Control
    {
        #region Class members
        /// <summary>
        /// Button mode as toogle or normal
        /// </summary>
        private ButtonMode mode;
        /// <summary>
        /// Color code for Button's Pressed State
        /// </summary>
        public string[] PressedColor = new string[13] { "#78BDE2", "#7FC2E5", "#86C6E8", "#8CCAEB", "#93CEED", "#98D1EF", "#C4E5F6", "#C9E7F7", "#CEE9F8", "#D3ECF9", "#D8EEFA", "#DDF0FA", "#E1F2FB" };
        /// <summary>
        ///  Color code for Button's Actived State
        /// </summary>
        public string[] OnoverColor = new string[13] { "#ACDCF7", "#AFDEF8", "#B2E0F9", "#B5E2FA", "#B9E3FB", "#BCE5FC", "#BEE6FD", "#D9F0FC", "#DCF1FC", "#DEF2FC", "#E1F3FC", "#E4F4FC", "#E6F5FD" };
        /// <summary>
        ///  Color code for Button's Disabled State
        /// </summary>
        public string[] DisabledColor = new string[13] { "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", };
        /// <summary>
        ///  Color code for Button's DefaultColor State
        /// </summary>
        public string[] DefaultColor = new string[13] { "#D2D2D2", "#D4D4D4", "#D6D6D6", "#D8D8D8", "#DADADA", "#DBDBDB", "#DDDDDD", "#EBEBEB", "#ECECEC", "#EDEDED", "#EFEFEF", "#F0F0F0", "#F1F1F1" };
        /// <summary>
        /// Button Initiallizing on load
        /// </summary> 
        private bool initial = true;
        /// <summary>
        /// Assign initial Color to the Arrow
        /// </summary>
        private Color sbArrowColor = Color.Black;
        /// <summary>
        /// Assign initial color to the ArrowButton Background
        /// </summary>
        public SolidBrush ArrowBColor = new SolidBrush(Color.Transparent);
        /// <summary>
        /// Inidicates Button state
        /// </summary>
        private bool isChecked = false;
        /// <summary>
        /// Show  the DropDown on Button click 
        /// </summary>
        private bool showDropDownOnButtonClick = false;
        /// <summary>
        /// Position of the DropDown
        /// </summary>
        private Position dropDownPosition = Position.Bottom;
        /// <summary>
        /// Location of the mouse pointer on the button (x coordinates value)
        /// </summary>
        private int locationX;
        /// <summary>
        /// Location of the mouse pointer on the button (y coordinates value)
        /// </summary>
        private int locationY;
        /// <summary>
        /// Initial Button Border Color
        /// </summary>
        private Color buttonOuterColor = Color.Gray;
        /// <summary>
        /// Initial Button Shadow Color
        /// </summary>
        private Color buttonInnerColor = Color.White;
        /// <summary>
        /// Color values for button when it is in normal State
        /// </summary>
        private string[] buttonColors = new string[13] { "#D2D2D2", "#D4D4D4", "#D6D6D6", "#D8D8D8", "#DADADA", "#DBDBDB", "#DDDDDD", "#EBEBEB", "#ECECEC", "#EDEDED", "#EFEFEF", "#F0F0F0", "#F1F1F1" };
        /// <summary>
        /// Color values for button when it is in normal State
        /// </summary>
        private string[] arrowColors = new string[13] { "#D2D2D2", "#D4D4D4", "#D6D6D6", "#D8D8D8", "#DADADA", "#DBDBDB", "#DDDDDD", "#EBEBEB", "#ECECEC", "#EDEDED", "#EFEFEF", "#F0F0F0", "#F1F1F1" };
        /// <summary>
        /// Button's Arrow part Border color
        /// </summary>
        private Color arrowOuterColor = Color.Transparent;
        /// <summary>
        /// Button's Arrow part Shadow color
        /// </summary>
        private Color arrowInnerColor = Color.Transparent;
        /// <summary>
        /// Initial Spliter Color
        /// </summary>
        public Color SplitterInnerColor = Color.Transparent;
        /// <summary>
        /// spliter shadow color
        /// </summary>
        private ContextMenuStripEx buttonDropDown;
        /// <summary>
        /// BackColor
        /// </summary>
        protected Color prev_backcolor = SystemColors.Control;
        
        /// <summary>
        /// Button toogle mode indicator
        /// </summary>
        public bool ButtonToogleMode = false;
        /// <summary>
        /// Inidicate That it has MousePointer
        /// </summary>
        private bool hasMousePointer = false ;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

        #endregion

        # region Events

        /// <summary>
        /// Checked EventHandler for ToogleButton
        /// </summary>
        public event EventHandler Checked;
        /// <summary>
        /// UnChecked EventHandler for ToogleButton
        /// </summary>
        public event EventHandler UnChecked;
        /// <summary>
        /// DropDownItemEventHandler , when the dropdown item is clicked.
        /// </summary>
        public event ToolStripItemClickedEventHandler DropDowItemClicked
        {
            add
            {
                this.buttonDropDown.ItemClicked += value;
            }
            remove
            {
                this.buttonDropDown.ItemClicked -= value;
            }
        }

        #endregion

        # region ShouldSerialize

        protected bool ShouldSerializeButtonMode()
        {
            return this.ButtonMode != ButtonMode.Normal  ;
        }
        protected bool ShouldSerializeIsButtonChecked()
        {
            return this.IsButtonChecked != false;
        }
        protected void ResetButtonMode()
        {
            this.ButtonMode = ButtonMode.Normal;
        }
        protected bool ShouldSerializeDropDownIconColor()
        {
            return this.DropDownIconColor != System.Drawing.Color.Black;
        }
        protected bool ShouldSerializeRenderer()
        {
            return this.Renderer != null;
        }
        #endregion

        # region Properities
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
        /// Gets or sets value to enable or disable the Touchmode to the controls.
        /// </summary>
        /// <remarks>Scale factor will be updated automatically if scalefactor is equal to 1</remarks>
        [Browsable(true),DefaultValue(false),
        Category("Layout"), Description("Gets or sets value to enable or disable the Touchmode to the controls."),
    ]
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

        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
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

        public Color DropDownIconColor
        {
            get
            {
                return sbArrowColor ;
            }
            set
            {
                if (sbArrowColor  != value)
                {
                    sbArrowColor  = value;
                    if(sbArrowColor != Color.White )
                    originalArrowColor = sbArrowColor;
                }
                this.Invalidate();
            }
        }
        /// <summary>
        /// Holds the DropDown Items
        /// </summary>
        public ToolStripItemCollection DropDownItems
        {
            get
            {
                return this.buttonDropDown.Items;
            }
        }

        /// <summary>
        ///  get or set the value for VisualStyle
        /// </summary>
         [
            Category("Appearance")
         ]
        private SplitButtonVisualStyle style = SplitButtonVisualStyle.Default;
        public SplitButtonVisualStyle Style
        {
            get { return style; }
            set
            {
                style = value;
                if (style == SplitButtonVisualStyle.Default)
                {
                    Renderer = new SplitButtonRenderer();
                    if (this.DesignMode)
                        this.BackColor = SystemColors.Control;
                    this.ForeColor = Color.Black;
                    this.sbArrowColor = Color.Black;
                    this.buttonDropDown.Style = ContextMenuStripEx.ContextMenuStyle.Default;
                }
                else
                {
                    this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            
                    Renderer = new MetroSplitButtonRenderer();
                    this.ForeColor = Color.White;
                    if(this.DesignMode)
                        this.BackColor = ColorTranslator.FromHtml("#16A5DC");
                    this.buttonDropDown.Style = ContextMenuStripEx.ContextMenuStyle.Metro;
                    this.buttonDropDown.MetroColor = ControlPaint.LightLight(BackColor);
                    this.sbArrowColor = Color.White;
                }
            }
        }
           [
          Description("Show the DropDown on ButtonClick"),
          Category("Appearance")
        ]
        public bool ShowDropDownOnButtonClick
        {
            get { return showDropDownOnButtonClick; }
            set { showDropDownOnButtonClick = value; }
        }
        [
          Description("Specifies the position for the DropDown"),
          Category("Appearance")
        ]
        public Position DropDownPosition
        {
            get { return dropDownPosition; }
            set
            {
                dropDownPosition = value;
            }
        }
        ///<summary>
        /// get or set the value for ButtonToogleMode
        /// </summary>
        ///  
        [
           Description("Mode for the Button, either Normal or Toogled"),
           Category("Appearance")
         ]
        public ButtonMode ButtonMode
        {
            get
            {
                return mode;
            }
            set
            {
                if (mode != value)
                {
                    mode = value;
                    if (mode == ButtonMode.Toggle)
                        ButtonToogleMode = true;
                    else
                        ButtonToogleMode = false;

                    Invalidate();
                }

            }
        }
        /// <summary>
        /// Button Rendering Property
        /// </summary>
        private ISplitButtonRenderer renderer;

        public ISplitButtonRenderer Renderer
        {
            get
            {
                return renderer;
            }
            set
            {
                if (renderer != value)
                {
                    renderer = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// State of the Button Either Checked or UnChecked
        /// </summary>
        [
          Description("Checked or UnChecked State of the Button"),
          Category("Appearance")
        ]
        public  bool IsButtonChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    FireCheckedEvent(isChecked);

                }
                this.Invalidate();
                initial = true;
            }
        }

        #endregion

        # region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public SplitButton()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(SplitButton));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            Renderer = new SplitButtonRenderer();
            buttonDropDown = new ContextMenuStripEx();
            buttonDropDown.MouseEnter += new EventHandler(buttonDropDown_MouseEnter);
            buttonDropDown.MouseLeave += new EventHandler(buttonDropDown_MouseLeave);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor |
        ControlStyles.OptimizedDoubleBuffer |
        ControlStyles.AllPaintingInWmPaint |
        ControlStyles.UserPaint, true);
            metroArrowBackColor = this.BackColor;
            metroBackColor = this.BackColor;
            CTRLSIZE = new Size(75,23);
            this.MinimumSize = new Size(75,23);
        }
        #endregion

        # region Methods

        /// <summary>
        /// Fire while ButtonDropDown get Focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void buttonDropDown_MouseEnter(object sender, EventArgs e)
        {
            arrowOuterColor = Color.SteelBlue;
            arrowInnerColor = Color.Gray;
            arrowColors = PressedColor;
            this.Invalidate();
        }

        /// <summary>
        ///  Fire while ButtonDropDown lost Focus
        ///  </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void buttonDropDown_MouseLeave(object sender, EventArgs e)
        {
            arrowOuterColor = Color.Transparent;
            arrowInnerColor = Color.Transparent;
            arrowColors = DefaultColor;
            this.Invalidate();
        }

        /// <summary>
        ///It performs Button painting as setting Border Color, BackGround Color, Arrow Color, using Button Renderer
        /// </summary>
        /// <param name="e"></param>

        protected void PaintButton(PaintEventArgs e)
        {


            if (initial)
            {
                if (ButtonToogleMode)
                {
                    if (isChecked)
                    {
                        checkedcolors();
                    }
                    else
                    {
                        if (hasMousePointer)
                            uncheckedcolors();
                        else
                            defaultColors();
                        SplitterInnerColor = Color.Transparent;
                        
                    }
                }
                initial = false;
            }

            Point textpoint = new Point(10, 10);
            int StartPoint = this.Width - 15;
            int EndPoint = this.Width - 5;
            int HeightPoint = this.Height / 2;

            System.Drawing.Brush background = new System.Drawing.SolidBrush(System.Drawing.Color.White);
           
                try
                {
                    if (BackgroundImage == null)
                    {
                        if (this.Style == SplitButtonVisualStyle.Default)
                        {
                            GetGradientcolors(e, 0, 0, this.Width - 20, this.Height, 90, buttonColors, this.BackColor);
                            GetGradientcolors(e, this.Width - 20, 0, this.Width - 20, this.Height, 90, arrowColors, this.BackColor);
                        }
                        else
                        {
                            
                            SolidBrush solidBrush = new SolidBrush(Color.White);
                            e.Graphics.FillRectangle(solidBrush, new Rectangle(0, 0, this.Width, this.Height));
                            solidBrush = new SolidBrush( this.metroBackColor );
                            e.Graphics.FillRectangle(solidBrush, new Rectangle(0, 0, this.Width-20, this.Height));
                            solidBrush = new SolidBrush(metroArrowBackColor );
                            e.Graphics.FillRectangle(solidBrush, new Rectangle(this.Width - 20, 0, this.Width, this.Height));
                           // GetGradientcolors(e, this.Width - 20, 0, this.Width - 20, this.Height, 90, metroArrowBackColor , this.BackColor);

                            solidBrush.Dispose();
                        }
                    }
                    
                    Renderer.DrawBorder(e, this.Width, this.Height, 20, buttonOuterColor, buttonInnerColor, arrowOuterColor, arrowInnerColor, SplitterInnerColor);
                    Renderer.DrawArrow(this.Width - 20, 0, 20, this.Height, e, sbArrowColor);
                }
                catch (Exception f)
                {
                    MessageBox.Show(Convert.ToString(f));
                }
                try
                {                                      
                    Renderer.DrawText(e, this.Text, this.Font, this.ForeColor, this.Width, this.Height, 20);
                }
                catch (Exception f)
                {
                    MessageBox.Show("Exception " + Convert.ToString(f));
                }
         }

        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {

                base.BackColor = value;

                this.metroBackColor = Color.FromArgb (200,base.BackColor );
                this.metroArrowBackColor = this.metroBackColor;
                this.buttonDropDown.MetroColor = this.BackColor;
            }
        }
        /// <summary>
        ///
        /// </summary>

       // private Color metroArrowBackColor = Color.Red;
        private Color metroArrowBackColor = Color.Red;
        private Color metroBackColor = Color.Red;
        /// <summary>
        /// Perform Button Back Color
        /// </summary>
        /// <param name="e"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="angle"></param>
        /// <param name="gcolors"></param>
        /// <param name="backcolor"></param>

        public void GetGradientcolors(PaintEventArgs e, int x, int y, int width, int height, int angle, string[] gcolors, Color backcolor)
        {
            if (backcolor == SystemColors.Control)
            {
                int controlheight;
                Color startcolor;
                Color endcolor;

                if (height > 0)
                    controlheight = height / 13;
                else
                    controlheight = 1;

                height = controlheight;
                for (int count = 12; count >= 0; count--)
                {
                    startcolor = ColorTranslator.FromHtml(gcolors[count]);
                    endcolor = startcolor;
                    if (height != 0) 
                    {
                        if (width != 0)
                        {
                            Brush linearGradientBrush = new LinearGradientBrush(
                            new Rectangle(x, y, width, height), endcolor, startcolor, angle);
                            e.Graphics.FillRectangle(linearGradientBrush, new Rectangle(x, y, width, height));
                            linearGradientBrush.Dispose();
                        }
                    }
                    height = height + controlheight;
                    y = y + controlheight;

                }
            }
            else
            {
                int controlheight;
                Color startcolor;
                Color endcolor;
                if (height > 0)
                    controlheight = height / 13;
                else
                    controlheight = 1;

                height = controlheight;
                for (int count = 12; count >= 0; count--)
                {
                    startcolor = endcolor=backcolor;
                    if (height != 0)
                    {
                        if (width != 0)
                        {
                            Brush linearGradientBrush = new LinearGradientBrush(
                            new Rectangle(x, y, width, height), endcolor, startcolor, angle);
                            e.Graphics.FillRectangle(linearGradientBrush, new Rectangle(x, y, width, height));
                            linearGradientBrush.Dispose();
                        }
                    }
                    height = height + controlheight;
                    y = y + controlheight;
                }
            } 
        }

        private Color originalArrowColor = Color.Black;
        /// <summary>
        /// Border color and Background Color while the Button is in normal state
        /// </summary>
      
        protected void SetNormalStateColor()
        {
            if (ButtonToogleMode)
            {
                if (!isChecked)
                {
                    uncheckedcolors();
                    this.metroBackColor = Color.FromArgb(200, this.BackColor);
                }
                else
                {
                    checkedcolors();
                    this.metroBackColor = this.BackColor;
                }
            }
            else
            {
                this.metroBackColor = Color.FromArgb(200, this.BackColor);
            }
        }

        #endregion

        #region Override

        /// <summary></summary>
        /// <param name="mevent"></param>
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
          
            base.OnMouseDown(mevent);
            if (mevent.Button  == MouseButtons.Left)
            {
                if ((locationX > this.Width - 20))
                {
                    arrowColors = PressedColor;
                    arrowOuterColor = Color.SteelBlue;
                    metroArrowBackColor = this.BackColor;
                    if (this.Style == SplitButtonVisualStyle.Metro)
                    this.DropDownIconColor = Color.Black;
                    arrowInnerColor = Color.Gray;
                    showDropDown();
                }
                else if (ShowDropDownOnButtonClick)
                {
                    arrowColors = PressedColor;
                    arrowOuterColor = Color.SteelBlue;
                    metroArrowBackColor = this.BackColor;
                    if (this.Style == SplitButtonVisualStyle.Metro)
                        this.DropDownIconColor = Color.Black;
                    arrowInnerColor = Color.Gray;
                    showDropDown();
                    this.metroBackColor = this.BackColor;
                    buttonOuterColor = Color.SteelBlue;
                    buttonInnerColor = SplitterInnerColor = Color.Gray;
                    arrowColors = OnoverColor;
                    buttonColors = PressedColor;
                }
                else
                {
                    this.metroBackColor = this.BackColor ;
                    buttonOuterColor = Color.SteelBlue;
                    buttonInnerColor = SplitterInnerColor = Color.Gray;
                    arrowColors = OnoverColor;
                    buttonColors = PressedColor;
                }
                this.Invalidate();
            }
        }

        internal bool isButtonDropDownShowed = false;

        private void showDropDown()
        {
            if (DropDownPosition == Position.Bottom && !buttonDropDown.Visible)
            {
                this.buttonDropDown.Show(this, new Point(0, Height));
            }
            else if (DropDownPosition == Position.Right && !buttonDropDown.Visible)
            {
                this.buttonDropDown.Show(this, new Point(Width, 0));
            }
            else if (DropDownPosition == Position.Left && !buttonDropDown.Visible)
            {
                this.buttonDropDown.Show(this, new Point((0 - buttonDropDown.Width), 0));
            }
            else if (DropDownPosition == Position.Top && !buttonDropDown.Visible)
            {
               this.buttonDropDown.Show(this, new Point(0, (0 - buttonDropDown.Height)));
               if (!isButtonDropDownShowed)
                   relocatedDropDown();
            }
            else
                this.buttonDropDown.Hide();
            isButtonDropDownShowed = true;
        }
        private void relocatedDropDown()
        {
            this.buttonDropDown.Refresh();
            this.buttonDropDown.Show(this, new Point(0, (0 - buttonDropDown.Height)));
        }
        /// <summary></summary>
        /// <param name="e"></param>
        protected override void OnClick(EventArgs e)
        {
            if (locationX <= this.Width - 20)
            {
                if (!ButtonToogleMode)
                {
                    base.OnClick(e);
                }
                else
                {
                    IsButtonChecked = !IsButtonChecked;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isCheck"></param>
        private void FireCheckedEvent(bool isCheck)
        {
            if (isCheck)
            {
                if (Checked != null)
                    Checked(this, new EventArgs());
            }
            else
            {
                if (UnChecked != null)
                    UnChecked(this, new EventArgs());
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            hasMousePointer = true;
            base.OnMouseEnter(e);
            arrowColors = buttonColors = OnoverColor;
            buttonOuterColor = arrowOuterColor = Color.SteelBlue;
            arrowInnerColor = buttonInnerColor = SplitterInnerColor = Color.White;
            if (ButtonToogleMode)
            {
                if (!isChecked)
                {
                    uncheckedcolors();
                }
                else
                {
                    checkedcolors();
                    SplitterInnerColor = Color.Gray;
                }
            }
            this.Invalidate();
        }
        /// <summary>
        /// 
        /// </summary>
        private void checkedcolors()
        {
            this.metroBackColor = this.BackColor;
            buttonOuterColor = Color.SteelBlue;
            buttonInnerColor = Color.Gray;
            arrowColors = buttonColors = PressedColor;
      
        }
        /// <summary>
        /// 
        /// </summary>
        private void uncheckedcolors()
        {
            buttonOuterColor = Color.SteelBlue;
            buttonInnerColor = SplitterInnerColor = Color.White;
            arrowColors = buttonColors = OnoverColor;
        }
        /// <summary></summary>
        /// <param name="mevent"></param>
        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            base.OnMouseMove(mevent);
            locationX = mevent.X;
            locationY = mevent.Y;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnTextChanged(e);
        }
        /// <summary></summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            hasMousePointer = false;
            base.OnMouseLeave(e);
            defaultColors();
            if (ButtonToogleMode)
            {
                if (isChecked)
                {
                    checkedcolors();
                    SplitterInnerColor = arrowInnerColor = Color.Transparent;
                }
            }

            this.Invalidate();
        }
        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
        }
        private void defaultColors()
        {
            if (Style == SplitButtonVisualStyle.Default)
            {
                arrowColors = buttonColors = DefaultColor;
                this.DropDownIconColor = originalArrowColor;
                this.metroArrowBackColor = Color.FromArgb(200, base.BackColor);
                this.metroBackColor = Color.FromArgb(200, base.BackColor);
                buttonOuterColor = Color.Gray;
                buttonInnerColor = Color.White;
                arrowOuterColor = arrowInnerColor = SplitterInnerColor = Color.Transparent;
                ArrowBColor = new SolidBrush(Color.Transparent);
                ForeColor = Color.Black;
            }
            else
            {
                this.ForeColor = Color.White;
                this.BackColor = this.BackColor;// ColorTranslator.FromHtml("#16A5DC");
                this.buttonDropDown.Style = ContextMenuStripEx.ContextMenuStyle.Metro;
                this.buttonDropDown.MetroColor = ControlPaint.LightLight(BackColor);
                this.sbArrowColor = Color.White;
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            hasMousePointer = false;
            base.OnMouseLeave(e);
            defaultColors();
            if (ButtonToogleMode)
            {
                if (isChecked)
                {
                    checkedcolors();
                    SplitterInnerColor = arrowInnerColor = Color.Transparent;
                }
            }

            this.Invalidate();
        }
        /// <summary>
        ///Font changed
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }

        /// <summary></summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.Enabled)
            {
                buttonOuterColor = Color.Gray;
                arrowInnerColor = arrowOuterColor = buttonInnerColor = Color.Transparent;
                if(this.Style == SplitButtonVisualStyle.Metro)
                    sbArrowColor = ForeColor = Color.White;
                else
                    sbArrowColor = ForeColor = Color.Black;
                PaintButton(e);
            }
            else
            {
                buttonOuterColor = ColorTranslator.FromHtml("#A0A0A0");
                arrowInnerColor = arrowOuterColor = buttonInnerColor = Color.Transparent;
                sbArrowColor =  ForeColor = ColorTranslator.FromHtml("#A0A0A0");
                arrowColors = buttonColors = DisabledColor;
                PaintButton(e);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (!EnableTouchMode && this.DesignMode)
            {
                CTRLSIZE = this.Size;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            arrowOuterColor = buttonOuterColor = Color.SteelBlue;
            arrowInnerColor = buttonInnerColor = SplitterInnerColor = Color.White;
            if(isChecked && ButtonToogleMode )
            SplitterInnerColor = Color.Gray;
            arrowColors = buttonColors = OnoverColor;
            this.metroArrowBackColor = Color.FromArgb(200, base.BackColor);
            if(this.Style == SplitButtonVisualStyle.Metro)
                this.DropDownIconColor = Color.White;
            SetNormalStateColor();
            arrowInnerColor = Color.White;
            this.Invalidate();
        }
        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SplitButton
            // 
            this.Size = new System.Drawing.Size(100, 0);
            this.ResumeLayout(false);

        }
    }
    #endregion
}
