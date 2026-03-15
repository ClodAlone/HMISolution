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

#if !SILVERLIGHT && !NETFX_CORE && !WP

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Native;


namespace Syncfusion.Pdf.Graphics.Images
{
    /// <summary>
    /// Summary description for RtfToWmf.
    /// </summary>
    internal class RtfToImage
    {
#region Constants
        /// <summary>
        /// Message code.
        /// </summary>
        private const int EM_FORMATRANGE = 1081; // 0x0439
        /// <summary>
        /// Message code.
        /// </summary>
        private const int EM_DISPLAYBAND = 1075; // 0x0433
        /// <summary>
        /// Message code.
        /// </summary>
        private const int EM_SETEDITSTYLE = 1228; // 0x04cc
        /// <summary>
        /// Message code.
        /// </summary>
        private const int SES_EXTENDBACKCOLOR = 4; // 0x0004
        /// <summary>
        /// Type object of this class.
        /// </summary>
        private static readonly Type _type = typeof(RtfToImage);
        #endregion

#region Fields
        /// <summary>
        /// Rectangle needed for content displaying inside of rich text paint.
        /// </summary>
        private static System.Drawing.Rectangle s_virtualRect;
        #endregion

#region Constructors
        /// <summary>
        /// Creates new object.
        /// </summary>
        private RtfToImage()
        {
        }
        #endregion

#region Public methods
        /// <summary>
        /// Converts RTF text to Wmf  metafile.
        /// </summary>
        /// <param name="text">RTF text.</param>
        /// <param name="width">Width of the text.</param>
        /// <param name="height">Height of the text. May be -1.</param>
        /// <param name="type">Type of the image.</param>
        /// <returns>Image created from RTF.</returns>
        public static Image ConvertToImage(string text, float width, float height, PdfImageType type)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            Image img;

            lock (_type)
            {
                width = (float)Math.Ceiling(width);
                height = (float)Math.Ceiling(height);
                s_virtualRect = System.Drawing.Rectangle.Empty;
                RichTextBox richTextBox = new RichTextBox();
                richTextBox.AutoSize = true;
                richTextBox.AcceptsTab = true;
                richTextBox.DetectUrls = false;
                richTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                richTextBox.BackColor = Color.FromArgb(255, Color.White);
                richTextBox.Rtf = string.Empty;
                richTextBox.ScrollBars = RichTextBoxScrollBars.None;
                richTextBox.ContentsResized += new ContentsResizedEventHandler(ContentsResized);

                richTextBox.Bounds = new System.Drawing.Rectangle(0, 0, (int)width, 1);

                SetText(richTextBox, text);

                // NOTE: It's hack due to unexpected RichTextBox behavior.
                if (s_virtualRect.IsEmpty)
                {
                    SetText(richTextBox, string.Empty);
                    SetText(richTextBox, text);
                }

                // Check if height or width are zero and increase to 1 so that following calls won't fail.		
                width = (width == 0) ? 1 : width;
                height = (height <= 0) ? s_virtualRect.Height : height;

                // Set the RichTextBox edit style for extending the background fill mode.			
                GdiApi.SendMessage(richTextBox.Handle, EM_SETEDITSTYLE,
                    new IntPtr(SES_EXTENDBACKCOLOR), new IntPtr(SES_EXTENDBACKCOLOR));

                // paint RichTextBox content to MetaFile
                img = ToImage(richTextBox, width, height, type);
                richTextBox.Dispose();
            }

            return img;
        }
        #endregion

#region Implementation
        /// <summary>
        /// Retrieves data from RichTextBox control.
        /// </summary>
        /// <param name="richTextBox">RichTectBox control instance.</param>
        /// <param name="width">Width of the output.</param>
        /// <param name="height">Height of the output.</param>
        /// <param name="type">Type of the image.</param>
        /// <returns>Image created from RTF.</returns>
        private static Image ToImage(RichTextBox richTextBox, float width, float height, PdfImageType type)
        {
            if (richTextBox == null)
                throw new ArgumentNullException("richTextBox");

            Image image = null;

            if (type == PdfImageType.Metafile)
            {
                image = ConvertToMetafile(richTextBox, width, height);
            }
            else
            {
                image = ConvertToBitmap(richTextBox, width, height);
            }

            return image;
        }

