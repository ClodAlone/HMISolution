#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Any infringement will be prosecuted under
//  applicable laws. 
//
#endregion


using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Drawing
{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
    public class DoubleBufferSurface : IDisposable
    {
        BufferedGraphicsContext context = new BufferedGraphicsContext();
        BufferedGraphics bufferedGraphics;
        private Graphics targetGraphics;
        IntPtr dib;
        bool dirty = true;

        // TODO: will a DirtyBounds Rectangle help / will SetDibBits with smaller bounds be faster than painting whole area?

        public DoubleBufferSurface(Graphics targetGraphics, Rectangle targetRectangle)
        {
            bufferedGraphics = context.Allocate(targetGraphics, targetRectangle);
            this.targetGraphics = targetGraphics;
            dib = Getdib(GetContext(bufferedGraphics));
        }

        public void SetTargetGraphics(Graphics g)
        {
            if (this.targetGraphics != null)
                this.targetGraphics.Dispose();
            this.targetGraphics = g;
        }

        public Bitmap CreateDrawingSurfaceBitmap()
        {
            dirty = true;
            return Bitmap.FromHbitmap(this.dib);
        }

        public bool Dirty
        {
            get
            {
                return dirty;
            }
            set
            {
                dirty = value;
            }
        }

        public Graphics Graphics
        {
            get
            {
                dirty = true;
                return bufferedGraphics.Graphics;
            }
        }

        public void Render(Graphics target)
        {
            bufferedGraphics.Render(target);
            dirty = false;
        }

        public void Render()
        {
            if (!dirty)
                return;
            bufferedGraphics.Render(this.targetGraphics);
            dirty = false;
        }

        public Rectangle ScrollWindow(int xAmount, int yAmount, Rectangle rect, Rectangle clipRect)
        {
            int yAbs = Math.Abs(yAmount);
            int xAbs = Math.Abs(xAmount);

            Rectangle updateRect = rect;
            rect.Height -= yAbs;
            rect.Width -= xAbs;
            Rectangle srcRect = rect;
            Rectangle destRect = rect;

            if (yAmount < 0)
            {
                srcRect.Y += yAbs;
                updateRect.Y = rect.Bottom - yAbs;
            }
            else if (yAmount > 0)
            {
                destRect.Y += yAbs;
                updateRect.Height = yAbs;
            }

            if (xAmount < 0)
            {
                srcRect.X += xAbs;
                updateRect.X = rect.Right - xAbs;
            }
            else if (xAmount > 0)
            {
                destRect.X += xAbs;
                updateRect.Width = xAbs;
            }

            srcRect.Intersect(clipRect);
            destRect.Intersect(clipRect);
            updateRect.Intersect(clipRect);

            Bitmap bm = this.CreateDrawingSurfaceBitmap();
            Graphics.DrawImage(bm, destRect, srcRect, GraphicsUnit.Pixel);
            bm.Dispose();
            this.dirty = true;

            return updateRect;
        }

        BufferedGraphicsContext GetContext(BufferedGraphics bg)
        {
            FieldInfo fi = typeof(BufferedGraphics).GetField("context", BindingFlags.Instance | BindingFlags.NonPublic);
            return (BufferedGraphicsContext) fi.GetValue(bg);
        }
        IntPtr Getdib(BufferedGraphicsContext bgc)
        {
            FieldInfo fi = typeof(BufferedGraphicsContext).GetField("dib", BindingFlags.Instance | BindingFlags.NonPublic);
            return (IntPtr) fi.GetValue(bgc);
        }



        #region IDisposable Members

        public void Dispose()
        {
            bufferedGraphics.Dispose();
            context.Dispose();
            dib = IntPtr.Zero;
            if (targetGraphics != null)
                targetGraphics.Dispose();
        }

        #endregion
    }
#else
	public class DoubleBufferSurface : IDisposable
    {
        const int rop = 0xcc0020;
        private Size bufferSize;
        private IntPtr compatDC;
        private Graphics compatGraphics;
        private IntPtr dib;
        private IntPtr oldBitmap;
        private Point targetLoc;
        private Size virtualSize;
        private Graphics targetGraphics;
		bool dirty = true;

        // Avoid /unsafe flag, use reflection instead
		static Type typeOfGraphicsBufferManager;
		static Type typeOfNativeMethodsBITMAPINFO_FLAT;

		object graphicsBufferManager;
		object nativeMethodsBITMAPINFO_FLAT;

		static DoubleBufferSurface()
		{
			typeOfGraphicsBufferManager = typeof(Control).Assembly.GetType("System.Windows.Forms.GraphicsBufferManager+DibGraphicsBufferManager");
			typeOfNativeMethodsBITMAPINFO_FLAT = typeof(Control).Assembly.GetType("System.Windows.Forms.NativeMethods+BITMAPINFO_FLAT");
		}

		public DoubleBufferSurface(Graphics targetGraphics, Rectangle targetRectangle)
        {
			// Avoid /unsafe flag, use reflection instead
			graphicsBufferManager = Activator.CreateInstance(typeOfGraphicsBufferManager, true);
			
			this.targetGraphics = targetGraphics;
            this.targetLoc = new Point(targetRectangle.X, targetRectangle.Y);
            if (targetGraphics != null)
            {
                IntPtr ptr1 = targetGraphics.GetHdc();
                try
                {
                    this.CreateBuffer(ptr1, -this.targetLoc.X, -this.targetLoc.Y, targetRectangle.Width, targetRectangle.Height);
                }
                finally
                {
                    targetGraphics.ReleaseHdcInternal(ptr1);
                }
            }
        }

        public void SetTargetGraphics(Graphics g)
        {
            if (this.targetGraphics != null)
                this.targetGraphics.Dispose();
            this.targetGraphics = g;
        }

		public Bitmap CreateDrawingSurfaceBitmap()
		{
			dirty = true;
			return Bitmap.FromHbitmap(this.dib);
		}

		public bool Dirty
		{
			get
			{
				return dirty;
			}
			set
			{
				dirty = value;
			}
		}

        public Graphics Graphics
        {
            get
            {
				dirty = true;
                return compatGraphics;
            }
        }
         
        public void Render(Graphics target)
        {
            if (target != null)
            {
                IntPtr ptr1 = target.GetHdc();
                try
                {
                    this.RenderInternal(ptr1);
                }
                finally
                {
                    target.ReleaseHdcInternal(ptr1);
                }
				dirty = false;
			}
        }

        public void Render()
        {
			if (!dirty)
				return;
			Render(this.targetGraphics);
			dirty = false;
        }

           public Rectangle ScrollWindow(int xAmount, int yAmount, Rectangle rect, Rectangle clipRect)
        {
            int yAbs = Math.Abs(yAmount);
            int xAbs = Math.Abs(xAmount);

            Rectangle updateRect = rect;
            rect.Height -= yAbs;
            rect.Width -= xAbs;
            Rectangle srcRect = rect;
            Rectangle destRect = rect;

            if (yAmount < 0)
            {
                srcRect.Y += yAbs;
                updateRect.Y = rect.Bottom - yAbs;
            }
            else if (yAmount > 0)
            {
                destRect.Y += yAbs;
                updateRect.Height = yAbs;
            }

            if (xAmount < 0)
            {
                srcRect.X += xAbs;
                updateRect.X = rect.Right - xAbs;
            }
            else if (xAmount > 0)
            {
                destRect.X += xAbs;
                updateRect.Width = xAbs;
            }

            srcRect.Intersect(clipRect);
            destRect.Intersect(clipRect);
            updateRect.Intersect(clipRect);

            Bitmap bm = this.CreateDrawingSurfaceBitmap();
            Graphics.DrawImage(bm, destRect, srcRect, GraphicsUnit.Pixel);
            bm.Dispose();
            this.dirty = true;

            return updateRect;
        }

        private Graphics CreateBuffer(IntPtr src, int offsetX, int offsetY, int width, int height)
        {
            this.compatDC = NativeMethods.CreateCompatibleDC(src);
            if ((width > this.bufferSize.Width) || (height > this.bufferSize.Height))
            {
                int num1 = Math.Max(width, this.bufferSize.Width);
                int num2 = Math.Max(height, this.bufferSize.Height);
                this.DisposeBitmap();
                IntPtr ptr1 = IntPtr.Zero;
                this.dib = this.CreateCompatibleDIB(src, IntPtr.Zero, num1, num2, ref ptr1);
                this.bufferSize = new Size(num1, num2);
            }
            this.oldBitmap = NativeMethods.SelectObject(this.compatDC, this.dib);
            this.compatGraphics = Graphics.FromHdcInternal(this.compatDC);
            this.compatGraphics.TranslateTransform((float) -this.targetLoc.X, (float) -this.targetLoc.Y);
            this.virtualSize = new Size(width, height);
            return this.compatGraphics;
        }

        private IntPtr CreateCompatibleDIB(IntPtr hdc, IntPtr hpal, int ulWidth, int ulHeight, ref IntPtr ppvBits)
        {
            if (hdc == IntPtr.Zero)
            {
                throw new ArgumentNullException("hdc");
            }
            IntPtr ptr1 = IntPtr.Zero;
            NativeMethods.BITMAPINFO_FLAT bitmapinfo_flat1 = new NativeMethods.BITMAPINFO_FLAT();
            switch (NativeMethods.GetObjectType(hdc))
            {
                case 3:
                case 4:
                case 10:
                case 12:
                    if (this.bFillBitmapInfo(hdc, hpal, ref bitmapinfo_flat1))
                    {
                        bitmapinfo_flat1.bmiHeader_biWidth = ulWidth;
                        bitmapinfo_flat1.bmiHeader_biHeight = ulHeight;
                        if (bitmapinfo_flat1.bmiHeader_biCompression == 0)
                        {
                            bitmapinfo_flat1.bmiHeader_biSizeImage = 0;
                        }
                        else if (bitmapinfo_flat1.bmiHeader_biBitCount == 0x10)
                        {
                            bitmapinfo_flat1.bmiHeader_biSizeImage = (ulWidth * ulHeight) * 2;
                        }
                        else if (bitmapinfo_flat1.bmiHeader_biBitCount == 0x20)
                        {
                            bitmapinfo_flat1.bmiHeader_biSizeImage = (ulWidth * ulHeight) * 4;
                        }
                        else
                        {
                            bitmapinfo_flat1.bmiHeader_biSizeImage = 0;
                        }
                        bitmapinfo_flat1.bmiHeader_biClrUsed = 0;
                        bitmapinfo_flat1.bmiHeader_biClrImportant = 0;
                        ptr1 = NativeMethods.CreateDIBSection(hdc, ref bitmapinfo_flat1, 0, ref ppvBits, IntPtr.Zero, 0);
                        Win32Exception exception1 = null;
                        if (ptr1 == IntPtr.Zero)
                        {
                            exception1 = new Win32Exception(Marshal.GetLastWin32Error());
                        }
                        if (exception1 != null)
                        {
                            throw exception1;
                        }
                    }
                    return ptr1;
            }
            throw new ArgumentException("DCTypeInvalid");
        }

        private bool bFillBitmapInfo(IntPtr hdc, IntPtr hpal, ref NativeMethods.BITMAPINFO_FLAT pbmi)
        {
			IntPtr ptr1 = IntPtr.Zero;
            bool flag1 = false;
            try
            {
                ptr1 = NativeMethods.CreateCompatibleBitmap(hdc, 1, 1);
                if (ptr1 == IntPtr.Zero)
                {
                    throw new OutOfMemoryException("GraphicsBufferQueryFail");
                }
                pbmi.bmiHeader_biSize = Marshal.SizeOf(typeof(NativeMethods.BITMAPINFOHEADER));
                pbmi.bmiColors = new byte[0x400];
                NativeMethods.GetDIBits(hdc, ptr1, 0, 0, IntPtr.Zero, ref pbmi, 0);
                if (pbmi.bmiHeader_biBitCount <= 8)
                {
                    return this.bFillColorTable(hdc, hpal, ref pbmi);
                }
                if (pbmi.bmiHeader_biCompression == 3)
                {
                    NativeMethods.GetDIBits(hdc, ptr1, 0, pbmi.bmiHeader_biHeight, IntPtr.Zero, ref pbmi, 0);
                }
                flag1 = true;
            }
            finally
            {
                if (ptr1 != IntPtr.Zero)
                {
                    NativeMethods.DeleteObject(ptr1);
                    ptr1 = IntPtr.Zero;
                }
            }
            return flag1;
        }

#if true // Avoid having to compile with /unsafe flag
		private bool bFillColorTable(IntPtr hdc, IntPtr hpal, ref NativeMethods.BITMAPINFO_FLAT pbmi)
		{
			nativeMethodsBITMAPINFO_FLAT = Activator.CreateInstance(typeOfNativeMethodsBITMAPINFO_FLAT, false);

			foreach (FieldInfo fi in pbmi.GetType().GetFields(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance))
			{
				FieldInfo f2 = typeOfNativeMethodsBITMAPINFO_FLAT.GetField(fi.Name);
				f2.SetValue(nativeMethodsBITMAPINFO_FLAT, fi.GetValue(pbmi));
			}

			MethodInfo mi = typeOfGraphicsBufferManager.GetMethod("bFillColorTable", BindingFlags.NonPublic|BindingFlags.Instance);
			bool b = (bool) mi.Invoke(graphicsBufferManager, new object[] { hdc, hpal, nativeMethodsBITMAPINFO_FLAT });

			foreach (FieldInfo fi in pbmi.GetType().GetFields(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance))
			{
				FieldInfo f2 = typeOfNativeMethodsBITMAPINFO_FLAT.GetField(fi.Name);
				fi.SetValue(pbmi, f2.GetValue(nativeMethodsBITMAPINFO_FLAT));
			}

			return b;
		}
#else
        private unsafe bool bFillColorTable(IntPtr hdc, IntPtr hpal, ref NativeMethods.BITMAPINFO_FLAT pbmi)
        {
			bool flag1 = false;
            byte[] buffer1 = new byte[sizeof(NativeMethods.PALETTEENTRY) * 0x100];
            fixed (byte* numRef1 = pbmi.bmiColors)
            {
                fixed (byte* numRef2 = buffer1)
                {
                    NativeMethods.RGBQUAD* rgbquadPtr1 = (NativeMethods.RGBQUAD*) numRef1;
                    NativeMethods.PALETTEENTRY* paletteentryPtr1 = (NativeMethods.PALETTEENTRY*) numRef2;
                    int num2 = 1 << (pbmi.bmiHeader_biBitCount & 0x1f);
                    if (num2 <= 0x100)
                    {
                        uint num3;
                        IntPtr ptr1 = IntPtr.Zero;
                        if (hpal == IntPtr.Zero)
                        {
                            ptr1 = Graphics.GetHalftonePalette();
                            num3 = NativeMethods.GetPaletteEntries(ptr1, 0, num2, buffer1);
                        }
                        else
                        {
                            num3 = NativeMethods.GetPaletteEntries(hpal, 0, num2, buffer1);
                        }
                        if (num3 != 0)
                        {
                            for (int num1 = 0; num1 < num2; num1++)
                            {
                                rgbquadPtr1[num1].rgbRed = paletteentryPtr1[num1].peRed;
                                rgbquadPtr1[num1].rgbGreen = paletteentryPtr1[num1].peGreen;
                                rgbquadPtr1[num1].rgbBlue = paletteentryPtr1[num1].peBlue;
                                rgbquadPtr1[num1].rgbReserved = 0;
                            }
                            flag1 = true;
                        }
                    }
                }
            }
            return flag1;
        }
#endif

        private void DisposeBitmap()
        {
            if (this.dib != IntPtr.Zero)
            {
                NativeMethods.DeleteObject(this.dib);
                this.dib = IntPtr.Zero;
            }
        }


        private void DisposeDC()
        {
            if ((this.oldBitmap != IntPtr.Zero) && (this.compatDC != IntPtr.Zero))
            {
                NativeMethods.SelectObject(this.compatDC, this.oldBitmap);
                this.oldBitmap = IntPtr.Zero;
            }
            if (this.compatDC != IntPtr.Zero)
            {
                NativeMethods.DeleteDC(this.compatDC);
                this.compatDC = IntPtr.Zero;
            }
        }



        private void RenderInternal(IntPtr refTargetDC)
        {
            IntPtr ptr1 = this.compatGraphics.GetHdc();
            try
            {
                NativeMethods.BitBlt(refTargetDC, this.targetLoc.X, this.targetLoc.Y, this.virtualSize.Width, this.virtualSize.Height, ptr1, 0, 0, rop);
            }
            finally
            {
                this.compatGraphics.ReleaseHdc(ptr1);
            }
        }



    #region IDisposable Members

        public void Dispose()
        {
            this.DisposeBitmap();
            this.DisposeDC();
			((IDisposable) this.graphicsBufferManager).Dispose();
        }

    #endregion
    }

#endif
}
