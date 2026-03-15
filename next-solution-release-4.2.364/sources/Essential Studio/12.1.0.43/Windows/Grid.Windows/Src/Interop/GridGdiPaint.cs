//-------------------------------------------------------------------------------------------------
// <copyright file="GridGdiPaint.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Drawing.Text;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Runtime.InteropServices;

using Syncfusion.Styles;
using Syncfusion.Drawing;
using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Provides a set of advanced GDI Interop drawing routines that can be used
    /// instead of GDI plus routines to optimize drawing performance of the grid.
    /// Call these methods from <see cref="GridControlBase.DrawCellDisplayText"/> and <see cref="GridControlBase.FillRectangleHook"/> 
    /// as shown in example. You might also try turning off double buffering.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// protected override void OnDrawCellDisplayText(GridDrawCellDisplayTextEventArgs e)
    /// {
    ///        base.OnDrawCellDisplayText (e);
    /// <para/>
    ///        GridGdiPaint.AllowTextOut = false;
    ///        if (!e.Cancel)
    ///            e.Cancel = GridGdiPaint.Instance.DrawText(e.Graphics, e.DisplayText, e.TextRectangle, e.Style);
    /// }
    ///  <para/>
    /// protected override void OnFillRectangleHook(GridFillRectangleHookEventArgs e)
    /// {
    ///     base.OnFillRectangleHook (e);
    ///  <para/>
    ///     if (!e.Cancel)
    ///         e.Cancel = GridGdiPaint.Instance.FillRectangle(e.Graphics, e.Bounds, e.Brush);
    /// }
    ///  <para/>
    /// - Or -
    ///  <para/>
    /// public class PerformanceGridControl : GridControl
    /// {
    ///     private bool useGDI;
    ///  <para/>    
    ///     /// <summary>
    ///     /// Property UseGDI (bool)
    ///     /// </summary>
    ///     public bool UseGDI
    ///     {
    ///         get
    ///         {
    ///             return this.useGDI;
    ///         }
    ///         set
    ///         {
    ///             if (this.UseGDI != value)
    ///             {
    ///                 this.useGDI = value;
    ///                 Invalidate();
    ///             }
    ///         }
    ///     }
    /// <para/> 
    ///     private bool useDoubleBuffer = true;
    /// <para/>      
    ///        public bool UseDoubleBuffer
    ///        {
    ///         get
    ///         {
    ///             return this.useDoubleBuffer;
    ///         }
    ///         set
    ///         {
    ///             if (this.useDoubleBuffer != value)
    ///             {
    ///                 this.useDoubleBuffer = value;
    ///                 SetStyle(ControlStyles.Opaque, value);
    ///                 SetStyle(ControlStyles.DoubleBuffer, value);
    ///             }
    ///         }
    ///     }
    ///  <para/>
    ///     protected override void OnPaintBackground(PaintEventArgs pevent)
    ///     {
    ///         if (useDoubleBuffer)
    ///             base.OnPaintBackground (pevent);
    ///     }
    ///  <para/>
    ///     protected override void OnDrawCellDisplayText(GridDrawCellDisplayTextEventArgs e)
    ///     {
    ///         base.OnDrawCellDisplayText (e);
    /// <para/> 
    ///         if (!useGDI || e.Cancel)
    ///             return;
    ///  <para/>   
    ///            e.Cancel = GridGdiPaint.Instance.DrawText(e.Graphics, e.DisplayText, e.TextRectangle, e.Style);
    ///        }
    ///  <para/>   
    ///        protected override void OnFillRectangleHook(GridFillRectangleHookEventArgs e)
    ///     {
    ///         base.OnFillRectangleHook (e);
    ///  <para/>
    ///         if (!useGDI || e.Cancel)
    ///             return;
    ///  <para/>
    ///         e.Cancel = GridGdiPaint.Instance.FillRectangle(e.Graphics, e.Bounds, e.Brush);
    ///     }
    /// }
    /// </code>
    /// </example>
    public class GridGdiPaint
    {
        /// <summary>
        /// Specifies if GDI TextOut routine can be used as is when text is left-aligned and top-aligned.
        /// Set this false if text might need to be clipped.
        /// </summary>
        public static bool AllowTextOut = true;

        /// <summary>
        /// Specifies if GDI DrawText routine should always be used and text should be clipped. When you 
        /// set this true the performance of the GDI drawing routine will be the same as for GDIplus DrawString
        /// since text needs to be clipped everytime it is drawn. If you want to force GDI DrawText on a cell
        /// by cell basis you can specify style.Trimming = System.Drawing.StringTrimming.Character instead
        /// and leave ForceDrawText = false.
        /// </summary>
        public static bool ForceDrawText = false;

        bool dumpConsole = false;

        [ThreadStatic]
        static GridGdiPaint instance;

        Hashtable safedHFonts;
        Hashtable safedHBrushs;

        // Just to keep weak references alive
        ArrayList brushes;
        ArrayList fonts;

        /// <summary>
        /// Gets an instance of this calls
        /// </summary>
        public static GridGdiPaint Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GridGdiPaint();
                }

                return instance;
            }
        }

        GridGdiPaint()
        {
            fonts = new ArrayList();
            safedHFonts = new Hashtable();
            safedHBrushs = new Hashtable();
            brushes = new ArrayList();
            Application.Idle += new EventHandler(Application_Idle);
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            brushes.Clear();
            fonts.Clear();
            safedHFonts.Clear();
            safedHBrushs.Clear();
        }
        
        /// <overload>
        /// Draws the text emulating the Graphics.DrawString method if possible.
        /// </overload>
        /// <summary>
        /// Draws the text emulating the Graphics.DrawString method if possible.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="displayText">Display text.</param>
        /// <param name="textRectangle">Text rectangle.</param>
        /// <param name="style">Cell style information.</param>
        /// <returns>true if text could be painted; false if text could not be painted (e.g. because it needs to be drawn rotated).</returns>
        /// <example>
        /// <code lang="C#">
        /// protected override void OnDrawCellDisplayText(GridDrawCellDisplayTextEventArgs e)
        /// {
        ///        base.OnDrawCellDisplayText (e);
        /// <para/>
        ///        GridGdiPaint.AllowTextOut = false;
        ///        if (!e.Cancel)
        ///            e.Cancel = GridGdiPaint.Instance.DrawText(e.Graphics, e.DisplayText, e.TextRectangle, e.Style, e.Bounds);
        /// }
        ///  <para/>
        /// protected override void OnFillRectangleHook(GridFillRectangleHookEventArgs e)
        /// {
        ///     base.OnFillRectangleHook (e);
        /// <para/> 
        ///     if (!e.Cancel)
        ///         e.Cancel = GridGdiPaint.Instance.FillRectangle(e.Graphics, e.Bounds, e.Brush);
        /// }
        /// </code>
        /// </example>
        public bool DrawText(Graphics g, string displayText, Rectangle textRectangle, GridStyleInfo style)
        {
            Rectangle r = Rectangle.Ceiling(g.ClipBounds);
            r.Intersect(textRectangle);

            return DrawText(g, displayText, textRectangle, style, r);
        }

        /// <summary>
        /// Draws the text emulating the Graphics.DrawString method if possible.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="displayText">Display text.</param>
        /// <param name="textRectangle">Text rectangle.</param>
        /// <param name="style">Cell style information.</param>
        /// <param name="clipBounds">The clip bounds of the text. When empty or same as textRectangle there will be no explicit clipping (but this does not
        /// affect DT_NOCLIP setting of GDI DrawText routine). If specified
        /// then output will be clipped by setting IntersectClipRect and DT_NOCLIP option is used for DrawText.</param>
        /// <returns>true if text could be painted; false if text could not be painted (e.g. because it needs to be drawn rotated).</returns>
        /// <example>
        /// <code lang="C#">
        /// protected override void OnDrawCellDisplayText(GridDrawCellDisplayTextEventArgs e)
        /// {
        ///        base.OnDrawCellDisplayText (e);
        /// <para/>
        ///        GridGdiPaint.AllowTextOut = false;
        ///        if (!e.Cancel)
        ///        {
        ///            GridControlBase grid = (GridControlBase) sender;
        ///            Rectangle clipBounds = grid.ViewLayout.RangeInfoToRectangle(GridRangeInfo.Cell(e.Style.CellIdentity.RowIndex, e.Style.CellIdentity.ColIndex), GridCellSizeKind.VisibleSize);
        ///            e.Cancel = GridGdiPaint.Instance.DrawText(e.Graphics, e.DisplayText, e.TextRectangle, e.Style, e.Bounds, clipBounds);
        ///        }
        /// }
        ///  <para/>
        /// protected override void OnFillRectangleHook(GridFillRectangleHookEventArgs e)
        /// {
        ///     base.OnFillRectangleHook (e);
        /// <para/> 
        ///     if (!e.Cancel)
        ///         e.Cancel = GridGdiPaint.Instance.FillRectangle(e.Graphics, e.Bounds, e.Brush);
        /// }
        /// </code>
        /// </example>
        public bool DrawText(Graphics g, string displayText, Rectangle textRectangle, GridStyleInfo style, Rectangle clipBounds)
        {
            return DrawText(g, displayText, textRectangle, style, clipBounds, false);
        }

        /// <summary>
        /// Draws the text emulating the Graphics.DrawString method if possible.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="displayText">Display text.</param>
        /// <param name="textRectangle">Text rectangle.</param>
        /// <param name="style">Cell style information.</param>
        /// <param name="clipBounds">The clip bounds of the text. When empty or same as textRectangle there will be no explicit clipping (but this does not
        /// affect DT_NOCLIP setting of GDI DrawText routine). If specified
        /// then output will be clipped by setting IntersectClipRect and DT_NOCLIP option is used for DrawText.</param>
        /// <param name="isRightToLeft">true for RTL languages, false if left to right. Affects Horizontal Alignemnt.</param>
        /// <returns>true if text could be painted; false if text could not be painted (e.g. because it needs to be drawn rotated).</returns>
        /// <example>
        /// <code lang="C#">
        /// protected override void OnDrawCellDisplayText(GridDrawCellDisplayTextEventArgs e)
        /// {
        ///        base.OnDrawCellDisplayText (e);
        /// <para/>
        ///        GridGdiPaint.AllowTextOut = false;
        ///        if (!e.Cancel)
        ///        {
        ///            GridControlBase grid = (GridControlBase) sender;
        ///            Rectangle clipBounds = grid.ViewLayout.RangeInfoToRectangle(GridRangeInfo.Cell(e.Style.CellIdentity.RowIndex, e.Style.CellIdentity.ColIndex), GridCellSizeKind.VisibleSize);
        ///            e.Cancel = GridGdiPaint.Instance.DrawText(e.Graphics, e.DisplayText, e.TextRectangle, e.Style, e.Bounds, clipBounds);
        ///        }
        /// }
        ///  <para/>
        /// protected override void OnFillRectangleHook(GridFillRectangleHookEventArgs e)
        /// {
        ///     base.OnFillRectangleHook (e);
        ///  <para/>
        ///     if (!e.Cancel)
        ///         e.Cancel = GridGdiPaint.Instance.FillRectangle(e.Graphics, e.Bounds, e.Brush);
        /// }
        /// </code>
        /// </example>
        public bool DrawText(Graphics g, string displayText, Rectangle textRectangle, GridStyleInfo style, Rectangle clipBounds, bool isRightToLeft)
        {
            if (style.Font.Orientation != 0)
            {
                return false;
            }

            IntPtr hdc = g.GetHdc();
            try
            {
                IntPtr oldFont;
                IntPtr chfont = IntPtr.Zero;
                GridFontInfo fontInfo = style.ReadOnlyFont;

                if (fontInfo.Italic)
                {
                    textRectangle.Width--;
                }

                WeakReference w = (WeakReference)safedHFonts[fontInfo];
                if (w != null && w.IsAlive)
                {
                    GridWeakRefHFont wf = (GridWeakRefHFont)w.Target;
                    GC.KeepAlive(wf);
                    chfont = wf.Hfont;
                }
                else
                {
                    ////Console.WriteLine("Create " + fontInfo.Facename.ToString());
                    //// Need to create andl later destroy specific font handle
                    chfont = this.CreateHFont(fontInfo);
                    GridWeakRefHFont wf = new GridWeakRefHFont(safedHFonts, chfont);
                    GC.KeepAlive(wf);
                    safedHFonts[fontInfo] = new WeakReference(wf);
                    fonts.Add(wf);
                }

                oldFont = NativeMethods.SelectObject(hdc, chfont);

                ////int oldBackColor = NativeMethods.SetBkColor(hdc, NativeMethods.COLORREFToRGB(style.BackColor.ToArgb()));
                int oldTextColor = NativeMethods.SetTextColor(hdc, NativeMethods.COLORREFToRGB(style.TextColor.ToArgb()));
                int oldBkMode = NativeMethods.SetBkMode(hdc, NativeMethods.TRANSPARENT);

                NativeMethods.RECT rect = NativeMethods.RECT.FromXYWH(textRectangle.Left, textRectangle.Top, textRectangle.Width, textRectangle.Height);

                int options = NativeMethods.DT_NOCLIP | NativeMethods.DT_SINGLELINE;
                options |= NativeMethods.DT_EDITCONTROL;

                bool advanced = !AllowTextOut;
                bool useDrawText = ForceDrawText;
                switch (style.HorizontalAlignment)
                {
                    case GridHorizontalAlignment.Left:
                        if (isRightToLeft)
                        {
                            advanced = true;
                            options |= NativeMethods.DT_RIGHT;
                        }

                        break;
                    case GridHorizontalAlignment.Center:
                        advanced = true;
                        options |= NativeMethods.DT_CENTER;
                        break;
                    case GridHorizontalAlignment.Right:
                        if (isRightToLeft)
                        {
                            options |= NativeMethods.DT_LEFT;
                        }
                        else
                        {
                            advanced = true;
                            options |= NativeMethods.DT_RIGHT;
                        }

                        break;
                }

                switch (style.VerticalAlignment)
                {
                    case GridVerticalAlignment.Top:
                        break;
                    case GridVerticalAlignment.Middle:
                        advanced = true;
                        options |= NativeMethods.DT_VCENTER;
                        break;
                    case GridVerticalAlignment.Bottom:
                        advanced = true;
                        options |= NativeMethods.DT_BOTTOM;
                        break;
                }

                switch (style.Trimming)
                {
                    case StringTrimming.Character:
                    case StringTrimming.Word:
                        useDrawText = true;
                        options &= ~NativeMethods.DT_NOCLIP;
                        break;
                    case StringTrimming.EllipsisCharacter:
                        useDrawText = true;
                        options |= NativeMethods.DT_END_ELLIPSIS;
                        break;
                    case StringTrimming.EllipsisWord:
                        useDrawText = true;
                        options |= NativeMethods.DT_WORD_ELLIPSIS;
                        break;
                    case StringTrimming.EllipsisPath:
                        useDrawText = true;
                        options |= NativeMethods.DT_PATH_ELLIPSIS;
                        break;
                }

                switch (style.HotkeyPrefix)
                {
                    case HotkeyPrefix.None:
                    case HotkeyPrefix.Hide:
                        options |= NativeMethods.DT_NOPREFIX;
                        break;

                    default:
                        useDrawText = true;
                        break;
                }

                if (style.WrapText)
                {
                    options |= NativeMethods.DT_WORDBREAK;
                    options &= ~NativeMethods.DT_SINGLELINE;
                    advanced = true;
                }

                bool shouldClip = !(clipBounds.IsEmpty || clipBounds.Contains(textRectangle));

                NativeMethods.RECT oldClip = new NativeMethods.RECT();
                if (shouldClip)
                {
                    NativeMethods.GetClipBox(hdc, ref oldClip);
                    NativeMethods.IntersectClipRect(hdc, clipBounds.X, clipBounds.Y, clipBounds.Right, clipBounds.Bottom);

                    options |= NativeMethods.DT_NOCLIP;
                }
                if (useDrawText)
                {
                    if (dumpConsole)
                    {
                        Console.WriteLine("DT {0}", displayText);
                    }
                    NativeMethods.DrawText(hdc, displayText, displayText.Length, ref rect, options);
                }
                else if (advanced)
                {
                    if (dumpConsole)
                    {
                        Console.WriteLine("DTE {0}", displayText);
                    }
                    DrawTextLikeMultiLineEdit(hdc, displayText, textRectangle, options, Rectangle.Empty);
                }
                else
                {
                    if (dumpConsole)
                    {
                        Console.WriteLine("TO {0}", displayText);
                    }

                    NativeMethods.ExtTextOut(hdc, textRectangle.Left, textRectangle.Top, 0, ref rect, displayText, displayText.Length, null);
                }

                if (shouldClip)
                {
                    NativeMethods.SelectClipRgn(hdc, IntPtr.Zero);
                    NativeMethods.IntersectClipRect(hdc, oldClip.left, oldClip.top, oldClip.right, oldClip.bottom);
                }

                NativeMethods.SelectObject(hdc, oldFont);
                NativeMethods.SetTextColor(hdc, oldTextColor);
                ////NativeMethods.SetBkColor(hdc, oldBackColor);
                NativeMethods.SetBkMode(hdc, oldBkMode);
            }
            finally
            {
                g.ReleaseHdc(hdc);
            }

            return true;
        }

        /// <summary>
        /// Fills the specified rectangle with the specified brush emulating Graphics.FillRectangle method
        /// </summary>
        /// <param name="graphics">Graphics context.</param>
        /// <param name="bounds">Rectangle bounds.</param>
        /// <param name="brush">Fill brush.</param>
        /// <returns>true if text could be painted; false if text could not be painted (e.g. color is alphablended or gradient).</returns>
        /// <example>
        /// <code lang="C#">
        /// protected override void OnDrawCellDisplayText(GridDrawCellDisplayTextEventArgs e)
        /// {
        ///        base.OnDrawCellDisplayText (e);
        /// <para/>
        ///        GridGdiPaint.AllowTextOut = false;
        ///        if (!e.Cancel)
        ///            e.Cancel = GridGdiPaint.Instance.DrawText(e.Graphics, e.DisplayText, e.TextRectangle, e.Style);
        /// }
        ///  <para/>
        /// protected override void OnFillRectangleHook(GridFillRectangleHookEventArgs e)
        /// {
        ///     base.OnFillRectangleHook (e);
        ///  <para/>
        ///     if (!e.Cancel)
        ///         e.Cancel = GridGdiPaint.Instance.FillRectangle(e.Graphics, e.Bounds, e.Brush);
        /// }
        /// </code>
        /// </example>
        public bool FillRectangle(Graphics graphics, RectangleF bounds, BrushInfo brush)
        {
            BrushInfo interior = brush;
            if (interior.Style != BrushStyle.Solid || interior.BackColor.A != 255)
            {
                return false;
            }
            else
            {
                // style has color
                int rgb = NativeMethods.COLORREFToRGB(interior.BackColor.ToArgb());

                IntPtr hdc = graphics.GetHdc();
                try
                {
                    NativeMethods.RECT rect = NativeMethods.RECT.FromXYWH((int)bounds.Left, (int)bounds.Top, (int)bounds.Width, (int)bounds.Height);

                    ////                    if (false)
                    ////                    {
                    ////                        // ExtTextOut allows us to draw without having to create a Brush handle.
                    ////                        int oldBkColor = NativeMethods.SetBkColor(hdc, rgb);
                    ////                        NativeMethods.ExtTextOut(hdc, (int) bounds.Left, (int) bounds.Top, NativeMethods.ETO_OPAQUE, ref rect, "", 0, null);
                    ////                        NativeMethods.SetBkColor(hdc, oldBkColor);
                    ////                    }
                    ////                    else
                    {
                        ////if (brush.BackColor == SystemColors.Control)
                        ////    Console.WriteLine(brush.ToString());

                        IntPtr chbrush;
                        WeakReference w = (WeakReference)safedHBrushs[rgb];
                        if (w != null && w.IsAlive)
                        {
                            GridWeakRefHBrush wf = (GridWeakRefHBrush)w.Target;
                            GC.KeepAlive(wf);
                            ////                            Console.WriteLine("Reuse" + brush.ToString());
                            chbrush = wf.Hbrush;
                        }
                        else
                        {
                            ////                        Console.WriteLine("Create " + brush.ToString());
                            //// Need to create andl later destroy specific brush handle
                            chbrush = CreateHBrush(brush);
                            GridWeakRefHBrush wf = new GridWeakRefHBrush(safedHBrushs, chbrush);
                            GC.KeepAlive(wf);
                            safedHBrushs[rgb] = new WeakReference(wf);
                            brushes.Add(wf);
                        }

                        IntPtr oldBrush = NativeMethods.SelectObject(hdc, chbrush);
                        if (dumpConsole)
                        {
                            Console.WriteLine("FR {0} {1}", bounds, brush);
                        }

                        NativeMethods.FillRect(hdc, ref rect, chbrush);
                        NativeMethods.SelectObject(hdc, oldBrush);
                    }
                }
                finally
                {
                    graphics.ReleaseHdc(hdc);
                }
            }

            return true;
        }

        internal int ComputeLineBreaks(IntPtr pDC, string sz, ref Rectangle rc, int nFormat, int[] LineBreaks, int[] LineLengths, int nMaxBreaks)
        {
            if (sz.Trim().Length == 0)
            {
                return 0;
            }

            bool bWrapText = (nFormat & NativeMethods.DT_WORDBREAK) != 0;

            NativeMethods.RECT rect = NativeMethods.RECT.FromXYWH(rc.X, rc.Y, rc.Width, rc.Height);
            int nRight = rc.Width + 1;
            int nLineBreaks = 0;

            int dyHeight;
            int tmExternalLeading;
            if (Marshal.SystemDefaultCharSize == 1)
            {
                NativeMethods.TEXTMETRICA tm = new NativeMethods.TEXTMETRICA();
                NativeMethods.GetTextMetricsA(pDC, ref tm);
                dyHeight = tm.tmHeight + tm.tmExternalLeading;
                tmExternalLeading = tm.tmExternalLeading;
            }
            else
            {
                NativeMethods.TEXTMETRICW tm = new NativeMethods.TEXTMETRICW();
                NativeMethods.GetTextMetricsW(pDC, ref tm);
                dyHeight = tm.tmHeight + tm.tmExternalLeading;
                tmExternalLeading = tm.tmExternalLeading;
            }

            if (dyHeight <= 0)
            {
                return 0;
            }

            int nLine;
            if ((rc.Height - (dyHeight / 3)) <= dyHeight)
            {
                nLine = 0;
            }
            else
            {
                nLine = Math.Max(0, rc.Height / dyHeight);
            }

            int nCount = sz.Length;

            NativeMethods.SIZE size = new NativeMethods.SIZE();
            int n = 0;

            while (n < nCount)
            {
                int nWords = 0;
                int nIndex = n, nIndexLastWord;
                int nLineLength, nLineLengthLastWord;

                nLineLength = 0;

                do
                {
                    nLineLengthLastWord = nLineLength;

                    nIndexLastWord = nIndex;

                    // skip blanks (before word)
                    while (nIndex < nCount && sz[nIndex] == ' ')
                    {
                        nIndex++;
                        nLineLength++;
                    }

                    // regular characters
                    while (nIndex < nCount && " \r\n".IndexOf(sz[nIndex]) == -1)
                    {
                        nLineLength++;
                        nIndex++;
                    }
                    
                    NativeMethods.GetTextExtentPoint32(pDC, sz.Substring(n), nLineLength, ref size);

                    nWords++;

                    // skip blanks (after word)
                    while (nIndex < nCount && sz[nIndex] == ' ')
                    {
                        nIndex++;
                        nLineLength++;
                    }
                    // repeat until the word does not fit into the line
                } 
                while (size.cx <= nRight && nIndex < nCount && sz[nIndex] != '\r' && sz[nIndex] != '\n');

                rect.top += size.cy + 1;
                LineBreaks[nLineBreaks] = n;

                if (nIndex < nCount && sz[nIndex] == '\r' && size.cx <= nRight)
                {
                    // manual line feed
                    NativeMethods.GetTextExtentPoint32(pDC, sz.Substring(n), nLineLength, ref size);
                    LineLengths[nLineBreaks++] = nLineLength;

                    if (sz[n = nIndex + 1] == '\n')
                    {
                        n++;
                    }

                    if (n == nCount)
                    {
                        LineBreaks[nLineBreaks] = n;
                        LineLengths[nLineBreaks++] = 0;
                    }
                }
                else if (nIndex < nCount && sz[nIndex] == '\n' && size.cx <= nRight)
                {
                    // manual line feed
                    NativeMethods.GetTextExtentPoint32(pDC, sz.Substring(n), nLineLength, ref size);
                    LineLengths[nLineBreaks++] = nLineLength;
                    n = nIndex + 1;

                    if (n == nCount)
                    {
                        LineBreaks[nLineBreaks] = n;
                        LineLengths[nLineBreaks++] = 0;
                    }
                }
                else if (size.cx <= nRight)
                {
                    // rest of text completly fits into the line
                    LineLengths[nLineBreaks++] = nLineLength;
                    break;
                }
                else if (nWords > 1 && nLine > 0 && bWrapText)
                {
                    // several words, cut last word
                    LineLengths[nLineBreaks++] = nLineLengthLastWord;
                    n = nIndexLastWord;
                }
                else
                {
                    // only one word or only one line
                    nIndex = n;
                    nLineLength = 0;
                    size.cx = 0;

                    do
                    {
                        nLineLength++;
                        nIndex++;
                        NativeMethods.GetTextExtentPoint32(pDC, sz.Substring(n), nLineLength + 1, ref size);
                    }
                    while (nIndex < nCount && size.cx <= nRight);

                    LineLengths[nLineBreaks++] = nLineLength;
                    n = nIndex;
                }

                // next line
                if (nLine-- == 0 || nLineBreaks == nMaxBreaks || !bWrapText)
                {
                    break;
                }
            }

            // check vertical alignment
            if (rect.top < rect.bottom)
            {
                if ((nFormat & NativeMethods.DT_VCENTER) != 0)
                {
                    rc.Y = rc.Y + ((rc.Bottom - rect.top + tmExternalLeading) / 2);
                }
                else if ((nFormat & NativeMethods.DT_BOTTOM) != 0)
                {
                    rc.Y = rc.Y + (rc.Bottom - rect.top);
                }
            }

            return nLineBreaks;
        }

        internal int GetMultiLineTextBreakCount(IntPtr pDC, string lpszString, int nCount, Rectangle rc, int nFormat)
        {
            int[] LineBreaks = new int[32];
            int[] LineLengths = new int[32];

            int nLineBreaks = ComputeLineBreaks(pDC, lpszString, ref rc, nFormat, LineBreaks, LineLengths, 32);

            return nLineBreaks;
        }

        internal int DrawTextLikeMultiLineEdit(IntPtr pDC, string lpszString, Rectangle rc, int nFormat, Rectangle lpClipRect)
        {
            int[] LineBreaks = new int[32];
            int[] LineLengths = new int[32];

            int nCount = lpszString.Length;

            // regular text
            int nLineBreaks = ComputeLineBreaks(pDC, lpszString, ref rc, nFormat, LineBreaks, LineLengths, 32);

            NativeMethods.RECT rect = NativeMethods.RECT.FromXYWH(rc.X, rc.Y, rc.Width, rc.Height);
            if (lpClipRect.IsEmpty)
            {
                lpClipRect = rc;
            }

            int dyHeight;
            if (Marshal.SystemDefaultCharSize == 1)
            {
                NativeMethods.TEXTMETRICA tm = new NativeMethods.TEXTMETRICA();
                NativeMethods.GetTextMetricsA(pDC, ref tm);
                dyHeight = tm.tmHeight + tm.tmExternalLeading;
            }
            else
            {
                NativeMethods.TEXTMETRICW tm = new NativeMethods.TEXTMETRICW();
                NativeMethods.GetTextMetricsW(pDC, ref tm);
                dyHeight = tm.tmHeight + tm.tmExternalLeading;
            }

            // EDIT uses different height for larger fonts.
            // This value has been determined by experiment. I simply
            // compared the results by using different values and
            // 20 seemed to work fine.
            if (dyHeight >= 20)
            {
                dyHeight--;
            }

            for (int nBreak = 0; nBreak < nLineBreaks; nBreak++)
            {
                int nLeft = -1;
                string pszLine = lpszString.Substring(LineBreaks[nBreak]);
                int nBytes = LineLengths[nBreak];
                NativeMethods.SIZE size = new NativeMethods.SIZE();
                bool bNeedClip = false;
                ////Rectangle rectOldClip;

                //// Implement some special clipping when text is
                //// only one character. Let's show as much as possible
                //// of this one character if it must be clipped
                //// (possibly also shift it to the left boundary of
                //// the clipping rectangle).
                if (nBytes == pszLine.Length)
                {
                    NativeMethods.GetTextExtentPoint32(pDC, pszLine, nBytes, ref size);

                    //// clip when only one visible char does not
                    //// fit into clipping rectangle
                    if (size.cx > lpClipRect.Width)
                    {
                        bNeedClip = true;
                        nLeft = lpClipRect.Left;
                    }
                    else if (size.cx > rect.Width)
                    {
                        //// adjust left so that the char fits into the
                        //// clipping rectangle
                        nLeft = Math.Min(lpClipRect.Right - size.cx, rect.left);
                    }
                }

                //// Adjust left so that text will be correctly aligned
                if (nLeft == -1 && (nFormat & (NativeMethods.DT_CENTER | NativeMethods.DT_RIGHT)) != 0)
                {
                    if (size.cx == 0)
                    {
                        NativeMethods.GetTextExtentPoint32(pDC, pszLine, nBytes, ref size);
                    }

                    if ((nFormat & NativeMethods.DT_RIGHT) != 0)
                    {
                        nLeft = Math.Max(rect.left, rect.right - size.cx);
                    }
                    else if ((nFormat & NativeMethods.DT_CENTER) != 0)
                    {
                        nLeft = Math.Max(rect.left, rect.left + ((rect.right - rect.left - size.cx) / 2));
                    }
                }

                //// default is to use the left of the preffered rectangle
                if (nLeft == -1)
                {
                    nLeft = rect.left;
                }

                if (bNeedClip)
                {
                    bool bCanClip = true;

                    //// Metafiles don't support SelectClipRgn!
                    ////                        bCanClip = false;

                    if (bCanClip)
                    {
                        // It's an expensive operation but there is no way
                        // around clipping.
                        NativeMethods.RECT oldClip = new NativeMethods.RECT();
                        NativeMethods.GetClipBox(pDC, ref oldClip);
                        NativeMethods.IntersectClipRect(pDC, lpClipRect.X, lpClipRect.Y, lpClipRect.Right, lpClipRect.Bottom);

                        NativeMethods.ExtTextOut(pDC, nLeft, rect.top, 0, ref oldClip, pszLine, nBytes, null);

                        //// restore original clipping region
                        NativeMethods.SelectClipRgn(pDC, IntPtr.Zero);
                        NativeMethods.IntersectClipRect(pDC, oldClip.left, oldClip.top, oldClip.right, oldClip.bottom);
                    }
                    // else
                    // there is no way that we can draw that character
                    // without messing up the other drawing. So, the
                    // compromise is not to print that half char
                }
                else
                {
                    // simply draw the text without clipping (that's very fast)
                    NativeMethods.ExtTextOut(pDC, nLeft, rect.top, 0, ref rect, pszLine, nBytes, null);
                }

                rect.top += dyHeight;
            }

            return 0;
        }

        IntPtr CreateHFont(Font font)
        {
            return font.ToHfont();
            ////            NativeMethods.LOGFONT logFont = new NativeMethods.LOGFONT();
            ////            logFont.lfFaceName = font.Name;
            ////            logFont.lfHeight = (int) font.SizeInPoints;
            ////            logFont.lfItalic = (byte) (font.Italic ? 1 : 0);
            ////            logFont.lfStrikeOut = (byte) (font.Strikeout ? 1 : 0);
            ////            logFont.lfUnderline = (byte) (font.Underline ? 1 : 0);
            ////            logFont.lfWeight = font.Bold ? 900 : 300;
            ////            return NativeMethods.CreateFontIndirect(ref logFont);
        }

        IntPtr CreateHFont(GridFontInfo font)
        {
            return CreateHFont(font.GdipFont);
            ////            NativeMethods.LOGFONT logFont = new NativeMethods.LOGFONT();
            ////            logFont.lfFaceName = font.Facename;
            ////            logFont.lfHeight = (int) (font.Size * 128/72f); // TODO: Take also FontUnit and LOGPIXELSX into consideration
            ////            logFont.lfItalic = (byte) (font.Italic ? 1 : 0);
            ////            logFont.lfStrikeOut = (byte) (font.Strikeout ? 1 : 0);
            ////            logFont.lfUnderline = (byte) (font.Underline ? 1 : 0);
            ////            logFont.lfWeight = font.Bold ? 900 : 300;
            ////            return NativeMethods.CreateFontIndirect(ref logFont);
        }

        IntPtr CreateHBrush(BrushInfo brush)
        {
            NativeMethods.LOGBRUSH lb = new NativeMethods.LOGBRUSH();
            lb.lbColor = NativeMethods.COLORREFToRGB(brush.BackColor.ToArgb());
            return NativeMethods.CreateBrushIndirect(ref lb);
        }
    }

    class GridWeakRefHFont : Disposable
    {
        Hashtable table;
        IntPtr hfont;

        public GridWeakRefHFont(Hashtable table, IntPtr hfont)
        {
            this.table = table;
            this.hfont = hfont;
        }

        protected override void Dispose(bool disposing)
        {
            NativeMethods.DeleteObject(hfont);
            base.Dispose(disposing);
        }

        public IntPtr Hfont
        {
            get
            {
                return this.hfont;
            }

            set
            {
                this.hfont = value;
            }
        }
    }

    class GridWeakRefHBrush : Disposable
    {
        Hashtable table;
        IntPtr hbrush;

        public GridWeakRefHBrush(Hashtable table, IntPtr hbrush)
        {
            this.table = table;
            this.hbrush = hbrush;
        }

        protected override void Dispose(bool disposing)
        {
            NativeMethods.DeleteObject(hbrush);
            base.Dispose(disposing);
        }

        public IntPtr Hbrush
        {
            get
            {
                return this.hbrush;
            }

            set
            {
                this.hbrush = value;
            }
        }
    }