        /// <summary>
        /// Converts rtf to metafile.
        /// </summary>
        /// <param name="richTextBox">RichTextBox control.</param>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Height of the image.</param>
        /// <returns>Image from RTF.</returns>
        private static Image ConvertToMetafile(RichTextBox richTextBox, float width, float height)
        {
            if (richTextBox == null)
                throw new ArgumentNullException("richTextBox");

            MemoryStream ms = new MemoryStream();
            IntPtr refHdc = GdiApi.CreateDC("DISPLAY", null, null, IntPtr.Zero);

            RectangleF rect = new RectangleF(0, 0, width, height);
            Metafile metafile = new Metafile(ms, refHdc, rect, MetafileFrameUnit.Pixel, EmfType.EmfOnly);

            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(metafile);
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            IntPtr hdc = g.GetHdc();
            DrawRtf(richTextBox, hdc, rect);
            g.ReleaseHdc(hdc);

            g.Dispose();
            GdiApi.DeleteDC(refHdc);

            // NOTE: Just for debug.
            //using( FileStream fs = new FileStream( "C:\\Temp\\test.emf", FileMode.Create,
            //  FileAccess.Write, FileShare.Read ) )
            //{
            //  ms.WriteTo( fs );
            //}

            ms.Close();

            return metafile;
        }

        /// <summary>
        /// Converts rtf to metafile.
        /// </summary>
        /// <param name="richTextBox">HtmlRichTextBox control.</param>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Height of the image.</param>
        /// <returns>Image from RTF.</returns>
        internal static Image ConvertToMetafile(RichTextBoxExt richTextBox, float width, float height)
        {
            if (richTextBox == null)
                throw new ArgumentNullException("richTextBox");

            MemoryStream ms = new MemoryStream();
            IntPtr refHdc = GdiApi.CreateDC("DISPLAY", null, null, IntPtr.Zero);

            richTextBox.RightMargin = (int)width - 5;

            RectangleF rect = new RectangleF(0, 0, width, height);
            Metafile metafile = new Metafile(ms, refHdc, rect, MetafileFrameUnit.Pixel, EmfType.EmfOnly);

            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(metafile);
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            IntPtr hdc = g.GetHdc();
            DrawRtf(richTextBox, hdc, rect);
            g.ReleaseHdc(hdc);

            g.Dispose();
            GdiApi.DeleteDC(refHdc);

            // NOTE: Just for debug.
            //using (FileStream fs = new FileStream("C:\\Temp\\test.emf", FileMode.Create,
            //  FileAccess.Write, FileShare.Read))
            //{
            //    ms.WriteTo(fs);
            //}

            ms.Close();

            return metafile;
        }

        /// <summary>
        /// Converts rtf to bitmap.
        /// </summary>
        /// <param name="richTextBox">RichTextBox control.</param>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Height of the image.</param>
        /// <returns>Image from RTF.</returns>
        private static Image ConvertToBitmap(RichTextBox richTextBox, float width, float height)
        {
            if (richTextBox == null)
                throw new ArgumentNullException("richTextBox");

            RectangleF rect = new RectangleF(0, 0, width, height);
            Bitmap bitmap = new Bitmap((int)width, (int)height);

            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            IntPtr hdc = g.GetHdc();

            DrawRtf(richTextBox, hdc, rect);

            g.ReleaseHdc(hdc);
            g.Dispose();

            return bitmap;
        }

