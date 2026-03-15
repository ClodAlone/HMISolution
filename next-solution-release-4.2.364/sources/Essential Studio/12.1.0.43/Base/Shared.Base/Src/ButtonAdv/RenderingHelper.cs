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
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Runtime.InteropServices;
#endregion

namespace Syncfusion.Windows.Forms
{
    /// <summary>
    /// Show or Hide the prefix character
    /// </summary>
    internal enum ButtonMnemonicHelper
    {
        None = 0,
        HidePrefix = 1,  // Ignores the ampersand (&) prefix character in the text
        NoPrefix = 2     // Turns off processing of prefix characters
    }

    /// <summary>
    /// Summary description for RenderingHelper.
    /// </summary>
    internal sealed class RenderingHelper
    {
        #region Class constants
        /// <summary></summary>
        private const int DEF_DISABLED_TEXT_OFFSET = 1;
        /// <summary></summary>
        private const float DEF_MIN_BRIGHTNESS = 0.5f;
        /// <summary></summary>
        private const float DEF_MIN_BRIGHTNESS_COEF = 0.9f;
        /// <summary></summary>
        private const float DEF_BRIGHTNESS_COEF = 1.2f;
        /// <summary></summary>
        internal const ContentAlignment anyRight = ContentAlignment.BottomRight | ContentAlignment.MiddleRight | ContentAlignment.TopRight;
        /// <summary></summary>
        internal const ContentAlignment anyBottom = ContentAlignment.BottomRight | ContentAlignment.BottomCenter | ContentAlignment.BottomLeft;
        /// <summary></summary>
        internal const ContentAlignment anyCenter = ContentAlignment.BottomCenter | ContentAlignment.MiddleCenter | ContentAlignment.TopCenter;
        /// <summary></summary>
        internal const ContentAlignment anyMiddle = ContentAlignment.MiddleRight | ContentAlignment.MiddleCenter | ContentAlignment.MiddleLeft;
        /// <summary>
        /// 
        /// </summary>
        internal static readonly TextFormatFlags DT_FLAGS = TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl;

        #endregion

        #region Class Initialize/Finalize methods
        /// <summary></summary>
        private RenderingHelper()
        {
            throw new NotSupportedException("Instance of this class not allowed.");
        }
        #endregion

        #region Class Public Methods

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        /// <summary>
        /// Calculates rectangle to draw on it backgroundImage, depending on imageLayout
        /// </summary>  
        /// <param name="bounds">Bounds of the control</param>
        /// <param name="backgroundImage">Background image of the control</param>
        /// <param name="imageLayout">Image layout of the background image</param>
        public static Rectangle CalculateBackgroundImageRectangle(Rectangle bounds, Image backgroundImage, ImageLayout imageLayout)
        {
            Rectangle rectangle1 = bounds;
            if (backgroundImage != null)
            {
                switch (imageLayout)
                {
                    case ImageLayout.None:
                        rectangle1.Size = backgroundImage.Size;
                        return rectangle1;

                    case ImageLayout.Tile:
                        return rectangle1;

                    case ImageLayout.Center:
                        {
                            rectangle1.Size = backgroundImage.Size;
                            Size size1 = bounds.Size;
                            if (size1.Width > rectangle1.Width)
                            {
                                rectangle1.X = (size1.Width - rectangle1.Width) / 2;
                            }
                            if (size1.Height > rectangle1.Height)
                            {
                                rectangle1.Y = (size1.Height - rectangle1.Height) / 2;
                            }
                            return rectangle1;
                        }
                    case ImageLayout.Stretch:
                        rectangle1.Size = bounds.Size;
                        return rectangle1;

                    case ImageLayout.Zoom:
                        {
                            Size size2 = backgroundImage.Size;
                            float single1 = ((float)bounds.Width) / ((float)size2.Width);
                            float single2 = ((float)bounds.Height) / ((float)size2.Height);
                            if (single1 < single2)
                            {
                                rectangle1.Width = bounds.Width;
                                rectangle1.Height = (int)((size2.Height * single1) + 0.5);
                                if (bounds.Y >= 0)
                                {
                                    rectangle1.Y = (bounds.Height - rectangle1.Height) / 2;
                                }
                                return rectangle1;
                            }
                            rectangle1.Height = bounds.Height;
                            rectangle1.Width = (int)((size2.Width * single2) + 0.5);
                            if (bounds.X >= 0)
                            {
                                rectangle1.X = (bounds.Width - rectangle1.Width) / 2;
                            }
                            return rectangle1;
                        }
                }
            }
            return rectangle1;
        }
#endif

