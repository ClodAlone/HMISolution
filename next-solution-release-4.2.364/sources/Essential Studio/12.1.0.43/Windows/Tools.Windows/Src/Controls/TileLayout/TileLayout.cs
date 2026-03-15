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
using Syncfusion.Collections;
using System.Collections;
using System.Drawing.Design;

namespace Syncfusion.Windows.Forms.Tools
{

    # region TileLayout

    [ToolboxItem(true),
    ToolboxBitmap(typeof(TileLayout), "ToolboxIcons.TileLayout.bmp"),
    Description("Tile view of items"),
    Designer(typeof(Syncfusion.Windows.Forms.Tools.Design.TileLayoutDesigner))]
    public partial class TileLayout : GradientPanel
    {

        # region Members

        /// <summary>
        /// TimerDrag Timer
        /// </summary>
        internal Timer TimerDrag = new Timer();

        /// <summary>
        /// Title Font
        /// </summary>
        private Font titleFont = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        
        /// <summary>
        /// Main flow layout
        /// </summary>
        private MainLayout mainLayout = new MainLayout();
        
        /// <summary>
        ///  Current hover layoutgroup
        /// </summary>
        private String currentHoverLayoutGroup = string.Empty;

        /// <summary>
        ///  Current hover imagestreamer
        /// </summary>
        private int currentHoverImageStreamer = 0;

        /// <summary>
        ///  Dummy imagestreamer's mouse point
        /// </summary>
        private Point dummyImageStreamerMousePoint = new Point(0, 0);

        /// <summary>
        /// Shows item preview
        /// </summary>
        private bool showItemPreview = false;

        /// <summary>
        /// Indicates mouse point on the new group
        /// </summary>
        private bool onNewGroup = false;

        /// <summary>
        /// Indicates to allow new group
        /// </summary>
        private bool allowNewGroup = false;

        /// <summary>
        ///  Indicates transparency
        /// </summary>
        private int transparency = 10;

        /// <summary>
        /// Indicates exit image
        /// </summary>
        private Image exitImage;

        /// <summary>
        /// Indicates new group image
        /// </summary>
        private Image newGroupImage;

        /// <summary>
        /// Indicates show preview image
        /// </summary>
        private Image showPreviewImage;

        /// <summary>
        /// Indicates to show accessories panel
        /// </summary>
        private bool showAccessoriesPanel = false;
        
        /// <summary>
        /// Indicates the dragger form
        /// </summary>
        internal DraggerForm dragForm;

        /// <summary>
        /// Indicates the dummy imagestreamer
        /// </summary>
        private ImageStreamer dummyImageStreamer = new ImageStreamer();
        
        /// <summary>
        /// Sets the parent form flat
        /// </summary>
        private bool setParentFormFlat = false;

        /// <summary>
        /// Contains group collection
        /// </summary>
        private TileGroupCollection groups = new TileGroupCollection();

        /// <summary>
        ///  Shows group title
        /// </summary>
        private bool showGroupTitle = false;

        /// <summary>
        ///  Indicates new group indicator color
        /// </summary>
        private Color newGroupIndicatorColor = Color.Black;

        /// <summary>
        /// Indicates the text
        /// </summary>

        /// <summary>
        /// Indicates the allignment of the text
        /// </summary>
        private TextAlignment textAlignment = TextAlignment.Left;

        /// <summary>
        /// Indicates the items
        /// </summary>
        private int items = 0;

        /// <summary>
        /// Indicates the accessories form
        /// </summary>
        internal AccessoriesForm accessoriesForm = null;

        /// <summary>
        /// Indicates the list of child imagestreamer
        /// </summary>
        private List<ImageStreamer> ChildStreamer = new List<ImageStreamer>();

        /// <summary>
        /// Indicates the current dragging streamer
        /// </summary>
        internal ImageStreamer draggingImageStreamer = new ImageStreamer();

        /// <summary>
        /// Indicates the new group rectangle
        /// </summary>
        private Rectangle newGroupRect;
        
        /// <summary>
        /// Indicates current selected group index
        /// </summary>
        internal int OnGroupIndex;

        /// <summary>
        /// Indicates the imagestreamer flower
        /// </summary>
        internal ImageStreamer dummyFlower = new ImageStreamer();

        /// <summary>
        /// Indicates mouse on the accessories panel
        /// </summary>
        bool isOnAccessoriesPanelButton = false;

        /// <summary>
        /// Indicates the show new group button
        /// </summary>
        AccessoriesButton ShowNewGroup;

        /// <summary>
        /// Indicates theexit button
        /// </summary>
        AccessoriesButton IsExit;

        /// <summary>
        /// Indicates the show preview button
        /// </summary>
        AccessoriesButton ShowPreview;

        /// <summary>
        /// Indicates accessories form opening
        /// </summary>
        bool isOpening = false;

        /// <summary>
        /// Indicates accessories form closing
        /// </summary>
        bool isClosing = false;

        /// <summary>
        /// Timer used to open accessories form
        /// </summary>
        internal Timer Opener;

        /// <summary>
        /// Timer used to close accessories form
        /// </summary>
        internal Timer Closer;

        /// <summary>
        /// List of position of the group
        /// </summary>
        internal List<TilePosition> PositionGroup = new List<TilePosition>();


        /// <summary>
        /// List of position of the items
        /// </summary>
        internal List<TilePosition> PositionItems = new List<TilePosition>();

