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
using System.Windows.Forms;

using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// A ComboBox that will show a list of fonts installed in the system.
    /// </summary>
    /// <remarks>
    /// <para>The <see cref="Fill"/> method will let you refill the combo box at any time.
    /// This method will be called initially from the constructor.</para>
    /// <para>
    /// You could get the selected Font text and construct a new Font as follows:
    /// <code>
    /// Font newFont = new Font(this.fontCombo.Text, 10.0);
    /// </code>
    /// </para>
    /// </remarks>
    [
    System.Drawing.ToolboxBitmap(typeof(FontComboBox), "ToolboxIcons.FontComboBox.bmp"),
    Description("Represents a ComboBox that will show a list of fonts installed in a system."),
    ToolboxItem(true)
    ]
    public class FontComboBox :
        ThemedComboBox
    {
        #region Members
        private AutoComplete m_autoComplete = null;
        private bool m_bUseAutoComplete = false;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

        /// <summary>
        ///Box Height
        /// </summary>
        private static int height = 15;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the FontComboBox class.
        /// </summary>
        public FontComboBox()
        {
            base.DrawMode = this.DrawMode;
            this.Sorted = false;

            // AutoComplete
            m_autoComplete = new AutoComplete();
            m_autoComplete.SetAutoComplete(this, AutoCompleteModes.Disabled);
            CTRLSIZE = this.Size;
          //  this.Fill();
        }
        #endregion
        #region For Touch

        bool isScaling = false;

        bool _touchMode = false;
		[DefaultValue(false)]
        public bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                        ApplyScaleToControl(1.5F);
                    else
                        ApplyScaleToControl(1);
                }
            }
        }

        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }

        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            isScaling = false;

            this.ResumeLayout();
            this.Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }
        /// <summary></summary>
        /// <param name="e"/>
        protected override void OnSizeChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnSizeChanged(e);
            if (!EnableTouchMode && CTRLSIZE != this.Size)
            {
                CTRLSIZE = this.Size;
            }
        }
        #endregion
        /// <summary>
        /// Gets or sets a value indicating whether the auto complete behaviour.
        /// This property functions only when the DropDownStyle is set to DropDown.
        /// </summary>
        [Category("Behaviour")]
        [Description("Indicates the auto complete behaviour. This property functions only when the DropDownStyle is set to DropDown.")]
        [DefaultValue(false)]
        public bool UseAutoComplete
        {
            get
            {
                return m_bUseAutoComplete;
            }
            set
            {
                if (m_bUseAutoComplete != value)
                {
                    m_bUseAutoComplete = value;
                    m_autoComplete.SetAutoComplete(this, value ? AutoCompleteModes.AutoAppend : AutoCompleteModes.Disabled);                  
                }
                if (m_bUseAutoComplete)
                    Fill();
            }
        }

        /// <summary>
        /// Gets  a value indicating that elements are drawn manually
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new DrawMode DrawMode
        {
            get
            {
                return DrawMode.OwnerDrawFixed;
            }
        }

        /// <summary>
        /// Gets an object representing the collection of the items contained in control.
        /// </summary>
       [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ComboBox.ObjectCollection Items
        {
            get
            {
                return base.Items;
            }
        }

        /// <summary>
        /// Fills the combo box at any time. This will be called automatically in the constructor.
        /// </summary>
        public void Fill()
        {
            this.Items.Clear();
            this.m_autoComplete.TableData.Rows.Clear();

            foreach (FontFamily ff in FontFamily.Families)
            {
                if (ff.IsStyleAvailable(FontStyle.Regular))
                {
                    this.Items.Add(ff.Name);
                    m_autoComplete.AddHistoryItem(ff.Name);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Items.Clear();

                if (this.m_autoComplete != null)
                {
                    this.m_autoComplete.Dispose();
                    this.m_autoComplete = null;
                }
            }

            base.Dispose(disposing);
        }
        
        protected override void DrawItemText(DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                Graphics g = e.Graphics;
                Color foreColor = this.ForeColor;
                string fontName = this.Items[e.Index].ToString();
                IntPtr hFont = CreateFont(fontName, this.ItemHeight);
                Font font = Font.FromHfont(hFont);

                NativeMethods.DeleteObject(hFont);

                using (StringFormat sfmt = (StringFormat)StringFormat.GenericTypographic.Clone())
                {
                    Point ptLocation = e.Bounds.Location;

                    if (RightToLeft.Yes == this.RightToLeft)
                    {
                        sfmt.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                        ptLocation.X += e.Bounds.Width;
                    }
                    using (Brush brush = new SolidBrush(foreColor))
                        g.DrawString(fontName, font, brush, ptLocation, sfmt);
                }
            }
        }

        /// <summary>
        /// Convert color from RGB to BRG.
        /// </summary>
        /// <param name="color">Color object </param>
        /// <returns>Returns color</returns>
        private Color ConvertColor(Color color)
        {
            return Color.FromArgb(0, color.B, color.G, color.R);
        }

        private IntPtr CreateFont(string fontName, int height)
        {
            NativeMethods.LOGFONT logFont = new NativeMethods.LOGFONT();
            logFont.lfFaceName = fontName;
            logFont.lfHeight = (int)height;
            logFont.lfCharSet = 1;            // DEFAULT_CHARSET
            logFont.lfItalic = (byte)0;      // No Italic
            logFont.lfStrikeOut = (byte)0;   // No StrikeOut
            logFont.lfUnderline = (byte)0;   // No Underline
            logFont.lfWeight = 300;           // FW_LIGHT
            logFont.lfQuality = 0;            // DEFAULT_QUALITY

            return NativeMethods.CreateFontIndirect(ref logFont);
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.ComboBox.DropDown"></see> event and sets DropDown according to ItemWidth.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"></see> that contains the event data.</param>
        protected override void OnDropDown(System.EventArgs e)
        {
            base.OnDropDown(e);
        }
    }

    /// <summary>
    /// A ListBox that will show a list of fonts installed in the system.
    /// </summary>
    /// <remarks>
    /// <para>The <see cref="Fill"/> method will let you refill the listbox at any time.
    /// This method will be called initially from the constructor.</para>
    /// <para>
    /// You could get the selected Font text and construct a new Font as follows:
    /// <code>
    /// Font newFont = new Font(this.fontListBox.Text, 10.0);
    /// </code>
    /// </para>
    /// </remarks>
    [
    System.Drawing.ToolboxBitmap(typeof(FontListBox), "ToolboxIcons.FontListBox.bmp"),
    Description("Represents a ListBox that will list the fonts installed in the system.")
    ]
    public class FontListBox : ListBox
    {
        /// <summary>
        /// Specifies an advanced appearance this control.
        /// </summary>
        private FontListBoxStyle style = FontListBoxStyle.Default;
        /// <summary></summary>
        private Color m_metroColor = Color.Empty;
        ///<summary></summary>
        private MetroColorTable metroColorTbl = new MetroColorTable();
        /// <summary>
        /// Default size of the control
        /// </summary>
        private static Size CTRLSIZE = default(Size);

        /// <summary>
        ///box height
        /// </summary>
        private static int BOXHEIGHT = default(int);
        /// <summary>
        /// RangeSlider Style
        /// </summary>
        public enum FontListBoxStyle
        {
            /// <summary>
            /// Classic appearance.
            /// </summary>
            Default,
            /// <summary>
            /// Metro-like appearance.
            /// </summary>
            Metro
        }
        public FontListBox()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(FontListBox));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            this.DoubleBuffered = true;
            base.DrawMode = this.DrawMode;
            this.Sorted = false;
            this.ItemHeight = 15;
            BOXHEIGHT= this.ItemHeight;
            CTRLSIZE = this.Size;
        }
        #region For Touch

        bool isScaling = false;

        bool _touchMode = false;
        /// <summary>
        ///gets or Sets the touchmode
        /// </summary>
		[DefaultValue(false)]
        public bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                        ApplyScaleToControl(1.5F);
                    else
                        ApplyScaleToControl(1);
                }
            }
        }
        private bool ShouldSerializeItems()
        {
            return this.Items.Count != 262;
        }
        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }
        /// <summary>
        ///Applies the scaling 
        /// </summary>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));

            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }
        /// <summary>
        ///Font changed
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }
        /// <summary></summary>
        /// <param name="e"/>
        protected override void OnSizeChanged(EventArgs e)
        {
            this.Invalidate();
            if (!EnableTouchMode && CTRLSIZE != this.Size)
            {
                CTRLSIZE = this.Size;
            }
            base.OnSizeChanged(e);

        }
        protected override void OnLocationChanged(EventArgs e)
        {
            if (this.Items.Count == 0 && !CheckIn)
            {
                foreach (FontFamily ff in FontFamily.Families)
                {
                    if (ff.IsStyleAvailable(FontStyle.Regular))
                        this.Items.Add(ff.Name);
                }
                CheckIn = true;
            }
            base.OnLocationChanged(e);
        }
        #endregion

        /// <summary>
        /// Gets or sets the theme color of the FontListBox
        /// </summary>
        [
        Browsable(true),
        Category("MetroColor"),
        RefreshProperties(RefreshProperties.Repaint),
        Description("Gets or sets the metro color of the control.")
        ]
        public Color MetroColor
        {
            get { return m_metroColor; }
            set
            {
                m_metroColor = value;
                Invalidate();
            }
        }
        /// <summary>
        /// Gets or sets an advanced appearance for the FontListBox
        /// </summary>
        [Description("Gets or sets an advanced appearance for the FontListBox.")]
        [Category("Appearance")]
        [DefaultValue(FontListBoxStyle.Default)]
        public FontListBoxStyle VisualStyle
        {
            get
            {
                return this.style;
            }
            set
            {
                if (this.style != value)
                {
                    this.style = value;
                    if (this.VisualStyle == FontListBoxStyle.Metro)
                    {                        
                        scrollFrame.VerticalScroller.Visible = true;
                        scrollFrame.VisualStyle = ScrollBarCustomDrawStyles.Metro;
                        scrollFrame.AttachedTo = this; 
                    }
                    else
                    {
                        scrollFrame.VerticalScroller.Visible = false;
                        scrollFrame.AttachedTo = this;
                    }
                    this.Invalidate();
                }
            }
        }
        ScrollersFrame scrollFrame = new ScrollersFrame();
        ///// <summary>
        ///// Gets or sets an advanced appearance for the FontListBox.
        ///// </summary>
        [Browsable(false)]
        public MetroColorTable ScrollMetroColorTable
        {
            get { return metroColorTbl; }
            set
            {
                metroColorTbl = value;
                scrollFrame.VerticalScroller.MetroColorTable = value ;
                if (!DesignMode)
                {
                    if (this.Items.Count == 0)
                    {
                        Items.Clear();
                        foreach (FontFamily ff in FontFamily.Families)
                        {
                            if (ff.IsStyleAvailable(FontStyle.Regular))
                                this.Items.Add(ff.Name);
                        }
                    }
                }
                Invalidate();
            }
        } 
        /// <summary>
        /// Indicates whether the control is a multi column control.
        /// </summary>
        private bool multiColumn = false;

        /// <summary>
        /// Gets or sets a value indicating whether the control is a multi column control.
        /// </summary>
        [Browsable(false)]
        public new bool MultiColumn
        {
            get
            {
                return multiColumn;
            }
            set
            {
                multiColumn = value;
                if (multiColumn == true)
                {
                    throw new ArgumentException("MultiColumn must be false");
                }
            }
        }

        private int columnWidth = 0;
        [Browsable(false)]
        public new int ColumnWidth
        {
            get
            {
                return columnWidth;
            }
            set
            {
                if (value > 0)
                    columnWidth = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Font Font
        {
            get
            {
                return base.Font;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new DrawMode DrawMode
        {
            get
            {
                return DrawMode.OwnerDrawFixed;
            }
        }

        ///// <summary>
        ///// Gets the items. (overridden property)
        ///// </summary>
        //[Browsable(true)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new ListBox.ObjectCollection Items
        {
            get
            {
                return base.Items;
            }
        }

        /// <summary>
        /// Fills the combo box at any time. This will be called automatically in the constructor.
        /// </summary>
        protected override void OnMeasureItem(System.Windows.Forms.MeasureItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                string fontname = this.Items[e.Index].ToString();
                Graphics g = CreateGraphics();
                e.ItemHeight = (int)g.MeasureString(fontname, Syncfusion.Drawing.FontUtil.CreateFont(fontname, 10)).Height;

                if (e.ItemHeight > 20)
                    e.ItemHeight = 20;
                g.Dispose();
            }

            base.OnMeasureItem(e);
        }

        private IntPtr CreateFont(string fontName, int height)
        {
            NativeMethods.LOGFONT logFont = new NativeMethods.LOGFONT();
            logFont.lfFaceName = fontName;
            logFont.lfHeight = (int)height;
            logFont.lfCharSet = 1;            // DEFAULT_CHARSET
            logFont.lfItalic = (byte)0;      // No Italic
            logFont.lfStrikeOut = (byte)0;   // No StrikeOut
            logFont.lfUnderline = (byte)0;   // No Underline
            logFont.lfWeight = 300;           // FW_LIGHT
            logFont.lfQuality = 0;            // DEFAULT_QUALITY

            return NativeMethods.CreateFontIndirect(ref logFont);
        }
        bool CheckIn = false;
        protected override void OnDrawItem(System.Windows.Forms.DrawItemEventArgs e)
        {
            if (e.Index >= 0 && Items.Count > 0)
            {
                Graphics g = e.Graphics;
                Color bgColor = this.BackColor;
                Color fgColor = this.ForeColor;

                string fontName = this.Items[e.Index].ToString();

                IntPtr hFont = this.CreateFont(fontName, this.ItemHeight);
                Font nfont = Font.FromHfont(hFont);
                NativeMethods.DeleteObject(hFont);

                if (SelectionMode != SelectionMode.None && (e.State & DrawItemState.Selected) > 0)
                {
                    bgColor = SystemColors.Highlight;
                    fgColor = SystemColors.HighlightText;
                }

                if (!Enabled)
                {
                    fgColor = SystemColors.ControlDark;
                }

                StringFormat sfmt = (StringFormat)StringFormat.GenericTypographic.Clone();
                Point ptLocation = e.Bounds.Location;

                if (RightToLeft.Yes == this.RightToLeft)
                {
                    sfmt.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                    ptLocation.X += e.Bounds.Width;
                }
                using(Brush brush =new SolidBrush(bgColor))
                    g.FillRectangle(brush, e.Bounds);
                Syncfusion.Drawing.PanoseFontFamilyTypes pfftype = Syncfusion.Drawing.FontUtil.PanoseFontFamilyType(g, nfont);

                if (pfftype == Syncfusion.Drawing.PanoseFontFamilyTypes.PAN_FAMILY_PICTORIAL
                    || pfftype == Syncfusion.Drawing.PanoseFontFamilyTypes.PAN_ANY)
                {
                    using (Brush brush = new SolidBrush(fgColor))
                        g.DrawString(fontName + "  ", new Font("Arial", 9), brush, ptLocation, sfmt);
                    ptLocation.X = ptLocation.X + (int)g.MeasureString(fontName + "  ", Syncfusion.Drawing.FontUtil.CreateFont("Arial", 10)).Width;
                }
                using (Brush brush = new SolidBrush(fgColor))
                    g.DrawString(fontName, nfont, brush, ptLocation, sfmt);

                if (SelectionMode == SelectionMode.None && (e.State & DrawItemState.Focus) > 0)
                {
                    ControlPaint.DrawFocusRectangle(g, e.Bounds);
                }
                sfmt.Dispose();
                nfont.Dispose();
            }
            base.OnDrawItem(e);
        }

        /// <summary>
        /// Use auto complete.
        /// </summary>
        private bool m_bUseAutoComplete = false;

        /// <summary>
        /// Gets or sets a value indicating whether the control use auto complete.
        /// </summary>
        [Description("Gets or sets use auto complete.")]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool UseAutoComplete
        {
            get
            {
                return m_bUseAutoComplete;
            }
            set
            {
                m_bUseAutoComplete = value;
            }
        }

        /// <summary>
        /// Find item index with auto complete string.
        /// </summary>
        /// <param name="autoCompleteString">AutoComplete String </param>
        /// <returns>Returns item</returns>
        private int FindItem(string autoCompleteString)
        {
            int index = -1;
            string autoComplite = autoCompleteString.ToLower();

            for (int i = 0; i < this.Items.Count; i++)
            {
                string item = (string)this.Items[i];

                if (item.Length >= autoComplite.Length)
                {
                    item = item.ToLower();

                    int num = item.IndexOf(autoComplite, 0, autoComplite.Length);
                    if (num != -1)
                    {
                        index = i;
                        break;
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// auto complete string.
        /// </summary>
        private string m_sAutoCompleteString = String.Empty;

        /// <summary>
        /// Need skip auto complete string reset.
        /// </summary>
        private bool m_bSkipAutoCompleteStringReset = false;

        /// <summary>
        /// process auto complete.
        /// </summary>
        /// <param name="keyData">Key data</param>
        /// <returns>Key processed. </returns>
        protected virtual bool ProcessAutoComplete(Keys keyData)
        {
            bool processed = false;

            Keys key = keyData & (~Keys.Modifiers);
            bool bAlt = (Control.ModifierKeys & Keys.Alt) == Keys.Alt;
            if (((key >= Keys.A && key <= Keys.Z) || (key >= Keys.D0 && key <= Keys.D9) || keyData == Keys.Space) && !bAlt)  
            {
                m_sAutoCompleteString += key.ToString();

                int index = FindItem(m_sAutoCompleteString);

                if (index >= 0)
                {
                    m_bSkipAutoCompleteStringReset = true;
                    this.SelectedIndex = index;
                }
                else
                {
                    index = FindItem(key.ToString());
                    if (index >= 0)
                    {
                        m_bSkipAutoCompleteStringReset = true;

                        this.SelectedIndex = index;
                        m_sAutoCompleteString = key.ToString();
                    }
                    else
                    {
                        m_sAutoCompleteString = String.Empty;
                    }
                }

                processed = true;
            }
            else if (key != Keys.ControlKey && key != Keys.ShiftKey)
            {
                m_sAutoCompleteString = String.Empty;
            }

            return processed;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool processed = false;

            if (keyData == Keys.PageDown
                || keyData == Keys.PageUp)
            {
                if ((keyData == Keys.PageDown && this.SelectedIndex == this.Items.Count - 1)
                    || (keyData == Keys.PageUp && this.SelectedIndex == 0))
                    return false;

                int curSel = this.SelectedIndex;
                int linesVisible = this.Height / this.GetItemHeight(0) - 1;
                switch (keyData)
                {
                    case Keys.PageDown:
                        curSel += linesVisible;
                        break;
                    case Keys.PageUp:
                        curSel = curSel - linesVisible;
                        break;
                }

                if (curSel != this.SelectedIndex)
                {
                    if (curSel > this.Items.Count - 1)
                        curSel = this.Items.Count - 1;
                    if (curSel < 0)
                        curSel = 0;
                    this.SetSelected(curSel, true);

                    this.SelectedIndex = curSel;
                }
                //this.SelectedItem = null;
                //this.SelectedIndex = -1;
            }

            if (UseAutoComplete)
            {
                processed = this.ProcessAutoComplete(keyData);
            }

            if (!processed)
            {
                processed = base.ProcessCmdKey(ref msg, keyData);
            }

            return processed;
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (!m_bSkipAutoCompleteStringReset)
            {
                m_sAutoCompleteString = String.Empty;
            }
            else
            {
                m_bSkipAutoCompleteStringReset = false;
            }

            base.OnSelectedIndexChanged(e);
        }
    }
}