#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using Syncfusion.Collections;
using System.Collections;
using System.Drawing;
using System.Drawing.Design;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Drawing2D;
using Syncfusion.Windows.Forms.Design;
using System.Windows.Forms.Design;
using Syncfusion.Runtime.InteropServices;
using System.Reflection;
using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Carousel Control - a circular conveyor used on which objects are displayed and rotated.
    /// The Carousel control provides a 3D interface for displaying objects.
    /// </summary>
    [ToolboxItem(true), ToolboxBitmap(typeof(Carousel), "ToolboxIcons.Carousel.png"), Docking(DockingBehavior.AutoDock)]
    [Designer(typeof(CarouselDesigner))]
    public class Carousel : Control, IVisualStyle
    {
        #region Variables

        bool objReached = false;
        bool ltr = false;
        bool isMouseDown = false;

        CarouselElement activeCarouselItem = null;
        CarouselElement m_Selected = null;
        CarouselElement selElement = null;

        Point endPt = Point.Empty;
        Point stPont = Point.Empty;
        Point _ptFirst = Point.Empty;
        Point _ptSecond = Point.Empty;

        int x0, y0, nRadX, nRadY, _iArguments;

        double PI_FACT = Math.PI / 180.0f, AngleActualTemp;

        float m_Alfa = 0;

        internal int clikedItemId = -1;

        Timer mainTimer = new Timer();

        internal List<PreviewElement> previewElementCollection = new List<PreviewElement>();
        List<CarouselElement> previewImageCollection = new List<CarouselElement>();

        /// <summary>
        /// Occurs when an item in the carousel view is selected
        /// </summary>
        public virtual event OnCarouselItemSelectionChangedEventHandler OnCarouselItemSelectionChanged;

        /// <summary>
        /// Occurs when an item in the carousel view is being selected
        /// </summary>
        public virtual event OnCarouselItemSelectionChangingEventHandler OnCarouselItemSelectionChanging;

        /// <summary>
        /// Occurs when a selected item arrives to the center of the carousel view
        /// </summary>
        public virtual event OnCarouselItemFocusedEventHandler OnCarouselItemFocused;

        #endregion

        /// <summary>
        /// Constructor. Creates a new instance of carousel.
        /// </summary>
        public Carousel()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(Carousel));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            mainTimer.Interval = 1;
            mainTimer.Tick += new EventHandler(mainTimer_Tick);
            EvaluateInitializationSettings();
            NativeMethods.SetupStructSizes();
        }

        /// <summary>
        /// Preset the calculated value used in rendering images
        /// </summary>
        private void EvaluateInitializationSettings()
        {
            x0 = Width / 2;
            y0 = Height / 2;
            nRadX = (Width / 2) * 7 / 10;
            nRadY = (int)(nRadX / this.Perspective);
            AngleActualTemp = (180 + (this.TransitionSpeed)) * PI_FACT;
        }

        #region Properties

        private bool rotateAlways = false;
        /// <summary>
        /// Gets or sets a value to rotate the child items continuously
        /// </summary>
        [Description("Gets or sets a value to rotate the child items continuously"), Category("Behavior")]
        public bool RotateAlways
        {
            get
            {
                return rotateAlways;
            }
            set
            {
                rotateAlways = value;
                if (!this.ImageSlides && !this.DesignMode && this.layoutManager != null)
                {
                    if (!this.layoutManager.transForming)
                        this.layoutManager.BeginTransform();
                    else
                        this.layoutManager.EndTransform();
                }
                this.Refresh();
            }
        }
        /// <summary>
        /// Sets the custom bounds if true
        /// </summary>
        private bool useCustomBounds = false;
        /// <summary>
        /// Gets or Sets whether custom bounds should be used
        /// </summary>
        [Description("Gets or Sets whether custom bounds should be used"), Category("Appearance")]
        public bool UseCustomBounds
        {
            get
            {
                return useCustomBounds;
            }
            set
            {
                if (useCustomBounds != value)
                    useCustomBounds = value;
                if(this.layoutManager!=null)
                    this.layoutManager.PerformLayout();
            }
        }
        /// <summary>
        /// Sets the carouselItembounds
        /// </summary>
        private int carouselBounds = 150;
        /// <summary>
        /// Gets or Sets the Carousel Idtem Bounds
        /// </summary>
        [Description("Gets or Sets the Carousel Item Bounds"), Category("Appearance"),]
        public int CarouselBounds
        {
            get
            {
                return carouselBounds;
            }
            set
            {
                if(carouselBounds!=value)
                    carouselBounds = value;
            }   
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ShouldSerializeCarouselBounds()
        {
            return carouselBounds != 150;
        }
        /// <summary>
        /// 
        /// </summary>
        private void ResetCarouselBounds()
        {
            carouselBounds = 150;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ShouldSerializeUseCustomBounds()
        {
            return useCustomBounds != false;
        }
        /// <summary>
        /// 
        /// </summary>
        void ResetUseCustomBounds()
        {
            useCustomBounds = false;
        }
        private string designText = string.Empty;
        /// <summary>
        /// Gets or sets a text to display in the Design mode, when ImageSlides is enabled
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Description("Gets or sets a text to display in the Design mode, when ImageSlides is enabled"), Category("Behavior")]
        public string DesignText
        {
            get
            {
                return designText;
            }
            set
            {
                designText = value;
                this.Refresh();
            }
        }

        private int padX = 0;
        /// <summary>
        /// Gets or sets a value to shift the items with respect to X - axis.
        /// </summary>
        [Description("Gets or sets a value to shift the items with respect to X - axis."), Category("Behavior")]
        public int PadX
        {
            get
            {
                return padX;
            }
            set
            {
                padX = value;
                this.Refresh();
            }
        }

        private int padY = 0;
        /// <summary>
        /// Gets or sets a value to shift the items with respect to Y - axis.
        /// </summary>
        [Description("Gets or sets a value to shift the items with respect to X - axis."), Category("Behavior")]
        public int PadY
        {
            get
            {
                return padY;
            }
            set
            {
                padY = value;
                this.Refresh();
            }
        }



        private bool showImagePreview = false;
        /// <summary>
        /// Gets or Sets a value to preview the selected image
        /// </summary>
        [Description("Gets or Sets a value to preview the selected image"), Category("Behavior")]
        public bool ShowImagePreview
        {
            get
            {
                return showImagePreview;
            }
            set
            {
                showImagePreview = value;
                this.Refresh();
            }
        }

        private bool showImageShadow = true;
        /// <summary>
        /// Gets or sets a value to display shadow for the images
        /// </summary>
        /// <remarks>Turn off this will increase performance. </remarks>
        [Description("Gets or sets a value to display shadow for the images"), Category("Behavior")]
        public bool ShowImageShadow
        {
            get
            {
                return showImageShadow;
            }
            set
            {
                showImageShadow = value;
                this.Refresh();
            }
        }

        private bool useOriginalImageinPreview = false;
        /// <summary>
        /// Gets or sets a value to display original image for preview, else compressed image will be used
        /// </summary>
        /// <remarks>For high resolution images, its preview may affect performance</remarks>
        [Description("Gets or sets a value to display original image for preview, else compressed image will be used"), Category("Behavior")]
        public bool UseOriginalImageinPreview
        {
            get
            {
                return useOriginalImageinPreview;
            }
            set
            {
                useOriginalImageinPreview = value;
                this.Refresh();
            }
        }

        private Color highlightColor = Color.White;
        /// <summary>
        /// Gets or sets a color to render the selection rectangle of images
        /// </summary>
        [Description("Gets or sets a color to render the selection rectangle of images"), Category("Behavior")]
        public Color HighlightColor
        {
            get
            {
                return highlightColor;
            }
            set
            {
                highlightColor = value;
                this.Refresh();
            }
        }

        private Color imageshadeColor = Color.Black;
        /// <summary>
        /// Gets or sets a color to shade the images in view
        /// </summary>
        [Description("Gets or sets a color to shade the images in view"), Category("Behavior")]
        public Color ImageshadeColor
        {
            get
            {
                return imageshadeColor;
            }
            set
            {
                imageshadeColor = value;
                this.Refresh();
            }
        }

        private CircularList circularViews = null;
        /// <summary>
        /// Internal collection of child items
        /// </summary>
        internal CircularList CircularViews
        {
            get
            {
                return circularViews;
            }
            set
            {
                circularViews = value;
            }
        }

        private Control activeItem = null;
        /// <summary>
        /// Currently active items / Control in the view
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public Control ActiveItem
        {
            get
            {
                return activeItem;
            }
            set
            {
                activeItem = value;
            }
        }

        private Image activeImage = null;
        /// <summary>
        /// Currently active items / Control in the view
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public Image ActiveImage
        {
            get
            {
                return activeImage;
            }
            set
            {
                activeImage = value;
            }
        }

        /// <summary>
        /// Used internally for control's layout
        /// </summary>
        internal CarouselLayoutManager layoutManager = null;

        private ItemCollection items = null;

        /// <summary>
        /// Gets or sets the items associated with the Carousel.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(CustomCollectionEditor), typeof(UITypeEditor))]
        public ItemCollection Items
        {
            get
            {
                if (items == null)
                {
                    items = new ItemCollection(this);
                }

                return items;
            }
            set
            {
                if (!ImageSlides)
                {
                    items = value;
                    PerformLayout();
                }
            }
        }

        private CarouselImageCollection imageListCollection = null;
        /// <summary>
        /// Gets or sets a collection of items to display in the carousel
        /// </summary>
        /// <remarks>First priority in loading images to control</remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Description("Gets or sets a collection of items to display in the carousel"), Category("Behavior")]
        public CarouselImageCollection ImageListCollection
        {
            get
            {
                if (imageListCollection == null)
                {
                    imageListCollection = new CarouselImageCollection(this);
                }

                return imageListCollection;
            }
            set
            {
                imageListCollection = value;
            }
        }

        private ImageList imageList = null;
        /// <summary>
        /// Imagelist which contains images to populate in the control
        /// </summary>
        /// <remarks>Second priority in loading images to control</remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Description("Imagelist which contains images to populate in the control"), Category("Behavior")]
        public ImageList ImageList
        {
            get
            {
                return imageList;
            }
            set
            {
                imageList = value;
                if (imageSlides)
                {
                    if (this.Items.Count > 0)
                    {
                        this.Items.Clear();
                    }
                    RemoveImages();
                    LoadImages();
                }
            }
        }

        private string filePath = string.Empty;
        /// <summary>
        /// FilePath where the control can fetch images to display
        /// </summary>
        /// <remarks>Last priority in loading images to control</remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("Behavior"), Description("Address of a location where the control can fetch images to display")]
        public string FilePath
        {
            get
            {
                return filePath;
            }
            set
            {
                filePath = value;
                if (imageSlides)
                {
                    if (this.Items.Count > 0)
                    {
                        this.Items.Clear();
                    }
                    RemoveImages();
                    LoadImages();
                }
            }
        }



        bool imageSlides = false;
        /// <summary>
        /// Gets or Sets a value to display images in the control
        /// </summary>
        [Description("Gets or Sets a value to display images in the control"), Category("Behavior")]
        public bool ImageSlides
        {
            get
            {
                return imageSlides;
            }
            set
            {
                imageSlides = value;
                CheckImageAvailabilty();
                if (value)
                    this.ActiveItem = null;
                else
                    this.ActiveImage = null;
                this.Refresh();
            }
        }


        private float perspective = 4;
        /// <summary>
        /// Gets or Sets a value to render the items in specified angle
        /// </summary>
        [Description("Gets or Sets a value to render the items in specified angle"), Category("Behavior")]
        public float Perspective
        {
            get
            {
                return perspective;
            }
            set
            {
                if (value < 2)
                    value = 2;
                perspective = value;

            }
        }

        private float speed = 2;
        /// <summary>
        /// Gets or Sets a value to rotate the child objects of the control
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Description("Gets or Sets a value to rotate the child objects of the control"), Category("Behavior")]
        public float TransitionSpeed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
                if (this.layoutManager != null)
                    this.layoutManager.m_AlphaAxes = value;

            }
        }

        private CarouselPath carouselPath = CarouselPath.Default;
        /// <summary>
        /// Gets or sets a value to the path in which the child items in carousel should traverse
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Description("Gets or sets a value to the path in which the child items in carousel should traverse"), Category("Behavior")]
        public CarouselPath CarouselPath
        {
            get
            {
                return carouselPath;
            }
            set
            {
                carouselPath = value;
                switch (value)
                {
                    case CarouselPath.Linear:
                    case CarouselPath.Orbital:
                    case CarouselPath.Oval:
                        this.Perspective = 2;
                        break;
                    case CarouselPath.Default:
                        this.Perspective = 4;
                        break;
                }
                this.Refresh();

            }
        }

        private CarouselVisualStyle visualStyle = CarouselVisualStyle.Default;
        /// <summary>
        /// Gets or Sets a value to apply visual style to the child controls
        /// </summary>
        [Description("Gets or Sets a value to apply visual style to the child controls"), Category("Behavior")]
        public CarouselVisualStyle VisualStyle
        {
            get
            {
                return visualStyle;
            }
            set
            {
                visualStyle = value;
                switch (visualStyle)
                {
                    case CarouselVisualStyle.OfficeStyle:
                        {
                            foreach (Control ctrl in this.Controls)
                            {
                                if (ctrl is ButtonAdv)
                                {
                                    (ctrl as ButtonAdv).UseVisualStyle = true;
                                }
                            }
                        }
                        SkinManager.SetVisualStyle(this, VisualTheme.Office2007Blue);
                        break;
                    case CarouselVisualStyle.Metro:
                        {
                            foreach (Control ctrl in this.Controls)
                            {
                                if (ctrl is ButtonAdv)
                                {
                                    (ctrl as ButtonAdv).UseVisualStyle = true;
                                }
                            }
                        }
                        SkinManager.SetVisualStyle(this, "Metro");
                        break;
                    default:
                        {
                            foreach (Control ctrl in this.Controls)
                            {
                                if (ctrl is ButtonAdv)
                                {
                                    (ctrl as ButtonAdv).UseVisualStyle = false;
                                }
                            }
                        }
                        SkinManager.SetVisualStyle(this, "Default");
                        break;
                }
                this.Refresh();
            }
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Set angle and speed for the child items to rotate
        /// </summary>
        private void SetAxes()
        {
            if (ltr)
            {
                m_Alfa += -(this.TransitionSpeed);
            }
            else
            {
                m_Alfa += this.TransitionSpeed;
            }

            if (m_Alfa > 360)
                m_Alfa = m_Alfa - 360;

            if (m_Alfa < 0)
                m_Alfa = m_Alfa + 360;
        }

        /// <summary>
        /// Helper to configure the preview elements and its settings
        /// </summary>
        private void ConfigurePreviewElementSettings()
        {
            int maxDX = Bounds.Width / 2;
            int maxDY = Bounds.Height / 2;

            for (int i = 0; i < previewElementCollection.Count; i++)
            {
                PreviewElement f = previewElementCollection[i];

                Rectangle rc;

                int newWidth = 0;
                int newHeight = 0;

                if (newWidth * f.PreviewBitmap.Height / f.PreviewBitmap.Width > maxDY)
                {
                    newWidth = maxDX;
                    newHeight = newWidth * f.PreviewBitmap.Height / f.PreviewBitmap.Width;
                }
                else
                {
                    newHeight = maxDY;
                    newWidth = newHeight * f.PreviewBitmap.Width / f.PreviewBitmap.Height;
                }

                rc = new Rectangle(Bounds.Width / 2 - newWidth / 2,
                    Bounds.Height / 2 - newHeight / 2,
                    newWidth,
                    newHeight);

                switch (f.PreviewBitmapState)
                {
                    case 0:
                        {
                            f.PreviewBitmapRect = new Rectangle(
                                f.PreviewBitmapStartRect.X + (rc.X - f.PreviewBitmapStartRect.X) * f.PreviewBitmapPerc / 100,
                                f.PreviewBitmapStartRect.Y + (rc.Y - f.PreviewBitmapStartRect.Y) * f.PreviewBitmapPerc / 100,
                                f.PreviewBitmapStartRect.Width + (rc.Width - f.PreviewBitmapStartRect.Width) * f.PreviewBitmapPerc / 100,
                                f.PreviewBitmapStartRect.Height + (rc.Height - f.PreviewBitmapStartRect.Height) * f.PreviewBitmapPerc / 100
                                );

                            f.PreviewBitmapPerc += 5;

                            if (f.PreviewBitmapPerc > 100)
                            {
                                f.PreviewBitmapPerc = 100;
                                f.PreviewBitmapState = 0;
                            }
                        }
                        break;
                    case 1:
                        break;
                    case 2:
                        {
                            f.PreviewBitmapRect = new Rectangle(
                                f.PreviewBitmapRect.X + (f.PreviewObject.m_Rect.X - f.PreviewBitmapRect.X) * (100 - f.PreviewBitmapPerc) / 100,
                                f.PreviewBitmapRect.Y + (f.PreviewObject.m_Rect.Y - f.PreviewBitmapRect.Y) * (100 - f.PreviewBitmapPerc) / 100,
                                f.PreviewBitmapRect.Width + (f.PreviewObject.m_Rect.Width - f.PreviewBitmapRect.Width) * (100 - f.PreviewBitmapPerc) / 100,
                                f.PreviewBitmapRect.Height + (f.PreviewObject.m_Rect.Height - f.PreviewBitmapRect.Height) * (100 - f.PreviewBitmapPerc) / 100
                                );

                            f.PreviewBitmapPerc -= 5;

                            if (f.PreviewBitmapPerc < 0)
                            {
                                previewElementCollection.RemoveAt(i);
                                i--;
                            }
                        }
                        break;
                }

            }
        }

        Rectangle centerRec = Rectangle.Empty; bool allow = true;

        void mainTimer_Tick(object sender, EventArgs e)
        {
            if (!this.DesignMode && this.ImageSlides)
            {
                if (allow)
                {
                    if (selElement != null)
                    {
                        endPt = new Point(selElement.m_Rect.Location.X + selElement.m_Rect.Width, selElement.m_Rect.Location.Y + selElement.m_Rect.Height);
                        stPont = new Point(selElement.m_Rect.Location.X, selElement.m_Rect.Location.Y + selElement.m_Rect.Height);
                    }

                    centerRec = CenterImageRectangle();

                    if (RotateAlways)
                    {
                        SetAxes();
                    }
                    else
                    {
                        if (isMouseDown && selElement != null && (centerRec.Contains(/*selElement.m_Rect.Location*/stPont) || centerRec.Contains(selElement.m_Rect.Location)))
                        {
                            if (ltr && (centerRec.Contains(/*selElement.m_Rect.Location*/stPont) || centerRec.Contains(selElement.m_Rect.Location)))
                            {
                                isMouseDown = false;
                                objReached = true;
                                this.activeCarouselItem = selElement;
                                this.ActiveImage = selElement.m_bmpOriginal;
                                CarouselItemFocusedArgs args = new CarouselItemFocusedArgs(this);
                                if (OnCarouselItemFocused != null)
                                {
                                    OnCarouselItemFocused(this, args);
                                }
                            }
                            else if (!ltr && centerRec.Contains(endPt))
                            {
                                isMouseDown = false;
                                objReached = true;
                                this.activeCarouselItem = selElement;
                                this.ActiveImage = selElement.m_bmpOriginal;
                                CarouselItemFocusedArgs args = new CarouselItemFocusedArgs(this);
                                if (OnCarouselItemFocused != null)
                                {
                                    OnCarouselItemFocused(this, args);
                                }
                            }
                            else
                            {
                                SetAxes();
                            }
                        }
                        else if ((isMouseDown && selElement != null && ((!centerRec.Contains(/*selElement.m_Rect.Location*/stPont)) && !centerRec.Contains(selElement.m_Rect.Location))))
                        {
                            if (!objReached)
                            {
                                SetAxes();
                            }
                        }
                    }
                }
                this.Refresh();
                ConfigurePreviewElementSettings();
            }
        }

        /// <summary>
        /// Helper used to display images in carousel view
        /// </summary>
        private void RenderRunTimeObjects()
        {

            EvaluateInitializationSettings();

            for (int i = 0; i < previewImageCollection.Count; i++)
            {
                CarouselElement t = previewImageCollection[i];

                previewImageCollection[i].m_dAngleActual = (previewImageCollection[i].m_dAngleOriginal + m_Alfa) * PI_FACT;

                previewImageCollection[i].m_dDistanceFromScreen = 10 + 10 * Math.Cos(previewImageCollection[i].m_dAngleActual);

                int x, y;
                switch (this.CarouselPath)
                {
                    case CarouselPath.Orbital:
                        x = x0 + (int)(nRadX * Math.Sin(t.m_dAngleActual));
                        y = y0 - (int)(nRadY * Math.Sin(t.m_dAngleActual));
                        break;
                    case CarouselPath.Linear:
                        x = x0 + (int)(nRadX * Math.Cos(t.m_dAngleActual));
                        y = y0 - (int)(nRadY * Math.Cos(t.m_dAngleActual));
                        break;
                    case CarouselPath.Oval:
                        x = x0 + (int)(nRadX * Math.Cos(t.m_dAngleActual));
                        y = y0 - (int)(nRadY * Math.Sin(t.m_dAngleActual));
                        break;
                    default:
                        x = x0 + (int)(nRadX * Math.Sin(t.m_dAngleActual));
                        y = y0 - (int)(nRadY * Math.Cos(t.m_dAngleActual));
                        break;
                }
                
                ///////////////////////////////////////////////////////

                float dSize = (float)(80 - 20 * Math.Cos(t.m_dAngleActual));

                dSize = dSize / 100;

                // getting location
                t.m_Rect.X = (int)(x - (t.m_bmpMain.Width * dSize) / 2) + PadX;
                t.m_Rect.Y = (int)(y - (t.m_bmpMain.Height * dSize)) + PadY;
                t.m_Rect.Width = (int)(t.m_bmpMain.Width * dSize);
                t.m_Rect.Height = (int)(t.m_bmpMain.Height * dSize);

                t.m_RectShadow.X = (int)(x - (t.m_bmpMain.Width * dSize) / 2) + PadX;
                t.m_RectShadow.Y = (int)y + PadY;
                t.m_RectShadow.Width = (int)(t.m_bmpMain.Width * dSize);
                t.m_RectShadow.Height = (int)(t.m_bmpMain.Height * dSize);
            }
        }

        /// <summary>
        /// Helper used to display the images as preview
        /// </summary>
        /// <param name="objectGraphics">Graphics used to draw images</param>
        private void RenderPreviewObjects(Graphics objectGraphics)
        {
            Color c = this.ImageshadeColor;

            previewImageCollection.Sort(delegate(CarouselElement p1, CarouselElement p2) { return p2.m_dDistanceFromScreen.CompareTo(p1.m_dDistanceFromScreen); });

            int nIndex = -1;

            Point ptClient = PointToClient(MousePosition);

            for (int i = previewImageCollection.Count - 1; i >= 0 && nIndex == -1; i--)
            {
                CarouselElement t = previewImageCollection[i];

                if (t.m_Rect.Contains(ptClient))
                    nIndex = i;
            }
            m_Selected = null;

            if (nIndex != -1)
                m_Selected = previewImageCollection[nIndex];

            for (int i = 0; i < previewImageCollection.Count; i++)
            {
                CarouselElement t = previewImageCollection[i];

                float dTrasp = (float)(100 + 100 * Math.Cos(t.m_dAngleActual));

                objectGraphics.DrawImage(t.m_bmpMain, t.m_Rect);
                if (ShowImageShadow)
                    objectGraphics.DrawImage(t.m_bmpShadow, t.m_RectShadow);

                if (nIndex != i)
                {
                    SolidBrush sb = new SolidBrush(Color.FromArgb((int)dTrasp, c.R, c.G, c.B));
                    objectGraphics.FillRectangle(sb, t.m_Rect);
                    if (ShowImageShadow)
                        objectGraphics.FillRectangle(sb, t.m_RectShadow);
                }

                if (nIndex == i)
                    objectGraphics.DrawRectangle(new Pen(HighlightColor), t.m_Rect);
            }
            if (ShowImagePreview)
            {
                for (int i = 0; i < previewElementCollection.Count; i++)
                {
                    PreviewElement f = previewElementCollection[i];

                    float[][] ptsArray ={ 
                    new float[] {1, 0, 0, 0, 0},
                    new float[] {0, 1, 0, 0, 0},
                    new float[] {0, 0, 1, 0, 0},
                    new float[] {0, 0, 0, f.PreviewBitmapPerc/100.0f, 0}, 
                    new float[] {0, 0, 0, 0, 1}};
                    ColorMatrix clrMatrix = new ColorMatrix(ptsArray);
                    ImageAttributes imgAttributes = new ImageAttributes();
                    imgAttributes.SetColorMatrix(clrMatrix,
                    ColorMatrixFlag.Default,
                    ColorAdjustType.Bitmap);
                    //objectGraphics.InterpolationMode = InterpolationMode.Bicubic;
                    //if (ShowImagePreview)
                    objectGraphics.DrawImage(f.PreviewBitmap, f.PreviewBitmapRect, 0, 0, f.PreviewBitmap.Width, f.PreviewBitmap.Height, GraphicsUnit.Pixel, imgAttributes);
                }
            }
        }

        /// <summary>
        /// Returns the value of rectangle where the image should stop after selected
        /// </summary>
        /// <returns></returns>
        internal Rectangle CenterImageRectangle()
        {
            EvaluateInitializationSettings();
            int x1Temp = x0 + (int)(nRadX * Math.Sin(AngleActualTemp));
            int y1Temp = y0 - (int)(nRadY * Math.Cos(AngleActualTemp));

            float dSize = (float)(80 - 20 * Math.Cos(AngleActualTemp));

            dSize = dSize / 100;

            // getting location 
            int p1 = (int)(x1Temp - (this.activeCarouselItem.m_bmpMain.Width * dSize) / 2) + PadX;
            int p2 = (int)(y1Temp - (this.activeCarouselItem.m_bmpMain.Height * dSize)) + PadY;

            if (this.CarouselPath == CarouselPath.Linear || this.CarouselPath == CarouselPath.Orbital)
                p2 = (Height / 2) + padY;

            if (this.CarouselPath == CarouselPath.Default)
                p2 = p2 + (this.activeCarouselItem.m_bmpMain.Height) / 2;
            Point p = new Point(p1, p2);

            int wid = ((int)(TransitionSpeed) * 10) + 200;
            int height = this.activeCarouselItem.m_bmpMain.Height + 25;
            if (this.CarouselPath == CarouselPath.Default)
                height = (this.activeCarouselItem.m_bmpMain.Height / 2) + 25;
            Rectangle r1 = new Rectangle(p, new Size(wid, height));
            return r1;
        }

        /// <summary>
        /// Load images to the collection once the ImageSlides is true
        /// </summary>
        void LoadImages()
        {
            if (this.ImageListCollection.Count > 0)
            {
                foreach (CarouselImage img in ImageListCollection)
                {
                    previewImageCollection.Add(new CarouselElement(img.ItemImage));
                }
            }
            else if (ImageList != null)
            {

                foreach (Image image in imageList.Images)
                {
                    previewImageCollection.Add(new CarouselElement(image));
                }
            }
            else if (!string.IsNullOrEmpty(FilePath))
            {
                String[] arStrings = Directory.GetFiles(FilePath);
                for (int i = 0; i < arStrings.Length; i++)
                {
                    previewImageCollection.Add(new CarouselElement(arStrings[i]));
                }
            }

            if (previewImageCollection.Count > 0)
            {
                for (int i = 0; i < previewImageCollection.Count; i++)
                    previewImageCollection[i].m_dAngleOriginal = (float)i * (360.0f) / (float)previewImageCollection.Count;

                for (int i = 0; i < previewImageCollection.Count; i++)
                    previewImageCollection[i].m_dAngleOriginal = (float)i * (360.0f) / (float)previewImageCollection.Count;

                mainTimer.Enabled = true;

                double index = Math.Ceiling(((double)previewImageCollection.Count / 2));
                activeCarouselItem = previewImageCollection[(int)index];
                this.ActiveImage = activeCarouselItem.m_bmpOriginal;
            }
        }

        /// <summary>
        /// Unloads the images from the collection
        /// </summary>
        void RemoveImages()
        {
            previewImageCollection.Clear();
        }

        /// <summary>
        /// Populate the images / controls in the carousel
        /// </summary>
        internal void Populate()
        {
            CheckImageAvailabilty();
            if (!(Disposing || IsDisposed))
            {
                UpdateQueue();
                PerformLayout();
                if (this.Parent != null)
                {
                    this.Parent.ResumeLayout(true);
                    this.Parent.Invalidate(true);
                    this.Parent.Refresh();
                }
            }
        }

        /// <summary>
        /// Updates the internal collection which holds the value
        /// </summary>
        private void UpdateQueue()
        {
            CircularViews = new CircularList(Items);
        }

        /// <summary>
        /// Used to backup the controls while loading images
        /// </summary>
        private void CheckImageAvailabilty()
        {
            if (ImageSlides)
            {
                SetBackUp(this.items);
                RemoveImages();
                LoadImages();
            }
            else
            {
                RemoveImages();
                RestoreBackUp();
            }

        }

        /// <summary>
        /// Backups the controls in the Carousel
        /// </summary>
        /// <param name="srcItems"></param>
        private void SetBackUp(ItemCollection srcItems)
        {
            foreach (Control ctrl in this.Items)
            {
                this.Controls.Remove(ctrl);
            }

        }

        /// <summary>
        /// Restore the list of controls from the backup
        /// </summary>
        private void RestoreBackUp()
        {
            foreach (Control ctrl in this.Items)
            {
                if (!this.Controls.Contains(ctrl))
                    this.Controls.Add(ctrl);
            }

        }

        /// <summary>
        /// Returns the currently selected element
        /// </summary>
        /// <param name="location">mouse down location</param>
        /// <returns>carousel element</returns>
        private CarouselElement GetSelectedElement(Point location)
        {
            Point ptClient = PointToClient(location);
            CarouselElement selectedCarouselElement = null;
            int nIndex = -1;
            for (int i = 0; i < previewImageCollection.Count; i++)
            {
                CarouselElement t = previewImageCollection[i];

                if (t.m_Rect.Contains(ptClient))
                    nIndex = i;
            }

            selectedCarouselElement = null;

            if (nIndex != -1)
            {
                selectedCarouselElement = previewImageCollection[nIndex];
            }

            return selectedCarouselElement;
        }

        #endregion

        #region Overriden Members

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.DesignMode)
                ControlPaint.DrawFocusRectangle(e.Graphics, new Rectangle(0, 0, this.Width, this.Height));

            if (this.ImageSlides && this.DesignMode)
            {
                string displayText = string.IsNullOrEmpty(DesignText)? "Carousel's Imageslides property is enabled." + Environment.NewLine + "The images will be populated in the control at runtime":DesignText;
                e.Graphics.DrawString(displayText, new Font("Times New Roman", 11f, FontStyle.Bold), Brushes.Gray, new Point(this.Width / 4, this.Height / 4));
            }
            if (this.ImageSlides && !this.DesignMode && this.previewImageCollection.Count > 0)
            {
                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

                RenderRunTimeObjects();

                RenderPreviewObjects(e.Graphics);

            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            layoutManager = new CarouselLayoutManager(this);

            int visibleItems = Items.Count;

            if (visibleItems <= 0) return;

            double index = Math.Ceiling(((double)visibleItems / 2));

            ActiveItem = Items[(int)index];

            PerformLayout();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            layoutManager.Dispose();

            base.OnHandleDestroyed(e);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            e.Control.MouseDown += new MouseEventHandler(Control_MouseDown);
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            e.Control.MouseDown -= new MouseEventHandler(Control_MouseDown);
        }

        void Control_MouseDown(object sender, MouseEventArgs e)
        {
            ActiveItem = sender as Control;

            CarouselItemSelectionChangingArgs args = new CarouselItemSelectionChangingArgs(this);

            if (OnCarouselItemSelectionChanging != null)
            {
                OnCarouselItemSelectionChanging(this, args);
            }
            
            if (!args.Cancel)
            {

                CarouselItemSelectionChangedArgs selArgs = new CarouselItemSelectionChangedArgs(this);

                if (OnCarouselItemSelectionChanged != null)
                {
                    OnCarouselItemSelectionChanged(this, selArgs);
                }

                int x0 = this.Width / 2;
                int y0 = this.Height / 2;
                this.clikedItemId = this.Controls.IndexOf(sender as Control);
                if ((((float)(this.Controls[this.clikedItemId].Location.X - x0)) * 3.0f) / ((float)x0) < 0)
                {
                    this.layoutManager.m_AlphaAxes = -(this.TransitionSpeed);
                    this.layoutManager.LTR = true;
                }
                else
                {
                    this.layoutManager.m_AlphaAxes = this.TransitionSpeed;
                    this.layoutManager.LTR = false;
                }

                if (!this.layoutManager.transForming)
                    this.layoutManager.BeginTransform();
                else
                    this.layoutManager.EndTransform();
            }
        }

        internal void ItemTransitionEnded()
        {
            CarouselItemFocusedArgs args = new CarouselItemFocusedArgs(this);
            if (OnCarouselItemFocused != null)
            {
                OnCarouselItemFocused(this, args);
            }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            if (layoutManager != null)
                layoutManager.PerformLayout();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                foreach (Control item in Items)
                    item.Dispose();

                Items.Clear();
                ImageListCollection.Clear();
                previewElementCollection.Clear();
                previewImageCollection.Clear();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (this.ImageSlides)
            {
                CarouselItemSelectionChangingArgs args = new CarouselItemSelectionChangingArgs(this);

                if (OnCarouselItemSelectionChanging != null)
                {
                    OnCarouselItemSelectionChanging(this, args);
                }

                allow = !args.Cancel;

                int x0 = this.Width / 2;
                int y0 = this.Height / 2;
                isMouseDown = true;
                objReached = false;

                if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                {
                    for (int i = 0; i < previewElementCollection.Count; i++)
                    {
                        PreviewElement f = previewElementCollection[i];

                        if (f.PreviewBitmapState != 2)
                            f.PreviewBitmapState = 2;
                    }
                    selElement = GetSelectedElement(MousePosition);
                    if (!args.Cancel)
                    {
                        if (selElement != null)
                            this.ActiveImage = selElement.m_bmpOriginal;
                        CarouselItemSelectionChangedArgs selArgs = new CarouselItemSelectionChangedArgs(this);

                        if (OnCarouselItemSelectionChanged != null)
                        {
                            OnCarouselItemSelectionChanged(this, selArgs);
                        }

                        if (selElement != null)
                        {
                            if ((((float)(selElement.m_Rect.Location.X - x0)) * 3.0f) / ((float)x0) < 0)
                            {
                                ltr = true;
                            }
                            else
                            {
                                ltr = false;
                            }
                        }
                    }

                    if (selElement != null)
                    {
                        PreviewElement f = new PreviewElement();

                        f.PreviewObject = selElement;
                        if (UseOriginalImageinPreview)
                            f.PreviewBitmap = new Bitmap(selElement.m_bmpOriginal);
                        else
                            f.PreviewBitmap = new Bitmap(selElement.m_bmpMain);
                        f.PreviewBitmapStartRect = selElement.m_Rect;
                        f.PreviewBitmapState = 0;
                        f.PreviewBitmapPerc = 0;

                        previewElementCollection.Add(f);
                    }
                }
            }
        }

        #endregion

        #region IVisualStyle Members
        private string style;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
            }
        }

        #endregion

        #region Touch
        protected override void WndProc(ref Message m)
        {
            bool handled;
            handled = false;

            switch (m.Msg)
            {
                case NativeMethods.WM_GESTURENOTIFY:
                    {
                        // This is the right place to define the list of gestures
                        // that this application will support. By populating 
                        // GESTURECONFIG structure and calling SetGestureConfig 
                        // function. We can choose gestures that we want to 
                        // handle in our application. In this app we decide to 
                        // handle all gestures.
                        NativeMethods.GESTURECONFIG gc = new NativeMethods.GESTURECONFIG();
                        gc.dwID = 0;                // gesture ID
                        gc.dwWant = NativeMethods.GC_ALLGESTURES; // settings related to gesture
                        // ID that are to be turned on
                        gc.dwBlock = 0; // settings related to gesture ID that are
                        // to be     

                        // We must p/invoke into user32 [winuser.h]
                        bool bResult = NativeMethods.SetGestureConfig(
                            Handle, // window for which configuration is specified
                            0,      // reserved, must be 0
                            1,      // count of GESTURECONFIG structures
                            ref gc, // array of GESTURECONFIG structures, dwIDs 
                            // will be processed in the order specified 
                            // and repeated occurances will overwrite 
                            // previous ones
                           NativeMethods._gestureConfigSize // sizeof(GESTURECONFIG)
                        );

                        if (!bResult)
                        {
                            throw new Exception("Error in execution of SetGestureConfig");
                        }
                    }
                    handled = true;
                    break;

                case NativeMethods.WM_GESTURE:
                    handled = DecodeGesture(ref m);
                    break;
                default:
                    handled = false;
                    break;
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// Function to decode gestures and apply appropriate actions
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private bool DecodeGesture(ref Message m)
        {
            NativeMethods.GESTUREINFO gi;

            try
            {
                gi = new NativeMethods.GESTUREINFO();
            }
            catch (Exception excep)
            {
                Debug.Print("Could not allocate resources to decode gesture");
                Debug.Print(excep.ToString());

                return false;
            }

            gi.cbSize = NativeMethods._gestureInfoSize;

            // Load the gesture information.
            // We must p/invoke into user32 [winuser.h]
            if (!NativeMethods.GetGestureInfo(m.LParam, ref gi))
            {
                return false;
            }

            switch (gi.dwID)
            {
                case NativeMethods.GID_BEGIN:
                case NativeMethods.GID_END:
                    break;

                case NativeMethods.GID_ZOOM:
                    switch (gi.dwFlags)
                    {
                        case NativeMethods.GF_BEGIN:
                            {

                            }
                            _iArguments = (int)(gi.ullArguments & NativeMethods.ULL_ARGUMENTS_BIT_MASK);
                            _ptFirst.X = gi.ptsLocation.x;
                            _ptFirst.Y = gi.ptsLocation.y;
                            _ptFirst = PointToClient(_ptFirst);
                            break;

                        default:
                            // We read here the second point of the gesture. This
                            // is middle point between fingers in this new 
                            // position.
                            _ptSecond.X = gi.ptsLocation.x;
                            _ptSecond.Y = gi.ptsLocation.y;
                            _ptSecond = PointToClient(_ptSecond);
                            // We have to calculate zoom center point 
                            Point ptZoomCenter = new Point((_ptFirst.X + _ptSecond.X) / 2,
                                                        (_ptFirst.Y + _ptSecond.Y) / 2);

                            // The zoom factor is the ratio of the new
                            // and the old distance. The new distance 
                            // between two fingers is stored in 
                            // gi.ullArguments (lower 4 bytes) and the old 
                            // distance is stored in _iArguments.
                            double k = (double)(gi.ullArguments & NativeMethods.ULL_ARGUMENTS_BIT_MASK) /
                                        (double)(_iArguments);

                            // Now we process zooming in/out of the object
                            //_dwo.Zoom(k, ptZoomCenter.X, ptZoomCenter.Y);

                            if (k > 1)
                            {
                                this.Perspective -= 1f;
                            }
                            else if (k <= 1)
                            {
                                if (this.Perspective < 12)
                                    this.Perspective += 1f;
                                else
                                    this.Perspective = 12;
                            }

                            break;
                    }
                    break;

                case NativeMethods.GID_PAN:
                    switch (gi.dwFlags)
                    {
                        case NativeMethods.GF_BEGIN:
                            {
                                selElement = GetSelectedElement(new Point(gi.ptsLocation.x, gi.ptsLocation.y));
                                if (selElement != null)
                                {
                                    if ((((float)(selElement.m_Rect.Location.X - x0)) * 3.0f) / ((float)x0) < 0)
                                    {
                                        ltr = true;
                                    }
                                    else
                                    {
                                        ltr = false;
                                    }
                                    isMouseDown = true;
                                    objReached = false;
                                }
                            }
                            //_ptFirst.X = gi.ptsLocation.x;
                            //_ptFirst.Y = gi.ptsLocation.y;
                            //_ptFirst = PointToScreen(_ptFirst);
                            break;
                        case NativeMethods.GF_END:
                            {
                            }

                            break;
                        default:
                            break;
                    }
                    break;
            }

            return true;
        }
        #endregion



    }


    /// <summary>
    /// An internal helper used for layout of the carousel and its child items
    /// </summary>
    internal class CarouselLayoutManager
    {
        #region Variables

        private Carousel _carousel;

        internal float m_Alpha = 0;

        internal float m_AlphaAxes = 3;

        double PI_FACT = Math.PI / 180.0f;

        const double ANGLEORIGINAL = 180;

        const double ALFA = 0;

        Point centerPoint = Point.Empty;

        Timer m_Timer = new Timer();

        internal bool transForming = false;

        Point stPoint = Point.Empty;
        Point endPoint = Point.Empty;
        Point btmPoint = Point.Empty;
        internal bool LTR = false;
        #endregion

        /// <summary>
        /// Delegate for Carousel's layoutManager
        /// </summary>
        public delegate void InvokeDelegate();

        #region Ctor

        /// <summary>
        /// Creates a new instance of carousel's layout manager
        /// </summary>
        /// <param name="carousel"></param>
        public CarouselLayoutManager(Carousel carousel)
        {
            _carousel = carousel;
            m_Timer.Interval = 1;
            m_Timer.Tick += new EventHandler(m_Timer_Tick);
            m_AlphaAxes = this._carousel.TransitionSpeed;
            m_Timer.Enabled = false;
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Releases any references to the carousel control.
        /// </summary>
        public void Dispose()
        {
            this._carousel = null;
            if (this.m_Timer.Enabled)
                this.m_Timer.Enabled = false;
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Returns a value of rectangle where the selected control should stop after selection
        /// </summary>
        /// <returns></returns>
        internal Rectangle CenterRectangle()
        {
            int x0 = _carousel.Width / 2;
            int y0 = _carousel.Height / 2;

            int nRadX = (_carousel.Width / 2) * 7 / 10;
            int nRadY = (int)(nRadX / _carousel.Perspective);
            double AngleActual;

            if (this._carousel.CarouselPath != CarouselPath.Orbital && this._carousel.CarouselPath != CarouselPath.Linear)
            {
                AngleActual = (ANGLEORIGINAL + (_carousel.TransitionSpeed)) * PI_FACT;

                int x1 = x0 + (int)(nRadX * Math.Sin(AngleActual));
                int y1 = y0 - (int)(nRadY * Math.Cos(AngleActual));


                float dSize = (float)(80 - 20 * Math.Sin(AngleActual));

                dSize = dSize / 100;

                // getting location 
                int p1 = (int)(x1 - (this._carousel.ActiveItem.Size.Width * dSize) / 2);
                int p2 = (int)(y1 - (this._carousel.ActiveItem.Size.Height * dSize));
                Point p = new Point(p1, p2);
                int wid = ((int)(_carousel.TransitionSpeed) * 3) + 20;
                Rectangle r1 = new Rectangle(p, new Size(wid, this._carousel.ActiveItem.Size.Height + 10));
                return r1;
            }
            else
            {
                Point p = new Point(x0 - ((this._carousel.ActiveItem.Size.Width / 2) + (int)_carousel.TransitionSpeed), y0);
                int wid = ((int)(_carousel.TransitionSpeed) * 3) + 40;
                Rectangle r2 = new Rectangle(p, new Size(wid, this._carousel.ActiveItem.Size.Height + 10));
                return r2;
            }

        }
        // <summary>
        /// Sets the custom bounds if true
        /// </summary>
        private bool useCustomBounds = false;
        /// <summary>
        /// Gets or Sets whether custom bounds should be used
        /// </summary>
        [Description("Gets or Sets whether custom bounds should be used"), Category("Appearance")]
        internal bool UseCustomBounds
        {
            get
            {
                return _carousel.UseCustomBounds;
            }
            set
            {
                if (useCustomBounds != value)
                    useCustomBounds = value;
            }
        }
        /// <summary>
        /// Performs layout changes when control's Layout is called
        /// </summary>
        public void PerformLayout()
        {
            if(UseCustomBounds)
                _carousel.Width = _carousel.CarouselBounds;
            int x0 = _carousel.Width / 2;
            int y0 = _carousel.Height / 2;
            int nRadX = (_carousel.Width / 2) * 7 / 10;
            int nRadY = (int)(nRadX / _carousel.Perspective);


            if (_carousel.CircularViews != null)
            {
                foreach (Control item in _carousel.CircularViews)
                {
                    double AngleOriginal;
                    Double.TryParse(item.Tag.ToString(), out AngleOriginal);

                    double AngleActual = (AngleOriginal + m_Alpha) * PI_FACT;

                    int x1, y1;
                    switch (this._carousel.CarouselPath)
                    {
                        case CarouselPath.Orbital:
                            x1 = x0 + (int)(nRadX * Math.Sin(AngleActual));
                            y1 = y0 - (int)(nRadY * Math.Sin(AngleActual));
                            break;
                        case CarouselPath.Linear:
                            x1 = x0 + (int)(nRadX * Math.Cos(AngleActual));
                            y1 = y0 - (int)(nRadY * Math.Cos(AngleActual));
                            break;
                        case CarouselPath.Oval:
                            x1 = x0 + (int)(nRadX * Math.Cos(AngleActual));
                            y1 = y0 - (int)(nRadY * Math.Sin(AngleActual));
                            break;
                        default:
                            x1 = x0 + (int)(nRadX * Math.Sin(AngleActual));
                            y1 = y0 - (int)(nRadY * Math.Cos(AngleActual));
                            break;
                    }

                    float dSize = (float)(80 - 20 * Math.Cos(AngleActual));
                    dSize = dSize / 100;

                    // getting location 
                    int p1 = (int)(x1 - (item.Size.Width * dSize) / 2);
                    int p2 = (int)(y1 - (item.Size.Height * dSize));
                    item.Location = new Point(p1, p2);
                }
            }
        }

        /// <summary>
        /// Begins circular transformation
        /// </summary>
        internal void BeginTransform()
        {
            transForming = true;
            m_Timer.Stop();
            m_Timer.Enabled = false;
            m_Timer.Enabled = true;
            m_Timer.Start();
        }

        /// <summary>
        /// Stops transformation
        /// </summary>
        internal void EndTransform()
        {
            transForming = false;
            m_Timer.Stop();
            m_Timer.Enabled = false;
        }

        void m_Timer_Tick(object sender, EventArgs e)
        {
            m_Alpha += m_AlphaAxes;

            if (m_Alpha > 360)
                m_Alpha = m_Alpha - 360;

            if (m_Alpha < 0)
                m_Alpha = m_Alpha + 360;

            int maxDX = this._carousel.Bounds.Width / 2;
            int maxDY = this._carousel.Bounds.Height / 2;

            Rectangle r = CenterRectangle();

            if (!this._carousel.RotateAlways && this._carousel.Controls[this._carousel.clikedItemId] != null)
            {
                endPoint = new Point(this._carousel.Controls[this._carousel.clikedItemId].Location.X + this._carousel.Controls[this._carousel.clikedItemId].Width, this._carousel.Controls[this._carousel.clikedItemId].Location.Y + this._carousel.Controls[this._carousel.clikedItemId].Height);

                stPoint = new Point(this._carousel.Controls[this._carousel.clikedItemId].Location.X, this._carousel.Controls[this._carousel.clikedItemId].Location.Y + this._carousel.Controls[this._carousel.clikedItemId].Height);
            }
            if (this._carousel.RotateAlways)
            {
                this.PerformLayout();
            }
            else
            {
                if (r.Contains(endPoint) || r.Contains(stPoint))
                {
                    this.EndTransform();
                    this._carousel.ActiveItem = this._carousel.Controls[this._carousel.clikedItemId];
                    this._carousel.clikedItemId = -1;
                    this._carousel.ItemTransitionEnded();
                }
                else
                {
                    this.PerformLayout();
                }
            }
            this._carousel.Refresh();
        }
        #endregion

    }

    /// <summary>
    /// Internal collection which holds the list of child items to be populated in the control.
    /// </summary>
    public class CircularList : List<Control>
    {
        public CircularList(ItemCollection items)
            : base(items.AsEnumerable())
        {
            for (int i = 0; i < items.Count; i++)
            {
                double ang = (float)i * (360.0f) / (float)items.Count; ;
                items[i].Tag = ang;
            }
        }
    }


    /// <summary>
    /// Delegate for OnCarouselItemSelectionChanged eventhandler
    /// </summary>
    /// <param name="sender">carousel</param>
    /// <param name="e">event args</param>
    public delegate void OnCarouselItemSelectionChangedEventHandler(object sender, CarouselItemSelectionChangedArgs e);

    /// <summary>
    /// Event args for OnCarouselItemSelectionChanged event.
    /// </summary>
    public class CarouselItemSelectionChangedArgs : SyncfusionEventArgs
    {
        Carousel carousel = null;
        #region [ Constructor]
        /// <summary>
        /// Initializes a new instance of the <see cref="CarouselItemSelectionChangedArgs"/> class.
        /// </summary>
        public CarouselItemSelectionChangedArgs(Carousel owner)
        {
            carousel = owner;
        }

        #endregion

        #region [ Public Properties ]
        /// <summary>
        /// Gets or sets the SelectedControl
        /// </summary>
        public Control SelectedControl
        {
            get
            {
                return carousel.ActiveItem;
            }
        }

        /// <summary>
        /// Gets or sets the SelectedImage [if ImageSlides is true]
        /// </summary>
        public Image SelectedImage
        {
            get
            {
                return carousel.ActiveImage;
            }
        }

        /// <summary>
        /// Returns the currently active carousel path
        /// </summary>
        public CarouselPath ActiveCarouselPath
        {
            get
            {
                return carousel.CarouselPath;
            }
        }
        #endregion
    }


    /// <summary>
    /// Delegate for OnCarouselItemSelectionChanging eventhandler
    /// </summary>
    /// <param name="sender">carousel</param>
    /// <param name="e">event args</param>
    public delegate void OnCarouselItemSelectionChangingEventHandler(object sender, CarouselItemSelectionChangingArgs e);

    /// <summary>
    /// Event args for OnCarouselItemSelectionChanging event.
    /// </summary>
    public class CarouselItemSelectionChangingArgs : SyncfusionCancelEventArgs
    {
        Carousel carousel = null;
        #region [ Constructor]
        /// <summary>
        /// Initializes a new instance of the <see cref="CarouselItemSelectionChangingArgs"/> class.
        /// </summary>
        public CarouselItemSelectionChangingArgs(Carousel owner)
        {
            carousel = owner;
        }

        #endregion

        #region [ Public Properties ]
        /// <summary>
        /// Gets or sets the SelectedControl
        /// </summary>
        public Control SelectedControl
        {
            get
            {
                return carousel.ActiveItem;
            }
        }

        /// <summary>
        /// Gets or sets the SelectedImage [if ImageSlides is true]
        /// </summary>
        public Image SelectedImage
        {
            get
            {
                return carousel.ActiveImage;
            }
        }

        /// <summary>
        /// Returns the currently active carousel path
        /// </summary>
        public CarouselPath ActiveCarouselPath
        {
            get
            {
                return carousel.CarouselPath;
            }
        }
        #endregion
    }

    /// <summary>
    /// Delegate for OnItemFocusedEvent
    /// </summary>
    /// <param name="sender">carousel</param>
    /// <param name="e">event args</param>
    public delegate void OnCarouselItemFocusedEventHandler(object sender, CarouselItemFocusedArgs e);

    /// <summary>
    /// Event args for OnItemFocused event
    /// </summary>
    public class CarouselItemFocusedArgs : SyncfusionEventArgs
    {
        Carousel carousel = null;
        #region [ Constructor ]
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemFocusedArgs"/> class.
        /// </summary>
        public CarouselItemFocusedArgs(Carousel owner)
        {
            carousel = owner;
        }

        #endregion

        #region [ Public Properties ]
        /// <summary>
        /// Gets the currently focused control
        /// </summary>
        public Control FocusedControl
        {
            get
            {
                return carousel.ActiveItem;
            }
        }

        /// <summary>
        /// Gets the currently focused image [if ImageSlides is true]
        /// </summary>
        public Image FocusedImage
        {
            get
            {
                return carousel.ActiveImage;
            }
        }

        /// <summary>
        /// Returns the active carousel path
        /// </summary>
        public CarouselPath ActiveCarouselPath
        {
            get
            {
                return carousel.CarouselPath;
            }
        }
        #endregion
    }

    /// <summary>
    /// Desginer class for Carousel
    /// </summary>
    public class CarouselDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        /// <summary>
        /// Designer ActionList collection
        /// </summary>
        private System.ComponentModel.Design.DesignerActionListCollection actionLists;

        /// <summary>
        ///  Initializes a new instance of the CheckBoxAdvDesigner class
        /// </summary>
        public CarouselDesigner()
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
                    this.actionLists.Add(new CarouselActionList(this.Component));
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
    public class CarouselActionList : SyncActionListBase<Carousel>
    {
        /// <summary>
        /// Initializes a new instance of the CheckBoxAdvActionList class.
        /// </summary>
        /// <param name="component"> Represents component</param>
        public CarouselActionList(IComponent component)
            : base(component)
        {
        }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name
        {
            get
            {
                string name = "Carousel";
                if (this.Control != null)
                {
                    Carousel control = this.Control as Carousel;
                    name = control.Name;
                }

                return name;
            }

            set
            {
                SetValue("Name", value);
            }
        }

        /// <summary>
        /// Gets or sets a value to rotate the child items continuously
        /// </summary>
        public bool RotateAlways
        {
            get
            {
                bool rotateAlways = false;
                if (this.Control != null)
                {
                    Carousel control = this.Control as Carousel;
                    rotateAlways = control.RotateAlways;
                }

                return rotateAlways;
            }

            set
            {
                SetValue("RotateAlways", value);
            }
        }
        /// <summary>
        /// Gets or Sets a value to preview the selected image
        /// </summary>
        public bool ShowImagePreview
        {
            get
            {
                bool showImagePreview = false;
                if (this.Control != null)
                {
                    Carousel control = this.Control as Carousel;
                    showImagePreview = control.ShowImagePreview;
                }

                return showImagePreview;
            }

            set
            {
                SetValue("ShowImagePreview", value);
            }
        }
        /// <summary>
        /// Gets or sets a value to display shadow for the images
        /// </summary>
        public bool ShowImageShadow
        {
            get
            {
                bool showImageShadow = false;
                if (this.Control != null)
                {
                    Carousel control = this.Control as Carousel;
                    showImageShadow = control.ShowImageShadow;
                }

                return showImageShadow;
            }

            set
            {
                SetValue("RotateAlways", value);
            }
        }

        /// <summary>
        /// Gets or Sets a value to display images in the control
        /// </summary>
        public bool ImageSlides
        {
            get
            {
                bool imageSlides = false;
                if (this.Control != null)
                {
                    Carousel control = this.Control as Carousel;
                    imageSlides = control.ImageSlides;
                }

                return imageSlides;
            }

            set
            {
                SetValue("ImageSlides", value);
            }
        }

        /// <summary>
        /// Gets or sets the items associated with the Carousel.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(CustomCollectionEditor), typeof(UITypeEditor))]
        public ItemCollection Items
        {
            get
            {
                ItemCollection items = null;
                if (this.Control != null)
                {
                    Carousel control = this.Control as Carousel;
                    items = control.Items;
                }

                return items;
            }

            set
            {
                SetValue("Items", value);
            }
        }

        /// <summary>
        /// Gets or sets a collection of items to display in the carousel
        /// </summary>
        public CarouselImageCollection ImageListCollection
        {
            get
            {
                CarouselImageCollection imageListCollection = null;
                if (this.Control != null)
                {
                    Carousel control = this.Control as Carousel;
                    imageListCollection = control.ImageListCollection;
                }

                return imageListCollection;
            }

            set
            {
                SetValue("ImageListCollection", value);
            }
        }

        /// <summary>
        /// Imagelist which contains images to populate in the control
        /// </summary>
        public ImageList ImageList
        {
            get
            {
                ImageList imageList = null;
                if (this.Control != null)
                {
                    Carousel control = this.Control as Carousel;
                    imageList = control.ImageList;
                }

                return imageList;
            }

            set
            {
                SetValue("ImageList", value);
            }
        }

        /// <summary>
        /// Gets or sets a visual style to the controls
        /// </summary>
        public CarouselVisualStyle VisualStyle
        {
            get
            {
                CarouselVisualStyle cVisualStyle = CarouselVisualStyle.Default;
                if (this.Control != null)
                {
                    Carousel control = this.Control as Carousel;
                    cVisualStyle = control.VisualStyle;
                }

                return cVisualStyle;
            }

            set
            {
                SetValue("VisualStyle", value);
            }
        }

        /// <summary>
        /// Overrridden InitializeActionList.
        /// </summary>
        protected override void InitializeActionList()
        {
            // Design Category
            this.AddDesignerActionHeaderItem("Design");
            this.AddDesignerActionPropertyItem("Name", "Name", "Design", "Indicates the name used in code to identify the object.");

            // Appearance category.
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("VisualStyle", "VisualStyle", "Appearance", "Applies VisualStyle to the child controls");

            // Behavior Category
            this.AddDesignerActionHeaderItem("Behavior");
            this.AddDesignerActionPropertyItem("RotateAlways", "RotateAlways", "Behavior", "Gets or sets the control to rotate its child objects.");
            this.AddDesignerActionPropertyItem("ImageSlides", "ImageSlides", "Behavior", "Gets or sets the control to populate with images.");

            if (this.ImageSlides)
            {
                // StateImages Category
                this.AddDesignerActionHeaderItem("ImageSlides Properties");
                this.AddDesignerActionPropertyItem("ImageListCollection", "ImageListCollection", "ImageSlides Properties", "Gets or sets the ImageListCollection to populate the control.");
                this.AddDesignerActionPropertyItem("ImageList", "ImageList", "ImageSlides Properties", "Gets or sets the ImageList to populate the control.");
                this.AddDesignerActionPropertyItem("ShowImageShadow", "ShowImageShadow", "ImageSlides Properties", "Gets or sets the value to display shadow for images.");
                this.AddDesignerActionPropertyItem("ShowImagePreview", "ShowImagePreview", "ImageSlides Properties", "Gets or sets the value to preview the image on click.");
            }
            else
            {
                this.AddDesignerActionPropertyItem("Items", "Items", "Behavior", "Gets or sets the control to populate with child controls/objects.");
            }
        }
    }
}
