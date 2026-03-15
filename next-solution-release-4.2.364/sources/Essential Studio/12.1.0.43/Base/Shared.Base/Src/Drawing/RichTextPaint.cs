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

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.IO;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;
using Syncfusion.ComponentModel;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Drawing
{
	
	/// <internalonly/>
	[Syncfusion.Documentation.DocumentationExclude]
	public class RichTextPaint 
	{
		static int FormatRange(RichTextBox richTextControl, NativeMethods.FORMATRANGE fr, bool display) 
		{
			return (int) NativeMethods.SendMessage(richTextControl.Handle, RichTextBoxConstants.EM_FORMATRANGE,
				display,
				fr);
		}

		static int DisplayBand(RichTextBox richTextControl, NativeMethods.RECT rc) 
		{
			return (int) NativeMethods.SendMessage(richTextControl.Handle, RichTextBoxConstants.EM_DISPLAYBAND,
				IntPtr.Zero,
				ref rc);
		}

		/// <internalonly/>
		public static void PaintStatic(Graphics g, RichTextBox richTextControl, bool printing, Rectangle pageBounds, Rectangle targetBounds, Rectangle clipRect, Color backColor, int zoom)
		{
			RectangleF clipBounds = g.ClipBounds;
			bool clipped = true;  //changed for defect 11018
			//bool clipped = false;
            		//if ((int) clipBounds.Top > targetBounds.Top || clipBounds.Left > targetBounds.Left
            		//     || clipBounds.Bottom < targetBounds.Bottom || clipBounds.Right < targetBounds.Right)
            		//{
			//	clipped = true;
			//}
			clipBounds.Intersect(targetBounds);
			clipBounds.Intersect(pageBounds);
			if (!clipRect.IsEmpty)
				clipBounds.Intersect(clipRect);
			IntPtr hdc = g.GetHdc();

			int bkclr = NativeMethods.RGBToCOLORREF(backColor.ToArgb());

			int logPixels1 = NativeMethods.GetDeviceCaps(hdc, 88);

			float f;
			float f2;

			if (printing)
			{
				IntPtr hdc2 = NativeMethods.GetDC(IntPtr.Zero);
				int logPixels2 = NativeMethods.GetDeviceCaps(hdc2, 88);
				NativeMethods.ReleaseDC(IntPtr.Zero, hdc2);

				NativeMethods.SIZE windowExt;
				NativeMethods.SIZE viewportExt;
				const int MM_ANISOTROPIC = 8; // 0x0008 

				// Just in case there is non-standard DPI setting for screen
				// we need to zoom the rtf drawing.
				NativeMethods.SetMapMode(hdc, MM_ANISOTROPIC);
				NativeMethods.SetWindowExtEx(hdc, 96, 96, out windowExt);
				NativeMethods.SetViewportExtEx(hdc, logPixels2, logPixels2, out viewportExt);
                if (logPixels1 == 96)
                {
                    f2 = 1.0f;
                    f = 1440f / logPixels1;
                }
                else
                {
                    f = 14.4f * 96 / logPixels2;
                    //f2 = 3.0f;//logPixels1*14.4f/logPixels2;
                    f2 = logPixels1 / 100f * 96 / logPixels2;// *14.4f / logPixels2;
                }
            }
            else
            {
                f2 = 1.0f;
                f = 1440f / logPixels1;
            }


			NativeMethods.RECT rcPage = new NativeMethods.RECT(pageBounds);
			NativeMethods.RECT rc = new NativeMethods.RECT(targetBounds);
			NativeMethods.RECT rcClip = new NativeMethods.RECT(Rectangle.Ceiling(clipBounds));
			NativeMethods.RECT rcClipPage = new NativeMethods.RECT(Rectangle.Ceiling(clipBounds));
			NativeMethods.RECT rcClipBox = new NativeMethods.RECT();

			rcPage.left = (int) (rcPage.left * f);
			rcPage.top = (int) (rcPage.top *f);
			rcPage.right = (int) (rcPage.right *f);
			rcPage.bottom = (int) (rcPage.bottom *f);

			rc.left = (int) (rc.left * f);
			rc.top = (int) (rc.top *f);
            if (richTextControl.RightToLeft == RightToLeft.Yes)
            {
                rc.right = (int)((rc.right - (2 * f2)) * f);
            }
            else
            {
                rc.right = (int)(rc.right * f);
            }
			rc.bottom = (int) (rc.bottom *f);

			rcClipPage.left = (int) (rcClipPage.left * f);
			rcClipPage.top = (int) (rcClipPage.top *f);
			rcClipPage.right = (int) ((rcClipPage.right) *f);
			rcClipPage.bottom = (int) ((rcClipPage.bottom) *f);

			rcClip.left = (int) (rcClip.left * f2);
			rcClip.top = (int) (rcClip.top *f2);
			rcClip.right = (int) ((rcClip.right) *f2);
			rcClip.bottom = (int) ((rcClip.bottom) *f2);

			NativeMethods.FORMATRANGE fr = new NativeMethods.FORMATRANGE();
			fr.rcPage = rcPage;
			fr.rc = rc;
			
			// Print as much text as can fit in the cell.
			NativeMethods.CHARRANGE chrg = new NativeMethods.CHARRANGE();
			chrg.cpMin = 0;
			chrg.cpMax = -1;
			fr.chrg = chrg;

			fr.hdcTarget = hdc;
			fr.hdc = hdc;

			if (clipped)
			{
				NativeMethods.GetClipBox(hdc, ref rcClipBox);
				NativeMethods.IntersectClipRect(hdc, rcClip.left, rcClip.top, rcClip.right, rcClip.bottom);
			}


			// Now, draw the Rtf cell

			// ZoomFactor does not work
			//			this.ZoomFactor = zoom/100f;
			//			if (printing)
			//				this.ZoomFactor *= 10*logPixelsX/96f;

			// This fixes back color when drawing to screen.
			richTextControl.BackColor = Color.FromArgb(255, backColor);

			// Tried the following but did not have any effect on back color when printing.
			// When printing it will always stay white ...
			//
			//			NativeMethods.SetBkColor(hdc, bkclr);
			//			IntPtr hBrush = NativeMethods.CreateSolidBrush(bkclr);
			//			IntPtr hSavedBrush = NativeMethods.SelectObject(hdc, hBrush);
			//			NativeMethods.FillRect(hdc, ref rcClip, hBrush);
			/* Background Modes */
			//#define TRANSPARENT         1
			//#define OPAQUE              2
			//#define BKMODE_LAST         2
			//NativeMethods.SetBkMode(hdc, 2);

			FormatRange(richTextControl, null, false); // required by RichEdit to clear out cache
			FormatRange(richTextControl, fr, false);
			DisplayBand(richTextControl, rcClipPage);
			FormatRange(richTextControl, null, false); // required by RichEdit to clear out cache
			
			if (clipped)
			{
				NativeMethods.SelectClipRgn(hdc, IntPtr.Zero);
				NativeMethods.IntersectClipRect(hdc, rcClipBox.left, rcClipBox.top, rcClipBox.right, rcClipBox.bottom);
			}

			//NativeMethods.SelectObject(hdc, hSavedBrush);
			//NativeMethods.DeleteObject(hBrush);

			g.ReleaseHdc(hdc);
		}

		/// <internalonly/>
		public static void DrawRichText(Graphics g, RichTextBox richTextControl, string rtf, bool printing, Rectangle pageBounds, Rectangle targetBounds, Rectangle clipRect, Color backColor, bool wordWrap, int zoom) 
		{
			DrawRichText(g, richTextControl, rtf, printing, pageBounds, targetBounds, clipRect, backColor, wordWrap, zoom, false);
		}

		/// <internalonly/>
		public static void DrawRichText(Graphics g, RichTextBox richTextControl, string rtf, bool printing, Rectangle pageBounds, Rectangle targetBounds, Rectangle clipRect, Color backColor, bool wordWrap, int zoom, bool isRightToLeft) 
		{
			richTextControl.WordWrap = wordWrap;
			richTextControl.RightToLeft = isRightToLeft ? RightToLeft.Yes : RightToLeft.No;

			if (IsValidRtf(rtf))
				richTextControl.Rtf = rtf;
			else
				richTextControl.Text = rtf;

			richTextControl.CreateControl();
			PaintStatic(g, richTextControl, printing, pageBounds, targetBounds, clipRect, backColor, zoom);
		}

		/// <internalonly/>
		public static void DrawRichText(Graphics g, string rtf, bool printing, Rectangle pageBounds, Rectangle targetBounds, Color backColor, Rectangle clipRect, bool wordWrap, int zoom) 
		{
			DrawRichText(g, rtf, printing, pageBounds, targetBounds, backColor, clipRect, wordWrap, zoom, false);
		}

		/// <internalonly/>
		public static void DrawRichText(Graphics g, string rtf, bool printing, Rectangle pageBounds, Rectangle targetBounds, Color backColor, Rectangle clipRect, bool wordWrap, int zoom, bool isRightToLeft) 
		{
			RichTextBox richTextControl = new RichTextBox();
			DrawRichText(g, richTextControl, rtf, printing, pageBounds, targetBounds, clipRect, backColor, wordWrap, zoom, isRightToLeft);
			richTextControl.Dispose();
		}

		/// <internalonly/>
		public static bool IsValidRtf(string rtf)
		{
			rtf = rtf.Trim(new char[] { '\0', '\r', '\n', ' ', '\t' });
			return rtf.StartsWith(@"{\rtf") && rtf.EndsWith("}");
		}

		#region RichTextEMFDrawing
		
		// NOTE: Examine correct type of this constant.
		private const EmfType EMF_TYPE = EmfType.EmfOnly;

		/// <summary>
		/// Convert between inches and twips (1/1440 inch, used by Win32 API calls).
		/// </summary>
		/// <param name="n">Value in inches.</param>
		/// <returns>Value in twips.</returns>
		static private int ConvertInchesToTwips(int n)
		{
			return (int)((float)n*1440);
		}

		static private int ConvertInchesToTwips(float f)
		{
			return (int)(f*1440f);
		}

		static private RectangleF ConvertPixelsToInches(RectangleF rcpixels, float dpix, float dpiy)
		{
			float x = rcpixels.X/dpix;
			float y = rcpixels.Y/dpiy;
			float width = rcpixels.Width/dpix;
			float height = rcpixels.Height/dpiy;
			return new System.Drawing.RectangleF(x, y, width, height);
		}

		public static byte[] DrawRichTextToByteArray(Graphics gphx, string text, float width, float height, Color backcolor)
		{
			byte[] img;

			RichTextBox richTextBox = new RichTextBox();
			richTextBox.AcceptsTab = true;
			richTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			richTextBox.BackColor = Color.FromArgb(255, backcolor);

			richTextBox.Bounds = new System.Drawing.Rectangle(0, 0, (int)width, (int)height);		

			if(IsValidRtf(text))
				richTextBox.Rtf = text;
			else
				richTextBox.Text = text;
			
			// Check if height or width are zero and increase to 1 so that following calls won't fail.		
			width = (width == 0) ? 1 : width;
			height = (height == 0) ? 1 : height;

			// Set the RichTextBox edit style for extending the background fill mode.			
			NativeMethods.SendMessage(richTextBox.Handle, RichTextBoxConstants.EM_SETEDITSTYLE, new IntPtr(RichTextBoxConstants.SES_EXTENDBACKCOLOR), new IntPtr(RichTextBoxConstants.SES_EXTENDBACKCOLOR));

			// Create a Bitmap.
			Bitmap bitmap = new Bitmap(1, 1, gphx);
			// Wrap a Graphics around the Bitmap.
			Graphics rtfGraphics = Graphics.FromImage(bitmap);
			IntPtr refHdc = rtfGraphics.GetHdc();

			// paint RichTextBox content to MetaFile
			img = BytesFromRichText(richTextBox, refHdc, 0, 0, width, height);

			// TODO: remove non-transparent background

			rtfGraphics.ReleaseHdc(refHdc);
			rtfGraphics.Dispose();
			bitmap.Dispose();
			richTextBox.Dispose();

			return img;
		}

		private static byte[] BytesFromRichText(RichTextBox richTextBox, IntPtr refHdc, float left, float top, float width, float height)
		{
			MemoryStream ms = new MemoryStream();
			Metafile mf = new Metafile(ms, refHdc, new RectangleF(0, 0, width, height), MetafileFrameUnit.Pixel, RichTextPaint.EMF_TYPE);

			Graphics g = Graphics.FromImage(mf);
			g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
			IntPtr hdc = g.GetHdc();
			
			NativeMethods.CHARRANGE cr;
			cr.cpMin = 0;
			cr.cpMax = richTextBox.TextLength;
			
			float dpix = NativeMethods.GetDeviceCaps(refHdc, 88 /*LOGPIXELSX*/);
			float dpiy = NativeMethods.GetDeviceCaps(refHdc, 90 /*LOGPIXELSY*/);
			RectangleF rctextbox = RichTextPaint.ConvertPixelsToInches(new RectangleF(left,top,width,height), dpix, dpiy);
			
			NativeMethods.RECT rc;	// margins
			rc.left		= RichTextPaint.ConvertInchesToTwips(rctextbox.Left);
			rc.top		= RichTextPaint.ConvertInchesToTwips(rctextbox.Top);
			rc.right	= RichTextPaint.ConvertInchesToTwips(rctextbox.Width);
			rc.bottom	= RichTextPaint.ConvertInchesToTwips(rctextbox.Height);
			
			NativeMethods.FORMATRANGE fr = new NativeMethods.FORMATRANGE();
			fr.chrg		 = cr;
			fr.hdc		 = hdc;
			fr.hdcTarget = hdc;
			fr.rc		 = rc;
			fr.rcPage	 = rc;

			// Flush out any information that might be cached by the rich edit control.
			IntPtr wpar = new IntPtr(0);
			IntPtr lpar = new IntPtr(0);
			IntPtr res = NativeMethods.SendMessage(richTextBox.Handle, RichTextBoxConstants.EM_FORMATRANGE, wpar, lpar);

			lpar = Marshal.AllocCoTaskMem(Marshal.SizeOf(fr)); 
			Marshal.StructureToPtr(fr, lpar, false);
			res = NativeMethods.SendMessage(richTextBox.Handle, RichTextBoxConstants.EM_FORMATRANGE, wpar, lpar);
			Marshal.FreeCoTaskMem(lpar);
			
			NativeMethods.RECT rcdisplay = new NativeMethods.RECT(rc.left, rc.top, rc.right, rc.bottom);
			lpar = Marshal.AllocCoTaskMem(Marshal.SizeOf(rcdisplay));
			Marshal.StructureToPtr(rcdisplay, lpar, false);
			NativeMethods.SendMessage(richTextBox.Handle, RichTextBoxConstants.EM_DISPLAYBAND, new IntPtr(0), lpar);
			Marshal.FreeCoTaskMem(lpar);

			// Remove the rich edit control information cache.			
			wpar = new IntPtr(0);
			lpar = new IntPtr(0);
			res = NativeMethods.SendMessage(richTextBox.Handle, RichTextBoxConstants.EM_FORMATRANGE, wpar, lpar);

			g.ReleaseHdc(hdc);
			g.Dispose();

			ms.Close();
			mf.Dispose();

			return ms.ToArray();
		}

		#endregion // RichTextEMFDrawing

	}

	class TextBoxConstants
	{
		public const int EM_CANUNDO = 198; // 0x00c6 
		public const int EM_CHARFROMPOS = 215; // 0x00d7 
		public const int EM_EMPTYUNDOBUFFER = 205; // 0x00cd 
		public const int EM_FMTLINES = 200; // 0x00c8 
		public const int EM_GETFIRSTVISIBLELINE = 206; // 0x00ce 
		public const int EM_GETHANDLE = 189; // 0x00bd 
		public const int EM_GETLIMITTEXT = 213; // 0x00d5 
		public const int EM_GETLINE = 196; // 0x00c4 
		public const int EM_GETLINECOUNT = 186; // 0x00ba 
		public const int EM_GETMARGINS = 212; // 0x00d4 
		public const int EM_GETMODIFY = 184; // 0x00b8 
		public const int EM_GETPASSWORDCHAR = 210; // 0x00d2 
		public const int EM_GETRECT = 178; // 0x00b2 
		public const int EM_GETSEL = 176; // 0x00b0 
		public const int EM_GETTHUMB = 190; // 0x00be 
		public const int EM_GETWORDBREAKPROC = 209; // 0x00d1 
		public const int EM_LIMITTEXT = 197; // 0x00c5 
		public const int EM_LINEFROMCHAR = 201; // 0x00c9 
		public const int EM_LINEINDEX = 187; // 0x00bb 
		public const int EM_LINELENGTH = 193; // 0x00c1 
		public const int EM_LINESCROLL = 182; // 0x00b6 
		public const int EM_POSFROMCHAR = 214; // 0x00d6 
		public const int EM_REPLACESEL = 194; // 0x00c2 
		public const int EM_SCROLL = 181; // 0x00b5 
		public const int EM_SCROLLCARET = 183; // 0x00b7 
		public const int EM_SETHANDLE = 188; // 0x00bc 
		public const int EM_SETLIMITTEXT = 197; // 0x00c5 
		public const int EM_SETMARGINS = 211; // 0x00d3 
		public const int EM_SETMODIFY = 185; // 0x00b9 
		public const int EM_SETPASSWORDCHAR = 204; // 0x00cc 
		public const int EM_SETREADONLY = 207; // 0x00cf 
		public const int EM_SETRECT = 179; // 0x00b3 
		public const int EM_SETRECTNP = 180; // 0x00b4 
		public const int EM_SETSEL = 177; // 0x00b1 
		public const int EM_SETTABSTOPS = 203; // 0x00cb 
		public const int EM_SETWORDBREAKPROC = 208; // 0x00d0 
		public const int EM_UNDO = 199; // 0x00c7 
	};

	class RichTextBoxConstants 
	{
		static RichTextBoxConstants()  
		{
			RichTextBoxConstants.RICHEDIT_DLL10 = @"RichEd32.DLL";
			RichTextBoxConstants.RICHEDIT_DLL20 = @"RichEd20.DLL";
			RichTextBoxConstants.RICHEDIT_DLL30 = RichTextBoxConstants.RICHEDIT_DLL20;
			RichTextBoxConstants.RICHEDIT_CLASS10A = @"RICHEDIT";
			RichTextBoxConstants.RICHEDIT_CLASS20A = @"RichEdit20A";
			RichTextBoxConstants.RICHEDIT_CLASS20W = @"RichEdit20W";
			RichTextBoxConstants.RICHEDIT_CLASS30A = RichTextBoxConstants.RICHEDIT_CLASS20A;
			RichTextBoxConstants.RICHEDIT_CLASS30W = RichTextBoxConstants.RICHEDIT_CLASS20W;
			RichTextBoxConstants.DLL_RICHEDIT = RichTextBoxConstants.RICHEDIT_DLL30;
			RichTextBoxConstants.WC_RICHEDITA = RichTextBoxConstants.RICHEDIT_CLASS30A;
			RichTextBoxConstants.WC_RICHEDITW = RichTextBoxConstants.RICHEDIT_CLASS30W;
			RichTextBoxConstants.CF_RTF = @"Rich Text Format";
			RichTextBoxConstants.CF_RTFNOOBJS = @"Rich Text Format Without Objects";
			RichTextBoxConstants.CF_RETEXTOBJ = @"RichEdit Text and Objects";
			RichTextBoxConstants.WCH_EMBEDDING = ' ';
		}

		internal const int cchTextLimitDefault = 32767; // 0x7fff 
		internal readonly static string CF_RETEXTOBJ;
		internal readonly static string CF_RTF;
		internal readonly static string CF_RTFNOOBJS;
		internal const int CFE_ALLCAPS = 128; // 0x0080 
		internal const int CFE_AUTOBACKCOLOR = 67108864; // 0x4000000 
		internal const int CFE_AUTOCOLOR = 1073741824; // 0x40000000 
		internal const int CFE_BOLD = 1; // 0x0001 
		internal const int CFE_DISABLED = 8192; // 0x2000 
		internal const int CFE_EMBOSS = 2048; // 0x0800 
		internal const int CFE_HIDDEN = 256; // 0x0100 
		internal const int CFE_IMPRINT = 4096; // 0x1000 
		internal const int CFE_ITALIC = 2; // 0x0002 
		internal const int CFE_LINK = 32; // 0x0020 
		internal const int CFE_OUTLINE = 512; // 0x0200 
		internal const int CFE_PROTECTED = 16; // 0x0010 
		internal const int CFE_REVISED = 16384; // 0x4000 
		internal const int CFE_SHADOW = 1024; // 0x0400 
		internal const int CFE_SMALLCAPS = 64; // 0x0040 
		internal const int CFE_STRIKEOUT = 8; // 0x0008 
		internal const int CFE_SUBSCRIPT = 65536; // 0x10000 
		internal const int CFE_SUPERSCRIPT = 131072; // 0x20000 
		internal const int CFE_UNDERLINE = 4; // 0x0004 
		internal const int CFM_ALL = -134217665; // 0xf800003f 
		internal const int CFM_ALL2 = -16777217; // 0xfeffffff 
		internal const int CFM_ALLCAPS = 128; // 0x0080 
		internal const int CFM_ANIMATION = 262144; // 0x40000 
		internal const int CFM_BACKCOLOR = 67108864; // 0x4000000 
		internal const int CFM_BOLD = 1; // 0x0001 
		internal const int CFM_CHARSET = 134217728; // 0x8000000 
		internal const int CFM_COLOR = 1073741824; // 0x40000000 
		internal const int CFM_DISABLED = 8192; // 0x2000 
		internal const int CFM_EFFECTS = 1073741887; // 0x4000003f 
		internal const int CFM_EFFECTS2 = 1141080063; // 0x44037fff 
		internal const int CFM_EMBOSS = 2048; // 0x0800 
		internal const int CFM_FACE = 536870912; // 0x20000000 
		internal const int CFM_HIDDEN = 256; // 0x0100 
		internal const int CFM_IMPRINT = 4096; // 0x1000 
		internal const int CFM_ITALIC = 2; // 0x0002 
		internal const int CFM_KERNING = 1048576; // 0x100000 
		internal const int CFM_LCID = 33554432; // 0x2000000 
		internal const int CFM_LINK = 32; // 0x0020 
		internal const int CFM_OFFSET = 268435456; // 0x10000000 
		internal const int CFM_OUTLINE = 512; // 0x0200 
		internal const int CFM_PROTECTED = 16; // 0x0010 
		internal const int CFM_REVAUTHOR = 32768; // 0x8000 
		internal const int CFM_REVISED = 16384; // 0x4000 
		internal const int CFM_SHADOW = 1024; // 0x0400 
		internal const int CFM_SIZE = -2147483648; // 0x80000000 
		internal const int CFM_SMALLCAPS = 64; // 0x0040 
		internal const int CFM_SPACING = 2097152; // 0x200000 
		internal const int CFM_STRIKEOUT = 8; // 0x0008 
		internal const int CFM_STYLE = 524288; // 0x80000 
		internal const int CFM_SUBSCRIPT = 196608; // 0x30000 
		internal const int CFM_SUPERSCRIPT = 196608; // 0x30000 
		internal const int CFM_UNDERLINE = 4; // 0x0004 
		internal const int CFM_UNDERLINETYPE = 8388608; // 0x800000 
		internal const int CFM_WEIGHT = 4194304; // 0x400000 
		internal const int CFU_CF1UNDERLINE = 255; // 0x00ff 
		internal const int CFU_INVERT = 254; // 0x00fe 
		internal const int CFU_UNDERLINE = 1; // 0x0001 
		internal const int CFU_UNDERLINEDOTTED = 4; // 0x0004 
		internal const int CFU_UNDERLINEDOUBLE = 3; // 0x0003 
		internal const int CFU_UNDERLINENONE = 0;
		internal const int CFU_UNDERLINEWORD = 2; // 0x0002 
		internal readonly static string DLL_RICHEDIT;
		internal const int ECO_AUTOHSCROLL = 128; // 0x0080 
		internal const int ECO_AUTOVSCROLL = 64; // 0x0040 
		internal const int ECO_AUTOWORDSELECTION = 1; // 0x0001 
		internal const int ECO_NOHIDESEL = 256; // 0x0100 
		internal const int ECO_READONLY = 2048; // 0x0800 
		internal const int ECO_SAVESEL = 32768; // 0x8000 
		internal const int ECO_SELECTIONBAR = 16777216; // 0x1000000 
		internal const int ECO_VERTICAL = 4194304; // 0x400000 
		internal const int ECO_WANTRETURN = 4096; // 0x1000 
		internal const int ECOOP_AND = 3; // 0x0003 
		internal const int ECOOP_OR = 2; // 0x0002 
		internal const int ECOOP_SET = 1; // 0x0001 
		internal const int ECOOP_XOR = 4; // 0x0004 
		internal const int EM_AUTOURLDETECT = 1115; // 0x045b 
		internal const int EM_CANPASTE = 1074; // 0x0432 
		internal const int EM_CANREDO = 1109; // 0x0455 
		internal const int EM_CHARFROMPOS = 1063; // 0x0427 
		internal const int EM_CONVPOSITION = 1132; // 0x046c 
		internal const int EM_DISPLAYBAND = 1075; // 0x0433 
		internal const int EM_EXGETSEL = 1076; // 0x0434 
		internal const int EM_EXLIMITTEXT = 1077; // 0x0435 
		internal const int EM_EXLINEFROMCHAR = 1078; // 0x0436 
		internal const int EM_EXSETSEL = 1079; // 0x0437 
		internal const int EM_FINDTEXT = 1080; // 0x0438 
		internal const int EM_FINDTEXTEX = 1103; // 0x044f 
		internal const int EM_FINDTEXTEXW = 1148; // 0x047c 
		internal const int EM_FINDTEXTW = 1147; // 0x047b 
		internal const int EM_FINDWORDBREAK = 1100; // 0x044c 
		internal const int EM_FORMATRANGE = 1081; // 0x0439 
		internal const int EM_GETAUTOURLDETECT = 1116; // 0x045c 
		internal const int EM_GETBIDIOPTIONS = 1225; // 0x04c9 
		internal const int EM_GETCHARFORMAT = 1082; // 0x043a 
		internal const int EM_GETEDITSTYLE = 1229; // 0x04cd 
		internal const int EM_GETEVENTMASK = 1083; // 0x043b 
		internal const int EM_GETIMECOLOR = 1129; // 0x0469 
		internal const int EM_GETIMECOMPMODE = 1146; // 0x047a 
		internal const int EM_GETIMEMODEBIAS = 1151; // 0x047f 
		internal const int EM_GETIMEOPTIONS = 1131; // 0x046b 
		internal const int EM_GETLANGOPTIONS = 1145; // 0x0479 
		internal const int EM_GETLIMITTEXT = 1061; // 0x0425 
		internal const int EM_GETOLEINTERFACE = 1084; // 0x043c 
		internal const int EM_GETOPTIONS = 1102; // 0x044e 
		internal const int EM_GETPARAFORMAT = 1085; // 0x043d 
		internal const int EM_GETPUNCTUATION = 1125; // 0x0465 
		internal const int EM_GETREDONAME = 1111; // 0x0457 
		internal const int EM_GETSCROLLPOS = 1245; // 0x04dd 
		internal const int EM_GETSELTEXT = 1086; // 0x043e 
		internal const int EM_GETTEXTEX = 1118; // 0x045e 
		internal const int EM_GETTEXTLENGTHEX = 1119; // 0x045f 
		internal const int EM_GETTEXTMODE = 1114; // 0x045a 
		internal const int EM_GETTEXTRANGE = 1099; // 0x044b 
		internal const int EM_GETTYPOGRAPHYOPTIONS = 1227; // 0x04cb 
		internal const int EM_GETUNDONAME = 1110; // 0x0456 
		internal const int EM_GETWORDBREAKPROCEX = 1104; // 0x0450 
		internal const int EM_GETWORDWRAPMODE = 1127; // 0x0467 
		internal const int EM_GETZOOM = 1248; // 0x04e0 
		internal const int EM_HIDESELECTION = 1087; // 0x043f 
		internal const int EM_OUTLINE = 1244; // 0x04dc 
		internal const int EM_PASTESPECIAL = 1088; // 0x0440 
		internal const int EM_POSFROMCHAR = 1062; // 0x0426 
		internal const int EM_RECONVERSION = 1149; // 0x047d 
		internal const int EM_REDO = 1108; // 0x0454 
		internal const int EM_REQUESTRESIZE = 1089; // 0x0441 
		internal const int EM_SCROLLCARET = 1073; // 0x0431 
		internal const int EM_SELECTIONTYPE = 1090; // 0x0442 
		internal const int EM_SETBIDIOPTIONS = 1224; // 0x04c8 
		internal const int EM_SETBKGNDCOLOR = 1091; // 0x0443 
		internal const int EM_SETCHARFORMAT = 1092; // 0x0444 
		internal const int EM_SETEDITSTYLE = 1228; // 0x04cc 
		internal const int EM_SETEVENTMASK = 1093; // 0x0445 
		internal const int EM_SETFONTSIZE = 1247; // 0x04df 
		internal const int EM_SETIMECOLOR = 1128; // 0x0468 
		internal const int EM_SETIMEMODEBIAS = 1150; // 0x047e 
		internal const int EM_SETIMEOPTIONS = 1130; // 0x046a 
		internal const int EM_SETLANGOPTIONS = 1144; // 0x0478 
		internal const int EM_SETOLECALLBACK = 1094; // 0x0446 
		internal const int EM_SETOPTIONS = 1101; // 0x044d 
		internal const int EM_SETPALETTE = 1117; // 0x045d 
		internal const int EM_SETPARAFORMAT = 1095; // 0x0447 
		internal const int EM_SETPUNCTUATION = 1124; // 0x0464 
		internal const int EM_SETSCROLLPOS = 1246; // 0x04de 
		internal const int EM_SETTARGETDEVICE = 1096; // 0x0448 
		internal const int EM_SETTEXTMODE = 1113; // 0x0459 
		internal const int EM_SETTYPOGRAPHYOPTIONS = 1226; // 0x04ca 
		internal const int EM_SETUNDOLIMIT = 1106; // 0x0452 
		internal const int EM_SETWORDBREAKPROCEX = 1105; // 0x0451 
		internal const int EM_SETWORDWRAPMODE = 1126; // 0x0466 
		internal const int EM_SETZOOM = 1249; // 0x04e1 
		internal const int EM_STOPGROUPTYPING = 1112; // 0x0458 
		internal const int EM_STREAMIN = 1097; // 0x0449 
		internal const int EM_STREAMOUT = 1098; // 0x044a 
		internal const int EMO_ENTER = 1; // 0x0001 
		internal const int EMO_EXIT = 0;
		internal const int EMO_EXPAND = 3; // 0x0003 
		internal const int EMO_EXPANDDOCUMENT = 1; // 0x0001 
		internal const int EMO_EXPANDSELECTION = 0;
		internal const int EMO_GETVIEWMODE = 5; // 0x0005 
		internal const int EMO_MOVESELECTION = 4; // 0x0004 
		internal const int EMO_PROMOTE = 2; // 0x0002 
		internal const int EN_ALIGNLTR = 1808; // 0x0710 
		internal const int EN_ALIGNRTL = 1809; // 0x0711 
		internal const int EN_CORRECTTEXT = 1797; // 0x0705 
		internal const int EN_DRAGDROPDONE = 1804; // 0x070c 
		internal const int EN_DROPFILES = 1795; // 0x0703 
		internal const int EN_IMECHANGE = 1799; // 0x0707 
		internal const int EN_LINK = 1803; // 0x070b 
		internal const int EN_MSGFILTER = 1792; // 0x0700 
		internal const int EN_OBJECTPOSITIONS = 1802; // 0x070a 
		internal const int EN_OLEOPFAILED = 1801; // 0x0709 
		internal const int EN_PARAGRAPHEXPANDED = 1805; // 0x070d 
		internal const int EN_PROTECTED = 1796; // 0x0704 
		internal const int EN_REQUESTRESIZE = 1793; // 0x0701 
		internal const int EN_SAVECLIPBOARD = 1800; // 0x0708 
		internal const int EN_SELCHANGE = 1794; // 0x0702 
		internal const int EN_STOPNOUNDO = 1798; // 0x0706 
		internal const int ENM_CHANGE = 1; // 0x0001 
		internal const int ENM_CORRECTTEXT = 4194304; // 0x400000 
		internal const int ENM_DRAGDROPDONE = 16; // 0x0010 
		internal const int ENM_DROPFILES = 1048576; // 0x100000 
		internal const int ENM_IMECHANGE = 8388608; // 0x800000 
		internal const int ENM_KEYEVENTS = 65536; // 0x10000 
		internal const int ENM_LANGCHANGE = 16777216; // 0x1000000 
		internal const int ENM_LINK = 67108864; // 0x4000000 
		internal const int ENM_MOUSEEVENTS = 131072; // 0x20000 
		internal const int ENM_NONE = 0;
		internal const int ENM_OBJECTPOSITIONS = 33554432; // 0x2000000 
		internal const int ENM_PARAGRAPHEXPANDED = 32; // 0x0020 
		internal const int ENM_PROTECTED = 2097152; // 0x200000 
		internal const int ENM_REQUESTRESIZE = 262144; // 0x40000 
		internal const int ENM_SCROLL = 4; // 0x0004 
		internal const int ENM_SCROLLEVENTS = 8; // 0x0008 
		internal const int ENM_SELCHANGE = 524288; // 0x80000 
		internal const int ENM_UPDATE = 2; // 0x0002 
		internal const int ES_DISABLENOSCROLL = 8192; // 0x2000 
		internal const int ES_EX_NOCALLOLEINIT = 16777216; // 0x1000000 
		internal const int ES_NOIME = 524288; // 0x80000 
		internal const int ES_NOOLEDRAGDROP = 8; // 0x0008 
		internal const int ES_SAVESEL = 32768; // 0x8000 
		internal const int ES_SELECTIONBAR = 16777216; // 0x1000000 
		internal const int ES_SELFIME = 262144; // 0x40000 
		internal const int ES_SUNKEN = 16384; // 0x4000 
		internal const int ES_VERTICAL = 4194304; // 0x400000 
		internal const int FR_DOWN = 1; // 0x0001 
		internal const int FR_MATCHCASE = 4; // 0x0004 
		internal const int FR_WHOLEWORD = 2; // 0x0002 
		internal const int GCM_RIGHTMOUSEDROP = 32768; // 0x8000 
		internal const int GT_DEFAULT = 0;
		internal const int GT_USECRLF = 1; // 0x0001 
		internal const int GTL_CLOSE = 4; // 0x0004 
		internal const int GTL_DEFAULT = 0;
		internal const int GTL_NUMBYTES = 16; // 0x0010 
		internal const int GTL_NUMCHARS = 8; // 0x0008 
		internal const int GTL_PRECISE = 2; // 0x0002 
		internal const int GTL_USECRLF = 1; // 0x0001 
		internal const int ICM_LEVEL2 = 2; // 0x0002 
		internal const int ICM_LEVEL2_5 = 3; // 0x0003 
		internal const int ICM_LEVEL2_SUI = 4; // 0x0004 
		internal const int ICM_LEVEL3 = 1; // 0x0001 
		internal const int ICM_NOTOPEN = 0;
		internal const int IMF_AUTOFONT = 2; // 0x0002 
		internal const int IMF_AUTOFONTSIZEADJUST = 16; // 0x0010 
		internal const int IMF_AUTOKEYBOARD = 1; // 0x0001 
		internal const int IMF_CLOSESTATUSWINDOW = 8; // 0x0008 
		internal const int IMF_DUALFONT = 128; // 0x0080 
		internal const int IMF_FORCEACTIVE = 64; // 0x0040 
		internal const int IMF_FORCEDISABLE = 4; // 0x0004 
		internal const int IMF_FORCEENABLE = 2; // 0x0002 
		internal const int IMF_FORCEINACTIVE = 128; // 0x0080 
		internal const int IMF_FORCENONE = 1; // 0x0001 
		internal const int IMF_FORCEREMEMBER = 256; // 0x0100 
		internal const int IMF_IMEALWAYSSENDNOTIFY = 8; // 0x0008 
		internal const int IMF_IMECANCELCOMPLETE = 4; // 0x0004 
		internal const int IMF_MULTIPLEEDIT = 1024; // 0x0400 
		internal const int IMF_UIFONTS = 32; // 0x0020 
		internal const int IMF_VERTICAL = 32; // 0x0020 
		internal const int lDefaultTab = 720; // 0x02d0 
		internal const int MAX_TAB_STOPS = 32; // 0x0020 
		internal const int OLEOP_DOVERB = 1; // 0x0001 
		internal const int PC_DELIMITER = 4; // 0x0004 
		internal const int PC_FOLLOWING = 1; // 0x0001 
		internal const int PC_LEADING = 2; // 0x0002 
		internal const int PC_OVERFLOW = 3; // 0x0003 
		internal const int PFA_CENTER = 3; // 0x0003 
		internal const int PFA_JUSTIFY = 4; // 0x0004 
		internal const int PFA_LEFT = 1; // 0x0001 
		internal const int PFA_RIGHT = 2; // 0x0002 
		internal const int PFE_DONOTHYPHEN = 64; // 0x0040 
		internal const int PFE_KEEP = 2; // 0x0002 
		internal const int PFE_KEEPNEXT = 4; // 0x0004 
		internal const int PFE_NOLINENUMBER = 16; // 0x0010 
		internal const int PFE_NOWIDOWCONTROL = 32; // 0x0020 
		internal const int PFE_PAGEBREAKBEFORE = 8; // 0x0008 
		internal const int PFE_RTLPARA = 1; // 0x0001 
		internal const int PFE_SIDEBYSIDE = 128; // 0x0080 
		internal const int PFE_TABLECELL = 16384; // 0x4000 
		internal const int PFE_TABLECELLEND = 32768; // 0x8000 
		internal const int PFE_TABLEROW = 49152; // 0xc000 
		internal const int PFM_ALIGNMENT = 8; // 0x0008 
		internal const int PFM_ALL = -2147483585; // 0x8000003f 
		internal const int PFM_ALL2 = -1056965121; // 0xc0fffdff 
		internal const int PFM_BORDER = 2048; // 0x0800 
		internal const int PFM_DONOTHYPHEN = 4194304; // 0x400000 
		internal const int PFM_EFFECTS = -1057030144; // 0xc0ff0000 
		internal const int PFM_KEEP = 131072; // 0x20000 
		internal const int PFM_KEEPNEXT = 262144; // 0x40000 
		internal const int PFM_LINESPACING = 256; // 0x0100 
		internal const int PFM_NOLINENUMBER = 1048576; // 0x100000 
		internal const int PFM_NOWIDOWCONTROL = 2097152; // 0x200000 
		internal const int PFM_NUMBERING = 32; // 0x0020 
		internal const int PFM_NUMBERINGSTART = 32768; // 0x8000 
		internal const int PFM_NUMBERINGSTYLE = 8192; // 0x2000 
		internal const int PFM_NUMBERINGTAB = 16384; // 0x4000 
		internal const int PFM_OFFSET = 4; // 0x0004 
		internal const int PFM_OFFSETINDENT = -2147483648; // 0x80000000 
		internal const int PFM_PAGEBREAKBEFORE = 524288; // 0x80000 
		internal const int PFM_RIGHTINDENT = 2; // 0x0002 
		internal const int PFM_RTLPARA = 65536; // 0x10000 
		internal const int PFM_SHADING = 4096; // 0x1000 
		internal const int PFM_SIDEBYSIDE = 8388608; // 0x800000 
		internal const int PFM_SPACEAFTER = 128; // 0x0080 
		internal const int PFM_SPACEBEFORE = 64; // 0x0040 
		internal const int PFM_STARTINDENT = 1; // 0x0001 
		internal const int PFM_STYLE = 1024; // 0x0400 
		internal const int PFM_TABLE = -1073741824; // 0xc0000000 
		internal const int PFM_TABSTOPS = 16; // 0x0010 
		internal const int PFN_BULLET = 1; // 0x0001 
		internal readonly static string RICHEDIT_CLASS10A;
		internal readonly static string RICHEDIT_CLASS20A;
		internal readonly static string RICHEDIT_CLASS20W;
		internal readonly static string RICHEDIT_CLASS30A;
		internal readonly static string RICHEDIT_CLASS30W;
		internal readonly static string RICHEDIT_DLL10;
		internal readonly static string RICHEDIT_DLL20;
		internal readonly static string RICHEDIT_DLL30;
		internal const int SCF_ALL = 4; // 0x0004 
		internal const int SCF_DEFAULT = 0;
		internal const int SCF_SELECTION = 1; // 0x0001 
		internal const int SCF_USEUIRULES = 8; // 0x0008 
		internal const int SCF_WORD = 2; // 0x0002 
		internal const int SEL_EMPTY = 0;
		internal const int SEL_MULTICHAR = 4; // 0x0004 
		internal const int SEL_MULTIOBJECT = 8; // 0x0008 
		internal const int SEL_OBJECT = 2; // 0x0002 
		internal const int SEL_TEXT = 1; // 0x0001 
		internal const int SES_ALLOWBEEPS = 256; // 0x0100 
		internal const int SES_BEEPONMAXTEXT = 2; // 0x0002 
		internal const int SES_BIDI = 4096; // 0x1000 
		internal const int SES_EMULATE10 = 16; // 0x0010 
		internal const int SES_EMULATESYSEDIT = 1; // 0x0001 
		internal const int SES_EXTENDBACKCOLOR = 4; // 0x0004 
		internal const int SES_LOWERCASE = 1024; // 0x0400 
		internal const int SES_MAPCPS = 8; // 0x0008 
		internal const int SES_NOIME = 128; // 0x0080 
		internal const int SES_NOINPUTSEQUENCECHK = 2048; // 0x0800 
		internal const int SES_SCROLLONKILLFOCUS = 8192; // 0x2000 
		internal const int SES_UPPERCASE = 512; // 0x0200 
		internal const int SES_USEAIMM = 64; // 0x0040 
		internal const int SES_USECRLF = 32; // 0x0020 
		internal const int SES_XLTCRCRLFTOCR = 16384; // 0x4000 
		internal const int SF_RTF = 2; // 0x0002 
		internal const int SF_RTFNOOBJS = 3; // 0x0003 
		internal const int SF_TEXT = 1; // 0x0001 
		internal const int SF_TEXTIZED = 4; // 0x0004 
		internal const int SF_UNICODE = 16; // 0x0010 
		internal const int SFF_PLAINRTF = 16384; // 0x4000 
		internal const int SFF_SELECTION = 32768; // 0x8000 
		internal const int TM_MULTICODEPAGE = 32; // 0x0020 
		internal const int TM_MULTILEVELUNDO = 8; // 0x0008 
		internal const int TM_PLAINTEXT = 1; // 0x0001 
		internal const int TM_RICHTEXT = 2; // 0x0002 
		internal const int TM_SINGLECODEPAGE = 16; // 0x0010 
		internal const int TM_SINGLELEVELUNDO = 4; // 0x0004 
		internal const int UID_CUT = 4; // 0x0004 
		internal const int UID_DELETE = 2; // 0x0002 
		internal const int UID_DRAGDROP = 3; // 0x0003 
		internal const int UID_PASTE = 5; // 0x0005 
		internal const int UID_TYPING = 1; // 0x0001 
		internal const int UID_UNKNOWN = 0;
		internal const int VM_NORMAL = 4; // 0x0004 
		internal const int VM_OUTLINE = 2; // 0x0002 
		internal const int WB_CLASSIFY = 3; // 0x0003 
		internal const int WB_LEFTBREAK = 6; // 0x0006 
		internal const int WB_MOVEWORDLEFT = 4; // 0x0004 
		internal const int WB_MOVEWORDNEXT = 5; // 0x0005 
		internal const int WB_MOVEWORDPREV = 4; // 0x0004 
		internal const int WB_MOVEWORDRIGHT = 5; // 0x0005 
		internal const int WB_NEXTBREAK = 7; // 0x0007 
		internal const int WB_PREVBREAK = 6; // 0x0006 
		internal const int WB_RIGHTBREAK = 7; // 0x0007 
		internal const int WBF_BREAKAFTER = 64; // 0x0040 
		internal const int WBF_BREAKLINE = 32; // 0x0020 
		internal const int WBF_CLASS = 15; // 0x000f 
		internal const int WBF_CUSTOM = 512; // 0x0200 
		internal const int WBF_ISWHITE = 16; // 0x0010 
		internal const int WBF_LEVEL1 = 128; // 0x0080 
		internal const int WBF_LEVEL2 = 256; // 0x0100 
		internal const int WBF_OVERFLOW = 64; // 0x0040 
		internal const int WBF_WORDBREAK = 32; // 0x0020 
		internal const int WBF_WORDWRAP = 16; // 0x0010 
		internal readonly static string WC_RICHEDITA;
		internal readonly static string WC_RICHEDITW;
		internal readonly static char WCH_EMBEDDING;
		internal const int WM_CONTEXTMENU = 123; // 0x007b 
		internal const int WM_PRINTCLIENT = 792; // 0x0318 
		internal const int yHeightCharPtsMost = 1638; // 0x0666 
	}

}
