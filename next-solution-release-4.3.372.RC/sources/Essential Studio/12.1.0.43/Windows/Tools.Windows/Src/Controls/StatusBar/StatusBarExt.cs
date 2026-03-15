#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Tools.Controls.StatusBar
{
    /// <summary>
    /// This control represents StatusBarAdv control with own Themes Drawing support.
    /// This means ability to use Themed StatusBarAdv on
    /// non-themed OS.
    /// Use <see cref="ColorScheme"/> property to draw control with selected theme.
    /// <see cref="ThemesEnabled"/> property must be set to true to 
    /// draw control with theme.
    /// </summary>
    [Description("Represents a StatusBarAdv control with own Themes Drawing support")]
    public class StatusBarExt : StatusBarAdv,IVisualStyle
    {
        #region Class Constants

        /// <summary>
        /// Transparent color for grip image.
        /// </summary>
        private static readonly Color _clrTransparent = Color.Red;

        /// <summary>
        /// Name bitmap file for Gripper.
        /// </summary>
        private const string DEF_NAME_GRIP_IMAGE = "Grip.bmp";

        /// <summary>
        /// Path to images.
        /// </summary>
        private const string DEF_PATH_TO_IMAGES_FORMAT = "Syncfusion.Windows.Forms.Tools.Controls.StatusBar.Images.{0}";

        /// <summary>
        /// Width for left gradient.
        /// </summary>
        private const int DEF_LEFT_GRADIENT_WIDTH = 50;

        /// <summary>
        /// Height for top gradient.
        /// </summary>
        private const int DEF_TOP_GRADIENT_HEIGHT = 4;

        /// <summary>
        /// Height for bottom gradient.
        /// </summary>
        private const int DEF_BOOTOM_GRADIENT_HEIGHT = 6;


        #endregion


        #region Class Members

        /// <summary>
        /// Office2007 Color scheme.
        /// </summary>
        public Office2007Theme m_enColorScheme = Office2007Theme.Blue;
        /// <summary>
        /// Office2010 color scheme
        /// </summary>
        public Office2010Theme m_ColorScheme = Office2010Theme.Blue;
        /// <summary>
        ///StatusBarExt visual style
        /// </summary>
        public VisualStyle vStyle = VisualStyle.Default;
        /// <summary>
        /// 
        /// </summary>
        private Color clrTopGradient = Color.Empty;
        /// <summary>
        /// 
        /// </summary>
        private Color clrFillGradient = Color.Empty;
        /// <summary>
        /// 
        /// </summary>
        private Color clrBottomGradient = Color.Empty;
        
        #endregion

        #region Class Properties

        /// <summary>
        /// Gets or sets color scheme.
        /// </summary>
        [
        Category("Appearance"),
        Description("Color scheme."),
        DefaultValue(Office2007Theme.Blue)
        ]
        public Office2007Theme Office2007ColorScheme
        {
            get
            {
                return m_enColorScheme;
            }
            set
            {
                if (value != m_enColorScheme)
                {
                    m_enColorScheme = value;
                    OnColorSchemeChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets color scheme.
        /// </summary>
        [
        Category("Appearance"),
        Description("Color scheme."),
        DefaultValue(Office2010Theme.Blue)
        ]
        public Office2010Theme Office2010ColorScheme
        {
            get
            {
                return m_ColorScheme;
            }
            set
            {
                if (value != m_ColorScheme)
                {
                    m_ColorScheme = value;
                    OnColorSchemeChanged();
                }
            }
        }

        /// <summary>
        /// Gets or Sets the Visual Style
        /// </summary>
        public VisualStyle VisualStyle
        {
            get
            {
                return vStyle;
            }
            set
            {
                if (vStyle != value)
                {
                    vStyle = value;
                    if (this.ThemesEnabled)
                    {
                        for (int i = 0; i < this.Panels.Length; i++)
                        {
                            (this.Panels[i] as StatusBarAdvPanel).BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                        }
                        OnColorSchemeChanged();
                    }
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
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

                if (value == "Office2007Blue")
                {
                    VisualStyle = VisualStyle.Office2007;
                    Office2007ColorScheme = Office2007Theme.Blue;
                }
                else if (value == "Office2007Silver")
                {
                    VisualStyle = VisualStyle.Office2007;
                    Office2007ColorScheme = Office2007Theme.Silver;
                }
                else if (value == "Office2007Black")
                {
                    VisualStyle = VisualStyle.Office2007;
                    Office2007ColorScheme = Office2007Theme.Black;
                }
                else if (value == "Office2010Blue")
                {
                    VisualStyle = VisualStyle.Office2010;
                    Office2010ColorScheme = Office2010Theme.Blue;
                }
                else if (value == "Office2010Silver")
                {
                    VisualStyle = VisualStyle.Office2010;
                    Office2010ColorScheme = Office2010Theme.Silver;
                }
                else if (value == "Office2010Black")
                {
                    VisualStyle = VisualStyle.Office2010;
                    Office2010ColorScheme = Office2010Theme.Black;
                }
                else if (value == "Managed")
                {
                    VisualStyle = VisualStyle.Office2007;
                    Office2007ColorScheme = Office2007Theme.Managed;
                }
                else if (value == "Metro")
                    VisualStyle = VisualStyle.Metro;
                else
                    VisualStyle = VisualStyle.Default;
            }
        }

        /// <summary>
        /// Sets the default border color
        /// </summary>
        private void SetDefaultBorderColor()
        {
            Color b_Color = SystemColors.ControlDark;
            if (vStyle == StatusBar.VisualStyle.Metro)
                b_Color = Color.LightGray;
            this.BorderColor = b_Color;
            this.ForeColor = Color.Black;
            for (int i = 0; i < this.Panels.Length; i++)
            {
                (this.Panels[i] as StatusBarAdvPanel).BorderColor = b_Color;
            }
        }
        /// <summary>
        /// Sets the bordercolor for Office2007 style
        /// </summary>
        /// <param name="scheme"></param>
        private void SetOffice2007BorderColor(Office2007Theme scheme)
        {
            if (vStyle == StatusBar.VisualStyle.Office2007)
            {
                switch (scheme)
                {
                    case Office2007Theme.Blue:
                        {
                            this.BorderColor = Color.FromArgb(205, 224, 245);
                            this.ForeColor = Color.FromArgb(62,77,140);
                            for (int i = 0; i < this.Panels.Length; i++)
                            {
                                (this.Panels[i] as StatusBarAdvPanel).BorderColor = Color.FromArgb(205, 224, 245);
                            }
                            break;
                        }
                    case Office2007Theme.Black:
                        {
                            this.BorderColor = Color.FromArgb(59, 59, 59);
                            this.ForeColor = Color.White;
                            for (int i = 0; i < this.Panels.Length; i++)
                            {
                                (this.Panels[i] as StatusBarAdvPanel).BorderColor = Color.FromArgb(59,59,59);
                            }
                            break;
                        }
                    case Office2007Theme.Silver:
                        {
                            this.BorderColor = Color.FromArgb(210, 215, 222);
                            this.ForeColor = Color.FromArgb(74,81,90);
                            for (int i = 0; i < this.Panels.Length; i++)
                            {
                                (this.Panels[i] as StatusBarAdvPanel).BorderColor = Color.FromArgb(210,215,222);
                            }
                            break;
                        }
                    case Office2007Theme.Managed:
                        {
                            this.BorderColor = Color.FromArgb(210, 215, 222);
                            this.ForeColor = Color.Black;
                            for (int i = 0; i < this.Panels.Length; i++)
                            {
                                (this.Panels[i] as StatusBarAdvPanel).BorderColor = Color.FromArgb(210, 215, 222);
                            }
                            break;
                        }
                }
            }
        }
        /// <summary>
        /// sets the border color for office2010 style
        /// </summary>
        /// <param name="scheme"></param>
        private void SetOffice2010BorderColor(Office2010Theme scheme)
        {
            if (vStyle == StatusBar.VisualStyle.Office2010)
            {
                switch (scheme)
                {
                    case Office2010Theme.Blue:
                        {
                            this.BorderColor = Color.FromArgb(199, 216, 238);
                            this.ForeColor = Color.FromArgb(62, 77, 140);
                            for (int i = 0; i < this.Panels.Length; i++)
                            {
                                (this.Panels[i] as StatusBarAdvPanel).BorderColor = Color.FromArgb(199, 216, 238);
                            }
                            break;
                        }
                    case Office2010Theme.Black:
                        {
                            this.BorderColor = Color.FromArgb(106, 106, 106);
                            this.ForeColor = Color.White;
                            for (int i = 0; i < this.Panels.Length; i++)
                            {
                                (this.Panels[i] as StatusBarAdvPanel).BorderColor = Color.FromArgb(106, 106, 106);
                            }
                            break;
                        }
                    case Office2010Theme.Silver:
                        {
                            this.BorderColor = Color.FromArgb(211, 212, 214);
                            this.ForeColor = Color.FromArgb(74, 81, 90);
                            for (int i = 0; i < this.Panels.Length; i++)
                            {
                                (this.Panels[i] as StatusBarAdvPanel).BorderColor = Color.FromArgb(211, 212, 214);
                            }
                            break;
                        }
                    case Office2010Theme.Managed:
                        {
                            this.BorderColor = Color.FromArgb(210, 215, 222);
                            this.ForeColor = Color.Black;
                            for (int i = 0; i < this.Panels.Length; i++)
                            {
                                (this.Panels[i] as StatusBarAdvPanel).BorderColor = Color.FromArgb(210, 215, 222);
                            }
                            break;
                        }
                }
            }
        }
        /// <summary>
        /// gets the office2010managed colors
        /// </summary>
        private void ApplyOffice2010ManagedTheme()
        {
            Office2010Colors managed = Office2010Colors.GetColorTable(Office2010Theme.Managed);

            clrTopGradient = managed.StatusBarExtTopGradient;
            clrFillGradient = managed.StatusBarExtFillColor;
            clrBottomGradient = managed.StatusBarExtBottomGradient;
        }
        /// <summary>
        /// gets the office2007 managed color
        /// </summary>
        private void ApplyOffice2007ManagedTheme()
        {
            Office2007Colors managed = Office2007Colors.GetColorTable(Office2007Theme.Managed);
            
            clrTopGradient = managed.StatusBarExtTopGradient;
            clrFillGradient = managed.StatusBarExtFillColor;
            clrBottomGradient = managed.StatusBarExtBottomGradient;

        }
        
        #endregion

        #region Class Static Members

        /// <summary>
        /// Image for grip button.
        /// </summary>
        private static Image m_gripImage = null;

        #endregion

        #region Class Static Properties

        /// <summary>
        /// Gets image for grip button.
        /// </summary>
        public static Image GripImage
        {
            get
            {
                return m_gripImage;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        static StatusBarExt()
        {
            m_gripImage = GetImage(DEF_NAME_GRIP_IMAGE);
        }
        /// <summary>
        /// Constructor for StatusBarExt
        /// </summary>
        public StatusBarExt()
        {
            this.ThemesEnabled = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BorderColor = SystemColors.Control;
            Office2007Colors.ManagedColorsApplied += new Office2007Colors.ManagedColorsAppliedEventHandler(Office2007Colors_ManagedColorsApplied);
            Office2010Colors.ManagedColorsApplied += new Office2010Colors.ManagedColorsAppliedEventHandler(Office2010Colors_ManagedColorsApplied);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        void Office2010Colors_ManagedColorsApplied(Office2010Colors.ManagedColorsAppliedEventArgs args)
        {
            ApplyOffice2010ManagedTheme();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        void Office2007Colors_ManagedColorsApplied(Office2007Colors.ManagedColorsAppliedEventArgs args)
        {
            ApplyOffice2007ManagedTheme();
        }

        #endregion

        #region Class Events

        /// <summary>
        /// Occurs when <see cref="ColorScheme"/> is changed.
        /// </summary>
        [
        Description("Occurs when ColorScheme is changed."),
        Category("Property Changed")
        ]
        public event EventHandler ColorSchemeChanged;

        #endregion

        #region Class Event Raisers
        /// <summary>
        /// 
        /// </summary>
        private void RaiseColorSchemeChanged()
        {
            if (ColorSchemeChanged != null)
            {
                ColorSchemeChanged(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnColorSchemeChanged()
        {
            if (vStyle == StatusBar.VisualStyle.Default || vStyle == StatusBar.VisualStyle.Metro)
                SetDefaultBorderColor();
            else if (vStyle == StatusBar.VisualStyle.Office2007)
                SetOffice2007BorderColor(Office2007ColorScheme);
            else if (vStyle == StatusBar.VisualStyle.Office2010)
                SetOffice2010BorderColor(Office2010ColorScheme);
            Invalidate();

            RaiseColorSchemeChanged();
        }

        #endregion

        #region Class Utility Methods

        /// <summary>
        /// Drawing background.
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected virtual void DrawBackground(PaintEventArgs e)
        {
            ColorBlend cb = new ColorBlend(12);
            cb.Positions = new float[] { 0.0F, 0.10F, 0.15F, 0.20F, 0.25F, 0.45F, 0.54F, 0.65F, 0.75F, 0.8F, 0.9F, 1.0F };
            Rectangle rect = new Rectangle(ClientRectangle.X,ClientRectangle.Y,ClientRectangle.Width,ClientRectangle.Height);

            if (Office2007ColorScheme == Office2007Theme.Blue)
            {
                LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(211, 228, 250), Color.FromArgb(211, 228, 250), LinearGradientMode.Vertical);
                e.Graphics.FillRectangle(br, rect);
                cb.Colors = new Color[] { Color.FromArgb(230,238,254), Color.FromArgb(229,236,253), Color.FromArgb(225,234,251),
                    Color.FromArgb(221,232,250), Color.FromArgb(216,230,249), Color.FromArgb(205,224,245),
                    Color.FromArgb(209,226,247), Color.FromArgb(213,228,247), Color.FromArgb(217,230,248), 
                    Color.FromArgb(221,232,249), Color.FromArgb(225,234,251), Color.FromArgb(226,236,253)};
                br.InterpolationColors = cb;
                e.Graphics.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                br.Dispose();
            }
            else if (Office2007ColorScheme == Office2007Theme.Black)
            {
                LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(78,78,78), Color.FromArgb(78,78,78), LinearGradientMode.Vertical);
                e.Graphics.FillRectangle(br, rect);
                cb.Colors = new Color[] { Color.FromArgb(74,74,74), Color.FromArgb(73,73,73), Color.FromArgb(72,72,72),
                    Color.FromArgb(71,71,71), Color.FromArgb(69,69,69), Color.FromArgb(59,59,59),
                    Color.FromArgb(61,61,61), Color.FromArgb(63,63,63), Color.FromArgb(65,65,65), 
                    Color.FromArgb(67,67,67), Color.FromArgb(69,69,69), Color.FromArgb(71,71,71)};
                br.InterpolationColors = cb;
                e.Graphics.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                br.Dispose();
            }
            else if (Office2007ColorScheme == Office2007Theme.Silver)
            {
                LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(240,244,248), Color.FromArgb(240,244,248), LinearGradientMode.Vertical);
                e.Graphics.FillRectangle(br, rect);
                cb.Colors = new Color[] { Color.FromArgb(230,234,238), Color.FromArgb(229,233,237), Color.FromArgb(228,232,236),
                    Color.FromArgb(227,231,236), Color.FromArgb(226,230,235), Color.FromArgb(210,215,222),
                    Color.FromArgb(198,202,211), Color.FromArgb(204,207,216), Color.FromArgb(210,216,222), 
                    Color.FromArgb(217,222,229), Color.FromArgb(224,228,233), Color.FromArgb(229,233,237)};
                br.InterpolationColors = cb;
                e.Graphics.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                br.Dispose();
            }
            else if (Office2007ColorScheme == Office2007Theme.Managed)
            {
                if (clrTopGradient == Color.Empty)
                {
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(211, 228, 250), Color.FromArgb(211, 228, 250), LinearGradientMode.Vertical);
                    e.Graphics.FillRectangle(br, rect);
                    cb.Colors = new Color[] { Color.FromArgb(230,238,254), Color.FromArgb(229,236,253), Color.FromArgb(225,234,251),
                    Color.FromArgb(221,232,250), Color.FromArgb(216,230,249), Color.FromArgb(205,224,245),
                    Color.FromArgb(209,226,247), Color.FromArgb(213,228,247), Color.FromArgb(217,230,248), 
                    Color.FromArgb(221,232,249), Color.FromArgb(225,234,251), Color.FromArgb(226,236,253)};
                    br.InterpolationColors = cb;
                    e.Graphics.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                    br.Dispose();
                }
                else
                {
                    LinearGradientBrush br = new LinearGradientBrush(rect, clrTopGradient, clrFillGradient, LinearGradientMode.Vertical);
                    e.Graphics.FillRectangle(br, rect);
                    ColorBlend cB = new ColorBlend(2);
                    cB.Positions = new float[] { 0.0F, 0.25F, 1.0F };
                    cB.Colors = new Color[] { clrTopGradient, clrFillGradient, clrBottomGradient };
                    br.InterpolationColors = cB;
                    e.Graphics.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                    br.Dispose();
                }
            }
        }
        /// <summary>
        /// Drawing background for 2010 style.
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected virtual void DrawOffice2010Background(PaintEventArgs e)
        {
            ColorBlend cb = new ColorBlend(12);
            cb.Positions = new float[] { 0.0F, 0.10F, 0.15F, 0.20F, 0.25F, 0.45F, 0.54F, 0.65F, 0.75F, 0.8F, 0.9F, 1.0F };
            Rectangle rect = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            if (Office2010ColorScheme == Office2010Theme.Blue)
            {
                LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(225, 234, 255), Color.FromArgb(226, 234, 255), LinearGradientMode.Vertical);
                e.Graphics.FillRectangle(br, rect);
                cb.Colors = new Color[] { Color.FromArgb(218,231,245), Color.FromArgb(217,230,244), Color.FromArgb(215,230,244),
                    Color.FromArgb(213,227,243), Color.FromArgb(211,226,243), Color.FromArgb(199,216,238),
                    Color.FromArgb(190,209,234), Color.FromArgb(195,213,236), Color.FromArgb(200,217,238), 
                    Color.FromArgb(205,223,240), Color.FromArgb(210,227,242), Color.FromArgb(215,229,244)};
                br.InterpolationColors = cb;
                e.Graphics.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                br.Dispose();
            }
            else if (Office2010ColorScheme == Office2010Theme.Black)
            {
                LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(130,130,130), Color.FromArgb(130,130,130), LinearGradientMode.Vertical);
                e.Graphics.FillRectangle(br, rect);
                cb.Colors = new Color[] { Color.FromArgb(125,125,125), Color.FromArgb(124,124,124), Color.FromArgb(121,121,121),
                    Color.FromArgb(119,119,119), Color.FromArgb(117,117,117), Color.FromArgb(106,106,106),
                    Color.FromArgb(98,98,98), Color.FromArgb(102,102,102), Color.FromArgb(106,106,106), 
                    Color.FromArgb(108,108,108), Color.FromArgb(112,112,112), Color.FromArgb(116,116,116)};
                br.InterpolationColors = cb;
                e.Graphics.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                br.Dispose();
            }
            else if (Office2010ColorScheme == Office2010Theme.Silver)
            {
                LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(248, 250, 255), Color.FromArgb(248, 250, 255), LinearGradientMode.Vertical);
                e.Graphics.FillRectangle(br, rect);
                cb.Colors = new Color[] { Color.FromArgb(244,246,249), Color.FromArgb(241,243,246), Color.FromArgb(238,240,243),
                    Color.FromArgb(235,237,240), Color.FromArgb(211,212,214), Color.FromArgb(213,214,216),
                    Color.FromArgb(217,218,219), Color.FromArgb(224,225,226), Color.FromArgb(229,230,231), 
                    Color.FromArgb(234,235,237), Color.FromArgb(237,239,241), Color.FromArgb(240,241,244)};
                br.InterpolationColors = cb;
                e.Graphics.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                br.Dispose();
            }
            else if (Office2010ColorScheme == Office2010Theme.Managed)
            {
                if (clrTopGradient == Color.Empty)
                {
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(225, 234, 255), Color.FromArgb(226, 234, 255), LinearGradientMode.Vertical);
                    e.Graphics.FillRectangle(br, rect);
                    cb.Colors = new Color[] { Color.FromArgb(218,231,245), Color.FromArgb(217,230,244), Color.FromArgb(215,230,244),
                    Color.FromArgb(213,227,243), Color.FromArgb(211,226,243), Color.FromArgb(199,216,238),
                    Color.FromArgb(190,209,234), Color.FromArgb(195,213,236), Color.FromArgb(200,217,238), 
                    Color.FromArgb(205,223,240), Color.FromArgb(210,227,242), Color.FromArgb(215,229,244)};
                    br.InterpolationColors = cb;
                    e.Graphics.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                    br.Dispose();
                }
                else
                {
                    LinearGradientBrush br = new LinearGradientBrush(rect, clrTopGradient, clrFillGradient, LinearGradientMode.Vertical);
                    e.Graphics.FillRectangle(br, rect);
                    ColorBlend cB = new ColorBlend(2);
                    cB.Positions = new float[] { 0.0F, 0.25F, 1.0F };
                    cB.Colors = new Color[] { clrTopGradient, clrFillGradient, clrBottomGradient };
                    br.InterpolationColors = cB;
                    e.Graphics.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                    br.Dispose();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void DrawMetroBackGround(PaintEventArgs e)
        {
            Rectangle rect = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            LinearGradientBrush br = new LinearGradientBrush(rect, Color.White, Color.White, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(br, rect);
        }
        /// <summary>
        /// Drawing grip button.
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected virtual void DrawGrip(PaintEventArgs e)
        {
            if (StatusBarExt.GripImage != null)
            {
                bool bIsMirrored = GetIsMirrored();
                Point location = new Point(bIsMirrored ? 0 : ClientRectangle.Width - StatusBarExt.GripImage.Width, ClientRectangle.Height - StatusBarExt.GripImage.Height);
                Rectangle rectGripper = new Rectangle(location, StatusBarExt.GripImage.Size);

                if (bIsMirrored)
                {
                    // if RightToLeft property set in Yes
                    using (CMirroredDrawer mdDrawer = new CMirroredDrawer(e.Graphics, rectGripper, bIsMirrored))
                    {
                        Graphics gfxCanvas = mdDrawer.VirtualGfx;
                        Rectangle rectCanvas = mdDrawer.VirtualBounds;
                        gfxCanvas.DrawImage(StatusBarExt.GripImage, rectCanvas);
                    }
                }
                else
                {
                    e.Graphics.DrawImage(StatusBarExt.GripImage, rectGripper.X, rectGripper.Y);
                }
            }
        }

        /// <summary>
        /// Loads bitmap from manifest.
        /// </summary>
        /// <param name="bitmapName">The bitmap name.</param>
        /// <returns>Reference to bitmap; NULL if bitmap failed to load.</returns>
        private static Image GetImage(string bitmapName)
        {
            string fullResourceName = string.Format(DEF_PATH_TO_IMAGES_FORMAT, bitmapName);

            Type type = typeof(StatusBarExt);
            Assembly assembly = type.Module.Assembly;
            Stream stream = assembly.GetManifestResourceStream(fullResourceName);
            Bitmap bmp = null;

            if (stream != null)
            {
                bmp = new Bitmap(stream);
                bmp.MakeTransparent(_clrTransparent);
            }

            return bmp;
        }

        #endregion

        #region Class Overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.ThemesEnabled)
            {
                if (this.VisualStyle == VisualStyle.Office2007)
                    DrawBackground(e);
                else if (this.VisualStyle == VisualStyle.Office2010)
                    DrawOffice2010Background(e);
                else if (this.VisualStyle == VisualStyle.Metro)
                    DrawMetroBackGround(e);
                if(this.SizingGrip)
                    DrawGrip(e);
            }
        }

        #endregion
    }

    /// <summary>
    /// Enumeration for visualstyle
    /// </summary>
    public enum VisualStyle
    {
        Default,
        Office2007,
        Office2010,
        Metro
    }
}
