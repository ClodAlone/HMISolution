#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using Syncfusion.HTMLUI.Base.Utility;
using Syncfusion.Windows.Forms.HTMLUI;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
using System.Drawing.Drawing2D;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;IMG&gt; Tag.
    /// </summary>
    [ElementTag(TagName.Img)]
    public class IMGElementImpl
      : BaseElement
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Img;

        /// <summary>
        /// Default image when needed image is not accessed.
        /// </summary>
        private const string DEF_IMAGE = "Syncfusion.Windows.Forms.HTMLUI.images.no_image.bmp";

        /// <summary>
        /// Width of the border around the default image from the left / top.
        /// </summary>
        private const int DEF_LEFT_TOP_BORDER_WIDTH = 2;

        /// <summary>
        /// Width of the border around the default image from the right / bottom.
        /// </summary>
        private const int DEF_RIGHT_BOTTOM_BORDER_WIDTH = 1;

        /// <summary>
        /// Holds type of reaction on attribute changing.
        /// </summary>
        private static ReactionCollection m_reactionType;

        /// <summary>
        /// Supported events.
        /// </summary>
        private static string[] DEF_SUPP_EVENTS;

        /// <summary>
        /// Holds all events.
        /// </summary>
        private static Hashtable m_eventHash;
        #endregion

        #region static members
        /// <summary>
        /// Represents the stream of the default image.
        /// </summary>
        private static Stream m_defImageStream = null;

        /// <summary>
        /// Default image.
        /// </summary>
        private static Bitmap m_defImage = null;
        #endregion

        #region Class members
        /// <summary>
        /// Bitmap of the image.
        /// </summary>
        private Bitmap m_bitmap;

        /// <summary>
        /// Stream which represents the image data.
        /// </summary>
        private Stream m_imgStream;

        /// <summary>
        /// Indicates whether the source of the image was found and loaded.
        /// </summary>
        private bool m_ImageFound;

        /// <summary>
        /// Indicates whether to draw alt text.
        /// </summary>
        private bool m_drawAltText;

        /// <summary>
        /// Event handler. Raised when active frame of the image has been changed.
        /// </summary>
        private EventHandler m_imgFrameChanged;

        /// <summary>
        /// Indicates whether the image is currently animating.
        /// </summary>
        private bool m_bIsAnimating;

        /// <summary>
        /// Indicates whether image was changed at runtime.
        /// </summary>
        private bool m_bImageChanged;
        #endregion

        #region Class Properties
        /// <summary>
        /// Returns an array of supported events.
        /// </summary>
        public override string[] SupportedEvents
        {
            get
            {
                return DEF_SUPP_EVENTS;
            }
        }

        /// <summary>
        /// Gets or sets the bitmap of the image which represents this element.
        /// </summary>
        public Bitmap Image
        {
            get
            {
                if (m_bitmap == null)
                    return IMGElementImpl.Default;

                return m_bitmap;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Image");

                if (m_bitmap != value)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_bitmap, value);
                    m_bitmap = value;

                    OnImageChanged(args);
                }
            }
        }

        /// <summary>
        /// Overridden. Returns an hashtable that contains the names of attributes as keys and type of reaction of its
        /// changing as value.
        /// </summary>
        internal override ReactionCollection Reaction
        {
            get
            {
                return m_reactionType;
            }
        }

        /// <summary>
        /// Gets the event handler pointing on the method that gets invoked when the active frame
        /// was changed.
        /// </summary>
        private EventHandler FrameChanged
        {
            get
            {
                if (m_imgFrameChanged == null)
                {
                    m_imgFrameChanged = new EventHandler(OnFrameChanged);
                }

                return m_imgFrameChanged;
            }
        }
        #endregion

        #region Class events
        /// <summary>
        /// Raised when image with specified src could not be found.
        /// </summary>
        public event NoImageEventHandler NoImage;

        /// <summary>
        /// Utility event. Raised on Bottom property change.
        /// </summary>
        public event ValueChangedEventHandler ImageChanged;
        #endregion

        #region Class static members
        /// <summary>
        /// Gets the default image.
        /// </summary>
        public static Bitmap Default
        {
            get
            {
                if (m_defImage == null)
                {
                    m_defImage = Utilities.ImageFromStream(DefaultStream);
                }

                return m_defImage;
            }
        }

        /// <summary>
        /// Gets the stream which represents the default image.
        /// </summary>
        public static Stream DefaultStream
        {
            get
            {
                if (m_defImageStream == null)
                {
                    m_defImageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(DEF_IMAGE);
                }

                return m_defImageStream;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes static members of the IMGElementImpl class 
        /// </summary>
        static IMGElementImpl()
        {
            // Add changing of the image to the reaction.
            m_reactionType = (ReactionCollection)m_reaction.Clone();
            string typeName = typeof(IMGElementImpl).ToString();
            m_reactionType.Add(typeName, "Image", ReactType.ReCalculatingDocument);
            Type type = typeof(IMGElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Initializes a new instance of the IMGElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public IMGElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
        }

        /// <summary>
        /// Overridden. Disposes bitmap and closes stream.
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();

            DisposeImg();

            if (m_imgFrameChanged != null)
            {
                m_imgFrameChanged = null;
            }
        }

        /// <summary>
        /// Disposes image.
        /// </summary>
        private void DisposeImg()
        {
            if (m_bitmap != null)
            {
                StopAnimation();

                m_bitmap = null;
            }

            // If we do not store our own default stream, then close it.
            if (m_imgStream != null && m_imgStream != IMGElementImpl.DefaultStream)
            {
                m_imgStream.Close();
                m_imgStream = null;
            }
        }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Raises NoImage event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnNoImage(NoImageEventArgs args)
        {
            RaiseNoImage(args);
        }

        /// <summary>
        /// Overridden. Returns an instance of the event.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <returns>Event object.</returns>
        protected override IHTMLEvent CreateEventInternal(string name)
        {
            return new HashElementEvents(this, m_eventHash, name);
        }

        /// <summary>
        /// Overridden. Calculates the size of the element for rendering.
        /// </summary>
        /// <returns>Size instance</returns>
      protected override Size CalculateSizeInternal()
        {
            if (m_bitmap == null)
            {
                LoadImage();
            }

            if (m_bitmap != null)
            {
                this.Size = m_bitmap.Size;
                m_ImageFound = true;
            }
            else
            {
                this.Size = Default.Size;
                m_ImageFound = false;
            }

            this.IsResizable = false;
            this.Type = ElementType.BlockFixedSize;

            SetType();
            LoadSizeFormAttributes();

            if (this.Type == ElementType.BlockFixedSize)
            {
                this.MinWidth = this.Width;
                this.MinHeight = this.Height;
            }

            return this.Size;
        }

        /// <summary>
        /// Overridden. Calculates the position of the element for rendering.
        /// </summary>
        protected override void CalculatePositionInternal()
        {
            BaseElement parent = (BaseElement)this.Parent;
            this.CurrentPosition = parent.CurrentPosition;

            CalculateChildPositions(this.CurrentPosition, parent.Bounds);
        }

        /// <summary>
        /// Overridden. Calculates the position for the child elements.
        /// </summary>
        /// <param name="curPosition">Current global position.</param>
        /// <param name="bounds">Bounds for this element.</param>
        /// <returns>Array of blocks.</returns>
        protected override BlocksCollection CalculateChildPositions(Point curPosition, Rectangle bounds)
        {
            m_blocks.Clear();
            m_curBlock = null;

            this.QuietMode = true;
            this.CurrentPosition = curPosition;
            this.Location = this.CurrentPosition;

            if (!this.IsVisible)
            {
                this.QuietMode = false;

                return new BlocksCollection();
            }

            Block block = CreateBlock(null);

            Rectangle elRect = new Rectangle(this.Location, this.Size);
            AddElement(block, this, elRect);

            this.QuietMode = false;
            OnLocationCalculated(EventArgs.Empty);

            return m_blocks;
        }

        /// <summary>
        /// Overridden. Calculates the format of the element from the array of possible formats.
        /// </summary>
        protected override void CalculateFormatInternal()
        {
            DefaultCalculateFormatInternal();
        }

        /// <summary>
        /// Overridden. Draws an image element.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void DrawElementInternal(PaintEventArgs e)
        {
            Rectangle globalRect = new Rectangle(this.Location, this.Size);
            Rectangle descRect = GlobalToClient(globalRect);
            Rectangle srcRect;

            if (e.ClipRectangle.IntersectsWith(descRect))
            {
                Image image = GetImageForDrawing();

                srcRect = new Rectangle(0, 0, image.Width, image.Height);

                // Draw original image.
                if (m_ImageFound)
                {
                    GraphicsUnit units = GraphicsUnit.Pixel;

                    StartAnimation();

                    UpdateActiveFrame(image);
                    BaseElement parent = (BaseElement)this.Parent;
                    if (parent != null && parent.Control != null && !parent.Control.SizeToFit)
                    {
                        Bitmap newImage = new Bitmap(image.Size.Width, image.Height);
                        using (Graphics gr = Graphics.FromImage(newImage))
                        {
                            gr.SmoothingMode = SmoothingMode.HighQuality;
                            gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            gr.DrawImage(image, new Rectangle(0, 0, image.Size.Width / 2, image.Height));
                        }
                        e.Graphics.DrawImage(newImage, descRect, srcRect, units);
                    }
                    else
                    {
                        e.Graphics.DrawImage(image, descRect, srcRect, units);
                    }
                }
                else 
                {
                    //// Draw default image.
                    Region oldClip = e.Graphics.Clip;

                    //// Draw bounds and set clip rectangle on the graphics.
                    DrawDefImageBounds(e.Graphics, descRect);

                    descRect.Width = Math.Min(Default.Width, this.Width);
                    descRect.Height = Math.Min(Default.Height, this.Height);

                    GraphicsUnit units = GraphicsUnit.Pixel;

                    e.Graphics.DrawImage(image, descRect, srcRect, units);
                    DrawAltText(e.Graphics, descRect.Location);

                    e.Graphics.Clip = oldClip;
                }

                DrawBorders(m_curBlock, e.Graphics);

                // Add image to the regions during printing.
                if (this.Document.IsPrinting && this.Document.TextRegionManager != null)
                {
                    TextRegion region = new TextRegion(globalRect.Y, descRect.Height);
                    this.Document.TextRegionManager.Add(region);
                }
            }
        }

        /// <summary>
        /// Overridden. Raised when attribute src has been changed.
        /// </summary>
        protected internal override void ReCalculateDocument()
        {
            if (!m_bImageChanged)
            {
                m_bitmap = null;
                LoadImage();
            }

            base.ReCalculateDocument();
        }

        /// <summary>
        /// Overriden. Shows the ToolTip on the element.
        /// </summary>
        protected override void ShowToolTip()
        {
            string tooltipValue = null;
            IHTMLAttribute titleAttr = this.Attributes[AttributeName.Title];
            IHTMLAttribute altAttr = this.Attributes[AttributeName.Alt];

            if (titleAttr != null && titleAttr.Value.Length > 0)
            {
                tooltipValue = titleAttr.Value;
            }
            else if (altAttr != null && altAttr.Value.Length > 0)
            {
                tooltipValue = altAttr.Value;
            }

            if (tooltipValue != null && tooltipValue.Length > 0)
            {
                this.Control.ToolTip.SetToolTip(this.Control, tooltipValue);
                this.Control.ToolTip.Active = true;
            }
        }

        /// <summary>
        /// Initializes element.
        /// </summary>
        protected internal override void InitializeElement()
        {
            LoadImage();
        }
        #endregion

        #region Class event raisers
        /// <summary>
        /// Raises the NoImage event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected void RaiseNoImage(NoImageEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            if (NoImage != null)
            {
                NoImage(this, args);
            }
        }

        /// <summary>
        /// Raises ImageChanged event if Image property value is changed.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnImageChanged(ValueChangedEventArgs args)
        {
            if (ImageChanged != null && !this.QuietMode)
            {
                ImageChanged(this, args);
            }

            // Invoke reaction processor method.
            try
            {
                BeforeValueChangedEventArgs args1 = new BeforeValueChangedEventArgs("Image", args);
                m_bImageChanged = true;
                Attributes_Changed(this, args1);
            }
            finally
            {
                m_bImageChanged = false;
            }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Loads the bitmap to the bitmap member.
        /// </summary>
        /// <returns>bool instance</returns>
        private bool LoadImage()
        {
            if (m_bitmap == null)
            {
                m_bitmap = GetBitmap();
            }

            return m_bitmap != null;
        }

        /// <summary>
        /// Returns the bitmap instance from the attributes of the image.
        /// </summary>
        /// <returns>Bitmap or NULL.</returns>
        private Bitmap GetBitmap()
        {
            HTMLAttributeImpl attr = (HTMLAttributeImpl)this.Attributes[AttributeName.Src];
            if (attr == null) return null;

            string path = attr.Value.Trim();
            if (path == null || path.Length == 0) return null;
            string fullPath;
            if (this.Document.ImageCache.Insert(path, out fullPath))
            {
                m_ImageFound = true;
                m_drawAltText = false;

                return this.Document.ImageCache[fullPath];
            }
            else 
            {
                //// Ask from user image to load.
                NoImageEventArgs args = new NoImageEventArgs();
                OnNoImage(args);

                //// User defined image.
                if (args.Stream != null && args.Stream.Length > 0)
                {
                    Bitmap img = Utilities.ImageFromStream(args.Stream);

                    // If stream is good and image has been created.
                    if (img != null && args.Key != null && args.Key.Length > 0)
                    {
                        m_ImageFound = true;
                        m_drawAltText = false;

                        if (this.Document.ImageCache.Insert(args.Key, img))
                        {
                            return img;
                        }

                        //// Image already exists in cache.
                        img.Dispose();

                        return this.Document.ImageCache[args.Key];
                    }
                }
            }

            m_ImageFound = false;

            return Utilities.ImageFromStream(DefaultStream);
        }

        /// <summary>
        /// If element has special size attributes, get it.
        /// </summary>
        private void LoadSizeFormAttributes()
        {
            // Element has attributes in style attribute.
            HTMLFormat format = this.Format as HTMLFormat;

            if (IsStyleWidth && format.WidthType == SizeTypeEx.Number)
            {
                this.Width = this.Format.Width;
            }

            if (IsStyleHeight && format.HeightType == SizeTypeEx.Number)
            {
                this.Height = this.Format.Height;
            }

            SetSizeFromAltText();
        }

        /// <summary>
        /// Sets the type of the element corresponding to its size attributes.
        /// </summary>
        private void SetType()
        {
            if (IsAttributeWidth)
            {
                HTMLAttributeImpl attr = (HTMLAttributeImpl)this.Attributes[AttributeName.Width];

                if (attr != null)
                {
                    AttributeToken type = AttributeParser.DetectType(attr.Value);

                    if (type == AttributeToken.Percent)
                    {
                        this.Type = ElementType.BlockResizable;
                        this.IsResizable = true;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the size of the element corresponding to the text of the ALT attribute.
        /// </summary>
        private void SetSizeFromAltText()
        {
            if (m_ImageFound) return;

            IHTMLAttribute attr = this.Attributes[AttributeName.Alt];

            if (attr == null || attr.Value == null || attr.Value.Length == 0) return;

            Size size;
            HTMLFormat format = this.Format as HTMLFormat;
                       
            if (IsStyleWidth && format.WidthType == SizeTypeEx.Number)
            {
                //// Image has fixed width.
                size = MeasureString(attr.Value, format, format.Width - Default.Width);
            }
            else
            {
                //// Image hasn't fixed width.
                size = MeasureString(attr.Value, format.Font);
                this.Width = size.Width + Default.Width;
            }

            if (!(IsStyleHeight && format.HeightType == SizeTypeEx.Number))
            {
                this.Height = Math.Max(this.Height, size.Height);
            }

            m_drawAltText = true;
        }

        /// <summary>
        /// Draws an alt text.
        /// </summary>
        /// <param name="g">Graphic context.</param>
        /// <param name="start">Start position of the element.</param>
        private void DrawAltText(Graphics g, Point start)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            if (!m_drawAltText) return;

            start.X += Default.Width;
            IHTMLAttribute attr = this.Attributes[AttributeName.Alt];
            if (attr == null || attr.Value == null || attr.Value.Length == 0) return;

            using (SolidBrush brush = new SolidBrush(this.Format.ForeColor))
            {
                g.DrawString(attr.Value, this.Format.Font, brush, start);
            }
        }

        /// <summary>
        /// Returns an image for drawing.
        /// </summary>
        /// <returns>Image will be painted.</returns>
        private Image GetImageForDrawing()
        {
            Image image = null;

            if (m_bitmap != null)
            {
                image = m_bitmap;
            }
            else
            {
                image = Default;
                m_ImageFound = false;
            }

            return image;
        }

        /// <summary>
        /// Draws the bounds around the default image.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="descRect">Destination rectangle.</param>
        private void DrawDefImageBounds(Graphics g, Rectangle descRect)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            int borderWidth = DEF_LEFT_TOP_BORDER_WIDTH + DEF_RIGHT_BOTTOM_BORDER_WIDTH;

            if (this.Width > (Default.Width + borderWidth)
              || this.Height > (Default.Height + borderWidth))
            {
                descRect.Size = this.Size;

                using (GDIUtils gdi = new GDIUtils())
                {
                    gdi.Draw3DBox(g, descRect, Canvas3DStyle.Raised);
                }

                descRect.Size = Default.Size;
                descRect.X += DEF_LEFT_TOP_BORDER_WIDTH;
                descRect.Y += DEF_LEFT_TOP_BORDER_WIDTH;

                // Set bounds.
                descRect = new Rectangle(descRect.X, descRect.Y, this.Width - borderWidth, this.Height - borderWidth);
            }
            else
            {
                descRect = new Rectangle(descRect.X, descRect.Y, this.Width, this.Height);
            }

            g.SetClip(descRect);
        }

        /// <summary>
        /// Raised when the active frame of image was changed.
        /// </summary>
        /// <param name="source">Sender of the event.</param>
        /// <param name="args">Event arguments.</param>
        private void OnFrameChanged(object source, EventArgs args)
        {
            lock (FrameChanged)
            {
                if (m_bIsAnimating)
                {
                    Rectangle globalRect = new Rectangle(this.Location, this.Size);
                    Rectangle destRect = GlobalToClient(globalRect);

                    // Invalidate area where image is painted.
                    this.Control.Invalidate(destRect);
                }
            }
        }

        /// <summary>
        /// Starts the animation if needed.
        /// </summary>
        private void StartAnimation()
        {
            lock (FrameChanged)
            {
                Image img = GetImageForDrawing();

                if (!m_bIsAnimating && ImageAnimator.CanAnimate(img))
                {
                    ImageAnimator.Animate(img, this.FrameChanged);
                    m_bIsAnimating = true;
                }
            }
        }

        /// <summary>
        /// Stops the animation of the image.
        /// </summary>
        private void StopAnimation()
        {
            lock (FrameChanged)
            {
                if (m_bIsAnimating)
                {
                    Image img = GetImageForDrawing();

                    ImageAnimator.StopAnimate(img, this.FrameChanged);
                    m_bIsAnimating = false;
                }
            }
        }

        /// <summary>
        /// Updates the active frame if is animating.
        /// </summary>
        /// <param name="image">Image instance</param>
        private void UpdateActiveFrame(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            lock (FrameChanged)
            {
                if (m_bIsAnimating)
                {
                    ImageAnimator.UpdateFrames(image);
                }
            }
        }
        #endregion
    }
}