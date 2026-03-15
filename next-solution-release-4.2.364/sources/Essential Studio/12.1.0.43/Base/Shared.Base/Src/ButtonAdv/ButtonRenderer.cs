#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;

using Syncfusion.Windows.Forms.Tools;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Syncfusion.Runtime.InteropServices;
using System.ComponentModel;
#endregion

namespace Syncfusion.Windows.Forms
{
    /// <summary><para>
    /// Basic class for all renderers. ButtonRenderer provides the basic plumbing that is needed by all renderers.
    /// </para><para>
    /// You can derive from ButtonRenderer to create your own renderers.
    /// </para></summary>
    internal class ButtonRenderer : IDisposable
    {
        #region Class members
        /// <summary></summary>
        protected Rectangle bounds;
        /// <summary></summary>
        private ButtonAdv m_button;
        /// <summary></summary>
        protected RectangleF imageRect;
        /// <summary></summary>
        protected PointF textPoint;
        /// <summary></summary>
        protected RectangleF textRect;
        /// <summary>True - class is disposed, otherwise False.</summary>
        private bool m_bDisposed;
        /// <summary></summary>
        private int borderWidth = 1;
        /// <summary></summary>
        private ContentAlignment textAlignment = ContentAlignment.MiddleCenter;
        #endregion

        #region Class static members
        /// <summary></summary>
        private static readonly int s_imageHorizontalMargin = 4;
        /// <summary></summary>
        private static readonly int s_imageVerticalMargin = 4;
        /// <summary></summary>
        private static readonly int s_textHorizontalMargin = 3;
        /// <summary></summary>
        private static readonly int s_textVerticalMargin = 3;
        /// <summary></summary>
        private static readonly Size s_defaultImageSize = new Size(15, 20);
        #endregion