        /// <summary></summary>
        /// <param name="g"/>
        /// <param name="bounds"/>
        /// <param name="image"/>
        /// <param name="param"/>
        public static void DrawImage(Graphics g, RectangleF bounds, Image image, Rectangle imageRect, DrawParams param)
        {
            float left = bounds.Left, top = bounds.Top;

            if (image.Height < bounds.Height)
            {
                top = (bounds.Height - image.Height) / 2;
            }

            if (image.Width < bounds.Width)
            {
                left = (bounds.Width - image.Width) / 2;
            }

            if (param.Enabled)
            {
                GraphicsUnit units = GraphicsUnit.Pixel;
                g.DrawImage(image, left, top, imageRect, units);
            }
            else
            {
                ControlPaint.DrawImageDisabled(g, image, (int)left, (int)top, param.ControlBackColor);
            }
        }

        /// <summary></summary>
        /// <param name="g"/>
        /// <param name="bounds"/>
        /// <param name="brush"/>
        /// <param name="pen"/>
        public static void DrawCircle(Graphics g, RectangleF bounds, Brush brush, Pen pen)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddEllipse(bounds);

                using (Region rgn = new Region(gp))
                {
                    g.FillRegion(brush, rgn);
                    g.DrawPath(pen, gp);
                }
            }
        }

        /// <summary></summary>
        /// <param name="g"/>
        /// <param name="bounds"/>
        /// <param name="brush"/>
        /// <param name="pen"/>
        public static void DrawDiamond(Graphics g, RectangleF bounds, Brush brush, Pen pen)
        {
            PointF[] points = new PointF[4]
				{
					new PointF( bounds.X + bounds.Width / 2, bounds.Y ),
					new PointF( bounds.Right, bounds.Y + bounds.Height / 2 ),
					new PointF( bounds.X + bounds.Width / 2, bounds.Bottom ),
					new PointF( bounds.X, bounds.Y + bounds.Height / 2 )
				};

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLines(points);
                gp.CloseAllFigures();

                using (Region rgn = new Region(gp))
                {
                    g.FillRegion(brush, rgn);
                    g.DrawPath(pen, gp);
                }
            }
        }

        /// <summary></summary>
        /// <param name="g"/>
        /// <param name="bounds"/>
        /// <param name="brush"/>
        /// <param name="pen"/>
        public static void DrawInvertedTriangle(Graphics g, RectangleF bounds, Brush brush, Pen pen)
        {
            PointF[] points = new PointF[3]
				{
					new PointF( bounds.X, bounds.Y ),
					new PointF( bounds.Right, bounds.Y ),
					new PointF( bounds.X + bounds.Width / 2, bounds.Bottom )
				};

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLines(points);
                gp.CloseAllFigures();

                using (Region rgn = new Region(gp))
                {
                    g.FillRegion(brush, rgn);
                    g.DrawPath(pen, gp);
                }
            }
        }

        /// <summary></summary>
        /// <param name="g"/>
        /// <param name="bounds"/>
        /// <param name="brush"/>
        /// <param name="pen"/>
        public static void DrawTriangle(Graphics g, RectangleF bounds, Brush brush, Pen pen)
        {
            PointF[] points = new PointF[3]
				{
					new PointF( bounds.X + bounds.Width / 2, bounds.Y ),
					new PointF( bounds.X, bounds.Bottom ),
					new PointF( bounds.Right, bounds.Bottom )
				};

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLines(points);
                gp.CloseAllFigures();

                using (Region rgn = new Region(gp))
                {
                    g.FillRegion(brush, rgn);
                    g.DrawPath(pen, gp);
                }
            }
        }

        /// <summary></summary>
        /// <param name="g"/>
        /// <param name="str"/>
        /// <param name="textColor"/>
        /// <param name="textFont"/>
        /// <param name="pt"/>
        /// <param name="param"/>
        /// <param name="drawShadow"/>
        /// <param name="autoEllipsis"/>
        public static void DrawText(Graphics g, string str, Color textColor, Font textFont, PointF pt, DrawParams param, bool drawShadow, bool autoEllipsis)
        {
            DrawText(g, str, textColor, textFont, pt, param, drawShadow, autoEllipsis, ButtonMnemonicHelper.None);
        }

        /// <summary></summary>
        /// <param name="g"/>
        /// <param name="str"/>
        /// <param name="textColor"/>
        /// <param name="textFont"/>
        /// <param name="pt"/>
        /// <param name="param"/>
        /// <param name="drawShadow"/>
        /// <param name="autoEllipsis"/>
        /// <param name="prefixChar"/>
        public static void DrawText(Graphics g, string str, Color textColor, Font textFont, PointF pt, DrawParams param, bool drawShadow, bool autoEllipsis,ButtonMnemonicHelper prefixChar)
        {
            if (autoEllipsis || drawShadow)
            {
                TextFormatFlags flags = DT_FLAGS;

                if (prefixChar == ButtonMnemonicHelper.HidePrefix)
                    flags |= TextFormatFlags.HidePrefix;
                else if (prefixChar == ButtonMnemonicHelper.NoPrefix)
                    flags |= TextFormatFlags.NoPrefix;
                if (param.RightToLeft)
                    flags |= TextFormatFlags.RightToLeft;

                if (drawShadow)
                    flags |= TranslateAlignment(param.Align) | TranslateLineAlignment(param.Align);

                if (autoEllipsis)
                    flags |= TextFormatFlags.EndEllipsis;

                if(autoEllipsis)
                    TextRenderer.DrawText(g, str, textFont, Rectangle.Round(param.Bounds), SystemColors.ControlText, flags);
                else
                    TextRenderer.DrawText(g, str, textFont, Rectangle.Round(param.Bounds), SystemColors.ControlDark, flags);
            }
            else DrawText(g, str, textColor, textFont, pt, param, prefixChar);
        }

        /// <summary></summary>
        /// <param name="g"/>
        /// <param name="str"/>
        /// <param name="textColor"/>
        /// <param name="textFont"/>
        /// <param name="pt"/>
        /// <param name="param"/>
        /// <param name="drawShadow"/>
        public static void DrawText(Graphics g, string str, Color textColor, Font textFont, PointF pt, DrawParams param, bool drawShadow)
        {
            if (drawShadow)
            {
                TextFormatFlags flags = DT_FLAGS | TranslateAlignment(param.Align) | TranslateLineAlignment(param.Align);

                if (param.RightToLeft)
                {
                    flags |= TextFormatFlags.RightToLeft;
                }

                TextRenderer.DrawText(g, str, textFont, Rectangle.Round(param.Bounds), SystemColors.ControlDark, flags);
            }
            else DrawText(g, str, textColor, textFont, pt, param);
        }

        /// <summary></summary>
        /// <param name="g"/>
        /// <param name="str"/>
        /// <param name="textColor"/>
        /// <param name="textFont"/>
        /// <param name="pt"/>
        /// <param name="param"/>
        public static void DrawText(Graphics g, string str, Color textColor, Font textFont, PointF pt, DrawParams param)
        {
            DrawText(g, str, textColor, textFont, pt, param, ButtonMnemonicHelper.None);
        }

        /// <summary></summary>
        /// <param name="g"/>
        /// <param name="str"/>
        /// <param name="textColor"/>
        /// <param name="textFont"/>
        /// <param name="pt"/>
        /// <param name="param"/>
        /// <param name="prefixChar"/>
        public static void DrawText(Graphics g, string str, Color textColor, Font textFont, PointF pt, DrawParams param,ButtonMnemonicHelper prefixChar)
        {
            Rectangle textRect = Rectangle.Round(param.Bounds);

            TextFormatFlags flags = DT_FLAGS | TranslateAlignment(param.Align) | TranslateLineAlignment(param.Align);

            if (prefixChar == ButtonMnemonicHelper.HidePrefix)
                flags |= TextFormatFlags.HidePrefix;
            else if (prefixChar == ButtonMnemonicHelper.NoPrefix)
                flags |= TextFormatFlags.NoPrefix;

            if (param.RightToLeft)
            {
                flags |= TextFormatFlags.RightToLeft;
            }

            if (param.Enabled)
            {
                TextRenderer.DrawText(g, str, textFont, textRect, textColor, flags);
            }
            else
            {
                DrawStringDisabled(g, str, textFont, param.ControlBackColor, textRect, flags);
            }
        }

        public static void DrawStringDisabled(Graphics g, string s, Font font, Color color, Rectangle rc, TextFormatFlags format)
        {
            Color colorDark = SystemColors.ControlDark;
            Color colorDarkDark = SystemColors.ControlDarkDark;

            int R = (colorDark.R + colorDarkDark.R) / 2;
            int G = (colorDark.G + colorDarkDark.G) / 2;
            int B = (colorDark.B + colorDarkDark.B) / 2;

            Color colorText = Color.FromArgb(R, G, B);

            rc.Offset(1, 1);
            TextRenderer.DrawText(g, s, font, rc, SystemColors.ControlLightLight, format);
            rc.Offset(-1, -1);
            TextRenderer.DrawText(g, s, font, rc, colorText, format);
        }

        /// <summary></summary>
        /// <param name="g"/>
        /// <param name="bounds"/>
        /// <param name="buttonImageType"/>
        /// <param name="offset"/>
        /// <param name="imageColor"/>
        public static void DrawRightLeftButton(Graphics g, RectangleF bounds, ButtonTypes buttonImageType, Point offset, Color imageColor)
        {
            // Draw arrow inside the button
            SolidBrush buttonBrush = new SolidBrush(imageColor);

            try
            {
                float x, y;
                RectangleF rect = bounds;
                float cx = rect.Height > 19 ? 8 : 7;
                float cy = rect.Height > 19 ? 9 : 7;

                // center the 9-pixel high arrow vertically
                y = rect.Top + ((rect.Height - 9) / 2);
                float y2 = rect.Top + ((rect.Height - cy) / 2);

                // if pressed, image gets moved
                y += offset.Y;

                switch (buttonImageType)
                {
                    case ButtonTypes.LeftEnd:
                        // Most left
                        x = rect.Left + ((rect.Width - cx) / 2) - 1 + offset.X;

                        g.FillRectangle(buttonBrush, x, y2, 1, cy);
                        goto case ButtonTypes.Left;

                    case ButtonTypes.Left:
                        // Left Arrow
                        x = rect.Left + ((rect.Width - cx) / 2) - 1 + offset.X;

                        g.FillRectangle(buttonBrush, x + 2, y + 4, 1, 1);
                        g.FillRectangle(buttonBrush, x + 3, y + 3, 1, 3);
                        g.FillRectangle(buttonBrush, x + 4, y + 2, 1, 5);
                        g.FillRectangle(buttonBrush, x + 5, y + 1, 1, 7);

                        if (rect.Height > 19)
                        {
                            g.FillRectangle(buttonBrush, x + 6, y, 1, 9);
                        }
                        break;

                    case ButtonTypes.RightEnd:
                        // Most Right
                        x = rect.Right - ((rect.Width - cx) / 2) + offset.X;

                        g.FillRectangle(buttonBrush, x, y2, 1, cy);
                        goto case ButtonTypes.Right;

                    case ButtonTypes.Right:
                        // Right Arrow
                        x = rect.Right - ((rect.Width - cx) / 2) + offset.X;

                        g.FillRectangle(buttonBrush, x - 2, y + 4, 1, 1);
                        g.FillRectangle(buttonBrush, x - 3, y + 3, 1, 3);
                        g.FillRectangle(buttonBrush, x - 4, y + 2, 1, 5);
                        g.FillRectangle(buttonBrush, x - 5, y + 1, 1, 7);
                        if (rect.Height > 19)
                        {
                            g.FillRectangle(buttonBrush, x - 6, y, 1, 9);
                        }
                        break;

                }
            }
            finally
            {
                buttonBrush.Dispose();
            }
        }

        /// <summary></summary>
        /// <param name="g"/>
        /// <param name="bounds"/>
        /// <param name="buttonImageType"/>
        /// <param name="offset"/>
        /// <param name="imageColor"/>
        public static void DrawUpDownButton(Graphics g, RectangleF bounds, ButtonTypes buttonImageType, Point offset, Color imageColor)
        {
            // Draw arrow inside the button
            using (SolidBrush buttonBrush = new SolidBrush(imageColor))
            {
                float x, y;

                float cx = 7; // Width of the arrow
                float cy = 4; // Height of the arrow
                float horizontalAdjust = 0;

                float horizontalMargin = (bounds.Width - cx) / 2;
                float verticalMargin = (bounds.Height - cy) / 2;

                x = bounds.Left + horizontalMargin + horizontalAdjust;
                y = bounds.Top + verticalMargin;

                switch (buttonImageType)
                {
                    case ButtonTypes.Down:
                        // Down Arrow
                        g.FillRectangle(buttonBrush, x, y, 7, 1);
                        g.FillRectangle(buttonBrush, x + 1, y + 1, 5, 1);
                        g.FillRectangle(buttonBrush, x + 2, y + 2, 3, 1);
                        g.FillRectangle(buttonBrush, x + 3, y + 3, 1, 1);
                        break;

                    case ButtonTypes.Up:
                        // Up Arrow
                        g.FillRectangle(buttonBrush, x + 3, y, 1, 1);
                        g.FillRectangle(buttonBrush, x + 2, y + 1, 3, 1);
                        g.FillRectangle(buttonBrush, x + 1, y + 2, 5, 1);
                        g.FillRectangle(buttonBrush, x, y + 3, 7, 1);
                        break;
                }
            }
        }

        /// <summary></summary>
        /// <param name="g"/>
        /// <param name="bounds"/>
        /// <param name="buttonImageType"/>
        /// <param name="offset"/>
        /// <param name="imageColor"/>
        public static void DrawComboXPDownButton(Graphics g, RectangleF bounds, ButtonTypes buttonImageType, Point offset, Color imageColor)
        {
            // Draw arrow inside the button
            using (SolidBrush buttonBrush = new SolidBrush(imageColor))
            {
                float x, y;

                float cx = 9; // Width of the arrow
                float cy = 6; // Height of the arrow
                float horizontalAdjust = 0;

                float horizontalMargin = (bounds.Width - cx) / 2;
                float verticalMargin = (bounds.Height - cy) / 2;

                x = bounds.Left + horizontalMargin + horizontalAdjust;
                y = bounds.Top + verticalMargin;

                switch (buttonImageType)
                {
                    case ButtonTypes.ComboXPDown:
                        // Down Arrow
                        g.FillRectangle(buttonBrush, x + 1, y, 1, 1);
                        g.FillRectangle(buttonBrush, x + 7, y, 1, 1);
                        g.FillRectangle(buttonBrush, x, y + 1, 3, 1);
                        g.FillRectangle(buttonBrush, x + 6, y + 1, 3, 1);
                        g.FillRectangle(buttonBrush, x + 1, y + 2, 3, 1);
                        g.FillRectangle(buttonBrush, x + 5, y + 2, 3, 1);
                        g.FillRectangle(buttonBrush, x + 2, y + 3, 5, 1);
                        g.FillRectangle(buttonBrush, x + 3, y + 4, 3, 1);
                        g.FillRectangle(buttonBrush, x + 4, y + 5, 1, 1);
                        break;
                }
            }
        }

        #endregion

        #region Class utility methods
        /// <summary></summary>
        /// <returns></returns>
        /// <param name="alignThis"/>
        /// <param name="withinThis"/>
        /// <param name="align"/>
        internal static Rectangle VAlignWithin(Size alignThis, Rectangle withinThis, ContentAlignment align)
        {
            if ((align & anyBottom) != ((ContentAlignment)0))
            {
                withinThis.Y += withinThis.Height - alignThis.Height;
            }
            else if ((align & anyMiddle) != ((ContentAlignment)0))
            {
                // Precedence rule is significant for alignment
                withinThis.Y += (withinThis.Height + alignThis.Height) / 2 - alignThis.Height;
            }

            withinThis.Height = alignThis.Height;

            return withinThis;
        }

        /// <summary></summary>
        /// <returns></returns>
        /// <param name="alignThis"/>
        /// <param name="withinThis"/>
        /// <param name="align"/>
        internal static Rectangle HAlignWithin(Size alignThis, Rectangle withinThis, ContentAlignment align)
        {
            if ((align & anyRight) != ((ContentAlignment)0))
            {
                withinThis.X += withinThis.Width - alignThis.Width;
            }
            else if ((align & anyCenter) != ((ContentAlignment)0))
            {
                withinThis.X += (withinThis.Width - alignThis.Width) / 2;
            }

            withinThis.Width = alignThis.Width;

            return withinThis;
        }

        /// <summary></summary>
        /// <returns></returns>
        /// <param name="align"/>
        internal static TextFormatFlags TranslateAlignment(ContentAlignment align)
        {
            if ((align & anyRight) != ((ContentAlignment)0))
            {
                return TextFormatFlags.Right;
            }
            else if ((align & anyCenter) != ((ContentAlignment)0))
            {
                return TextFormatFlags.HorizontalCenter;
            }

            return TextFormatFlags.Left;
        }

        /// <summary></summary>
        /// <returns></returns>
        /// <param name="align"/>
        internal static TextFormatFlags TranslateLineAlignment(ContentAlignment align)
        {
            if ((align & anyBottom) != ((ContentAlignment)0))
            {
                return TextFormatFlags.Bottom;
            }
            else if ((align & anyMiddle) != ((ContentAlignment)0))
            {
                return TextFormatFlags.VerticalCenter;
            }

            return TextFormatFlags.Top;
        }

        #endregion
    }

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
	[Flags]
	internal enum TextFormatFlags
	{
		/// <summary>Aligns the text on the bottom of the bounding rectangle. Applied only when the text is a single line.</summary>
		Bottom = 8,
		/// <summary>Applies the default formatting, which is left-aligned.</summary>
		Default = 0,
		/// <summary>Removes the end of trimmed lines, and replaces them with an ellipsis.</summary>
		EndEllipsis = 0x8000,
		/// <summary>Expands tab characters. The default number of characters per tab is eight. The <see cref="F:System.Windows.Forms.TextFormatFlags.WordEllipsis"></see>, <see cref="F:System.Windows.Forms.TextFormatFlags.PathEllipsis"></see>, and <see cref="F:System.Windows.Forms.TextFormatFlags.EndEllipsis"></see> values cannot be used with <see cref="F:System.Windows.Forms.TextFormatFlags.ExpandTabs"></see>.</summary>
		ExpandTabs = 0x40,
		/// <summary>Includes the font external leading in line height. Typically, external leading is not included in the height of a line of text.</summary>
		ExternalLeading = 0x200,
		/// <summary>Adds padding to the bounding rectangle to accommodate overhanging glyphs. </summary>
		GlyphOverhangPadding = 0,
		/// <summary>Applies to Windows 2000 and Windows XP only: </summary>
		HidePrefix = 0x100000,
		/// <summary>Centers the text horizontally within the bounding rectangle.</summary>
		HorizontalCenter = 1,
		/// <summary>Uses the system font to calculate text metrics.</summary>
		Internal = 0x1000,
		/// <summary>Aligns the text on the left side of the clipping area.</summary>
		Left = 0,
		/// <summary>Adds padding to both sides of the bounding rectangle.</summary>
		LeftAndRightPadding = 0x20000000,
		/// <summary>Modifies the specified string to match the displayed text. This value has no effect unless <see cref="F:System.Windows.Forms.TextFormatFlags.EndEllipsis"></see> or <see cref="F:System.Windows.Forms.TextFormatFlags.PathEllipsis"></see> is also specified.</summary>
		ModifyString = 0x10000,
		/// <summary>Allows the overhanging parts of glyphs and unwrapped text reaching outside the formatting rectangle to show.</summary>
		NoClipping = 0x100,
		/// <summary>Applies to Windows 98, Windows Me, Windows 2000, or Windows XP only:</summary>
		NoFullWidthCharacterBreak = 0x80000,
		/// <summary>Does not add padding to the bounding rectangle.</summary>
		NoPadding = 0x10000000,
		/// <summary>Turns off processing of prefix characters. Typically, the ampersand (&amp;) mnemonic-prefix character is interpreted as a directive to underscore the character that follows, and the double-ampersand (&amp;&amp;) mnemonic-prefix characters as a directive to print a single ampersand. By specifying <see cref="F:System.Windows.Forms.TextFormatFlags.NoPrefix"></see>, this processing is turned off. For example, an input string of "A&amp;bc&amp;&amp;d" with <see cref="F:System.Windows.Forms.TextFormatFlags.NoPrefix"></see> applied would result in output of "A&amp;bc&amp;&amp;d".</summary>
		NoPrefix = 0x800,
		/// <summary>Removes the center of trimmed lines and replaces it with an ellipsis. </summary>
		PathEllipsis = 0x4000,
		/// <summary>Applies to Windows 2000 or Windows XP only: </summary>
		PrefixOnly = 0x200000,
		/// <summary>Preserves the clipping specified by a <see cref="T:System.Drawing.Graphics"></see> object. Applies only to methods receiving an <see cref="T:System.Drawing.IDeviceContext"></see> that is a <see cref="T:System.Drawing.Graphics"></see>.</summary>
		PreserveGraphicsClipping = 0x1000000,
		/// <summary>Preserves the transformation specified by a <see cref="T:System.Drawing.Graphics"></see>. Applies only to methods receiving an <see cref="T:System.Drawing.IDeviceContext"></see> that is a <see cref="T:System.Drawing.Graphics"></see>.</summary>
		PreserveGraphicsTranslateTransform = 0x2000000,
		/// <summary>Aligns the text on the right side of the clipping area.</summary>
		Right = 2,
		/// <summary>Displays the text from right to left.</summary>
		RightToLeft = 0x20000,
		/// <summary>Displays the text in a single line.</summary>
		SingleLine = 0x20,
		/// <summary>Specifies the text should be formatted for display on a <see cref="T:System.Windows.Forms.TextBox"></see> control.</summary>
		TextBoxControl = 0x2000,
		/// <summary>Aligns the text on the top of the bounding rectangle.</summary>
		Top = 0,
		/// <summary>Centers the text vertically, within the bounding rectangle.</summary>
		VerticalCenter = 4,
		/// <summary>Breaks the text at the end of a word.</summary>
		WordBreak = 0x10,
		/// <summary>Trims the line to the nearest word and an ellipsis is placed at the end of a trimmed line.</summary>
		WordEllipsis = 0x40000
	}

	internal class TextRenderer
	{
    #region Constatnts
		const int MAX_SIZE = 0x7fffffff;

		const int DT_VCENTER = 0x00000004;
		const int DT_BOTTOM = 0x00000008;
		const int DT_SINGLELINE = 0x00000020;
		const int DT_CALCRECT = 0x00000400;
		const int DT_WORDBREAK = 0x00000010;

		const int VERTICAL_ADJUST = DT_VCENTER | DT_BOTTOM;
    #endregion

    #region Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dc"></param>
		/// <param name="text"></param>
		/// <param name="font"></param>
		/// <param name="bounds"></param>
		/// <param name="foreColor"></param>
		/// <param name="flags"></param>
		public static void DrawText(Graphics dc, string text, Font font, Rectangle bounds, Color foreColor, TextFormatFlags flags)
		{
			IntPtr hdc = dc.GetHdc();
            try
            {
			if (font != null)
			{
				IntPtr hFont = font.ToHfont();
				if (hFont != IntPtr.Zero)
				{
					IntPtr hObj = NativeMethods.SelectObject(hdc, hFont);

					int bkMode = NativeMethods.SetBkMode(hdc, 1);
					int color = NativeMethods.SetTextColor(hdc, ColorTranslator.ToWin32(foreColor));

					DRAWTEXTPARAMS dtparams = GetDrawParams(flags, font);

					bounds = AdjustForVerticalAlignment(hdc, text, bounds, (int)flags, dtparams);
					if (bounds.Width == MAX_SIZE)
					{
						bounds.Width -= bounds.X;
					}
					if (bounds.Height == MAX_SIZE)
					{
						bounds.Height -= bounds.Y;
					}

					NativeMethods.RECT rect = new NativeMethods.RECT(bounds);
					DrawTextEx(hdc, text, text.Length, ref rect, (int)flags, dtparams);

					NativeMethods.SetTextColor(hdc, color);
					NativeMethods.SetBkMode(hdc, bkMode);

					NativeMethods.SelectObject(hdc, hObj);
					NativeMethods.DeleteObject(hFont);
				}
			}
            }
            finally
            {
			    dc.ReleaseHdc( hdc );
            }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		/// <param name="font"></param>
		/// <param name="proposedSize"></param>
		/// <returns></returns>
		public static Size MeasureText(string text, Font font, Size proposedSize, TextFormatFlags flags)
		{
			Size szText = Size.Empty;

			IntPtr hdc = NativeMethods.CreateCompatibleDC(IntPtr.Zero);
			if (hdc != IntPtr.Zero)
			{
				IntPtr hFont = font.ToHfont();
				if (hFont != IntPtr.Zero)
				{
					IntPtr hObj = NativeMethods.SelectObject(hdc, hFont);

					DRAWTEXTPARAMS dtparams = GetDrawParams(TextFormatFlags.Default, font);

					int minWidth = dtparams.iLeftMargin + dtparams.iRightMargin + 1;

					if (proposedSize.Width <= minWidth)
					{
						proposedSize.Width = minWidth;
					}
					if (proposedSize.Height <= 0)
					{
						proposedSize.Height = 1;
					}

					NativeMethods.RECT rect = new NativeMethods.RECT(0, 0, proposedSize.Width, proposedSize.Height);
					DrawTextEx(hdc, text, text.Length, ref rect, DT_CALCRECT | ((int)flags & DT_WORDBREAK), dtparams);

					szText.Width = rect.Width;
					szText.Height = rect.Height;

					NativeMethods.SelectObject(hdc, hObj);
					NativeMethods.DeleteObject(hFont);
				}

				NativeMethods.DeleteDC(hdc);
			}

			return szText;
		}
    #endregion

    #region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="flags"></param>
		/// <returns></returns>
		static DRAWTEXTPARAMS GetDrawParams(TextFormatFlags flags, Font font)
		{
			int leftMargin = 0;
			int rightMargin = 0;

			if (font != null)
			{
				if ((flags & TextFormatFlags.NoPadding) == TextFormatFlags.GlyphOverhangPadding)
				{
					float overhangPadding = (float)font.Height / 6f;

					if ((flags & TextFormatFlags.LeftAndRightPadding) != TextFormatFlags.GlyphOverhangPadding)
					{
						leftMargin = (int)Math.Ceiling((double)overhangPadding);
						rightMargin = (int)Math.Ceiling((double)(overhangPadding * 1.5f));
					}
					else
					{
						leftMargin = (int)Math.Ceiling((double)(overhangPadding * 2f));
						rightMargin = (int)Math.Ceiling((double)(overhangPadding * 2.5f));
					}
				}
			}
			return new DRAWTEXTPARAMS(leftMargin, rightMargin);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hdc"></param>
		/// <param name="text"></param>
		/// <param name="bounds"></param>
		/// <param name="flags"></param>
		/// <param name="dtparams"></param>
		/// <returns></returns>
		static Rectangle AdjustForVerticalAlignment(IntPtr hdc, string text, Rectangle bounds, int flags, DRAWTEXTPARAMS dtparams)
		{
			Rectangle rc = bounds;

			if (!((((flags & VERTICAL_ADJUST) == 0) || ((flags & DT_SINGLELINE) != 0)) || ((flags & DT_CALCRECT) != 0)))
			{
				NativeMethods.RECT rect = new NativeMethods.RECT(bounds);

				flags |= DT_CALCRECT;

				int height = DrawTextEx(hdc, text, text.Length, ref rect, flags, dtparams);
				if (height <= bounds.Height)
				{
					if ((flags & DT_VCENTER) != 0)
					{
						rc.Y = rc.Y + rc.Height / 2 - height / 2;
					}
					else
					{
						rc.Y = rc.Bottom - height;
					}
				}
			}
			return rc;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="hdc"></param>
		/// <param name="lpString"></param>
		/// <param name="nCount"></param>
		/// <param name="lpRect"></param>
		/// <param name="uFormat"></param>
		/// <param name="lpDTParams"></param>
		/// <returns></returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		extern static int DrawTextEx(IntPtr hdc, string lpString, int nCount, ref NativeMethods.RECT lpRect, int flags, [In, Out] DRAWTEXTPARAMS lpDTParams);
    #endregion

    #region *** DRAWTEXTPARAMS
		[StructLayout(LayoutKind.Sequential)]
		private class DRAWTEXTPARAMS
		{
			private int cbSize;
			public int iTabLength;
			public int iLeftMargin;
			public int iRightMargin;
			public int uiLengthDrawn;

			public DRAWTEXTPARAMS(int leftMargin, int rightMargin)
			{
				this.cbSize = Marshal.SizeOf(typeof(DRAWTEXTPARAMS));
				this.iLeftMargin = leftMargin;
				this.iRightMargin = rightMargin;
				this.iTabLength = 0;
				this.uiLengthDrawn = 0;
			}
		}
    #endregion
	}
#endif
}