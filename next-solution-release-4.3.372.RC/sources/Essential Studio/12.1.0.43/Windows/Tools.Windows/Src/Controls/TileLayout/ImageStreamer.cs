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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    # region ImageStreamer
    [Designer(typeof(ImageStreamerDesigner), typeof(System.ComponentModel.Design.IDesigner))]
    [ToolboxItem(true),
    ToolboxBitmap(typeof(ImageStreamer), "ToolboxIcons.ImageStreamer.bmp"),
    Description("Image slide show")]
    public partial class ImageStreamer : Control
    {
        # region enum

        public enum StreamDirection
        {
            /// <summary>
            /// Flow from left to right
            /// </summary>
            LeftToRight,

            /// <summary>
            ///  Flow from right to left
            /// </summary>
            RightToLeft,

            /// <summary>
            /// Flow from top to bottom
            /// </summary>
            TopToBottom,

            /// <summary>
            /// Flow from bottom to top
            /// </summary>
            BottomToTop,

            /// <summary>
            /// flow horizontal flip
            /// </summary>
            HorizontalFlip
        }
        public enum TextStreamDirection
        {
            /// <summary>
            /// Flow from left to right
            /// </summary>
            LeftToRight,
            /// <summary>
            ///  Flow from right to left
            /// </summary>
            RightToLeft,
            /// <summary>
            /// Flow from top to bottom
            /// </summary>
            TopToBottom,
            /// <summary>
            /// Flow from bottom to top
            /// </summary>
            BottomToTop,
           ///// <summary>
            ///// flow horizontal flip
            ///// </summary>
            //HorizontalFlip
        }

        public enum ImageStreamerType
        {
            /// <summary>
            /// Sets notmal Tile
            /// </summary>
            Normal,



            /// <summary>
            /// Sets double tile Horizontically
            /// </summary>
            DoubleHorizontal,


        }

        # endregion

        # region Members

        /// <summary>
        /// Main text label
        /// </summary>
        private Label mainText = new Label();
        ///<summary>
        /// Gets or Set the value for dragging   
        ///</summary>
        private bool allowDragging = true;
        /// <summary>
        /// Sub text label
        /// </summary>
        private Label subText = new Label();

        /// <summary>
        /// List of images
        /// </summary>
        private List<Image> images = new List<Image>();

        private ImageListAdv imageCollection = new ImageListAdv();
        /// <summary>
        /// Image flow direction
        /// </summary>
        private StreamDirection streamDirection = StreamDirection.RightToLeft;
        /// <summary>
        /// Text flow direction
        /// </summary>
        private TextStreamDirection textAnimationDirection = TextStreamDirection.RightToLeft;

        /// <summary>
        /// Size type of the tile
        /// </summary>
        private ImageStreamerType sizeType = ImageStreamerType.Normal;

        /// <summary>
        /// Start image index
        /// </summary>
        private int startImageIndex = 0;

        /// <summary>
        /// Indicates the slid show
        /// </summary>
        private bool slideShow = false;

        /// <summary>
        /// Indicates the Text Animation
        /// </summary>
        private bool textAnimation = false;
        /// <summary>
        /// Indicates the internal back color
        /// </summary>
        private Color internalBackColor = Color.Transparent;

        /// <summary>
        /// Indicates to show navigator 
        /// </summary>
        internal bool showNavigator = false;

        /// <summary>
        /// Indicates the dummy bound
        /// </summary>
        private Rectangle dummyBound = new Rectangle();

        /// <summary>
        /// Inidcate to draw border
        /// </summary>
        private bool drawBorder = false;

        /// <summary>
        /// Slider speed
        /// </summary>
        Timer slider = new Timer();

        /// <summary>
        /// Slider animation speed
        /// </summary>
        Timer SliderAnimation = new Timer();

        /// <summary>
        /// Indicates mouse doen
        /// </summary>

        /// <summary>
        /// Indicates the dummy parent
        /// </summary>
        internal Control dummyParent;

        /// <summary>
        /// Inidcates navigation left arrow argb value
        /// </summary>
        int leftArrowArgb = 150;

        /// <summary>
        /// Inidcates navigation right arrow argb value
        /// </summary>
        int rightArowArgb = 150;

        /// <summary>
        /// Inidcates the left outer rectangle
        /// </summary>
        Rectangle rectLeftOuter = new Rectangle(0, 0, 0, 0);

        /// <summary>
        /// Inidcates the right outer rectangle
        /// </summary>
        Rectangle rectRightOuter = new Rectangle(0, 0, 0, 0);

        /// <summary>
        /// Inidcates the axis point
        /// </summary>
        private Point axis = new Point(0, 0);

        /// <summary>
        /// Indicates the streamer index
        /// </summary>
        private int streamer = 0;

        /// <summary>
        /// Indicates the layout group
        /// </summary>
        internal LayoutGroup lgroup = new LayoutGroup();

        /// <summary>
        /// Fires before sliding
        /// </summary>
        public event EventHandler BeforeSliding;

        /// <summary>
        /// Fires after slided
        /// </summary>
        public event EventHandler AfterSlided;


        # endregion

        # region Constructor

        public ImageStreamer()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(ImageStreamer));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            InitializeComponent();
            this.Reinit = true;
            this.SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
            slider.Tick += new EventHandler(slider_Tick);
            SliderAnimation.Tick += new EventHandler(SliderAnimation_Tick);
            axis = new Point(this.Width, this.Height);
            slider.Interval = 1000;
            SliderAnimation.Interval = 10;
            SliderAnimation.Stop();
            this.Type = ImageStreamerType.Normal;
            DummyBound = this.Bounds ;
            this.Controls.Add(MainText);
            this.Controls.Add(SubText);
            this.SubText.ForeColor = Color.White;
            SubText.MouseDown += new MouseEventHandler(SubText_MouseDown);
            SubText.MouseUp += new MouseEventHandler(SubText_MouseUp);
            MainText.MouseDown += new MouseEventHandler(SubText_MouseDown);
            MainText.MouseUp += new MouseEventHandler(SubText_MouseUp);
            SubText.TextChanged += new EventHandler(SubText_TextChanged);
            MainText.TextChanged += new EventHandler(MainText_TextChanged);
            SubText.Visible = false;
            MainText.Visible = false;
            this.SubText.Location = new Point(5, this.Height - 20);
            this.SubText.Size = new Size(100, 100);
            this.SubText.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainText.Location = new Point(this.MainText.Location.X - 3, this.MainText.Location.Y - 3);
            this.MainText.Font = new System.Drawing.Font("Segoe UI", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        }

        # endregion

        # region Properities

        /// <summary>
        /// Gets or sets the main text.
        /// </summary>
        [
        Description("Gets or sets the main text.")
        ]
        public Label MainText
        {
            get
            {
                return mainText;
            }

            set
            {
                mainText = value;
            }
        }

        /// <summary>
        ///Font changed
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }

        /// <summary>
        /// Gets or sets the sub text.
        /// </summary>
        [
        Description("Gets or sets the sub text.")
        ]
        public Label SubText
        {
            get
            {
                return subText;
            }
            set
            {
                subText = value;

            }
        }

        public bool AllowDragging
        {
            get
            { 
                return allowDragging; 
            }
            set 
            {
                allowDragging = value;
            }
        }
        /// <summary>
        /// Gets or sets the images.
        /// </summary>
        [
       Browsable(false),
       Description("Gets or sets the images."),
       Category("Behavior"),
       DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
       DefaultValue(true)
       ]
        public List<Image> Images
        {
            get
            {
                return images;
            }
            set
            {

                images = value;
            }

        }
        [
       Description("Gets or sets the imagelist.")
       ]
        internal bool Reinit = false;
        public ImageListAdv ImageCollection
        {
            get
            {
                if (imageCollection.Images.Count == 0 && Reinit)
                {
                    for (int i = 0; i < this.images.Count; i++)
                    {
                        imageCollection.Images.Add(this.images[i]);
                    }
                        Reinit = false;
                }
                else
                {
                    images.Clear();
                    for (int i = 0; i < imageCollection.Images.Count; i++)
                    {
                        this.images.Add(imageCollection.Images[i]);
                    }
                }
                return imageCollection;
            }

        }        
        /// <summary>
        /// Gets or sets the image flow direction.
        /// </summary>
        [
        Description("Gets or sets the image flow direction.")
        ]
        public StreamDirection ImageStreamDirection
        {
            get
            {
                return streamDirection;
            }
            set
            {
                streamDirection = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the text flow direction.
        /// </summary>
        [
        Description("Gets or sets the Text flow direction.")
        ]
        public TextStreamDirection TextAnimationDirection
        {
            get
            {
                return textAnimationDirection;
            }
            set
            {
                textAnimationDirection = value;
                this.Invalidate();
            }
        }
        /// <summary>
        /// Gets or sets the tile size for tile layout.
        /// </summary>
        [
        Description("Gets or sets tile size for tile layout.")
        ]
        public ImageStreamerType Type
        {
            get
            {
                return sizeType;
            }
            set
            {
                sizeType = value;
                if (sizeType == ImageStreamerType.Normal)
                {
                    this.Size = new Size(120, 120);
                }

                else if (sizeType == ImageStreamerType.DoubleHorizontal)
                {
                    this.Size = new Size(245, 120);

                }

                this.DummyBound = new Rectangle(0, 0, this.Size.Width, this.Size.Height);
            }
        }

        /// <summary>
        /// Gets or sets the slider speed.
        /// </summary>
        [
        Description("Gets or sets the slider spped.")
        ]
        public int SliderSpeed
        {
            get
            {
                return slider.Interval;
            }
            set
            {
                slider.Interval = value;
            }
        }

        /// <summary>
        /// Gets or sets the slider animation speed.
        /// </summary>
        [
        Description("Gets or sets the slider animation speed.")
        ]
        public int AnimationSpeed
        {
            get
            {
                return SliderAnimation.Interval;
            }
            set
            {
                SliderAnimation.Interval = value;
            }
        }

        /// <summary>
        /// Gets or sets the start image index.
        /// </summary>
        [
        Description("Gets or sets the start image index.")
        ]
        public int StartImageIndex
        {
            get
            {
                return startImageIndex;
            }
            set
            {
                startImageIndex = value;
            }

        }
        /// <summary>
        /// Gets or sets the slide show.
        /// </summary>
        [
        Description("Gets or sets the text animation.")
        ]
        public bool TextAnimation
        {
            get
            {
                return textAnimation;
            }
            set
            {
                textAnimation = value;
                if (!this.DesignMode)
                {
                    if (value)
                        slider.Start();
                    else
                        slider.Stop();
                }
            }
        }

        /// <summary>
        /// Gets or sets the slide show.
        /// </summary>
        [
        Description("Gets or sets the slide show.")
        ]
        public bool SlideShow
        {
            get
            {
                return slideShow;
            }
            set
            {
                slideShow = value;
                if (!(this.DesignMode))
                {
                    if (value)
                        slider.Start();
                    else
                        slider.Stop();
                }               
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override Color BackColor
        {
            get
            {
                if (!(this.Parent is LayoutGroup))
                    return internalBackColor;
                else
                    return Color.Transparent;
            }
            set
            {
                base.BackColor = value;
                internalBackColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the internal back color.
        /// </summary>
        [
        Description("Gets or sets the internal back color.")
        ]
        public Color InternalBackColor
        {
            get
            {
                return internalBackColor;
            }
            set
            {
                internalBackColor = value;
            }
        }

        /// <summary>
        /// Gets or sets to show navigator.
        /// </summary>
        [
        Description("Gets or sets to show navigator.")
        ]
        public bool ShowNavigator
        {
            get
            {
                return showNavigator;
            }
            set
            {
                showNavigator = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets to draw border.
        /// </summary>
        internal bool DrawBorder
        {
            get
            {
                return drawBorder;
            }
            set
            {
                if (drawBorder != value)
                {
                    drawBorder = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the dummy bound.
        /// </summary>
        internal Rectangle DummyBound
        {
            get
            {
                return dummyBound;
            }
            set
            {
                dummyBound = value;
                this.Invalidate();
            }
        }
        
        # endregion

        # region Overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            axis = new Point(this.Width, this.Height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            if (!(this.Parent is LayoutGroup))
                dummyBound = new Rectangle(0, 0, this.Width, this.Height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            SolidBrush brush = new SolidBrush(internalBackColor);
            e.Graphics.FillRectangle(brush, DummyBound);
            brush.Dispose();
            if (Images != null && Images.Count >= 2 && SlideShow )
            {
                if (StartImageIndex == 0)
                    leftArrowArgb = 200;
                else
                    leftArrowArgb = 200;
                if (StartImageIndex - 1 == this.Images.Count)
                    rightArowArgb = 200;
                else
                    rightArowArgb = 200;
                
                if (this.ImageStreamDirection == StreamDirection.BottomToTop)
                    StreamBottomToTop(e);
                else if (this.ImageStreamDirection == StreamDirection.TopToBottom)
                    StreamTopToBottom(e);
                else if (this.ImageStreamDirection == StreamDirection.RightToLeft)
                    streamRightToLeft(e);
                else if (this.ImageStreamDirection == StreamDirection.HorizontalFlip)
                    horizontalFlip(e);
                else
                    streamLeftToRight(e);
            }
            else if ((Images.Count >= 1 && (!SlideShow))||Images.Count==1)
            {
                e.Graphics.DrawImage(images[0], new Rectangle(DummyBound.X, DummyBound.Y , DummyBound.Width, DummyBound.Height));

            }
            if (TextAnimation)
                this.SubText.Visible = false;
            else
                this.SubText.Visible = true;
            if (this.TextAnimation)
            {
                if (this.TextAnimationDirection == TextStreamDirection.BottomToTop)
                {
                    textAnimationBottomTOTopDirection(e);
                }
                else if (this.TextAnimationDirection == TextStreamDirection.LeftToRight)
                {
                    textAnimationLeftToRight(e);
                }
                else if (this.TextAnimationDirection == TextStreamDirection.RightToLeft)
                {
                    textAnimationRightToLeft(e);
                }
                else
                {
                    textAnimationTopToBottom(e);
                }
            }
            if (DrawBorder && this.Name.Contains("tiledummy"))
            {
                Pen p = new Pen(ControlPaint.Dark(this.internalBackColor), 5);
                e.Graphics.DrawRectangle(p, this.DummyBound);
                p.Dispose();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            mouseUp(e);
            if (e.Button == MouseButtons.Left)
            base.OnMouseUp(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                mouseDown(e);
            }
        }


        # endregion
        
        # region Methods
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SubText_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SubText_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SubText_TextChanged(object sender, EventArgs e)
        {
            if (SubText.Text == string.Empty)
                SubText.Visible = false;
            else
                SubText.Visible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainText_TextChanged(object sender, EventArgs e)
        {
            if (MainText.Text == string.Empty)
                MainText.Visible = false;
            else
                MainText.Visible = true;
        }

        /// <summary>
        ///  MouseCaptureChanged will be raised when this IamgeStreamer is captured with mouse.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            base.OnMouseCaptureChanged(e);
            if (this.Parent != null && this.Parent.Parent != null && (this.Parent.Parent is TileLayout))
            {
            TileLayout tileLayout = (this.Parent.Parent as TileLayout);
            if (tileLayout.DummyImageStreamer != null && !tileLayout.shouldAnimate)
            {
                tileLayout.TimerDrag.Stop();
                tileLayout.resetImageStreamerBounds();
                tileLayout.shouldAnimate = true;
            }
            }
        }

        /// <summary>
        ///  Mouse up 
        /// </summary>
        /// <param name="e"></param>
        private void mouseUp(MouseEventArgs e)
        {
            
            this.Visible = true;
            if (this.Parent == null)
            {
                if (dummyParent != null)
                {
                    this.Parent = dummyParent;
                }
                else
                {
                    this.Parent = lgroup;
                }
            }
            if (this.Parent is DraggerForm)
            {
                //this.Parent.Parent = (this.Parent as DraggerForm).tile;
            }
            if (this.Parent.Parent is TileLayout)
            {

                TileLayout tileLayout = (this.Parent.Parent as TileLayout);

                tileLayout.shouldAnimate = true;
                int Index = 5000;

                ImageStreamer img = new ImageStreamer();
                foreach (Control group in tileLayout.Controls)
                {
                    if (group is LayoutGroup)
                    {
                        foreach (Control ctrl in group.Controls)
                        {
                            if (ctrl is ImageStreamer)
                            {
                                if (ctrl.Name.Contains("tiledummy"))
                                {
                                    img = ctrl as ImageStreamer;
                                    if (tileLayout.OnNewGroup)
                                    {
                                        tileLayout.Controls.Add(lgroup);
                                        tileLayout.Controls.SetChildIndex(lgroup, tileLayout.OnGroupIndex);
                                        Index = 0;
                                    }
                                    else
                                    {
                                        Index = ctrl.Parent.Controls.GetChildIndex(ctrl);
                                        lgroup = group as LayoutGroup;
                                    }
                                }
                                else if (tileLayout.OnNewGroup)
                                {
                                    tileLayout.Controls.Add(lgroup);
                                    tileLayout.Controls.SetChildIndex(lgroup, tileLayout.OnGroupIndex);
                                    Index = 0;
                                    tileLayout.OnNewGroup = false;
                                    tileLayout.calculateGroupPosition();

                                }

                            }
                        }
                    }
                    tileLayout.calculateDimensions();
                }

                if (Index != 5000)
                    tileLayout.dropFinalImageStream(this, Index, lgroup, img);

                tileLayout.TimerDrag.Stop();
                tileLayout.resetImageStreamerBounds();
                if (tileLayout.DummyImageStreamer != null && tileLayout.DummyImageStreamer.Parent != null)
                    tileLayout.DummyImageStreamer.Parent.Controls.Remove(tileLayout.DummyImageStreamer);
                tileLayout.DummyImageStreamer = null;
                if (tileLayout.dragForm != null)
                    tileLayout.dragForm.Dispose();


            }
            else
            {

            }
            this.Cursor = Cursors.Default;

            if (this.Parent is LayoutGroup)
            {
                (this.Parent as LayoutGroup).ArrangeControl();

                this.SubText.Location = new Point(5, this.Height - 20);
                this.SubText.Size = new Size(100, 100);
                this.SubText.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                this.MainText.Location = new Point(this.MainText.Location.X - 3, this.MainText.Location.Y - 3);
                this.MainText.Font = new System.Drawing.Font("Segoe UI", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            }
        }

        /// <summary>
        /// Mouse down
        /// </summary>
        /// <param name="e"></param>
        private void mouseDown(MouseEventArgs e)
        {
            if (this.Parent.Parent is TileLayout)
            {
                TileLayout tileLayout = (this.Parent.Parent as TileLayout);
                tileLayout.DummyImageStreamerMousePoint = new Point(e.X, e.Y);
                this.DummyBound = new Rectangle(5, 5, this.Width - 10, this.Height - 10);
                tileLayout.TimerDrag.Start();
                tileLayout.DummyImageStreamer = getDummyImageStreamer();
                this.Cursor = Cursors.Hand;
                tileLayout.shouldAnimate = false;
            }

            if (this.ImageStreamDirection == StreamDirection.LeftToRight || this.ImageStreamDirection == StreamDirection.RightToLeft)
            {
                rectLeftOuter = new Rectangle(25, (this.Height / 2) - 12, 25, 25);
                rectRightOuter = new Rectangle(this.Width - 50, (this.Height / 2) - 12, 25, 25);

            }
            else
            {
                rectLeftOuter = new Rectangle((this.Width / 2) - 12, 25, 25, 25);
                rectRightOuter = new Rectangle((this.Width / 2) - 12, this.Height - 50, 25, 25);

            }
            if (rectRightOuter.Contains(e.X, e.Y))
            {
                if (this.ImageStreamDirection == StreamDirection.LeftToRight || this.ImageStreamDirection == StreamDirection.RightToLeft)
                    ImageStreamDirection = StreamDirection.RightToLeft;
                else
                    ImageStreamDirection = StreamDirection.BottomToTop;

                SliderAnimation.Start();
            }

            if (rectLeftOuter.Contains(e.X, e.Y))
            {
                if (this.ImageStreamDirection == StreamDirection.LeftToRight || this.ImageStreamDirection == StreamDirection.RightToLeft)
                    ImageStreamDirection = StreamDirection.LeftToRight;
                else
                    ImageStreamDirection = StreamDirection.TopToBottom;
                SliderAnimation.Start();

            }
            if (this.Parent is LayoutGroup)
            {
                this.SubText.Location = new Point(10, this.Height - 22);
                this.SubText.Size = new Size(100, 100);
                this.SubText.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.MainText.Location = new Point(this.MainText.Location.X + 3, this.MainText.Location.Y + 3);
                this.MainText.Font = new System.Drawing.Font("Segoe UI", 27F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            }
            this.Invalidate();
        }

        /// <summary>
        /// Draws horizontal arrows
        /// </summary>
        /// <param name="g"></param>
        private void drawHorizontalArrows(Graphics g)
        {
            if (ShowNavigator)
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                SolidBrush brush = new SolidBrush(Color.FromArgb(leftArrowArgb, Color.White));
                Rectangle rectLeftOuter = new Rectangle(25, (this.Height / 2) - 12, 25, 25);
                Rectangle rectRightOuter = new Rectangle(this.Width - 50, (this.Height / 2) - 12, 25, 25);

                rectLeftOuter = new Rectangle(25, (this.Height / 2) - 12, 25, 25);
                rectRightOuter = new Rectangle(this.Width - 50, (this.Height / 2) - 12, 25, 25);

                Rectangle rectLeftInner = new Rectangle(27, (this.Height / 2) - 10, 21, 21);
                Rectangle rectRightInner = new Rectangle(this.Width - 48, (this.Height / 2) - 10, 21, 21);
                g.FillEllipse(brush, rectLeftOuter);
                brush = new SolidBrush(Color.FromArgb(rightArowArgb, Color.White));
                g.FillEllipse(brush, rectRightOuter);
                brush = new SolidBrush(Color.FromArgb(leftArrowArgb, Color.Black));
                g.FillEllipse(brush, rectLeftInner);
                brush = new SolidBrush(Color.FromArgb(rightArowArgb, Color.Black));
                g.FillEllipse(brush, rectRightInner);
                brush = new SolidBrush(Color.FromArgb(rightArowArgb, Color.White));
                brush.Dispose();
                brush = new SolidBrush(Color.FromArgb(leftArrowArgb, Color.White));
                Point[] points = new Point[] { new Point(rectLeftOuter.X + 15, rectLeftOuter.Y + 7), new Point(rectLeftOuter.X + 7, rectLeftOuter.Y + rectLeftOuter.Height / 2), new Point(rectLeftOuter.X + 15, rectLeftOuter.Y + rectLeftOuter.Height - 7) };
                g.FillPolygon(brush, points);

                brush = new SolidBrush(Color.FromArgb(rightArowArgb, Color.White));
                points = new Point[] { new Point(rectRightOuter.X + 10, rectRightOuter.Y + 7), new Point(rectRightOuter.X + 18, rectRightOuter.Y + rectRightOuter.Height / 2), new Point(rectRightOuter.X + 10, rectRightOuter.Y + rectRightOuter.Height - 7) };
                g.FillPolygon(brush, points);
                brush.Dispose();
            }

        }

        /// <summary>
        /// Draws vertical arrows
        /// </summary>
        /// <param name="g"></param>
        private void drawVerticalArrows(Graphics g)
        {
            if (ShowNavigator)
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                SolidBrush brush = new SolidBrush(Color.FromArgb(leftArrowArgb, Color.White));
                Rectangle rectLeftOuter = new Rectangle((this.Width / 2) - 12, 25, 25, 25);
                Rectangle rectRightOuter = new Rectangle((this.Width / 2) - 12, this.Height - 50, 25, 25);

                Rectangle rectLeftInner = new Rectangle((this.Width / 2) - 10, 27, 21, 21);
                Rectangle rectRightInner = new Rectangle((this.Width / 2) - 10, this.Height - 48, 21, 21);
                g.FillEllipse(brush, rectLeftOuter);
                brush = new SolidBrush(Color.FromArgb(rightArowArgb, Color.White));
                g.FillEllipse(brush, rectRightOuter);
                brush = new SolidBrush(Color.FromArgb(leftArrowArgb, Color.Black));
                g.FillEllipse(brush, rectLeftInner);
                brush = new SolidBrush(Color.FromArgb(rightArowArgb, Color.Black));
                g.FillEllipse(brush, rectRightInner);
                brush.Dispose();

                brush = new SolidBrush(Color.FromArgb(leftArrowArgb, Color.White));
                Point[] points = new Point[] { new Point(rectLeftOuter.X + 7, rectLeftOuter.Y + rectLeftOuter.Height - 10), new Point(rectLeftOuter.X + rectLeftOuter.Width /2, rectLeftOuter.Y + 7), new Point(rectLeftOuter.X + rectLeftOuter.Width - 7, rectLeftOuter.Y + rectLeftOuter.Height - 10) };
                g.FillPolygon(brush, points);

                brush = new SolidBrush(Color.FromArgb(rightArowArgb, Color.White));
                points = new Point[] { new Point(rectRightOuter.X + 7, rectRightOuter.Y + 10), new Point(rectRightOuter.X + rectRightOuter.Width / 2, rectRightOuter.Y + rectRightOuter.Height - 7), new Point(rectRightOuter.X + rectRightOuter.Width - 7, rectRightOuter.Y + 10) };
                g.FillPolygon(brush, points);
                brush.Dispose();
            }
        }

        /// <summary>
        /// Flow image right to left
        /// </summary>
        /// <param name="e"></param>
        private void streamRightToLeft(PaintEventArgs e)
        {
            if ((axis.X - streamer) < 10)
                streamer = axis.X;
            if (this.Images.Count  == StartImageIndex)
                startImageIndex = 0;
                e.Graphics.DrawImage(images[StartImageIndex], new Rectangle(DummyBound.X - streamer, DummyBound.Y, DummyBound.Width, DummyBound.Height));
               
            int i = - 1;
            if (this.Images.Count - 1 == StartImageIndex)
                i = startImageIndex;

                e.Graphics.DrawImage(images[StartImageIndex - i], new Rectangle(axis.X - streamer, DummyBound.Y, DummyBound.Width, DummyBound.Height));


            if ((axis.X - streamer) < 10)
            {
                streamer = 0;
                SliderAnimation.Stop();
                if(AfterSlided != null)
                AfterSlided(this, new EventArgs());
                StartImageIndex++;
            }

            //if (!this.SlideShow)
                drawHorizontalArrows(e.Graphics);
        }

        /// <summary>
        /// Flow image left to right
        /// </summary>
        /// <param name="e"></param>
        private void streamLeftToRight(PaintEventArgs e)
        {
            if ((axis.X - streamer) < 10)
                streamer = axis.X;
            if (this.Images.Count == StartImageIndex)
                startImageIndex = 0;

            e.Graphics.DrawImage(images[StartImageIndex], new Rectangle(DummyBound.X + streamer, DummyBound.Y , this.Width, this.Height));


            int i = -1;
            if (this.Images.Count - 1 == StartImageIndex)
                i = startImageIndex;

            e.Graphics.DrawImage(images[StartImageIndex - i], new Rectangle(-axis.X + streamer, DummyBound.Y , this.Width, this.Height));


            if ((axis.X - streamer) < 10)
            {
                streamer = 0;
                SliderAnimation.Stop();
                if (AfterSlided != null)
                AfterSlided(this, new EventArgs());
                StartImageIndex++;
            }
            drawHorizontalArrows(e.Graphics);
        }
       /// <summary>
        /// Flow Text right to left
        /// </summary>
        /// <param name="e"></param>
        private void textAnimationRightToLeft(PaintEventArgs e)
        {
            if ((axis.X - streamer) < 10)
                streamer = axis.X;
            SolidBrush brush = new SolidBrush(this.SubText.ForeColor);
            PointF points = new PointF(DummyBound.X + DummyBound.Width - streamer, DummyBound.Y);
            e.Graphics.DrawString(this.SubText.Text, this.Font, brush, points);
            brush.Dispose();
            if ((axis.X - streamer) < 10)
            {
                streamer = 0;
            }
        }
        /// <summary>
        /// Flow Text left to right
        /// </summary>
        /// <param name="e"></param>
        private void textAnimationLeftToRight(PaintEventArgs e)
        {
            if ((axis.X - streamer) < 10)
                streamer = axis.X;
            SolidBrush brush = new SolidBrush(this.SubText.ForeColor);
            PointF points = new PointF(DummyBound.X + streamer, DummyBound.Y);
            e.Graphics.DrawString(this.SubText.Text, this.Font,  brush, points);
            brush.Dispose();
            if ((axis.X - streamer) < 10)
            {
                streamer = 0;
            }
        }
        /// <summary>
        /// Flow Text Botton to Top
        /// </summary>
        /// <param name="e"></param>
        private void textAnimationBottomTOTopDirection(PaintEventArgs e)
        {
            if ((axis.Y - streamer) < 10)
                streamer = axis.Y;
            SolidBrush brush = new SolidBrush(this.SubText.ForeColor);
            PointF points = new PointF(DummyBound.X, DummyBound.Y + dummyBound.Height - streamer);
            e.Graphics.DrawString(this.SubText.Text,this.Font, brush, points);
            brush.Dispose();
            if ((axis.Y - streamer) < 10)
            {
                streamer = 0;
            }
        }
        /// <summary>
        /// Flow Text top to bottom
        /// </summary>
        /// <param name="e"></param>
        private void textAnimationTopToBottom(PaintEventArgs e)
        {
            if ((axis.Y - streamer) < 10)
                streamer = axis.Y;
            SolidBrush brush = new SolidBrush(this.SubText.ForeColor);
            PointF points = new PointF(0, 0 + streamer);
            e.Graphics.DrawString(this.SubText.Text, this.Font, brush, points);
            brush.Dispose();
            if ((axis.Y - streamer) < 10)
            {
                streamer = 0;
            }
        }
        /// <summary>
        /// Flow image horizontal flip
        /// </summary>
        /// <param name="e"></param>
        private void horizontalFlip(PaintEventArgs e)
        {
            if ((axis.X - streamer) < 10)
                streamer = axis.X;

            if (this.Images.Count == StartImageIndex)
                startImageIndex = 0;

            e.Graphics.DrawImage(images[StartImageIndex], new Rectangle(0 + streamer, 0, this.Width, this.Height));

            int i = -1;
            if (this.Images.Count - 1 == StartImageIndex)
                i = startImageIndex;

            e.Graphics.DrawImage(images[StartImageIndex - i], new Rectangle(axis.X - streamer, 0, this.Width, this.Height));


            if ((axis.X - streamer) < 10)
            {
                streamer = 0;
                SliderAnimation.Stop();
                if (AfterSlided != null)
                AfterSlided(this, new EventArgs());
                StartImageIndex++;
            }

            drawHorizontalArrows(e.Graphics);
        }
        
        /// <summary>
        /// Gets dummy imagestreamer
        /// </summary>
        /// <returns></returns>
        private ImageStreamer getDummyImageStreamer()
        {
            ImageStreamer dummyImageStreamer = new ImageStreamer();
            dummyImageStreamer.BackColor = this.BackColor;
            dummyImageStreamer.Type = this.Type;
            dummyImageStreamer.InternalBackColor = this.InternalBackColor;
            dummyImageStreamer.DummyBound = this.DummyBound;
            dummyImageStreamer.Size = this.Size;
            dummyImageStreamer.Name = this.Name + "dummy";
            dummyImageStreamer.Images = this.Images;
            dummyImageStreamer.StartImageIndex = this.StartImageIndex;
           // dummyImageStreamer.SlideShow = this.SlideShow;
            dummyImageStreamer.AnimationSpeed = this.AnimationSpeed;
            dummyImageStreamer.SliderSpeed = this.SliderSpeed;
            dummyImageStreamer.Images = this.Images;
            dummyImageStreamer.ImageStreamDirection = this.ImageStreamDirection;
            dummyImageStreamer.MainText = this.MainText;
            dummyImageStreamer.SubText = this.SubText;
            dummyImageStreamer.AllowDragging = this.AllowDragging;
            return dummyImageStreamer;
        }

        /// <summary>
        /// Flow image top to bottom
        /// </summary>
        /// <param name="e"></param>
        private void StreamTopToBottom(PaintEventArgs e)
        {
            if ((axis.Y - streamer) < 10)
                streamer = axis.Y;

            if (this.Images.Count == StartImageIndex)
                startImageIndex = 0;

            e.Graphics.DrawImage(images[StartImageIndex], new Rectangle(0, 0 + streamer, this.Width, this.Height));

            int i = -1;
            if (this.Images.Count - 1 == StartImageIndex)
                i = startImageIndex;

            e.Graphics.DrawImage(images[StartImageIndex - i], new Rectangle(0, -axis.Y + streamer, this.Width, this.Height));

            if ((axis.Y - streamer) < 10)
            {
                streamer = 0;
                SliderAnimation.Stop();
                StartImageIndex++;
                if (AfterSlided != null)
                AfterSlided(this, new EventArgs());
            }
            drawVerticalArrows(e.Graphics);
        }

        /// <summary>
        /// Flow image vertical flip
        /// </summary>
        /// <param name="e"></param>
        private void verticalFlip(PaintEventArgs e)
        {
            if ((axis.Y - streamer) < 10)
                streamer = axis.Y;

            if (this.Images.Count == StartImageIndex)
                startImageIndex = 0;

            e.Graphics.DrawImage(images[StartImageIndex], new Rectangle(0, 0 + streamer, this.Width, this.Height));
            
            int i = -1;
            if (this.Images.Count - 1 == StartImageIndex)
                i = startImageIndex;

            e.Graphics.DrawImage(images[StartImageIndex - i], new Rectangle(0, axis.Y - streamer, this.Width, this.Height));

            if ((axis.Y - streamer) < 10)
            {
                streamer = 0;
                SliderAnimation.Stop();
                StartImageIndex++;
                if (AfterSlided != null)
                AfterSlided(this, new EventArgs());
            }
            drawVerticalArrows(e.Graphics);
        }

        /// <summary>
        /// Flow image bottom to top
        /// </summary>
        /// <param name="e"></param>
        private void StreamBottomToTop(PaintEventArgs e)
        {
            if ((axis.Y - streamer) < 10)
                streamer = axis.Y;

            if (this.Images.Count == StartImageIndex)
                startImageIndex = 0;

            e.Graphics.DrawImage(images[StartImageIndex], new Rectangle(DummyBound.X, DummyBound.Y - streamer, DummyBound.Width, DummyBound.Height));

            int i = -1;
            if (this.Images.Count - 1 == StartImageIndex)
                i = startImageIndex;

            e.Graphics.DrawImage(images[StartImageIndex - i], new Rectangle(DummyBound.X, axis.Y - streamer, DummyBound.Width, DummyBound.Height));

            if ((axis.Y - streamer) < 10)
            {
                streamer = 0;
                SliderAnimation.Stop();
                StartImageIndex++;
                if(AfterSlided != null)
                AfterSlided(this, new EventArgs());
            }
            drawVerticalArrows(e.Graphics);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SliderAnimation_Tick(object sender, EventArgs e)
        {

            if ( this.Parent != null && this.Parent.Parent != null)
            {
                if (this.Parent.Parent is TileLayout)
                    if ((this.Parent.Parent as TileLayout).shouldAnimate)
                    {
                        streamer = streamer + 10;
                        this.Invalidate();
                    }
                    else
                    {
                    }
                else
                {
                    streamer = streamer + 10;
                    this.Invalidate();
                }
            }
            else
            {
                streamer = streamer + 10;
                this.Invalidate();
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void slider_Tick(object sender, EventArgs e)
        {
            if(BeforeSliding != null)
            BeforeSliding(this, new EventArgs());

            SliderAnimation.Start();
        }

        # endregion
        
        # region ShouldSerialize

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeType()
        {
            return Type != ImageStreamerType.Normal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeBackColor()
        {
            return this.BackColor != Color.Transparent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeShowNavigator()
        {
            return ShowNavigator != false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeTextAnimation()
        {
            return TextAnimation != false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeSlideShow()
        {
            return SlideShow != false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeSliderSpeed()
        {
            return SliderSpeed != 1000;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeAnimationSpeed()
        {
            return AnimationSpeed != 10;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeStartImageIndex()
        {
            return StartImageIndex != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeImageStreamDirection()
        {
            return ImageStreamDirection != StreamDirection.RightToLeft;
        }

        # endregion

    }
    /// <summary>
    /// ImageListAdv Designer
    /// </summary>
    [Designer(typeof(ImageStreamerDesigner), typeof(System.ComponentModel.Design.IDesigner))]
    public class ImageStreamerDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        /// <summary>
        /// Designer ActionList collection
        /// </summary>
        private System.ComponentModel.Design.DesignerActionListCollection actionLists;

        /// <summary>
        ///  Initializes a new instance of the CheckBoxAdvDesigner class
        /// </summary>
        public ImageStreamerDesigner()
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
                    this.actionLists.Add(new ImageStreamerActionList(this.Component));
                }

                return this.actionLists;
            }
        }

#endif
    }
    # endregion
}