        #region Class properties
        /// <summary>
        /// </summary>
        protected ButtonAdv Button
        {
            get
            {
                return this.m_button;
            }

            set
            {
                this.m_button = value;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// </summary>
        /// <param name="button"/>
        public ButtonRenderer(ButtonAdv button)
        {
            if (null == button)
                throw new ArgumentNullException("button");

            this.m_button = button;
            this.bounds = new Rectangle(button.Location, new Size(1, 1));
        }
        /// <summary>
        /// </summary>
        ~ButtonRenderer()
        {
            Dispose();
        }

        /// <summary>
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void OnDispose(bool disposing)
        {
            // for inheritors only...
        }
        /// <summary></summary>
        public void Dispose()
        {
            if (!m_bDisposed)
            {
                OnDispose(m_bDisposed);

                GC.SuppressFinalize(this);

                m_bDisposed = true;
            }
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Specifies region for drawing
        /// </summary>        
        public virtual Region GetRegion(Rectangle bounds)
        {
            return null;
        }

        /// <summary>
        /// Sets vista color scheme for the control.
        /// </summary>
        public virtual void SetColorScheme(Office2007Theme colorScheme)
        {
            // Do nothing here... 
        }
        /// <summary>
        /// Sets office 2010 color scheme for the control.
        /// </summary>
        public virtual void Set2010ColorScheme(Office2010Theme colorScheme)
        {
            // Do nothing here... 
        }
        /// <summary>
        /// Sets Metro color scheme for the control.
        /// </summary>
        public virtual void SetMetroColorScheme(MetroTheme  colorScheme)
        {
            // Do nothing here... 
        }
        public virtual void SetMetroColor(Color metroColor)
        { 
            // Do nothing here... 
        }
        /// <summary>
        ///     This function does much of the rendering. In the base <see cref="ButtonRenderer"/>, it does not
        ///     do anything.
        /// </summary>
        /// <param name="g" type="System.Drawing.Graphics"><para>
        ///     The graphics object to use.  
        ///     </para></param>
        public virtual void Render(Graphics g)
        {
            // Do nothing here... this method is for inheritors
        }

        /// <summary>
        /// Draws text on ButtonAdv with specified color
        /// </summary>
        ///<param name="g" type="System.Drawing.Graphics"><para>
        /// The graphics object to use.  
        /// </para></param>
        /// <param name="textColor">Color of the text</param>
        public virtual void DrawText(Graphics g, Color textColor)
        {
            ButtonAdv button = this.Button;

            DrawParams param = new DrawParams(button.Enabled, button.BackColor, textRect, textAlignment, button.RightToLeft == RightToLeft.Yes);

            bool drawShadow = (button.UseVisualStyle && !button.Enabled && button.Appearance == ButtonAppearance.Office2007);
            bool autoEllipsis = button.AutoEllipsis;

            ButtonMnemonicHelper prefixChar = ButtonMnemonicHelper.None;
            if (!this.Button.altPressed && this.Button.UseMnemonic && !button.IsDesignMode && !SystemInformationExt.KeyboardCuesAlwaysOn)
            { 
                prefixChar = ButtonMnemonicHelper.HidePrefix;
            }
            if (!button.UseMnemonic)
            {
                prefixChar = ButtonMnemonicHelper.NoPrefix;
            }
            if(!this.Button.IsBackStageButton)
                RenderingHelper.DrawText(g, this.Button.Text, textColor, button.Font, this.textPoint, param, drawShadow, autoEllipsis, prefixChar);
        }

        /// <summary>
        /// Draws background image, image and text
        /// </summary>
        /// <param name="g" type="System.Drawing.Graphics"><para>
        /// The graphics object to use.  
        /// </para></param>
        public virtual void DrawTextAndImage(Graphics g)
        {
            ButtonAdv button = this.Button;

            if (button.ButtonType == ButtonTypes.Normal)
            {

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                if (button.BackgroundImage != null)
                {
                    Rectangle rect = new Rectangle(button.ClientRectangle.X + borderWidth, button.ClientRectangle.Y + borderWidth,
                                                    button.ClientRectangle.Width - borderWidth * 2, button.ClientRectangle.Height - borderWidth * 2);

                    this.DrawBackgroundImage(g, button.BackgroundImage, button.BackgroundImageLayout,
                                              this.Button.ClientRectangle, rect, new Point(0, 0), button.RightToLeft);
                }
#endif

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                if ( button.BackgroundImage != null )
                {
                    Point point = new Point( borderWidth, borderWidth );
                    Size size = new Size( button.Width - borderWidth * 2, button.Height - borderWidth * 2 );
                    Rectangle rect = new Rectangle( point, size );

                    using ( TextureBrush brush1 = new TextureBrush( button.BackgroundImage, WrapMode.Tile ) )
                    {
                        g.FillRectangle( brush1, rect );
                    }
                }
#endif
                if (button.Image != null)
                {
                    this.DrawImage(g, this.imageRect, button.Image);
                }
                Color color = button.ForeColor;
               //if(button.State == ButtonAdvState.MouseOver && Button.Appearance == ButtonAppearance.Metro)
               //    color = ControlPaint.Dark ( button.ForeColor);
                this.DrawText(g, color);
            }
            else
            {
                // Image only
                this.DrawKnownImage(g, this.imageRect, button.ButtonType, Point.Empty, Color.Black);
            }
        }
        /// <summary></summary>
        /// <param name="g"/>
        /// <param name="bounds"/>
        /// <param name="buttonImageType"/>
        /// <param name="offset"/>
        /// <param name="imageColor"/>
        public virtual void DrawKnownImage(Graphics g, RectangleF bounds,
			                                      ButtonTypes buttonImageType, Point offset, Color imageColor)
        {
            switch (buttonImageType)
            {
                case ButtonTypes.Browse:
                    this.DrawImage(g, bounds, GetImage("Browse.bmp"));
                    break;

                case ButtonTypes.Calculator:
                    this.DrawImage(g, bounds, GetImage("Calculator.bmp"));
                    break;

                case ButtonTypes.Check:
                    this.DrawImage(g, bounds, GetImage("Check.bmp"));
                    break;

                case ButtonTypes.Currency:
                    this.DrawImage(g, bounds, GetImage("Currency.bmp"));
                    break;

                case ButtonTypes.Down:
                case ButtonTypes.Up:
                    RenderingHelper.DrawUpDownButton(g, bounds, buttonImageType, offset, imageColor);
                    break;

                case ButtonTypes.ComboXPDown:
                    RenderingHelper.DrawComboXPDownButton(g, bounds, buttonImageType, offset, imageColor);
                    break;

                case ButtonTypes.Left:
                case ButtonTypes.LeftEnd:
                case ButtonTypes.RightEnd:
                case ButtonTypes.Right:
                    RenderingHelper.DrawRightLeftButton(g, bounds, buttonImageType, offset, imageColor);
                    break;

                case ButtonTypes.Normal:
                    // This is not valid 
                    break;

                case ButtonTypes.Redo:
                    this.DrawImage(g, bounds, GetImage("Redo.bmp"));
                    break;

                case ButtonTypes.Undo:
                    this.DrawImage(g, bounds, GetImage("Undo.bmp"));
                    break;
            }
        }

        /// <summary>
        /// </summary>
        public virtual void ComputeTextPosition()
        {
            textAlignment = this.Button.TextAlign;
            if (this.Button.RightToLeft == RightToLeft.Yes)
            {
                switch (this.Button.TextAlign)
                {
                    case ContentAlignment.TopCenter:
                        textAlignment = ContentAlignment.TopCenter;
                        break;

                    case ContentAlignment.TopLeft:
                        textAlignment = ContentAlignment.TopRight;
                        break;

                    case ContentAlignment.TopRight:
                        textAlignment = ContentAlignment.TopLeft;
                        break;

                    case ContentAlignment.MiddleCenter:
                        textAlignment = ContentAlignment.MiddleCenter;
                        break;

                    case ContentAlignment.MiddleLeft:
                        textAlignment = ContentAlignment.MiddleRight;
                        break;

                    case ContentAlignment.MiddleRight:
                        textAlignment = ContentAlignment.MiddleLeft;
                        break;

                    case ContentAlignment.BottomCenter:
                        textAlignment = ContentAlignment.BottomCenter;
                        break;

                    case ContentAlignment.BottomLeft:
                        textAlignment = ContentAlignment.BottomRight;
                        break;

                    case ContentAlignment.BottomRight:
                        textAlignment = ContentAlignment.BottomLeft;
                        break;
                }
            }

            Rectangle rcText = CalcTextRectangle(textAlignment);

            this.textRect = rcText;

            Rectangle buttonRect = this.Button.ClientRectangle;

            int horizontalGap = Math.Max(0, (buttonRect.Width - 2 * s_textHorizontalMargin - rcText.Width) / 2);
            int verticalGap = Math.Max(0, (buttonRect.Height - 2 * s_textVerticalMargin - rcText.Height) / 2);

            switch (textAlignment)
            {
                case ContentAlignment.BottomCenter:
                    this.textPoint = new Point(horizontalGap + s_textHorizontalMargin, 2 * verticalGap + s_textVerticalMargin);
                    break;

                case ContentAlignment.BottomLeft:
                    this.textPoint = new Point(s_textHorizontalMargin, 2 * verticalGap + s_textVerticalMargin);
                    break;

                case ContentAlignment.BottomRight:
                    this.textPoint = new Point(horizontalGap * 2 + s_textHorizontalMargin, 2 * verticalGap + s_textVerticalMargin);
                    break;

                case ContentAlignment.MiddleCenter:
                    this.textPoint = new Point(horizontalGap + s_textHorizontalMargin, verticalGap + s_textVerticalMargin);
                    break;

                case ContentAlignment.MiddleLeft:
                    this.textPoint = new Point(s_textHorizontalMargin, verticalGap + s_textVerticalMargin);
                    break;

                case ContentAlignment.MiddleRight:
                    this.textPoint = new Point(horizontalGap * 2 + s_textHorizontalMargin, verticalGap + s_textVerticalMargin);
                    break;

                case ContentAlignment.TopCenter:
                    this.textPoint = new Point(horizontalGap + s_textHorizontalMargin, s_textVerticalMargin);
                    break;

                case ContentAlignment.TopLeft:
                    this.textPoint = new Point(s_textHorizontalMargin, s_textVerticalMargin);
                    break;

                case ContentAlignment.TopRight:
                    this.textPoint = new Point(horizontalGap * 2 + s_textHorizontalMargin, s_textVerticalMargin);
                    break;
            }

#if!(SyncfusionFramework1_1 || SyncfusionFramework1_0)

            if (Button.TextImageRelation != TextImageRelation.Overlay
                && Button.Image != null && Button.Text != String.Empty
                && Button.ButtonType == ButtonTypes.Normal)
            {
                LayoutTextAndImage();
            }
#endif
        }
        /// <summary>
        /// </summary>
        public virtual void ComputeImagePosition()
        {
            ButtonAdv button = this.Button;
            Rectangle buttonRect = button.ClientRectangle;
            Size imageSize = s_defaultImageSize;
            int hMargin = s_imageHorizontalMargin;
            int vMargin = s_imageVerticalMargin;
            int border = borderWidth;

            Point imagePoint = new Point(s_imageHorizontalMargin, s_imageVerticalMargin);

            if (button.Image != null && button.ButtonType == ButtonTypes.Normal)
            {
                imageSize = button.Image.Size;
            }
            else
            {
                imageSize = buttonRect.Size;
                hMargin = 0;
                vMargin = 0;
            }

            int horizontalGap = Math.Max(0, (buttonRect.Width - hMargin - imageSize.Width) / 2);
            int verticalGap = Math.Max(0, (buttonRect.Height - vMargin - imageSize.Height) / 2);

            ContentAlignment alignment = button.ImageAlign;
            if (button.RightToLeft == RightToLeft.Yes)
            {
                switch (button.ImageAlign)
                {
                    case ContentAlignment.TopCenter:
                        alignment = ContentAlignment.TopCenter;
                        break;

                    case ContentAlignment.TopLeft:
                        alignment = ContentAlignment.TopRight;
                        break;

                    case ContentAlignment.TopRight:
                        alignment = ContentAlignment.TopLeft;
                        break;

                    case ContentAlignment.MiddleCenter:
                        alignment = ContentAlignment.MiddleCenter;
                        break;

                    case ContentAlignment.MiddleLeft:
                        alignment = ContentAlignment.MiddleRight;
                        break;

                    case ContentAlignment.MiddleRight:
                        alignment = ContentAlignment.MiddleLeft;
                        break;

                    case ContentAlignment.BottomCenter:
                        alignment = ContentAlignment.BottomCenter;
                        break;

                    case ContentAlignment.BottomLeft:
                        alignment = ContentAlignment.BottomRight;
                        break;

                    case ContentAlignment.BottomRight:
                        alignment = ContentAlignment.BottomLeft;
                        break;
                }
            }

            switch (alignment)
            {
                case ContentAlignment.TopCenter:
                    imagePoint = new Point(((button.Width / 2) - (imageSize.Width / 2)), 0);
                    break;
                case ContentAlignment.TopLeft:
                    imagePoint = new Point(0, 0);
                    break;
                case ContentAlignment.TopRight:
                    imagePoint = new Point((button.Width - imageSize.Width), 0);
                    break;
                case ContentAlignment.MiddleCenter:
                    imagePoint = new Point(((button.Width / 2) - (imageSize.Width / 2)), ((button.Height / 2) - (imageSize.Height / 2)));
                    break;
                case ContentAlignment.MiddleLeft:
                    imagePoint = new Point(0, ((button.Height / 2) - (imageSize.Height / 2)));
                    break;
                case ContentAlignment.MiddleRight:
                    imagePoint = new Point((button.Width - imageSize.Width), ((button.Height / 2) - (imageSize.Height / 2)));
                    break;
                case ContentAlignment.BottomCenter:
                    imagePoint = new Point(((button.Width / 2) - (imageSize.Width / 2)), (button.Height - imageSize.Height));
                    break;
                case ContentAlignment.BottomLeft:
                    imagePoint = new Point(0, (button.Height - imageSize.Height));
                    break;
                case ContentAlignment.BottomRight:
                    imagePoint = new Point((button.Width - imageSize.Width), (button.Height - imageSize.Height));
                    break;
            }

            this.imageRect = new RectangleF(imagePoint,
                new SizeF((float)imageSize.Width, (float)imageSize.Height));

#if!(SyncfusionFramework1_1 || SyncfusionFramework1_0)
            if (Button.TextImageRelation != TextImageRelation.Overlay
                && Button.Image != null && Button.Text != String.Empty
                && Button.ButtonType == ButtonTypes.Normal)
            {
                LayoutTextAndImage();
            }
#endif

        }

        /// <summary>
        /// </summary>
        public virtual void RefreshStyle()
        {
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Helper function to get an image from within embedded resources.
        /// </summary>
        /// <param name="resourceName">The resource name to get.</param>
        /// <returns>An image; NULL if the image is not available.</returns>
        /// <remarks>
        /// The <see cref="Syncfusion.Windows.Forms.ButtonRenderer.GetImage(string)"/> class can take an image based on the
        /// <see cref="Syncfusion.Windows.Forms.ButtonRenderer.GetImage(string)"/> property. This helper function
        /// loads the images based on the resource name.
        /// </remarks>
        public static Image GetImage(string resourceName)
        {
            string prefix = "Syncfusion.Windows.Forms.ButtonAdv.resources.";
            string fullResourceName = prefix + resourceName;

            Type type = typeof(ButtonAdv);
            Assembly assembly = type.Module.Assembly;
            Stream stream = assembly.GetManifestResourceStream(fullResourceName);

            if (stream != null)
            {
                Bitmap bmp = new Bitmap(stream);
                bmp.MakeTransparent(Color.White);
                return bmp;
            }

            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <returns><c>True</c> if button has state <see cref="ButtonAdvState.Pressed"/>, otherwise <c>False</c>.</returns>
        protected bool IsPressed(ButtonAdv button)
        {
            return ((button.State & ButtonAdvState.Pressed) == ButtonAdvState.Pressed);
        }
        /// <summary></summary>
        /// <param name="button"></param>
        /// <returns><c>True</c> if button has state <see cref="ButtonAdvState.MouseOver"/>, otherwise <c>False</c>.</returns>
        protected bool IsMouseOver(ButtonAdv button)
        {
            return ((button.State & ButtonAdvState.MouseOver) == ButtonAdvState.MouseOver);
        }
        /// <summary></summary>
        /// <param name="button"></param>
        /// <returns><c>True</c> if button has state <see cref="ButtonAdvState.Default"/>, otherwise <c>False</c>.</returns>
        protected bool IsDefault(ButtonAdv button)
        {
            return ((button.State & ButtonAdvState.Default) == ButtonAdvState.Default);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="proposedSize"></param>
        /// <returns></returns>
        private Size GetButtonTextSize(Size proposedSize)
        {
            TextFormatFlags flags = RenderingHelper.DT_FLAGS;

            if (this.Button.RightToLeft == RightToLeft.Yes)
            {
                flags |= TextFormatFlags.RightToLeft;
            }
            if (!this.Button.UseMnemonic)
            {
                flags |= TextFormatFlags.NoPrefix;
            }
            Size szText = TextRenderer.MeasureText(this.Button.Text, this.Button.Font, proposedSize, flags);

            if (szText.Height > proposedSize.Height)
            {
                int lineHeight = TextRenderer.MeasureText(this.Button.Text, this.Button.Font, proposedSize, TextFormatFlags.SingleLine).Height;

                szText.Height = lineHeight < proposedSize.Height ? (proposedSize.Height / lineHeight) * lineHeight : lineHeight;
            }

            return szText;
        }
        #endregion

#if!(SyncfusionFramework1_1 || SyncfusionFramework1_0)
        #region TextImageRelation
        private void LayoutTextAndImage()
        {
            TextImageRelation relation = this.Button.TextImageRelation;
            if (Button.RightToLeft == RightToLeft.Yes && ((int)relation & 0xc) > 0)
            {
                relation = (TextImageRelation)((int)relation ^ 0xc);
            }

            ContentAlignment imageAlign = this.Button.ImageAlign;
            ContentAlignment textAlign = this.Button.TextAlign;
            if (Button.RightToLeft == RightToLeft.Yes)
            {
                imageAlign = GetRTLAlignment(imageAlign);
                textAlign = GetRTLAlignment(textAlign);
            }

            bool verticalAlignment = (relation == TextImageRelation.ImageAboveText
                           || relation == TextImageRelation.TextAboveImage);
            bool imageFirst = (relation == TextImageRelation.ImageAboveText
                           || relation == TextImageRelation.ImageBeforeText);

            Size clientSize = this.Button.ClientRectangle.Size;
            //borders
            int border = 3;
            clientSize = new Size(clientSize.Width - 2 * border, clientSize.Height - 2 * border);
            //borders
            Size textSize = GetButtonTextSize(clientSize);
            Size imageSize = Button.Image.Size;

            if (verticalAlignment)
            {
                imageSize = new Size(imageSize.Height, imageSize.Width);
                textSize = new Size(textSize.Height, textSize.Width);
                clientSize = new Size(clientSize.Height, clientSize.Width);
            }

            int imageAlignFlags = GetAlignFlags(imageAlign, relation, verticalAlignment);
            int textAlignFlags = GetAlignFlags(textAlign, relation, verticalAlignment);

            if (imageFirst)
            {
                float disp = GetImageDisplace(imageAlignFlags, textAlignFlags, imageSize.Width,
                    textSize.Width, clientSize.Width);

                if (disp != 0 && disp + imageSize.Width + textSize.Width > clientSize.Width)
                {
                    disp = Math.Max(0, clientSize.Width - imageSize.Width - textSize.Width);
                }

                if (disp == 0 && !verticalAlignment)
                {
                    int prefferedTextSize = clientSize.Width - imageSize.Width;
                    if (prefferedTextSize < textSize.Width)
                    {
                        textSize = GetButtonTextSize(new Size(prefferedTextSize, clientSize.Height));
                        disp = GetImageDisplace(imageAlignFlags, textAlignFlags, imageSize.Width,
                    textSize.Width, clientSize.Width);
                    }
                }
                disp += border;

                PointF imagePoint = Point.Round(new PointF(disp,
                    GetDefaultAlignPoint(imageAlignFlags >> 4, imageSize.Height, clientSize.Height) + border));
                this.imageRect = new RectangleF(imagePoint, imageSize);

                this.textPoint = new PointF(imageRect.Right,
                    GetDefaultAlignPoint(textAlignFlags >> 4, textSize.Height, clientSize.Height) + border);
                this.textRect = new RectangleF(textPoint, textSize);
            }
            else
            {

                float disp = GetTextDisplace(imageAlignFlags, textAlignFlags, imageSize.Width,
                    textSize.Width, clientSize.Width);

                if (disp + imageSize.Width + textSize.Width > clientSize.Width)
                {
                    disp = Math.Max(0, clientSize.Width - imageSize.Width - textSize.Width);
                }

                if (disp == 0 && !verticalAlignment)
                {
                    int prefferedTextSize = Math.Max(0, clientSize.Width - imageSize.Width);
                    if (prefferedTextSize < textSize.Width)
                    {
                        textSize = GetButtonTextSize(new Size(prefferedTextSize, clientSize.Height));
                    }
                }

                disp += border;

                this.textPoint = new PointF(disp,
                    GetDefaultAlignPoint(textAlignFlags >> 4, textSize.Height, clientSize.Height) + border);
                this.textRect = new RectangleF(textPoint, textSize);

                float size, imageX;
                float textWidth = textRect.Right - border;
                if ((textAlignFlags & 0xf) == 4 //text is on top
                    || (textWidth > clientSize.Width / 2)
                    || imageSize.Width > clientSize.Width / 2)
                {
                    size = Math.Max(0, clientSize.Width - textWidth);
                    imageX = GetDefaultAlignPoint(imageAlignFlags, imageSize.Width, (int)size);
                    imageX += textRect.Right;
                }
                else
                {
                    size = clientSize.Width / 2;
                    imageX = GetDefaultAlignPoint(imageAlignFlags, imageSize.Width, (int)size)
                        + size;
                }


                PointF imagePoint = new PointF(imageX,
                    GetDefaultAlignPoint(imageAlignFlags >> 4, imageSize.Height, clientSize.Height) + border);
                this.imageRect = new RectangleF(imagePoint, imageSize);
            }

            if (verticalAlignment)
            {
                this.textRect = new RectangleF(textRect.Y, textRect.X, textRect.Height, textRect.Width);
                this.imageRect = new RectangleF(imageRect.Y, imageRect.X, imageRect.Height, imageRect.Width);
            }
        }

        private int GetAlignFlags(ContentAlignment align, TextImageRelation relation, 
			bool vertical)
        {
            int top, bot, mid, bitA;

            top = 0xf;
            mid = 0xf0;
            bot = 0xf00;

            bitA = ((((int)align & top) > 0) ? 4 : 0)		//1 if Top
                    | ((((int)align & mid) > 0) ? 2 : 0)	//1 if Middle
                    | ((((int)align & bot) > 0) ? 1 : 0);	//1 if Bottom

            top = 0x111;
            mid = 0x222;
            bot = 0x444;

            bitA += ((((int)align & top) > 0) ? 64 : 0)		//1 if Top
                    | ((((int)align & mid) > 0) ? 32 : 0)	//1 if Middle
                    | ((((int)align & bot) > 0) ? 16 : 0);	//1 if Bottom

            if (!vertical)
            {
                int n = bitA & 0xf;
                bitA >>= 4;
                n <<= 4;
                bitA += n;
            }
            return bitA;
        }

        private float GetAlignPoint(int position, float size, int clientSize)
        {
            float delta = 0;
            float gap = Math.Max(0, (clientSize - size) / 2);
            switch (position)
            {
                case 1:
                    delta = 0;
                    break;
                case 2:
                    delta = gap / 2;
                    break;
                case 3:
                    delta = gap;
                    break;
                case 4:
                    delta = gap * 2;
                    break;
            }
            return delta;
        }

        private float GetDefaultAlignPoint(int alignment, float size, int clientSize)
        {
            float gap = Math.Max(0, (clientSize - size) / 2);
            float point = 0;
            switch (alignment & 0xf)
            {
                case 4://top
                    point = 0;
                    break;
                case 2://middle
                    point = gap;
                    break;
                case 1://bottom
                    point = gap * 2;
                    break;
            }
            return point;
        }

        private ContentAlignment GetRTLAlignment(ContentAlignment alignment)
        {
            int align = (int)alignment;
            int mask = 0x5;
            for (int i = 0; i < 3; i++)
            {
                if ((align & mask) > 0)
                {
                    align ^= mask;
                    break;
                }
                mask <<= 4;
            }
            return (ContentAlignment)align;
        }

        private float GetImageDisplace(int imageA, int textA, int imageSize, float textSize,
			int clientSize)
        {
            float disp;
            int position = 0;
            float size = imageSize + textSize;

            if ((textA & 0x6) > 0)//Top or Middle
            {
                switch (imageA & 0xf)
                {
                    case 1://Bottom
                        position = 3;
                        break;
                    case 2://Middle
                        position = 2;
                        break;
                    case 4://Top
                        position = 1;
                        break;
                }
            }
            else//Bottom
            {
                switch (imageA & 0xf)
                {
                    case 1://Bottom
                        position = 4;
                        break;
                    case 2://Middle
                        position = 3;
                        break;
                    case 4://Top
                        position = 1;
                        break;
                }
            }
            disp = GetAlignPoint(position, size, clientSize);
            return disp;
        }

        private float GetTextDisplace(int imageA, int textA, int imageSize, float textSize,
			int clientSize)
        {
            float disp;
            int position = 0;
            float size = imageSize + textSize;

            if ((textA & 0x4) > 0)//Top 
            {
                position = 1;
            }
            else if ((textA & 0x2) > 0)//Middle
            {
                switch (imageA & 0xf)
                {
                    case 1://Bottom
                        position = 3;
                        break;
                    case 2://Middle
                    case 4://Top
                        position = 2;
                        break;
                }
            }
            else//Bottom
            {
                switch (imageA & 0xf)
                {
                    case 1://Bottom
                        position = 4;
                        break;
                    case 2://Middle
                    case 4://Top
                        position = 3;
                        break;
                }
            }
            disp = GetAlignPoint(position, size, clientSize);
            return disp;
        }
        #endregion
#endif

        #region Class overrides
        /// <summary></summary>
        /// <returns></returns>
        /// <param name="g"/>
        /// <param name="align"/>
        protected virtual Rectangle CalcTextRectangle(ContentAlignment align)
        {
            Rectangle rcText = Rectangle.Inflate(this.Button.ClientRectangle, -s_textHorizontalMargin, -s_textVerticalMargin);

            Size szText = GetButtonTextSize(rcText.Size);
		 	
			rcText = RenderingHelper.HAlignWithin(szText, rcText, align);
            rcText = RenderingHelper.VAlignWithin(szText, rcText, align);

            if (this.Button.Padding.Left >= 0 || this.Button.Padding.Right >= 0 || this.Button.Padding.Top >= 0 || this.Button.Padding.Bottom >= 0)
            {
                rcText.X += this.Button.Padding.Left;
                rcText.Y += this.Button.Padding.Top;
                rcText.Width = rcText.Width - this.Button.Padding.Right - this.Button.Padding.Left;
                rcText.Height = rcText.Height - this.Button.Padding.Bottom - this.Button.Padding.Top;
            }
           
            return rcText;
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        /// <summary>
        /// Renders an background image.
        /// </summary>
        /// <param name="g">The graphics object to use.</param>
        /// <param name="backgroundImage">BackgroundImage to render.</param>
        /// <param name="backgroundImageLayout"> The layout of background image.</param>
        /// <param name="bounds"> The bounds of the control.</param>
        /// <param name="clipRect"> The rectangle to draw on it.</param>
        /// <param name="scrollOffset"> Offset of location of the image when layout is tile.</param>
        /// <param name="rightToLeft"> The rightToLeft property of the control.</param>
        protected virtual void DrawBackgroundImage(Graphics g, Image backgroundImage, ImageLayout backgroundImageLayout,
                                                    Rectangle bounds, Rectangle clipRect, 
                                                    Point scrollOffset, RightToLeft rightToLeft)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }
            if (backgroundImageLayout == ImageLayout.Tile)
            {
                using (TextureBrush brush1 = new TextureBrush(backgroundImage, WrapMode.Tile))
                {
                    if (scrollOffset != Point.Empty)
                    {
                        Matrix matrix1 = brush1.Transform;
                        matrix1.Translate((float)scrollOffset.X, (float)scrollOffset.Y);
                        brush1.Transform = matrix1;
                    }
                    g.FillRectangle(brush1, clipRect);
                    return;
                }
            }

            Rectangle rectangle1 = RenderingHelper.CalculateBackgroundImageRectangle(bounds, backgroundImage, backgroundImageLayout);

            if ((rightToLeft == RightToLeft.Yes) && (backgroundImageLayout == ImageLayout.None))
            {
                rectangle1.X += clipRect.Width - rectangle1.Width;
            }

            if (!clipRect.Contains(rectangle1))
            {
                if (backgroundImageLayout == ImageLayout.Stretch ||
                     backgroundImageLayout == ImageLayout.Zoom)
                {
                    rectangle1.Intersect(clipRect);
                    g.DrawImage(backgroundImage, rectangle1);
                }
                else if (backgroundImageLayout == ImageLayout.None)
                {
                    rectangle1.Offset(clipRect.Location);
                    Rectangle rectangle2 = rectangle1;
                    rectangle2.Intersect(clipRect);
                    Rectangle rectangle3 = new Rectangle(Point.Empty, rectangle2.Size);
                    g.DrawImage(backgroundImage, rectangle2, rectangle3.X, rectangle3.Y, rectangle3.Width, rectangle3.Height, GraphicsUnit.Pixel);
                }
                else
                {
                    Rectangle rectangle4 = rectangle1;
                    rectangle4.Intersect(clipRect);
                    Rectangle rectangle5 = new Rectangle(new Point(rectangle4.X - rectangle1.X, rectangle4.Y - rectangle1.Y), rectangle4.Size);
                    g.DrawImage(backgroundImage, rectangle4, rectangle5.X, rectangle5.Y, rectangle5.Width, rectangle5.Height, GraphicsUnit.Pixel);
                }
            }
            else
            {
                ImageAttributes attributes1 = new ImageAttributes();
                attributes1.SetWrapMode(WrapMode.TileFlipXY);
                g.DrawImage(backgroundImage, rectangle1, 0, 0, backgroundImage.Width, backgroundImage.Height, GraphicsUnit.Pixel, attributes1);
                attributes1.Dispose();
            }
        }
#endif

        /// <summary>
        /// Renders an image. Delegates to the <see cref="RenderingHelper"/> class.
        /// </summary>
        /// <param name="g">The graphics object to use.</param>
        /// <param name="bounds">The bounds to be used when rendering the image.</param>
        /// <param name="image">Image to render.</param>
        protected virtual void DrawImage(Graphics g, RectangleF bounds, Image image)
        {
            DrawParams param = new DrawParams();
            param.ControlBackColor = this.Button.BackColor;
            param.Enabled = this.Button.Enabled;

            ButtonAdv button = this.Button;
            Rectangle imageRectangle = new Rectangle(0, 0, image.Width, image.Height);

            int hMargin = s_imageHorizontalMargin;
            int vMargin = s_imageVerticalMargin;
            int x = 0;
            int y = 0;
            int width = 0;
            int height = 0;
            int widthOffset = 0;
            int heightOffset = 0;
            int border = borderWidth;

            int hOdd = 0;
            int vOdd = 0;

            if ((float)button.Width / 2 - button.Width / 2 == 0.0F)
            {
                hOdd = 1;
            }

            if ((float)button.Height / 2 - button.Height / 2 == 0.0F)
            {
                vOdd = 1;
            }

            Bitmap b = (Bitmap)image;
            b.SetResolution(g.DpiX, g.DpiY);
            image = (Image)b;

            ContentAlignment alignment = button.ImageAlign;
            if (button.RightToLeft == RightToLeft.Yes)
            {
                switch (button.ImageAlign)
                {
                    case ContentAlignment.TopCenter:
                        alignment = ContentAlignment.TopCenter;
                        break;

                    case ContentAlignment.TopLeft:
                        alignment = ContentAlignment.TopRight;
                        break;

                    case ContentAlignment.TopRight:
                        alignment = ContentAlignment.TopLeft;
                        break;

                    case ContentAlignment.MiddleCenter:
                        alignment = ContentAlignment.MiddleCenter;
                        break;

                    case ContentAlignment.MiddleLeft:
                        alignment = ContentAlignment.MiddleRight;
                        break;

                    case ContentAlignment.MiddleRight:
                        alignment = ContentAlignment.MiddleLeft;
                        break;

                    case ContentAlignment.BottomCenter:
                        alignment = ContentAlignment.BottomCenter;
                        break;

                    case ContentAlignment.BottomLeft:
                        alignment = ContentAlignment.BottomRight;
                        break;

                    case ContentAlignment.BottomRight:
                        alignment = ContentAlignment.BottomLeft;
                        break;
                }
            }

            switch (alignment)
            {
                case ContentAlignment.TopCenter:
                    if (image.Width - button.Width > 0)
                        widthOffset = (image.Width - button.Width) / 2;
                    else
                        widthOffset = 0;
                    width = button.Width - border * 2;
                    height = button.Height - vMargin - border * 2;

                    imageRectangle = new Rectangle(widthOffset, 0, width, height);
                    break;

                case ContentAlignment.TopLeft:
                    width = button.Width - hMargin - border * 2;
                    height = button.Height - vMargin - border * 2;

                    imageRectangle = new Rectangle(0, 0, width, height);
                    break;

                case ContentAlignment.TopRight:
                    if (image.Width - button.Width > 0)
                        x = image.Width - button.Width + border;
                    else
                        x = hOdd;
                    width = button.Width - border * 2;
                    height = button.Height - vMargin - border * 2;

                    imageRectangle = new Rectangle(x, 0, width, height);
                    break;

                case ContentAlignment.MiddleCenter:
                    if (image.Width - button.Width > 0)
                        widthOffset = (image.Width - button.Width) / 2;
                    else
                        widthOffset = 0;
                    if (image.Height - button.Height > 0)
                        heightOffset = (image.Height - button.Height) / 2;
                    else
                        heightOffset = 0;
                    width = button.Width - border * 2;
                    height = button.Height - border * 2;

                    imageRectangle = new Rectangle(widthOffset, heightOffset, width, height);
                    break;

                case ContentAlignment.MiddleLeft:
                    if (image.Height - button.Height > 0)
                        heightOffset = (image.Height - button.Height) / 2;
                    else
                        heightOffset = 0;
                    width = button.Width - hMargin - border * 2;
                    height = button.Height - border * 2;

                    imageRectangle = new Rectangle(0, heightOffset, width, height);
                    break;

                case ContentAlignment.MiddleRight:
                    if (image.Width - button.Width > 0)
                        x = image.Width - button.Width + border;
                    else
                        x = hOdd;
                    if (image.Height - button.Height > 0)
                        heightOffset = (image.Height - button.Height) / 2;
                    else
                        heightOffset = 0;
                    width = button.Width - border * 2;
                    height = button.Height - border * 2;

                    imageRectangle = new Rectangle(x, heightOffset, width, height);
                    break;

                case ContentAlignment.BottomCenter:
                    if (image.Height - button.Height > 0)
                        y = image.Height - button.Height + border;
                    else
                        y = border + vOdd;
                    if (image.Width - button.Width > 0)
                        widthOffset = (image.Width - button.Width) / 2;
                    else
                        widthOffset = hOdd;
                    width = button.Width - border * 2;
                    height = button.Height - border * 2;

                    imageRectangle = new Rectangle(widthOffset, y, width, height);
                    break;

                case ContentAlignment.BottomLeft:
                    if (image.Height - button.Height > 0)
                        y = image.Height - button.Height + border;
                    else
                        y = border + vOdd;
                    width = button.Width - hMargin - border * 2;
                    height = button.Height - border * 2;

                    imageRectangle = new Rectangle(0, y, width, height);
                    break;

                case ContentAlignment.BottomRight:
                    if (image.Width - button.Width > 0)
                        x = image.Width - button.Width + border;
                    else
                        x = hOdd;
                    if (image.Height - button.Height > 0)
                        y = image.Height - button.Height + border;
                    else
                        y = border + vOdd;
                    width = button.Width - border * 2;
                    height = button.Height - border * 2;

                    imageRectangle = new Rectangle(x, y, width, height);
                    break;
            }

            RenderingHelper.DrawImage(g, bounds, image, imageRectangle, param);
        }

        /// <summary>
        /// Renders an inverted triangle. Delegates to the <see cref="RenderingHelper"/> class.
        /// </summary>
        /// <param name="g">The graphics object to use.</param>
        /// <param name="bounds">The bounds to be used when rendering the shape.</param>
        /// <param name="brush">The brush to be used for filling the interior of the rendered triangle.</param>
        /// <param name="pen">The pen to be used for stroking the triangle.</param>
        protected virtual void DrawInvertedTriangle(Graphics g, RectangleF bounds, Brush brush, Pen pen)
        {
            RenderingHelper.DrawInvertedTriangle(g, bounds, brush, pen);
        }

        /// <summary>
        /// Renders a triangle. Delegates to the <see cref="RenderingHelper"/> class.
        /// </summary>
        /// <param name="g">The graphics object to use.</param>
        /// <param name="bounds">The bounds to be used when rendering the shape.</param>
        /// <param name="brush">The brush to be used for filling the interior of the rendered triangle.</param>
        /// <param name="pen">The pen to use for stroking the triangle.</param>
        protected virtual void DrawTriangle(Graphics g, RectangleF bounds, Brush brush, Pen pen)
        {
            RenderingHelper.DrawTriangle(g, bounds, brush, pen);
        }

        /// <summary></summary>
        /// <param name="bounds"/>
        protected internal virtual void SetBounds(Rectangle bounds)
        {
            this.bounds = bounds;
            ComputeTextPosition();
            ComputeImagePosition();
        }
        #endregion
    }

    /// <summary></summary>
    public class DrawParams
    {
        #region Class members
        /// <summary></summary>
        private ContentAlignment m_align = ContentAlignment.MiddleLeft;
        /// <summary></summary>
        private RectangleF m_bounds = Rectangle.Empty;
        /// <summary></summary>
        private Color m_backColor = Color.Empty;
        /// <summary></summary>
        private bool m_enabled;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bRightToLeft;
        #endregion

        #region Class properties
        /// <summary></summary>
        public Color ControlBackColor
        {
            get
            {
                return m_backColor;
            }
            set
            {
                if (value != m_backColor)
                {
                    m_backColor = value;
                }
            }
        }

        /// <summary></summary>
        public ContentAlignment Align
        {
            get
            {
                return m_align;
            }
            set
            {
                if (value != m_align)
                {
                    m_align = value;
                }
            }
        }

        /// <summary></summary>
        public RectangleF Bounds
        {
            get
            {
                return m_bounds;
            }
            set
            {
                if (value != m_bounds)
                {
                    m_bounds = value;
                }
            }
        }
        /// <summary></summary>
        public bool Enabled
        {
            get
            {
                return m_enabled;
            }
            set
            {
                m_enabled = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool RightToLeft
        {
            get { return m_bRightToLeft; }
            set { m_bRightToLeft = value; }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor
        /// </summary>
        public DrawParams()
        {

        }
        /// <summary>
        /// Constructor with variables initialization.
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="back"></param>
        /// <param name="bounds"></param>
        /// <param name="align"></param>
        public DrawParams(bool enabled, Color back, RectangleF bounds, ContentAlignment align)
            : this (enabled, back, bounds, align, false)
        { }

        public DrawParams(bool enabled, Color back, RectangleF bounds, ContentAlignment align, bool bRightToLeft)
        {
            m_enabled = enabled;
            m_backColor = back;
            m_bounds = bounds;
            m_align = align;
            m_bRightToLeft = bRightToLeft;
        }
        #endregion
    }
}
