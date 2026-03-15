#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Node that renders rich text.
    /// </summary>
    [Serializable]
    public class RichTextNode
        : Node
    {
        #region Class members
        private string m_strText;
        private string m_strRTFText;
        private bool m_bReadOnly;
        private Color m_clrBackground;
        private bool m_bUseBitmap = false;
        private TextCases m_txtCase = TextCases.None;
        private string m_strOriginalText = string.Empty;
        private const string c_str_DEF_RTF_HEADER = @"^({\\rtf1)";
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextNode"/> class.
        /// </summary>
        /// <param name="strRTFText">Text value including RTF codes.</param>
        /// <param name="rectBounds">RichTextNode bounding rectangle.</param>
        /// <param name="measureUnits">Specifies points measure units.</param>
        public RichTextNode(string strRTFText, RectangleF rectBounds, MeasureUnits measureUnits)
            : base()
        {
            if (rectBounds.Height <= 0 || rectBounds.Height <= 0)
                throw new ArgumentOutOfRangeException("Neither of RichTextNode dimension values can be 0 or less!!");

            rectBounds = MeasureUnitsConverter.ToPixels(rectBounds, measureUnits);
            //// calc pin offset
            SizeF szPinOffsetUnitIndependent = new SizeF(rectBounds.Width / 2, rectBounds.Height / 2);
            //// assign default PinLocation
            PointF ptPinPointUnitIndependent = new PointF(
                rectBounds.Location.X + szPinOffsetUnitIndependent.Width,
                rectBounds.Location.Y + szPinOffsetUnitIndependent.Height);
            //// assign node size value
            SizeF szSizeUnitIndependent = rectBounds.Size;
            //// Init BoundsInfo
            CreateBoundsInfo(ptPinPointUnitIndependent, szPinOffsetUnitIndependent, szSizeUnitIndependent);

            this.BoundsInfo.Unit = measureUnits;
            
            // assign new RTF text value
            this.RichText = strRTFText;
            UpdateBoundingRectangle();

            m_clrBackground = Color.Transparent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextNode"/> class.
        /// </summary>
        /// <param name="strRTFText">The STR RTF text.</param>
        /// <param name="rectBounds">The rect bounds.</param>
        public RichTextNode(string strRTFText, RectangleF rectBounds)
            : base()
        {
            if (rectBounds.Height <= 0 || rectBounds.Height <= 0)
                throw new ArgumentOutOfRangeException("Neither of RichTextNode dimension values can be 0 or less!!");

            // calc pin offset
            SizeF szPinOffsetUnitIndependent = new SizeF(rectBounds.Width / 2, rectBounds.Height / 2);

            // assign default PinLocation
            PointF ptPinPointUnitIndependent = new PointF(
                rectBounds.Location.X + szPinOffsetUnitIndependent.Width,
                rectBounds.Location.Y + szPinOffsetUnitIndependent.Height);

            // assign node size value
            SizeF szSizeUnitIndependent = rectBounds.Size;

            // Init BoundsInfo
            CreateBoundsInfo(ptPinPointUnitIndependent, szPinOffsetUnitIndependent, szSizeUnitIndependent);

            this.BoundsInfo.Unit = MeasureUnits.Pixel;

            // assign new RTF text value
            this.RichText = strRTFText;

            UpdateBoundingRectangle();
            m_clrBackground = Color.Transparent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextNode"/> class.
        /// </summary>
        /// <param name="src">Source object to copy from.</param>
        public RichTextNode(RichTextNode src)
            : base(src)
        {
            m_strText = src.m_strText;
            m_strRTFText = src.m_strRTFText;
            m_clrBackground = src.m_clrBackground;
            m_txtCase = src.m_txtCase;
            if (m_txtCase == TextCases.None)
                m_strOriginalText = ParseRtfText(m_strRTFText);
            UpdateBoundingRectangle();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextNode"/> class.
        /// </summary>
        /// <param name="info">Serialization state information.</param>
        /// <param name="context">Streaming context information.</param>
        protected RichTextNode(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_bUseBitmap = false;
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "colorBackground":
                        m_clrBackground = (Color)info.GetValue("colorBackground", typeof(Color));
                        break;
                    case "readonly":
                        m_bReadOnly = info.GetBoolean("readonly");
                        break;
                    case "rtf":
                        m_strRTFText = info.GetString("rtf");
                        break;
                    case "text":
                        m_strText = info.GetString("text");
                        break;
                    case "usebitmap":
                        m_bUseBitmap = info.GetBoolean("usebitmap");
                        break;
                    case "textcase":
                        m_txtCase = (TextCases)info.GetValue("textcase", typeof(TextCases));
                        break;
                }
            }
            if (m_txtCase == TextCases.None)
                m_strOriginalText = ParseRtfText(m_strRTFText);
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the value contained by the text object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Derived classes override this property in order to supply the
        /// text value in an implementation specific way.
        /// </para>
        /// </remarks>
        [Browsable(true)]
        [Category("General")]
        [Description("Text value to be displayed.")]
        public string Text
        {
            get 
            { 
                return m_strText; 
            }
            set
            {
                if (!m_bReadOnly && m_strText != value && OnPropertyChanging(this.FullContainerName, DPN.Text, value))
                {
                    // Create RichTextBox in order to update RTF text
                    using (RichTextBox rtfBoxTemp = new RichTextBox())
                    {
                        rtfBoxTemp.Rtf = m_strRTFText;
                        rtfBoxTemp.Text = value;

                        // update RTF text
                        m_strRTFText = rtfBoxTemp.Rtf;
                    }

                    // make history record
                    RecordPropertyChanged(DPN.Text);

                    // assign new value
                    m_strText = value;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.Text);
                }
            }
        }

        /// <summary>
        /// Gets or sets rich text include all RTF codes.
        /// </summary>
        [Browsable(true)]
        [Category("General")]
        [Description("Contains rich text include all RTF codes.")]
        public string RichText
        {
            get 
            { 
                return m_strRTFText; 
            }
            set
            {
                if (!m_bReadOnly && m_strRTFText != value && OnPropertyChanging(this.FullContainerName, DPN.RichText, value))
                {
                    // Create RichTextBox in order to update RTF text
                    using (RichTextBox rtfBoxTemp = new RichTextBox())
                    {
                        rtfBoxTemp.Rtf = value;

                        // update text
                        m_strText = rtfBoxTemp.Text;
                    }

                    // make history record
                    RecordPropertyChanged(DPN.RichText);

                    // assign new value
                    m_strRTFText = value;

                    if (m_txtCase == TextCases.None)
                        m_strOriginalText = ParseRtfText(m_strRTFText);

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.RichText);
                }
            }
        }

        /// <summary>
        /// Gets or sets case of text in the RichTextNode
        /// </summary>
        [Browsable(true)]
        [DefaultValue(TextCases.None)]
        [Category("Formatting")]
        [Description("Specifies the text case sensitive.")]
        public TextCases TextCase
        {
            get
            {
                return m_txtCase;
            }
            set
            {
                if (value != m_txtCase && OnPropertyChanging(this.FullContainerName, DPN.TextCase, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.TextCase);

                    // set new value
                    m_txtCase = value;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.TextCase);
                    UpdateText();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the text object is Read-only or not.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("Flag indicating if the text object is Read-only or not.")]
        public bool ReadOnly
        {
            get 
            { 
                return m_bReadOnly; 
            }
            set
            {
                if (m_bReadOnly != value && OnPropertyChanging(this.FullContainerName, DPN.ReadOnly, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.ReadOnly);

                    // assign new value
                    m_bReadOnly = value;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.ReadOnly);
                }
            }
        }

        /// <summary>
        /// Gets or sets color used to fill the background.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color used to fill the background.")]
        public Color BackgroundColor
        {
            get 
            { 
                return m_clrBackground; 
            }
            set
            {
                if (m_clrBackground != value && OnPropertyChanging(this.FullContainerName, DPN.BackgroundColor, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.BackgroundColor);

                    // assign new value
                    m_clrBackground = value;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.BackgroundColor);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to determine rendering mode.
        /// By default RichTextNode is rendered as EMF.
        /// </summary>
        public bool UseBitmap
        {
            get
            {
                return m_bUseBitmap;
            }
            set
            {
                if (value != m_bUseBitmap)
                    m_bUseBitmap = value;
            }
        }
        #endregion

        #region Class overrides

        #region rendering
        /// <summary>
        /// Renders shapes visual representation on given graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on</param>
        protected override void Render(Graphics gfx)
        {
            // 1 - call base method impementation
            base.Render(gfx);

            // 2 - Draw interior
            DrawInterior(gfx);

            // 3 - Draw border
            DrawBorder(gfx);
        }

        private void DrawBorder(Graphics gfx)
        {
            if (this.LineStyle.LineWidth > 0)
            {
                using (Pen pen = this.LineStyle.CreatePen())
                    gfx.DrawPath(pen, this.GraphicsPath);
            }
        }

        /// <summary>
        /// Draws node's interior on given graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on</param>
        private void DrawInterior(Graphics gfx)
        {
            // render text to given bitmap
            if (this.Text == string.Empty)
                return;

            SizeF szSize = MeasureUnitsConverter.ToPixels(this.Size, this.MeasurementUnit);
            System.Drawing.Rectangle rectBounds =
                new System.Drawing.Rectangle(0, 0, (int)Math.Ceiling(szSize.Width), (int)Math.Ceiling(szSize.Height));

            float fZoom = gfx.PageScale;
            System.Drawing.Drawing2D.GraphicsState save = gfx.Save();
            gfx.PageScale = 1;

            gfx.ScaleTransform(fZoom, fZoom, System.Drawing.Drawing2D.MatrixOrder.Append);
            gfx.SetClip(rectBounds);

            using (Image img = m_bUseBitmap ? RenderRTFToBitmap(rectBounds) : RenderRTFToMetafile(rectBounds))
            {
                gfx.FillRectangle(new SolidBrush(this.BackgroundColor), rectBounds);

                ImageAttributes imgAttr = new ImageAttributes();
                imgAttr.SetColorKey(Color.White, Color.Transparent);
                PointF[] destPoints = 
                { 
                    rectBounds.Location, 
                    new PointF( rectBounds.Right, rectBounds.Top ),
                    new PointF( rectBounds.Left, rectBounds.Bottom )
                };

                gfx.DrawImage(img, destPoints, new RectangleF(0, 0, rectBounds.Width, rectBounds.Height), GraphicsUnit.Pixel, imgAttr);
            }

            gfx.Restore(save);
        }
        #endregion

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new RichTextNode(this);
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("colorBackground", m_clrBackground);
            info.AddValue("readonly", m_bReadOnly);
            info.AddValue("rtf", m_strRTFText);
            info.AddValue("text", m_strText);
            info.AddValue("usebitmap", m_bUseBitmap);
            info.AddValue("textcase", m_txtCase);
        }
        #endregion

        #region Class helper methods
        private void UpdateText()
        {
            if (m_txtCase == TextCases.AllUpper)
                Text = Text.ToUpper();
            else if (m_txtCase == TextCases.AllLower)
                Text = Text.ToLower();
            else
                Text = m_strOriginalText;
        }

        private string ParseRtfText(string rtfText)
        {
            string strResult = rtfText;

            if (strResult != null && System.Text.RegularExpressions.Regex.Match(strResult, c_str_DEF_RTF_HEADER).Success)
            {
                // if current text in rtf format
                using (RichTextBox richConvertor = new RichTextBox())
                {
                    richConvertor.Rtf = strResult;
                    strResult = richConvertor.Text;
                }
            }

            return strResult;
        }

        private Image RenderRTFToBitmap(System.Drawing.Rectangle rectBmpBounds)
        {
            Model model = this.Root;

            // create bmp
            Bitmap bmpToReturn = new Bitmap(rectBmpBounds.Width, rectBmpBounds.Height, PixelFormat.Format32bppArgb);
            Color backcolor;

            // create graphics to draw on
            RichTextBoxDrawing.m_bTransparent = false;
            using (RichTextBoxDrawing rchTemp = new RichTextBoxDrawing())
            using (Graphics bmpgrfx = Graphics.FromImage(bmpToReturn))
            {
                // fill background
                if (model != null)
                    backcolor = model.BackgroundStyle.Color;
                else
                    backcolor = Color.White;

                bmpgrfx.Clear(backcolor);

                rchTemp.BackColor = backcolor;

                // set default parameters
                rchTemp.Rtf = this.RichText;
                rchTemp.WordWrap = true;

                // draw background
                rchTemp.Draw(bmpgrfx, rectBmpBounds);
            }
            bmpToReturn.MakeTransparent(backcolor);
            RichTextBoxDrawing.m_bTransparent = true;
            return bmpToReturn;
        }

        /// <summary>
        /// Draw formated text to metafile.
        /// </summary>
        /// <param name="rectBmpBounds">Metafile size.</param>
        /// <returns>RTF To Metafile image.</returns>
        private Image RenderRTFToMetafile(System.Drawing.Rectangle rectBmpBounds)
        {
            Metafile metafile = null;
            using (RichTextBoxDrawing rchTemp = new RichTextBoxDrawing())
            using (Bitmap bmp = new Bitmap(1, 1))
            using (Graphics gfxBmp = Graphics.FromImage(bmp))
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream();

                // create metafile to return
                IntPtr hdc = gfxBmp.GetHdc();

                metafile = new Metafile(stream, hdc);

                // create graphics to draw on
                using (Graphics gfxmeta = Graphics.FromImage(metafile))
                {
                    // set default parameters
                    rchTemp.Rtf = this.RichText;
                    rchTemp.WordWrap = true;

                    // draw background
                    using (SolidBrush backBrush = new SolidBrush(this.BackgroundColor))
                    {
                        gfxmeta.FillRectangle(backBrush, rectBmpBounds);
                        rchTemp.Draw(gfxmeta, rectBmpBounds);
                    }
                }
            }

            return metafile;
        }

        #endregion
    }

    internal class RichTextBoxDrawing : RichTextBox
    {
        #region Costants
        /// <summary>
        /// Convert the unit used by the .NET framework (1/100 inch) 
        /// and the unit used by Win32 API calls (twips 1/1440 inch)
        /// </summary>
        private const double anInch = 14.4d;
        private const int EM_FORMATRANGE = 1081;
        public static bool m_bTransparent = true;
        #endregion

        #region Extern methods
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr LoadLibrary(string strFilename);
        [DllImport("USER32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        #endregion

        #region Constructor
        public RichTextBoxDrawing()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextBoxDrawing"/> class.
        /// </summary>
        /// <param name="bTransparent">if set to <c>true</c> [b transparent].</param>
        public RichTextBoxDrawing(bool bTransparent)
            : this()
        {
            m_bTransparent = bTransparent;
        }
        #endregion

        #region Overrides
        private bool Version6()
        {
            string path = "{0}\\Microsoft Shared\\OFFICE14\\RICHED20.DLL";
            string file = string.Format(path, Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles));
            if (System.IO.File.Exists(file))
            {
                LoadLibrary(file);
                return true;
            }
            return false;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (Version6())
                {
                    if (Marshal.SystemDefaultCharSize == 1)
                    {
                        if (m_bTransparent)
                            cp.ExStyle |= 0x020;
                        cp.ClassName = "RichEdit60A";
                    }
                    else
                    {
                        if (m_bTransparent)
                            cp.ExStyle |= 0x020;
                        cp.ClassName = "RichEdit60W";
                    }
                }
                else
                {
                    if (LoadLibrary("msftedit.dll") != IntPtr.Zero)
                    {
                        if (m_bTransparent)
                            cp.ExStyle |= 0x020; // Transparent
                        cp.ClassName = "RICHEDIT50W";
                    }
                }
                return cp;
            }
        }
        #endregion

        #region Helper structs
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CHARRANGE
        {
            /// <summary>
            /// First character of range (0 for start of doc)
            /// </summary>
            public int cpMin;

            /// <summary>
            /// Last character of range (-1 for end of doc)
            /// </summary>
            public int cpMax;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FORMATRANGE
        {
            /// <summary>
            /// Actual DC to draw on
            /// </summary>
            public IntPtr hdc;

            /// <summary>
            /// Target DC for determining text formattings
            /// </summary>
            public IntPtr hdcTarget;

            /// <summary>
            /// Region of the DC to draw to (in twips)
            /// </summary>
            public RECT rc;

            /// <summary>
            /// Region of the whole DC (page size) (in twips)
            /// </summary>
            public RECT rcPage;

            /// <summary>
            /// Range of text to draw (see earlier declaration)
            /// </summary>
            public CHARRANGE chrg;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Draw formatted text to specific graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="rcRect">Draw area.</param>
        public void Draw(Graphics gfx, System.Drawing.Rectangle rcRect)
        {
            double dConversation = 15;
            this.BorderStyle = BorderStyle.None;
            this.ScrollBars = RichTextBoxScrollBars.None;

            if (gfx.DpiX >= 600)
                dConversation = anInch;

            Draw(0, this.TextLength, gfx, rcRect.Left, rcRect.Top, rcRect.Right, rcRect.Bottom, dConversation);
        }
        private int Draw(int charFrom, int charTo, Graphics g, int nLeft, int nTop, int nRight, int nBottom, double conversion)
        {
            // Calculate the area to render
            RECT rectToDraw;
            rectToDraw.Top = (int)(nTop * conversion);
            rectToDraw.Bottom = (int)(nBottom * conversion);
            rectToDraw.Left = (int)(nLeft * conversion);
            rectToDraw.Right = (int)(nRight * conversion);

            IntPtr hdc = g.GetHdc();

            FORMATRANGE fmtRange;

            // Indicate character from to character to 
            fmtRange.chrg.cpMax = charTo;
            fmtRange.chrg.cpMin = charFrom;

            // Use the same DC for measuring and rendering
            fmtRange.hdc = hdc;
            fmtRange.hdcTarget = hdc;

            // Indicate the area on page
            fmtRange.rc = rectToDraw;
            fmtRange.rcPage = rectToDraw;

            // Get the pointer to the FORMATRANGE structure in memory
            IntPtr lparam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fmtRange));
            Marshal.StructureToPtr(fmtRange, lparam, false);

            // Send the rendered data for drawing 
            IntPtr wparam = new IntPtr(1);
            IntPtr res = SendMessage(Handle, EM_FORMATRANGE, wparam, lparam);

            // Free the block of memory allocated
            Marshal.FreeCoTaskMem(lparam);

            // Release the device context handle obtained by a previous call
            g.ReleaseHdc(hdc);

            // Return last + 1 character
            return res.ToInt32();
        }

        #endregion
    }
}