        /// <summary>
        /// List of position of the group text
        /// </summary>
        internal List<TilePosition> groupTextPosition = new List<TilePosition>();

        /// <summary>
        /// Rectangle of the accessories region
        /// </summary>
        Rectangle AccessoriesRegion = new Rectangle();

        /// <summary>
        /// 
        /// </summary>
        internal bool shouldAnimate = true;
        
        # endregion
    
        # region Constructor

        public TileLayout()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(TileLayout));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            InitializeComponent();
            MainLayout.ContainerControl = this;
            MainLayout.LayoutMode = FlowLayoutMode.Vertical;
            MainLayout.HGap = MainLayout.VGap = 75;
            this.SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
            this.AutoScroll = true;
            this.Dock = DockStyle.Fill;
            this.BorderStyle = BorderStyle.None;
            TimerDrag.Tick += new EventHandler(TimerDrag_Tick);
            this.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            MainLayout.TopMargin = 100;
            MainLayout.HorzNearMargin = 100;
            ScrollersFrame scroll = new ScrollersFrame();
            scroll.AttachedTo = this;
            scroll.VisualStyle = ScrollBarCustomDrawStyles.Metro;
            //getDefaultGroup();

        }

        private void getDefaultGroup()
        {
            LayoutGroup group = new LayoutGroup ();
            this.Controls.Add(group);
            group.Controls.Add(getDefaultImageStreamer());
            group.Controls.Add(getDefaultImageStreamer());
            group.Controls.Add(getDefaultImageStreamer());
            group.Controls.Add(getDefaultImageStreamer());
        }

        private ImageStreamer getDefaultImageStreamer()
        {
            ImageStreamer imageStreamer = new ImageStreamer();
            imageStreamer.BackColor = Color.Gray;
            return imageStreamer ;
        }

        # endregion

        # region Properties
        
        /// <summary>
        /// Gets or sets font of the title text.
        /// </summary>
        [
        Category("Appearance - Styles"),
        Description("Gets or sets font of the title text.")
        ]
        public Font TitleFont
        {
            get
            {
                return titleFont;
            }
            set
            {
                titleFont = value;
            }
        }

        /// <summary>
        /// Gets or sets main inner layout.
        /// </summary>
        [
        Description("Gets or sets main flow.")
        ]
        public MainLayout MainLayout
        {
            get
            {
                return mainLayout;
            }
            set
            {
                mainLayout = value;
            }
        }

        /// <summary>
        /// Gets or sets current hover layout group.
        /// </summary>
        internal String CurrentHoverLayoutGroup
        {
            get
            {
                return currentHoverLayoutGroup;
            }
            set
            {
                currentHoverLayoutGroup = value;
            }
        }

        /// <summary>
        /// Gets or sets current hover image streamer.
        /// </summary>
        internal int CurrentHoverImageStreamer
        {
            get
            {
                return currentHoverImageStreamer;
            }
            set
            {
                currentHoverImageStreamer = value;
            }
        }

        /// <summary>
        /// Gets or sets dummy imagestreamer.
        /// </summary>
        internal Point DummyImageStreamerMousePoint
        {
            get
            {
                return dummyImageStreamerMousePoint;
            }
            set
            {
                dummyImageStreamerMousePoint = value;
            }
        }

        /// <summary>
        /// Gets or sets to show item preview.
        /// </summary>
        [
        Description("Gets or sets to show item preview.")
        ]
        public bool ShowItemPreview
        {
            get
            {
                return showItemPreview;
            }
            set
            {
                showItemPreview = value;
            }
        }

        /// <summary>
        /// Gets or sets mouse point on new group.
        /// </summary>
        internal bool OnNewGroup
        {
            get
            {
                return onNewGroup;
            }
            set
            {
                onNewGroup = value;
            }
        }

        /// <summary>
        /// Gets or sets to allow new group.
        /// </summary>
        [
        Description("Gets or sets to allow new group.")
        ]
        public bool AllowNewGroup
        {
            get
            {
                return allowNewGroup;
            }
            set
            {
                allowNewGroup = value;
            }
        }

        /// <summary>
        /// Gets display rectangle.
        /// </summary>
        public override Rectangle DisplayRectangle
        {
            get
            {
                return base.DisplayRectangle;
            }
        }

        /// <summary>
        /// Gets or sets transparency value.
        /// </summary>
        [
        Description("Gets or sets transparency value.")
        ]
        public int Transparency
        {
            get
            {
                return transparency;
            }
            set
            {
                if (transparency != value)
                {
                    transparency = value;
                    Form form = getParentForm();
                    if (form != null)
                    {
                        form.Opacity = (float)value / 10;
                    }
                }

            }
        }

        /// <summary>
        /// Gets or sets the exit button image.
        /// </summary>
        [
        Description("Gets or sets the exit button image.")
        ]
        public Image ExitButtonImage
        {
            get
            {
                return exitImage;
            }
            set
            {
                exitImage = value;
            }
        }

        /// <summary>
        /// Gets or sets the new group button image.
        /// </summary>
        [
        Description("Gets or sets the new group button image.")
        ]
        public Image NewGroupButtonImage
        {
            get
            {
                return newGroupImage;
            }
            set
            {
                newGroupImage = value;
            }
        }

        /// <summary>
        /// Gets or sets the show preview button image.
        /// </summary>
        [
        Description("Gets or sets the show preview button image.")
        ]
        public Image ShowPreviewButtonImage
        {
            get
            {
                return showPreviewImage;
            }
            set
            {
                showPreviewImage = value;


            }
        }

        /// <summary>
        /// Gets or sets to show accessories panel.
        /// </summary>
        [
        Description("Gets or sets to show accessories panel.")
        ]
        internal bool ShowAccessoriesPanel
        {
            get
            {
                return showAccessoriesPanel;
            }
            set
            {
                showAccessoriesPanel = value;
            }
        }

        /// <summary>
        /// Gets or sets the dummy imagestreamer.
        /// </summary>
        internal ImageStreamer DummyImageStreamer
        {
            get
            {

                return dummyImageStreamer;
            }
            set
            {
                dummyImageStreamer = value;
                if (dummyImageStreamer != null)
                {
                    dragForm = GetTransparentForm();


                    Point dummyImageStreamerPoint = new Point(Cursor.Position.X - this.Parent.Left - DummyImageStreamerMousePoint.X, Cursor.Position.Y - DummyImageStreamerMousePoint.Y - (this.Parent.Top + 20));

                    dragForm.Location = new Point(dummyImageStreamerPoint.X + this.Parent.Left, dummyImageStreamerPoint.Y + this.Parent.Top + 25);
                    dragForm.ShowInTaskbar = false;
                    dragForm.Show();
                    dragForm.Controls.Add(dummyImageStreamer);
                    dragForm.Size = dummyImageStreamer.Size;
                    dummyFlower = getDummyImageStreamer(dummyImageStreamer);

                }

            }
        }

        /// <summary>
        /// Gets or sets to make parent form flat.
        /// </summary>
        [
        Description("Gets or sets to make parent form flat.")
        ]
        public bool SetParentFormFlat
        {
            get
            {
                return setParentFormFlat;
            }
            set
            {
                setParentFormFlat = value;
                if (value)
                {
                    Form form = getParentForm();
                    if (form != null)
                    {
                        form.WindowState = FormWindowState.Maximized;
                        form.FormBorderStyle = FormBorderStyle.None;

                    }
                }
                else
                {
                    Form form = getParentForm();
                    if (form != null)
                    {
                        form.WindowState = FormWindowState.Normal;
                        form.FormBorderStyle = FormBorderStyle.Sizable;
                    }
                }
            }
        }


        /// <summary>
        /// Gets or sets the layout group collection.
        /// </summary>
        [
         Description("Gets or sets the group collection."),DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public TileGroupCollection Groups
        {
            get
            {
                for (int i = 0; i < groups.Count; i++)
                {
                    groups[i].Height = this.Height - 150;
                    groups[i].Width = 200;
                    this.Controls.Add(groups[i]);
                }

                if (groups.Count == 0)
                {
                    for (int i = 0; i < this.Controls.Count; i++)
                    {
                        groups.Add(this.Controls[i]);
                    }
                }

                return groups;
            }
            set
            {
                groups = value;

                for (int i = 0; i < groups.Count; i++)
                {
                    this.Controls.Add(groups[i]);
                }
            }
        }


        public int Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the text.
        /// </summary>
        [
        Description("Gets or sets the alignment of the text.")
        ]
        public TextAlignment TextAlignment
        {
            get
            {
                return textAlignment;
            }
            set
            {
                textAlignment = value;
            }
        }


        /// <summary>
        /// Gets or sets the color of the new group indicator.
        /// </summary>
        [
        Description("Gets or sets the color of the new group indicator.")
        ]
        public Color NewGroupIndicatorColor
        {
            get
            {
                return newGroupIndicatorColor;
            }
            set
            {
                newGroupIndicatorColor = value;
            }
        }

        /// <summary>
        /// Gets or sets to show the title of the group.
        /// </summary>
        [
        Description("Gets or sets to show the title of the group.")
        ]
        public bool ShowGroupTitle
        {
            get
            {
                return showGroupTitle;
            }
            set
            {
                showGroupTitle = value;
            }
        }
        
        # endregion

        # region Overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (ShowAccessoriesPanel && AccessoriesRegion.Contains(e.X, e.Y) && accessoriesForm == null)
            {
                accessoriesForm = GetAccessoriesForm();
                accessoriesForm.Show();
                accessoriesForm.Width = 5;
                setAcessoriesFormLocation();
                Opener.Start();
                this.Invalidate();
                isOnAccessoriesPanelButton = true;


            }
            else
            {
                if (accessoriesForm != null && e.Y < this.Bottom - 15)
                {
                    Closer.Start();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (this.DesignMode)
                ControlPaint.DrawFocusRectangle(e.Graphics, new Rectangle(0, 0, this.Width, this.Height));
            SolidBrush brush = new SolidBrush(this.ForeColor);
            Point textLocation = new Point(20, 20);
            if (textAlignment == TextAlignment.Right)
            {
                int left = (int)e.Graphics.MeasureString(this.Text, this.Font).Width;

                textLocation = new Point(this.Width - left - 20, 20);
            }
            else if (this.TextAlignment == TextAlignment.Center)
            {
                int left = (int)e.Graphics.MeasureString(this.Text, this.Font).Width;
                textLocation = new Point(this.Width / 2 - left / 2, 20);
            }
            e.Graphics.DrawString(this.Text, this.Font, brush, textLocation);

            if (ShowGroupTitle)
            {
                for (int i = 0; i < this.groupTextPosition.Count; i++)
                {

                    TilePosition tilePosition = this.groupTextPosition[i];
                    brush = new SolidBrush(tilePosition.ForeColor);
                    Rectangle rect = new Rectangle(tilePosition.Rect.X, tilePosition.Rect.Y - (int)e.Graphics.MeasureString(tilePosition.Text, tilePosition.Font).Height, tilePosition.Rect.Width, tilePosition.Rect.Height);
                    e.Graphics.DrawString(tilePosition.Text, tilePosition.Font, brush, rect);
                }
            }
            brush.Dispose();
            
            if (OnNewGroup)
            {
                Pen pen = new Pen(NewGroupIndicatorColor, 5);
                e.Graphics.DrawRectangle(pen, newGroupRect);
                pen.Dispose();
                SolidBrush solidBrush = new SolidBrush(Color.FromArgb(150, NewGroupIndicatorColor));
                e.Graphics.FillRectangle(solidBrush, newGroupRect);
                solidBrush.Dispose();
            }

            if (ShowAccessoriesPanel && isOnAccessoriesPanelButton)
            {
                //Pen pen = new Pen(Color.Black, 4);
                //e.Graphics.DrawRectangle(pen, AccessoriesRegion);
                //e.Graphics.DrawLine(pen, new Point(AccessoriesRegion.X + 2, AccessoriesRegion.Top + 1 + (AccessoriesRegion.Height / 2)), new Point((AccessoriesRegion.Width - 4) + (AccessoriesRegion.X + 2), AccessoriesRegion.Top + 1 + (AccessoriesRegion.Height / 2)));
                //pen = new Pen(Color.White, 2);
                //e.Graphics.DrawRectangle(pen, AccessoriesRegion);
                //e.Graphics.DrawLine(pen, new Point(AccessoriesRegion.X + 2, AccessoriesRegion.Top + 1 + (AccessoriesRegion.Height / 2)), new Point((AccessoriesRegion.Width - 4) + (AccessoriesRegion.X + 2), AccessoriesRegion.Top + 1 + (AccessoriesRegion.Height / 2)));
                //pen.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventargs"></param>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            calculateDimensions();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="levent"></param>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            calculateDimensions();
            this.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="se"></param>
        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            calculateDimensions();
            this.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            calculateDimensions();
            this.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnValidated(EventArgs e)
        {
            base.OnValidated(e);
        }

        # endregion

        # region Methods

        /// <summary>
        /// Get a transparent form
        /// </summary>
        /// <returns></returns>
        internal DraggerForm GetTransparentForm()
        {
            DraggerForm form = new DraggerForm();
            form.Opacity = 0.00;
            form.tile = this;
            form.FormBorderStyle = FormBorderStyle.None;
            return form;
        }

        /// <summary>
        /// Drop final imagestreamer to the group
        /// </summary>
        /// <param name="imageStreamer"></param>
        /// <param name="Index"></param>
        /// <param name="group"></param>
        /// <param name="dummyImgStm"></param>
        internal void dropFinalImageStream(ImageStreamer imageStreamer, int Index, LayoutGroup group, ImageStreamer dummyImgStm)
        {
            group.Controls.Remove(dummyImgStm);
            group.Controls.Add(imageStreamer);
            group.Controls.SetChildIndex(imageStreamer, Index);
        }

        /// <summary>
        ///  Drops imagestreamer for preview
        /// </summary>
        /// <param name="imageStreamer"></param>
        /// <returns></returns>
        internal bool dropImageStream( ImageStreamer imageStreamer)
        {
            if (dragForm.Opacity != 0.0)
            {
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is LayoutGroup)
                    {
                        if (ctrl.Name == CurrentHoverLayoutGroup)
                        if (ctrl.Name == CurrentHoverLayoutGroup || CurrentHoverLayoutGroup == "")
                        {
                            if (!ctrl.Controls.Contains(imageStreamer))
                            {
                                ctrl.Controls.Add(imageStreamer);
                                this.draggingImageStreamer.dummyParent  = ctrl;
                                ctrl.Invalidate();
                            }
                         
                            if (DummyImageStreamer != null)
                            {
                                Point dummyImageStreamerLocation= new Point(Cursor.Position.X - this.Parent.Left, Cursor.Position.Y - (this.Parent.Top + 20));

                                foreach (TilePosition tilePosition in this.PositionItems)
                                {

                                    if (tilePosition.Rect.Contains(dummyImageStreamerLocation))
                                    {
                                        CurrentHoverLayoutGroup = tilePosition.Name;
                                        if (ShowItemPreview)
                                        {
                                            imageStreamer.DrawBorder = true;
                                            imageStreamer.BackColor = Color.FromArgb(50, imageStreamer.InternalBackColor);
                                        }
                                        else
                                        {
                                            imageStreamer.DrawBorder = false;
                                            imageStreamer.BackColor = Color.Transparent;
                                        }

                                        if (ctrl.Controls.Contains(imageStreamer))
                                            ctrl.Controls.SetChildIndex(imageStreamer, tilePosition.Index);

                                    }
                                }
                                if (OnNewGroup && ctrl.Controls.Contains(imageStreamer))
                                {
                                      ctrl.Controls.Remove (imageStreamer);
                                }
                                return true;
                            }
                        }

                        
                    }
                }

            }
            return false;
        }

        /// <summary>
        /// gets a dummy imagestreamer.
        /// </summary>
        /// <param name="imgStreamer"></param>
        /// <returns></returns>
        private ImageStreamer getDummyImageStreamer(ImageStreamer imgStreamer)
        {
            ImageStreamer dummyImageStreamer = new ImageStreamer();
            dummyImageStreamer.BackColor = imgStreamer.BackColor;
            dummyImageStreamer.Type = imgStreamer.Type;
            dummyImageStreamer.InternalBackColor = imgStreamer.InternalBackColor;
            dummyImageStreamer.DummyBound = imgStreamer.DummyBound;
            dummyImageStreamer.Size = imgStreamer.Size;
            dummyImageStreamer.Name = imgStreamer.Name + "tiledummy";
            dummyImageStreamer.Visible = false;
            dummyImageStreamer.AllowDragging = imgStreamer.AllowDragging;
            if (this.ShowItemPreview)
            {
                dummyImageStreamer.Visible = true ;
                dummyImageStreamer.Images = imgStreamer.Images;
                dummyImageStreamer.StartImageIndex = imgStreamer.StartImageIndex;
                dummyImageStreamer.SlideShow = imgStreamer.SlideShow;
                dummyImageStreamer.AnimationSpeed = imgStreamer.AnimationSpeed;
                dummyImageStreamer.SliderSpeed = imgStreamer.SliderSpeed;
                dummyImageStreamer.Images = imgStreamer.Images;
                dummyImageStreamer.ImageStreamDirection = imgStreamer.ImageStreamDirection;
                dummyImageStreamer.MainText = imgStreamer.MainText;
                dummyImageStreamer.SubText = imgStreamer.SubText;
            }

            return dummyImageStreamer;
        }
      
        /// <summary>
        /// Create a new layour group.
        /// </summary>
        /// <param name="positionGroup"></param>
        /// <param name="cursorPoint"></param>
        internal void CreateNewGroup(List<TilePosition > positionGroup, Point cursorPoint)
        {
            if (positionGroup.Count > 0)
            {
                Rectangle lastGroupPosition = positionGroup[positionGroup.Count - 1].Rect;
                lastGroupPosition = new Rectangle(lastGroupPosition.X + lastGroupPosition.Width, lastGroupPosition.Y, 75, lastGroupPosition.Height);

                if (positionGroup.Count == 1)
                {
                    if (lastGroupPosition.Contains(cursorPoint))
                    {
                        OnNewGroup = true;
                        newGroupRect = lastGroupPosition;
                        this.Invalidate();
                        OnGroupIndex = positionGroup.Count;
                        
                    }
                    else
                    {
                        if (OnNewGroup)
                            this.Invalidate();
                        OnNewGroup = false;
                        newGroupRect = new Rectangle(0, 0, 0, 0);
                    }
                }

                for (int i = 1; i < positionGroup.Count; i++)
                {
                    Rectangle firstRect = positionGroup[i - 1].Rect;
                    Rectangle secondRect = positionGroup[i].Rect;
                    Rectangle gapRect = new Rectangle(firstRect.X + firstRect.Width, firstRect.Y, (secondRect.X - (firstRect.X + firstRect.Width)), firstRect.Height);
                    cursorPoint = new Point(Cursor.Position.X - this.Parent.Left, Cursor.Position.Y - (this.Parent.Top + 20));

                    if (gapRect.Contains(cursorPoint))
                    {
                        OnNewGroup = true;
                        newGroupRect = gapRect;
                            this.Invalidate();
                            OnGroupIndex = i;
                            break;
                    }
                    else if (lastGroupPosition.Contains ( cursorPoint))
                    {
                        OnNewGroup = true;
                        newGroupRect = lastGroupPosition;
                        this.Invalidate();
                        OnGroupIndex = positionGroup.Count;
                        break;
                    }
                    else
                    {
                        if (OnNewGroup)
                            this.Invalidate();
                        OnNewGroup = false;
                        newGroupRect = new Rectangle(0, 0, 0, 0);
                    }

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimerDrag_Tick(object sender, EventArgs e)
        {
            if (DummyImageStreamer != null)
            {
                Point dummyImageStreamerPoint = new Point(Cursor.Position.X - this.Parent.Left - DummyImageStreamerMousePoint.X, Cursor.Position.Y - DummyImageStreamerMousePoint.Y - (this.Parent.Top + 20));
                if (DummyImageStreamer.AllowDragging)
                dragForm.Location = new Point(dummyImageStreamerPoint.X + this.Parent.Left, dummyImageStreamerPoint.Y + this.Parent.Top + 25);
                if (dragForm.Opacity == 0.0)
                {
                    dragForm.LocationChanged += new EventHandler(dragForm_LocationChanged);
                  
                }
                 String CurrentLayout=string.Empty;
                foreach (TilePosition tilePosition in this.PositionGroup)
                {
                    if (tilePosition.Rect.Contains(dummyImageStreamerPoint))
                    {
                        CurrentHoverLayoutGroup = tilePosition.Name;
                            
                        dropImageStream(dummyFlower);
                        break;
                    }
                    else
                    {
                        int PositionGroupcount=this.PositionGroup.Count-1;
                        int lastrect=this.PositionGroup[0].Rect.X+this.PositionGroup[0].Rect.Width;
                        if(PositionGroupcount>0)
                        {
                           lastrect= this.PositionGroup[PositionGroupcount].Rect.X+this.PositionGroup[PositionGroupcount].Rect.Width;
                        }

                        foreach (TilePosition tilePosition1 in this.PositionGroup)
                        {
                            if(dummyImageStreamerPoint.X<tilePosition1.Rect.X)
                            {
                                CurrentLayout = tilePosition1.Name;
                                break;
                            }
                            else if(dummyImageStreamerPoint.X > lastrect)
                            {
                                CurrentLayout = this.PositionGroup[PositionGroupcount].Name;
                                break;
                            }
                        }
                        if (tilePosition.Name == CurrentLayout)
                        {
                            CurrentHoverLayoutGroup = tilePosition.Name;
                            dropImageStream(dummyFlower);
                            break;
                        }
                    }
                }
                if(AllowNewGroup)
                CreateNewGroup(this.PositionGroup, dragForm.Location);
                DummyImageStreamer.BringToFront();
            }
            else
                TimerDrag.Stop();

        }

        /// <summary>
        /// Sets accessories form location
        /// </summary>
        private void setAcessoriesFormLocation()
        {
            Form form = getParentForm();
            int borderWidth = form.FormBorderStyle == FormBorderStyle.None ? 0 : 7;
            accessoriesForm.Location = new Point(form.Right - accessoriesForm.Width - borderWidth, form.Bottom - this.Height - borderWidth);
            accessoriesForm.Height = this.Height-15;
        }

        /// <summary>
        /// gets a accessories form
        /// </summary>
        /// <returns></returns>
        private AccessoriesForm GetAccessoriesForm()
        {
            AccessoriesForm form = new AccessoriesForm();
            form.Height = this.Height;
             ShowNewGroup = new AccessoriesButton();
             IsExit = new AccessoriesButton();
             ShowPreview = new AccessoriesButton();

            TileGroup flow = new TileGroup();
            flow.LayoutMode = FlowLayoutMode.Vertical;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Disposed += new EventHandler(form_Disposed);
            form.BackColor = this.BackColor;
            form.Opacity = 0.8;
            
            ShowNewGroup.Size = new Size(95, 95);
            IsExit.Size = new Size(95, 95);
            ShowPreview.Size  = new Size(95, 95);
            form.Controls.Add(IsExit);
            form.Controls.Add(ShowNewGroup);
            form.Controls.Add(ShowPreview);

            IsExit.BackgroundImage = ExitButtonImage ;
            ShowPreview .BackgroundImage = ShowPreviewButtonImage;
            ShowNewGroup.BackgroundImage = NewGroupButtonImage;
            Opener = new Timer();
            Closer = new Timer();
            Closer.Interval = 10;
            Opener.Interval = 10;
            Opener.Tick += new EventHandler(Opener_Tick);
            Closer.Tick += new EventHandler(Closer_Tick);
            form.MouseLeave += new EventHandler(form_MouseLeave);

            IsExit.Click += new EventHandler(Exit_Click);
            ShowNewGroup.Click += new EventHandler(ShowNewGroup_Click);
            ShowPreview.Click += new EventHandler(ShowPreview_Click);
            return form;

        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ShowPreview_Click(object sender, EventArgs e)
        {
           this.ShowItemPreview = !this.ShowItemPreview;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ShowNewGroup_Click(object sender, EventArgs e)
        {
            this.AllowNewGroup = !this.AllowNewGroup;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Exit_Click(object sender, EventArgs e)
        {
            Form form = getParentForm();
            form.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void form_MouseLeave(object sender, EventArgs e)
        {
            if(!accessoriesForm .Bounds .Contains (Cursor.Position.X ,Cursor .Position .Y ))
            Closer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Closer_Tick(object sender, EventArgs e)
        {
            if (!isOpening)
            {
                if (accessoriesForm.Width > 5)
                {
                    accessoriesForm.Opacity = 0.4;
                    accessoriesForm.Width -= 15;
                    setAcessoriesFormLocation();
                    isClosing = true;
                }
                else
                {
                    Closer.Stop();
                    accessoriesForm.Dispose();
                    this.Invalidate();
                    isClosing = false;

                }
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Opener_Tick(object sender, EventArgs e)
        {
            if (!isClosing)
            {
                if (accessoriesForm.Width < 90)
                {
                    accessoriesForm.Width += 15;
                    isOpening = true;
                }
                else
                {
                    Opener.Stop();
                    isOpening = false;
                }
                setAcessoriesFormLocation();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void form_Disposed(object sender, EventArgs e)
        {
            isOnAccessoriesPanelButton = false;
            this.Invalidate();
            IsExit.Click -= new EventHandler(Exit_Click);
            ShowNewGroup.Click -= new EventHandler(ShowNewGroup_Click);
            ShowPreview.Click -= new EventHandler(ShowPreview_Click);
            accessoriesForm.MouseLeave -= new EventHandler(form_MouseLeave);
            accessoriesForm.Disposed -= new EventHandler(form_Disposed);
            accessoriesForm = null;
            Opener.Tick -= new EventHandler(Opener_Tick);
            Closer.Tick -= new EventHandler(Closer_Tick);
           
        }

        /// <summary>
        /// Gets the parent form.
        /// </summary>
        /// <returns></returns>
        private Form getParentForm()
        {
            if (this.Parent != null)
            {
                if (this.Parent is Form)
                {
                    return this.Parent as Form;
                }
                else
                {
                    return getParentForm();
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dragForm_LocationChanged(object sender, EventArgs e)
        {
            if(dragForm.Opacity != 0.5)
            foreach (Control ctrl in this.Controls)
            {

                if (ctrl is LayoutGroup)
                {
                    foreach (Control img in ctrl.Controls)
                    {
                        if (img is ImageStreamer)
                        {
                            if (this.DummyImageStreamer.Name == img.Name + "dummy")
                            {
                                img.Parent.Controls.Remove(img);
                                draggingImageStreamer = img as ImageStreamer;
                            }
                        }
                    }
                    (ctrl as LayoutGroup).ArrangeControl();
                }
            }
            dragForm.Opacity = 0.5;
            calculateGroupPosition();
        }

        /// <summary>
        /// Calcuates the group position
        /// </summary>
        internal void calculateGroupPosition()
        {
            PositionGroup.Clear();
            PositionItems.Clear();
            int i = 0;
                foreach (Control ctrl in this.Controls)
            {
               
                if (ctrl is LayoutGroup)
                {
                    TilePosition tilePosition = new TilePosition();
                    tilePosition.Name = ctrl.Name;
                    tilePosition.Rect = ctrl.Bounds;
                    tilePosition.Index = i;
                    PositionGroup.Add(tilePosition);
                    calculateImageStreamerPosition(ctrl as LayoutGroup);
                    i++;
                }
            }
        }

        /// <summary>
        /// Resets the imagesstreamer bounds from the dumy bounds
        /// </summary>
        internal void resetImageStreamerBounds()
        {
            this.ChildStreamer.Clear();
            foreach (Control group in this.Controls)
            {
                if (group is LayoutGroup)
                {
                    foreach (Control ctrl in group.Controls)
                    {
                        if (ctrl is ImageStreamer)
                        {
                            ImageStreamer img = (ctrl as ImageStreamer);
                            (ctrl as ImageStreamer).DummyBound = new Rectangle(0, 0, ctrl.Width, ctrl.Height);
                            ctrl.Invalidate();
                            ChildStreamer.Add(img);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets size for the dummy imagestreamer
        /// </summary>
        internal void setDummySize()
        {
            foreach (ImageStreamer img in this.ChildStreamer)
            {
                img.DummyBound = new Rectangle(5, 5, img.Width - 10, img.Height - 10);
            }
        }

        /// <summary>
        /// Calculates the imagestreamer position
        /// </summary>
        /// <param name="layoutGroup"></param>
        internal void calculateImageStreamerPosition(LayoutGroup layoutGroup)
        {
            int i = 0;
            foreach (Control ctrl in layoutGroup.Controls)
            {
                if (ctrl is ImageStreamer)
                {
                    TilePosition tilePosition = new TilePosition();
                    tilePosition.Name = ctrl.Name;
                    Rectangle rect = new Rectangle(ctrl.Parent.Left + ctrl.Bounds.Left, ctrl.Parent.Top + ctrl.Bounds.Top, ctrl.Bounds.Width, ctrl.Bounds.Height);

                    
                    tilePosition.Rect = rect;
                    tilePosition.Index = i;
                 

                        PositionItems.Add(tilePosition);
                        i++;
                 
                        (ctrl as ImageStreamer).DummyBound = new Rectangle(5, 5, ctrl.Width - 10, ctrl.Height - 10);
                        ctrl.Invalidate();
                    
                }
            }
        }

        /// <summary>
        /// Calculates the dimensions
        /// </summary>
        internal void calculateDimensions()
        {

            AccessoriesRegion = new Rectangle(this.DisplayRectangle.Width  - 15, this.DisplayRectangle.Height  - 15, 15, 15);
            if (this.accessoriesForm != null)
            {
                this.accessoriesForm.Left = this.Right;
                this.accessoriesForm.Height = this.Height;
            }
            groupTextPosition.Clear();
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is LayoutGroup)
                {
                    LayoutGroup group = (ctrl as LayoutGroup);
                    ctrl.Height = this.Height - 150;
                    group.setWidth();
                    TilePosition groupPosition = new TilePosition();
                    groupPosition.Rect = group.Bounds;
                    groupPosition.Font = group.Font;
                    groupPosition.Text = group.Name;
                    groupPosition.ForeColor = group.ForeColor;
                    groupTextPosition.Add(groupPosition);
                }
            }
            resetImageStreamerBounds();
            this.Invalidate();
        }

        # endregion

    }

    # endregion

    # region DraggerForm

    public class DraggerForm : Form
    {
        private TileLayout tileLayout = new TileLayout();
        /// <summary>
        ///  Tilelayout
        /// </summary>
        public TileLayout tile
        {
            get
            {
                return tileLayout;
            }
            set
            {
                tileLayout = value;
            }
        }
    }

    # endregion

    # region TilePosition

    public class TilePosition
    {
        /// <summary>
        /// Position rectangle
        /// </summary>
        public Rectangle Rect;
       
        /// <summary>
        /// Indicates name of the tile
        /// </summary>
        public string Name;

        /// <summary>
        /// Indicates index of the tile
        /// </summary>
        public int Index;

        /// <summary>
        /// Indicates the font of the tile
        /// </summary>
        public Font Font;
        
        /// <summary>
        /// Indicates the text of the tile
        /// </summary>
        public string Text;

        /// <summary>
        /// Indicates fore color of the tile
        /// </summary>
        public Color ForeColor;
    }

    # endregion

    # region AccessoriesButton

    [ToolboxItem(false)]
    public class AccessoriesButton : ButtonAdv
    {
        # region Members

        /// <summary>
        /// Indicates mouse on button
        /// </summary>
        bool onButton = false;

        # endregion

        # region Overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {

            //base.OnPaint(e);

            if (onButton)
            {
                SolidBrush brush = new SolidBrush(Color.Black);
                e.Graphics.FillRectangle(brush, new Rectangle(0, 0, this.Width, this.Height));
                brush.Dispose();
            }
            else
            {
                SolidBrush brush = new SolidBrush(this.Parent.BackColor  );
                e.Graphics.FillRectangle(brush, new Rectangle(0, 0, this.Width, this.Height));
                brush.Dispose();
            }
            if(this.BackgroundImage != null)
            e.Graphics.DrawImage (this.BackgroundImage ,new Rectangle (0,0,this.Width ,this.Height) );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            onButton = true;
  
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            onButton = false;
        }

        # endregion

    }

    # endregion

    # region AccessoriesForm

    public class AccessoriesForm : Form
    {
        # region Members

        /// <summary>
        /// Indicates current row
        /// </summary>
        int row;

        /// <summary>
        /// Indicates current column
        /// </summary>
        int column;

        /// <summary>
        /// Indicates first items in the row
        /// </summary>
        bool isFirst;

        # endregion

        # region Constructor

        public  AccessoriesForm()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor,true);
        }

        # endregion

        # region Overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.Opacity = 0.9;
            this.BackColor = Color.DarkGray  ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            ArrangeButtons();
        }

        # endregion

        # region Methods

        /// <summary>
        /// Arragne controls
        /// </summary>
        internal void ArrangeButtons()
        {

            foreach (Control ctrl in this.Controls)
            {

                int nc = 0;
                if (row * 120 + 10 > this.Height - 200)
                {
                    row = 0;
                    column++;
                }
                if (ctrl.Width == 120)
                {
                    if (isFirst)
                    {
                        ctrl.Location = new Point(column * 250 + nc, row * 125);
                        isFirst = false;
                        nc = 0;
                    }
                    else
                    {

                        ctrl.Location = new Point(column * 250 + 10 + 120, row * 125);
                        isFirst = true;
                        row++;
                    }
                }
                else
                {
                    if (!isFirst)
                        row++;
                    ctrl.Location = new Point(column * 250 + nc, row * 125);
                    isFirst = true;
                    row++;
                    nc = 0;
                }


            }
            isFirst = true;
            row = 0;
            column = 0;

        }

        # endregion

    }

    # endregion

    # region TileGroupCollection

    //  [Editor(typeof(TreeNodeAdvCollectionEditor), typeof(UITypeEditor))]
    public class TileGroupCollection : ArrayListExt
    {

        // Fields
        internal IComparer comparer = null;
        /// <summary>
        /// Creates a new instance of the collection.
        /// </summary>
        public TileGroupCollection()
        {
             
        }

        public event CollectionChangeEventHandler BeforeRemoving;

        public override void RemoveAt(int index)
        {
            OnBeforeRemoving(index);

            base.RemoveAt(index);
        }

        protected virtual void OnBeforeRemoving(int index)
        {
            LayoutGroup removingNode = this[index];

            CollectionChangeEventArgs e =
                new CollectionChangeEventArgs(CollectionChangeAction.Remove, removingNode);

            RaiseBeforeRemoving(e);
        }

        protected void RaiseBeforeRemoving(CollectionChangeEventArgs e)
        {
            if (this.BeforeRemoving != null)
            {
                BeforeRemoving(this, e);
            }
        }

        /// </override>
        protected override void OnCollectionChanged(CollectionChangeEventArgs args)
        {
            base.OnCollectionChanged(args);
        }

        /// <summary>
        /// Gets / sets a reference to the TreeNodeAdv at the specified index location in the
        /// collection.
        /// In C#, this property is the indexer for the TreeNodeAdvCollection class.
        /// </summary>
        /// <param name="index">The location of the TreeNodeAdv in the collection.</param>
        /// <value>The reference to the TreeNodeAdv.</value>
        public new LayoutGroup this[int index]
        {
            get
            {
                return (LayoutGroup)base[index];
            }
            set
            {
                base[index] = value;
            }
        }

        /// <summary>
        /// Adds a <see cref="TreeNodeAdv"/> to the collection.
        /// </summary>
        /// <param name="node">The <see cref="TreeNodeAdv"/> to add.</param>
        /// <returns>The position of the added node in the list.</returns>
        public virtual int Add(LayoutGroup node)
        {
            return base.Add(node);
        }
        /// <summary>
        /// Adds an array of TreeNodeAdv objects to the collection.
        /// </summary>
        /// <param name="items">An array of <see cref="TreeNodeAdv"/> objects to add to the collection.</param>
        public void AddRange(LayoutGroup[] items)
        {
            base.AddRange(items);
        }


        /// </override>

        public override void Sort()
        {
            Sort(SortOrder.Ascending);
        }

        /// <summary>
        /// Sorts the collection using the specified sort order.
        /// </summary>
        /// <param name="order">One of the <see cref="SortOrder"/> entries.</param>
        public virtual void Sort(SortOrder order)
        {
            if (order == SortOrder.None) return;
            if (this.comparer != null)
                this.Sort(comparer);
            else
                base.Sort();
            if (order == SortOrder.Descending)
                this.Reverse();
        }
    }

    # endregion
}