#if obsolete
        [
            ComVisible(false),
            System.Security.SuppressUnmanagedCodeSecurity(),
            Syncfusion.Documentation.DocumentationExclude()
        ]
        class NativeMethods
        {

            [StructLayout(LayoutKind.Sequential),
                Syncfusion.Documentation.DocumentationExclude()]
                internal struct POINT
            {
                internal int X;
                internal int Y;
        
                internal POINT(int x, int y)  
                {
                    this.X = x;
                    this.Y = y;
                }
            }

            [StructLayout(LayoutKind.Sequential),
                Syncfusion.Documentation.DocumentationExclude()]
                internal struct SIZE
            {
                internal int cx;
                internal int cy;
        
                internal SIZE(int cx, int cy)  
                {
                    this.cx = cx;
                    this.cy = cy;
                }
            }

            [StructLayout(LayoutKind.Sequential),
                Syncfusion.Documentation.DocumentationExclude()]
                internal struct RECT 
            {
                internal int left;
                internal int top;
                internal int right;
                internal int bottom;

                internal RECT(Rectangle rect)
                {
                    this.bottom = rect.Bottom;
                    this.left = rect.Left;
                    this.right = rect.Right;
                    this.top = rect.Top;
                }

                internal RECT(int left, int top, int right, int bottom)
                {
                    this.bottom = bottom;
                    this.left = left;
                    this.right = right;
                    this.top = top;
                }                
            
                internal static RECT FromXYWH(int x, int y, int width, int height)
                {
                    return new RECT(x, y, x+width, y+height);
                }

                internal int Width{get{return this.right-this.left;}}
                internal int Height{get{return this.bottom-this.top;}}
                
                public override /*Object*/ string ToString()
                {
                    return String.Concat(
                        "Left = ",
                        this.left,
                        " Top ",
                        this.top,
                        " Right = ",
                        this.right,
                        " Bottom = ",
                        this.bottom);
                } 
            }

            [StructLayout(LayoutKind.Sequential),
                Syncfusion.Documentation.DocumentationExclude()] 
                internal class COMRECT 
            {
                
                internal int left;
                internal int top;
                internal int right;
                internal int bottom;
            
                internal COMRECT()
                {
                } 
            
                internal COMRECT(int left, int top, int right, int bottom)
                {
                    this.left = left;
                    this.top = top;
                    this.right = right;
                    this.bottom = bottom;
                    return;
                } 
            
                // Methods
                public override /*Object*/ string ToString()
                {
                    return String.Concat(
                        "Left = ",
                        this.left,
                        " Top ",
                        this.top,
                        " Right = ",
                        this.right,
                        " Bottom = ",
                        this.bottom);
                } 
            
                internal static COMRECT FromXYWH(int x, int y, int width, int height)
                {
                    return new NativeMethods.COMRECT(x, y, (x + width), (y + height));
                } 
            
            } 

            [StructLayout(LayoutKind.Sequential),
                Syncfusion.Documentation.DocumentationExclude()] 
                internal struct MSG 
            {
                
                internal IntPtr hwnd;
                internal int message;
                internal IntPtr wParam;
                internal IntPtr lParam;
                internal int time;
                internal int pt_x;
                internal int pt_y;
            }

            [StructLayout(LayoutKind.Sequential),
                Syncfusion.Documentation.DocumentationExclude()]
                internal struct LOGBRUSH 
            {
                internal int lbStyle;
                internal int lbColor;
                internal IntPtr lbHatch;
            }
        
            [StructLayout(LayoutKind.Sequential),
                Syncfusion.Documentation.DocumentationExclude()]
                internal struct LOGFONT 
            {
                internal int lfHeight; 
                internal int lfWidth; 
                internal int lfEscapement; 
                internal int lfOrientation; 
                internal int lfWeight; 
                internal byte lfItalic; 
                internal byte lfUnderline; 
                internal byte lfStrikeOut; 
                internal byte lfCharSet; 
                internal byte lfOutPrecision; 
                internal byte lfClipPrecision; 
                internal byte lfQuality; 
                internal byte lfPitchAndFamily; 
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)] internal string lfFaceName;
            }
        

            [Syncfusion.Documentation.DocumentationExclude()]
            internal static int HIWORD(int n)
            {
                return ((n >> 16) & 0xffff/*=~0x0000*/);
            } 
        
            [Syncfusion.Documentation.DocumentationExclude()]
            internal static int LOWORD(int n)
            {
                return (n & 0xffff/*=~0x0000*/);
            } 

            [Syncfusion.Documentation.DocumentationExclude()]
            internal static int LOWORD(IntPtr n)  
            {
                return LOWORD((int) n);
            }
            
            [Syncfusion.Documentation.DocumentationExclude()]
            internal static int HIWORD(IntPtr n)  
            {
                return HIWORD((int) n);
            }

            [Syncfusion.Documentation.DocumentationExclude()]
            internal static int MAKELONG(int low, int high)  
            {
                return ((high << 16) | (low & 0xffff));
            }

            [Syncfusion.Documentation.DocumentationExclude()]
            internal static int MAKELPARAM(int low, int high)  
            {
                return ((high << 16) | (low & 0xffff));
            }

            [Syncfusion.Documentation.DocumentationExclude()]
            internal static int RGBToCOLORREF(int rgbValue)
            {
                int n0;
                n0 = (rgbValue & 255/*0xff*/) << 16/*0x10*/;
                rgbValue = (rgbValue & 16776960/*0xffff00*/);
                rgbValue = (rgbValue 
                    | (rgbValue >> 16/*0x10*/ & 255/*0xff*/));
                rgbValue = (rgbValue & 65535/*0xffff*/);
                rgbValue = (rgbValue | n0);
                return rgbValue;
            }

            [Syncfusion.Documentation.DocumentationExclude()]
            internal static int COLORREFToRGB(int colorRef)
            {
                int r = colorRef & 255/*0xff*/;
                int g = (colorRef >> 8) & 255/*0xff*/;
                int b = (colorRef >> 16) & 255/*0xff*/;

                int rgb = (r << 16) + (g << 8) + b;

                return rgb;
            }

            [Syncfusion.Documentation.DocumentationExclude()]
            internal static int GetRValue(int rgb)
            {
                return (rgb & 0xff0000) >> 16;
            }

            [Syncfusion.Documentation.DocumentationExclude()]
            internal static int GetGValue(int rgb)
            {
                return (rgb & 0x00ff00) >> 8;
            }

            [Syncfusion.Documentation.DocumentationExclude()]
            internal static int GetBValue(int rgb)
            {
                return (rgb & 0x0000ff);
            }

            public const int ETO_OPAQUE                  = 0x0002;
            public const int ETO_CLIPPED                  = 0x0004;
            public const int TRANSPARENT         = 1;
            public const int OPAQUE              =2;
            public const int PATCOPY = 15728673 /*0xF00021*/;
            public const int PATINVERT = 5898313 /*0x5A0049*/;

            public const int TA_NOUPDATECP               = 0;
            public const int TA_UPDATECP                 = 1;
            public const int TA_LEFT                     = 0;
            public const int TA_RIGHT                    = 2;
            public const int TA_CENTER                   = 6;
            public const int TA_TOP                      = 0;
            public const int TA_BOTTOM                   = 8;
            public const int TA_BASELINE                 = 24;


            public const int DT_LEFT = 0 /*0x0000*/;
            public const int DT_CENTER = 1;
            public const int DT_RIGHT = 2 /*0x0002*/;
            public const int DT_VCENTER = 4 /*0x0004*/;
            public const int DT_BOTTOM = 8 /*0x0004*/;
            public const int DT_WORDBREAK = 16 /*0x0004*/;
            public const int DT_SINGLELINE = 32 /*0x0020*/;
            public const int DT_NOCLIP = 256 /*0x0100*/;
            public const int DT_CALCRECT = 1024 /*0x0400*/;
            public const int DT_NOPREFIX = 2048 /*0x0800*/;
            public const int DT_EDITCONTROL = 8192 /*0x2000*/;
            public const int DT_EXPANDTABS = 64 /*0x0040*/;
            public const int DT_END_ELLIPSIS = 32768 /*0x8000*/;
            public const int DT_RTLREADING = 131072 /*0x20000*/;
            public const int DT_WORD_ELLIPSIS = 0x00040000;
            public const int DT_PATH_ELLIPSIS = 0x00004000;



            [StructLayout(LayoutKind.Sequential)] 
                internal struct TEXTMETRIC 
            {
                internal int tmHeight;
                internal int tmAscent;
                internal int tmDescent;
                internal int tmInternalLeading;
                internal int tmExternalLeading;
                internal int tmAveCharWidth;
                internal int tmMaxCharWidth;
                internal int tmWeight;
                internal int tmOverhang;
                internal int tmDigitizedAspectX;
                internal int tmDigitizedAspectY;
                internal char tmFirstChar;
                internal char tmLastChar;
                internal char tmDefaultChar;
                internal char tmBreakChar;
                internal byte tmItalic;
                internal byte tmUnderlined;
                internal byte tmStruckOut;
                internal byte tmPitchAndFamily;
                internal byte tmCharSet;
            }

            [DllImport("gdi32.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
            public static extern int GetTextExtentPoint32(IntPtr hDC, string str, int len, ref NativeMethods.SIZE size); 
   
            [DllImport("gdi32.dll", CharSet=CharSet.Auto, ExactSpelling=true, CallingConvention=CallingConvention.Winapi)] 
            public static extern bool GetClipBox(IntPtr hdc, ref NativeMethods.RECT lpRect); 
        
            [DllImport("gdi32.dll", CharSet=CharSet.Auto, ExactSpelling=true, CallingConvention=CallingConvention.Winapi)] 
            public static extern int IntersectClipRect(IntPtr hDC, int x1, int y1, int x2, int y2); 

            [DllImport("gdi32.dll", CharSet=CharSet.Auto, ExactSpelling=true, CallingConvention=CallingConvention.Winapi)] 
            public static extern int SelectClipRgn(IntPtr hDC, IntPtr hRgn); 

            [DllImport("gdi32.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
            public static extern bool GetTextMetricsW(IntPtr hdc, ref NativeMethods.TEXTMETRIC tm); 
                
            [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true, CallingConvention=CallingConvention.Winapi)] 
            public static extern int FillRect(IntPtr hdc, ref NativeMethods.RECT rect, IntPtr hbrush); 

            [DllImport("gdi32.dll", CharSet=CharSet.Auto, ExactSpelling=true, CallingConvention=CallingConvention.Winapi)] 
            public static extern int SetBkColor(IntPtr hDC, int clr); 

            [DllImport("gdi32.dll", CharSet=CharSet.Auto, ExactSpelling=true, CallingConvention=CallingConvention.Winapi)] 
            public static extern int SetTextAlign(IntPtr hDC, int align); 

            [DllImport("gdi32.dll", CharSet=CharSet.Auto, ExactSpelling=true, CallingConvention=CallingConvention.Winapi)] 
            public static extern int SetTextColor(IntPtr hDC, int clr); 

            [DllImport("gdi32", CharSet=CharSet.Auto, ExactSpelling=true)]
            extern internal static IntPtr CreateBrushIndirect(ref LOGBRUSH lb)  ;

            [DllImport("gdi32", CharSet=CharSet.Auto, ExactSpelling=true)]
            extern internal static IntPtr SelectObject(IntPtr hdc, IntPtr hObject)  ;

            [DllImport("gdi32", CharSet=CharSet.Auto)]
            extern internal static bool ExtTextOut(IntPtr hdc, int x, int y, int nOptions, ref RECT lpRect, string s, int nStrLength, int[] lpDx)  ;

            [DllImport("gdi32", CharSet=CharSet.Auto, ExactSpelling=true)]
            extern internal static bool PatBlt(IntPtr hdc, int x, int y, int nWidth, int nHeight, int dwRop)  ;

            [DllImport("gdi32")]
            extern internal static bool DeleteObject(IntPtr hObject)  ;

            [DllImport("gdi32", CharSet=CharSet.Auto, ExactSpelling=true)]
            public static extern IntPtr CreateFontIndirectW(ref LOGFONT lplf); 
    
            [DllImport("gdi32.dll", CharSet=CharSet.Auto, ExactSpelling=true, CallingConvention=CallingConvention.Winapi)] 
            public static extern int SetBkMode(IntPtr hDC, int nBkMode); 
    
            [DllImport("user32.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
            public static extern int DrawTextW(IntPtr hDC, string lpszString, int nCount, ref NativeMethods.RECT lpRect, int nFormat); 
        
        }
#endif
}