        /// <summary>
        /// Draws rtf on the graphics context.
        /// </summary>
        /// <param name="richTextBox">RichTextBox control.</param>
        /// <param name="hdc">Graphics context.</param>
        /// <param name="rect">Bounds of the image.</param>
        private static void DrawRtf(RichTextBox richTextBox, IntPtr hdc, RectangleF rect)
        {
            if (richTextBox == null)
                throw new ArgumentNullException("richTextBox");

            CHARRANGE cr;
            cr.cpMin = 0;
            cr.cpMax = richTextBox.TextLength;

            float dpix = GdiApi.GetDeviceCaps(hdc, 88 /*LOGPIXELSX*/ );
            float dpiy = GdiApi.GetDeviceCaps(hdc, 90 /*LOGPIXELSY*/ );
            RectangleF rctextbox = ConvertPixelsToInches(rect, dpix, dpiy);

            RECT rc;	// margins
            rc.left = ConvertInchesToTwips(rctextbox.Left);
            rc.top = ConvertInchesToTwips(rctextbox.Top);
            rc.right = ConvertInchesToTwips(rctextbox.Width);
            rc.bottom = ConvertInchesToTwips(rctextbox.Height);

            FORMATRANGE fr = new FORMATRANGE();
            fr.chrg = cr;
            fr.hdc = hdc;
            fr.hdcTarget = hdc;
            fr.rc = rc;
            fr.rcPage = rc;

            // Flush out any information that might be cached by the rich edit control.
            IntPtr wpar = new IntPtr(0);
            IntPtr lpar = new IntPtr(0);
            IntPtr res = GdiApi.SendMessage(richTextBox.Handle,
                EM_FORMATRANGE, wpar, lpar);

            lpar = Marshal.AllocCoTaskMem(Marshal.SizeOf(fr));
            Marshal.StructureToPtr(fr, lpar, false);
            res = GdiApi.SendMessage(richTextBox.Handle, EM_FORMATRANGE, wpar, lpar);
            Marshal.FreeCoTaskMem(lpar);

            RECT rcdisplay = new RECT(rc.left, rc.top, rc.right, rc.bottom);
            lpar = Marshal.AllocCoTaskMem(Marshal.SizeOf(rcdisplay));
            Marshal.StructureToPtr(rcdisplay, lpar, false);
            GdiApi.SendMessage(richTextBox.Handle, EM_DISPLAYBAND, new IntPtr(0), lpar);
            Marshal.FreeCoTaskMem(lpar);

            // Remove the rich edit control information cache.
            wpar = new IntPtr(0);
            lpar = new IntPtr(0);
            res = GdiApi.SendMessage(richTextBox.Handle, EM_FORMATRANGE, wpar, lpar);
        }

        /// <summary>
        /// Convert between inches and twips (1/1440 inch, used by Win32 API calls).
        /// </summary>
        /// <param name="n">Value in inches.</param>
        /// <returns>Value in twips.</returns>
        private static int ConvertInchesToTwips(int n)
        {
            return (int)((float)n * 1440);
        }

        /// <summary>
        /// Convert between inches and twips (1/1440 inch, used by Win32 API calls).
        /// </summary>
        /// <param name="f">Value in inches.</param>
        /// <returns>Value in twips.</returns>
        private static int ConvertInchesToTwips(float f)
        {
            return (int)(f * 1440f);
        }

        /// <summary>
        /// Convert between pixels and inches.
        /// </summary>
        /// <param name="rcpixels">Value in pixels.</param>
        /// <param name="dpix">Horizontal device resolution.</param>
        /// <param name="dpiy">Vertical device resolution.</param>
        /// <returns>Value in inches..</returns>
        private static RectangleF ConvertPixelsToInches(RectangleF rcpixels, float dpix, float dpiy)
        {
            float x = rcpixels.X / dpix;
            float y = rcpixels.Y / dpiy;
            float width = rcpixels.Width / dpix;
            float height = rcpixels.Height / dpiy;

            return new System.Drawing.RectangleF(x, y, width, height);
        }

        /// <summary>
        /// Determines whether [is valid RTF] [the specified RTF].
        /// </summary>
        /// <param name="rtf">The RTF.</param>
        /// <returns>
        /// 	if it is valid RTF, set to <c>true</c>.
        /// </returns>
        /// <internalonly/>
        public static bool IsValidRtf(string rtf)
        {
            rtf = rtf.Trim(new char[] { '\0', '\r', '\n', ' ', '\t' });
            return rtf.StartsWith(@"{\rtf") && rtf.EndsWith("}");
        }

        /// <summary>
        /// Sets RTF text box.
        /// </summary>
        /// <param name="richTextBox">Rich text box control.</param>
        /// <param name="text">Rtf text.</param>
        private static void SetText(RichTextBox richTextBox, string text)
        {
            if (richTextBox == null)
                throw new ArgumentNullException("richTextBox");
            if (text == null)
                throw new ArgumentNullException("text");

            if (IsValidRtf(text))
            {
                richTextBox.Rtf = text;
            }
            else
            {
                richTextBox.Text = text;
            }
        }
        #endregion

#region Event handlers
        /// <summary>
        /// Handles contentsResized event.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event parameters.</param>
        private static void ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            lock (_type)
            {
                s_virtualRect = e.NewRectangle;
            }
        }
        #endregion
    }
}
#endif