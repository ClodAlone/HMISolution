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

using System.Diagnostics;
using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Localization;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms
{
    /// <summary>
    /// Specifies constants that define the color groups for the ColorPicker controls.
    /// </summary>
    /// <remarks>
    /// The ColorUIGroup's enumeration is used for specifying the color tab pages to be 
    /// displayed in the <see cref="ColorUIControl"/> and the <see cref="ColorPickerButton"/> controls.
    /// <p>This enumeration has a FlagsAttribute that allows a combination of its member values.</p>
    /// </remarks>	
    [
    Flags(),
    Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(System.Drawing.Design.UITypeEditor)),
    Serializable()
    ]
    public enum ColorUIGroups
    {
        /// <summary>
        /// None of the color groups are displayed.
        /// </summary>
        None = 0		  /*0x0000*/,
        /// <summary>
        /// Displays the custom colors group.
        /// </summary>
        CustomColors = 1 /*0x0001*/,
        /// <summary>
        /// Displays the standard colors group.
        /// </summary>
        StandardColors = 2 /*0x0002*/,
        /// <summary>
        /// Displays the system colors group.
        /// </summary>
        SystemColors = 4 /*0x0004*/,
        /// <summary>
        /// Displays all color groups, but not displays UserColors groups.
        /// </summary>
        [Browsable(false)]
        All = 7,
        /// <summary>
        /// Displays the user colors group.
        /// </summary>
        UserColors = 8,
        /// <summary>
        /// Displays all color groups.
        /// </summary>
        [Browsable(false)]
        Full = 15
    }


    /// <summary>
    /// Specifies constants that define the selected color group in the ColorPicker controls.
    /// </summary>
    /// <remarks>
    /// The ColorUISelectedGroup enumeration is used for specifying the selected color group in the 
    /// <see cref="ColorUIControl"/> and the <see cref="ColorPickerButton"/> controls.
    /// </remarks>
    public enum ColorUISelectedGroup
    {
        /// <summary>
        /// No color group is selected.
        /// </summary>
        None = 0,
        /// <summary>
        /// The custom colors group is selected.
        /// </summary>
        CustomColors = 1,
        /// <summary>
        /// The standard colors group is selected.
        /// </summary>
        StandardColors,
        /// <summary>
        /// The system colors groups is selected.
        /// </summary>
        SystemColors,
        /// <summary>
        /// The user colors groups is selected.
        /// </summary>
        UserColors
    }

    /// <summary>
    /// Provides a standard interface for selecting colors.
    /// </summary>
    /// <remarks>
    /// The ColorUIControl implements a palette type visual interface for selecting colors at
    /// run-time similar to the color picker drop-down provided by the Visual Studio.NET environment.
    /// The ColorUIControl class offers a selection of colors divided into three color groupings arranged as
    /// tabs. The three color groupings are the SystemColors consisting of the colors defined
    /// within the <see cref="System.Drawing.SystemColors"/> class, the StandardColors consisting of the colors defined within
    /// <see cref="System.Drawing.Color"/> and a CustomColors providing a customizable color palette.
    /// The ColorUIControl control can be used either as a regular control hosted within a parent container
    /// or it can be used as a drop-down control in combination with the <see cref="ColorPickerButton"/> control.
    /// </remarks>
    /// <seealso cref="ColorPickerButton"/>
    /// <example>
    /// The following code creates a ColorUIControl, sets the color groups and adds an event handler 
    /// for the ColorUIControl.ColorSelected event:
    /// 
    /// <coderef file="Shared\Samples\ColorPickerDemo\CS\Form1.cs" name="ColorUIControl" lang="C#"><code lang="C#">
    ///		private void InitializeColorUIControl()
    ///		{
    ///			// Create the ColorUIControl.
    ///			Syncfusion.Windows.Forms.ColorUIControl clrUIControl = new Syncfusion.Windows.Forms.ColorUIControl();
    ///	
    ///			// Set the ColorGroups to be displayed
    ///			clrUIControl.ColorGroups = ( Syncfusion.Windows.Forms.ColorUIGroups.CustomColors|
    ///				Syncfusion.Windows.Forms.ColorUIGroups.StandardColors|
    ///				Syncfusion.Windows.Forms.ColorUIGroups.SystemColors );
    ///	
    ///			// Set the initially selected group and color.
    ///			clrUIControl.SelectedColorGroup = Syncfusion.Windows.Forms.ColorUISelectedGroup.SystemColors;	
    ///			clrUIControl.SelectedColor = SystemColors.ControlDark;
    ///
    ///			// Provide a handler for the ColorUIControl.ColorSelected event.
    ///			clrUIControl.ColorSelected += new EventHandler(this.OnColorSelected);
    ///		}
    ///
    ///		// Handler for the ColorUIControl.ColorSelected event.
    ///		private void OnColorSelected(object sender, System.EventArgs e)
    ///		{
    ///			Color clrselected = (sender as ColorUIControl).SelectedColor;
    ///		}</code></coderef>
    ///		
    /// <coderef file="Shared\Samples\ColorPickerDemo\VB\Form1.vb" name="ColorUIControl" lang="VB"><code lang="VB">
    ///        Private Sub InitializeColorUIControl()
    ///
    ///            ' Create an instance of the ColorUIControl.
    ///            Me.clrUIControl = New Syncfusion.Windows.Forms.ColorUIControl()
    ///
    ///            ' Set the color groups to be shown.
    ///            Me.clrUIControl.ColorGroups = Syncfusion.Windows.Forms.ColorUIGroups.CustomColors Or Syncfusion.Windows.Forms.ColorUIGroups.StandardColors
    ///
    ///            ' Set the initially selected group.
    ///            Me.clrUIControl.SelectedColorGroup = Syncfusion.Windows.Forms.ColorUISelectedGroup.CustomColors
    ///
    ///            ' Subscribe to the ColorUIControl.ColorSelected event.
    ///            AddHandler Me.clrUIControl.ColorSelected, New System.EventHandler(AddressOf clrUIControl_ColorSelected)
    ///
    ///        End Sub
    ///
    ///        ' Handler for the ColorUIControl.ColorSelected event.
    ///        Private Sub clrUIControl_ColorSelected(ByVal sender As Object, ByVal e As System.EventArgs)
    ///
    ///            Dim clrselected As Color = Me.clrUIControl.SelectedColor
    ///
    ///        End Sub 'clrUIControl_ColorSelected</code></coderef>
    /// 
    /// </example>
    [Designer(typeof(ColorUIDesigner), typeof(System.ComponentModel.Design.IDesigner))]
    [System.Drawing.ToolboxBitmap(typeof(Syncfusion.Windows.Forms.ColorUIControl), "ToolboxIcons.ColorUIControl.bmp")]
    [Description("Provides a standard interface for selecting colors.")]
    public class ColorUIControl : Control,IVisualStyle 
    {
        // Fields
        private Color clrSelected = Color.Empty;
        private ColorEditorTabControl tabControl = null;
        private TabPage customTabPage = null;
        private TabPage standardTabPage = null;
        private TabPage systemTabPage = null;
        private String strCustomTab = SR.GetString(SR.ColorEditorPaletteTab);
        private String strStandardTab = SR.GetString(SR.ColorEditorStandardTab);
        private String strSystemTab = SR.GetString(SR.ColorEditorSystemTab);
        private ListBox lbSystem;
        private ListBox lbStandard;
        private ColorPalette pal;
        private Color[] systemColorConstants;
        private Color[] standardColorConstants;
        private Color[] customColors;
        private bool commonHeightSet = false;
        private bool systemHeightSet = false;
        private ColorUISelectedGroup cuiSelectedGroup = ColorUISelectedGroup.None;
        private ColorUIGroups cuiGroups = ColorUIGroups.All;
        private BorderStyle borderStyle = BorderStyle.Fixed3D;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

        /// <summary>


        /// The text displayed on the user colors tab.
        /// </summary>
        private string m_sUserTabText = "User Colors";
        /// <summary>
        /// User tab page.
        /// </summary>
        private TabPage m_userTabPage = null;
        /// <summary>
        /// User colors palette.
        /// </summary>
        private ColorPalette m_userColorPalette;
        /// <summary>
        /// User custom colors.
        /// </summary>
        private Color[] m_userCustomColors;
        /// <summary>
        /// User custom colors collection.
        /// </summary>
        private ColorCollection m_userCustomColorsCollection = null;
        /// <summary>
        /// User colors collection.
        /// </summary>
        private ColorCollection m_userColorsCollection = null;
        /// <summary>
        /// Enable stretch custom colors panel on resize.
        /// </summary>
        private bool m_bCustomColorsStretchOnResize = false;
        /// <summary>
        /// Enable stretch user colors panel on resize.
        /// </summary>
        private bool m_bUserColorsStretchOnResize = false;
        /// <summary>
        /// Specifies an advanced appearance this control.
        /// </summary>
        private ColorUIStyle style = ColorUIStyle.Default;
        ///<summary></summary>
        private MetroColorTable metroColorTbl = new MetroColorTable();
        ///<summary></summary>
        private Color m_metroColor = ColorTranslator.FromHtml ("#119EDA");
        ///<summary></summary>
        private Color metroForeColor = Color.White ;
        /// <summary>
        /// Specifies a value to show / hide the user selection color swatches.
        /// </summary>
        private bool m_showUserSelectionColors = true;
        /// <summary>
        /// Specifies TabControl item height
        /// </summary>
        internal Size TabControlItemSize = new Size();

        public class ColorCollection : ICollection
        {
            /// <summary>
            /// Color array.
            /// </summary>
            private Color[] m_colorArray;
            /// <summary>
            /// Control for invalidate if color array was chenged.
            /// </summary>
            private Control m_control = null;

            internal ColorCollection(Color[] colorArray, Control control)
            {
                m_colorArray = colorArray;
                m_control = control;
            }

            public Color this[int index]
            {
                get
                {
                    return m_colorArray[index];
                }
                set
                {
                    if (m_colorArray[index] != value)
                    {
                        m_colorArray[index] = value;
                        if (m_control != null)
                        {
                            m_control.Invalidate();
                        }
                    }
                }
            }

            #region ICollection

            public bool IsSynchronized
            {
                get
                {
                    return m_colorArray.IsSynchronized;
                }
            }

            public int Count
            {
                get
                {
                    return m_colorArray.Length;
                }
            }

            public void CopyTo(Array array, int index)
            {
                m_colorArray.CopyTo(array, index);
            }

            public object SyncRoot
            {
                get
                {
                    return m_colorArray.SyncRoot;
                }
            }

            #endregion

            #region IEnumerable

            public IEnumerator GetEnumerator()
            {
                return m_colorArray.GetEnumerator();
            }

            #endregion
        }
        private bool flag = false;
        public bool Flag
        {
            get { return flag; }
            set { flag = value; }
        }


        ScrollersFrame scrollFrameSystem = new ScrollersFrame();
        ScrollersFrame scrollFrameStandard = new ScrollersFrame();

        /// <summary>
        /// Gets or sets the theme forecolor of the ColorUI
        /// </summary>
        [
       Browsable(true),
       Category("MetroColor"),
       RefreshProperties(RefreshProperties.Repaint),
       Description("Gets or sets the pressed background color of the control.")
       ]     
        public Color MetroForeColor
        {
            get { return metroForeColor; }
            set
            {
                //metroForeColor = this.Parent.ForeColor;
                metroForeColor = value;
            }
        }
        /// <summary>
        /// Gets or sets the theme color of the ColorUI
        /// </summary>
        [
        Browsable(true),
        Category("MetroColor"),
        RefreshProperties(RefreshProperties.Repaint),
        Description("Gets or sets the pressed background color of the control.")
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
        /// Gets or sets a value to show / hide the user selection color swatches.
        /// </summary>
        public bool ShowUserSelectionColors
        {
            get
            {
                return m_showUserSelectionColors;
            }
            set
            {
                if (m_showUserSelectionColors != value)
                    m_showUserSelectionColors = value;
            }
        }

        /// <summary>
        /// Gets or sets custom color to scroller.
        /// </summary>
        [Browsable(false)]
        public MetroColorTable ScrollMetroColorTable
        {
            get { return metroColorTbl; }
            set
            {
                metroColorTbl = value;
                scrollFrameSystem.VerticalScroller.MetroColorTable = value;
                scrollFrameStandard.VerticalScroller.MetroColorTable = value;
            }
        }
        /// <summary>
        /// Gets or sets enable stretch custom colors panel on resize.
        /// </summary>
        [DefaultValue(false)]
        [Description("Gets or sets enable stretch user colors panel on resize.")]
        [Category("Appearance")]
        public bool CustomColorsStretchOnResize
        {
            get
            {
                return m_bCustomColorsStretchOnResize;
            }
            set
            {
                if (m_bCustomColorsStretchOnResize != value)
                {
                    m_bCustomColorsStretchOnResize = value;

                    if (this.pal != null)
                    {
                        this.pal.StretchOnResize = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets an advanced appearance for the ColorUI.
        /// </summary>
        [Description("Gets or sets an advanced appearance for the ColorUI.")]
        [Category("Appearance")]
        [DefaultValue(ColorUIStyle.Default)]
        public ColorUIStyle VisualStyle
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
                    if (this.VisualStyle == ColorUIStyle.Metro)
                    {
                        //this.tabControl.MetroForeColor = this.ForeColor;
                        scrollFrameSystem.VisualStyle = ScrollBarCustomDrawStyles.Metro;
                        scrollFrameSystem.AttachedTo = lbSystem;

                        scrollFrameStandard.VisualStyle = ScrollBarCustomDrawStyles.Metro;
                        scrollFrameStandard.AttachedTo = lbStandard;
                        if (m_userCustomColors != null)
                            m_userColorPalette.BackColor = Color.White;
                        this.tabControl.setStyle(true);
                    }
                    else if (this.VisualStyle == ColorUIStyle.Office2010)
                    {
                        scrollFrameSystem.VisualStyle = ScrollBarCustomDrawStyles.Office2010;
                        scrollFrameSystem.AttachedTo = lbSystem;

                        scrollFrameStandard.VisualStyle = ScrollBarCustomDrawStyles.Office2010;
                        scrollFrameStandard.AttachedTo = lbStandard;

                        this.tabControl.setStyle(false);
                    }
                    else
                    {
                        this.tabControl.setStyle(false);
                    }
                    this.tabControl.Refresh();
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string skinstyle;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return skinstyle;
            }
            set
            {
                skinstyle = value;

                if (value == "Metro")
                    VisualStyle = ColorUIStyle.Metro;
                else
                    VisualStyle = ColorUIStyle.Default;
            }
        }

        /// <summary>
        /// Gets or sets enable stretch user colors panel on resize.
        /// </summary>
        [DefaultValue(false)]
        [Description("Gets or sets enable stretch user colors panel on resize.")]
        [Category("Appearance")]
        public bool UserColorsStretchOnResize
        {
            get
            {
                return m_bUserColorsStretchOnResize;
            }
            set
            {
                if (m_bUserColorsStretchOnResize != value)
                {
                    m_bUserColorsStretchOnResize = value;

                    if (m_userColorPalette != null)
                    {
                        m_userColorPalette.StretchOnResize = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets user custom colors.
        /// ColorGroups must be UserColors for use this property.
        /// </summary>
        [Browsable(false)]
        [Description("Gets user custom colors. ColorGroups must be UserColors for use this property.")]
        public ColorCollection UserCustomColors
        {
            get
            {
                if (this.m_userCustomColors == null)
                {
                    throw new ArgumentException("ColorGroups must be UserColors for use this property");
                }

                if (m_userCustomColorsCollection == null)
                {
                    m_userCustomColorsCollection = new ColorCollection(m_userCustomColors, m_userColorPalette);
                }

                return m_userCustomColorsCollection;
            }
        }

        /// <summary>
        /// Gets user colors.
        /// ColorGroups must be UserColors for use this property.
        /// </summary>
        [Browsable(false)]
        [Description("Gets user colors. ColorGroups must be UserColors for use this property.")]
        public ColorCollection UserColors
        {
            get
            {
                if (m_userColorPalette == null)
                {
                    throw new ArgumentException("ColorGroups must be UserColors for use this property");
                }

                if (m_userColorsCollection == null)
                {
                    m_userColorsCollection = new ColorCollection(m_userColorPalette.StandardColors, m_userColorPalette);
                }

                return m_userColorsCollection;
            }
        }

        /// <summary>
        /// Gets or sets the text displayed on the user colors tab.
        /// </summary>
        [Description("Gets or sets the text displayed on the user colors tab.")]
        [DefaultValue("User Colors")]
        public String UserTabName
        {
            get
            {
                return this.m_sUserTabText;
            }
            set
            {
                if (m_sUserTabText != value)
                {
                    m_sUserTabText = value;
                    if (m_userTabPage != null)
                        m_userTabPage.Text = value;
                }
            }
        }

        /// <summary>
        /// The ColorSelected event occurs when a color is selected from one of the palettes in the <see cref="ColorUIControl"/>.
        /// </summary>
        [
        Description("The ColorSelected event occurs when a color is selected from a color palette."),
        ]
        public event System.EventHandler ColorSelected;

        /// <summary>
        /// Gets or sets the color tabpages to be displayed by the control.
        /// </summary>
        /// <value>A <see cref="ColorUIGroups"/> value. The default is ColorUIGroups.All.</value>
        [
        Category("Color Selection"),
        Description("Specifies the color tabpages to be displayed by the control."),
        DefaultValue(ColorUIGroups.All)
        ]
        public ColorUIGroups ColorGroups
        {
            get { return this.cuiGroups; }

            set
            {
                if ((this.tabControl != null) && (this.cuiGroups != value))
                {
                    this.cuiGroups = value;
                    this.InitializeTabPages();
                }
                else
                    this.cuiGroups = value;
            }
        }

        /// <summary>
        /// Makes the tab associated with this color group the selected tab.
        /// </summary>
        /// <value>A <see cref="ColorUISelectedGroup"/> value.</value>
        [
        Category("Color Selection"),
        Description("Makes the tab associated with this colorgroup the selected tab.")
        ]
        public ColorUISelectedGroup SelectedColorGroup
        {
            get { return this.cuiSelectedGroup; }
            set
            {
                if (this.cuiSelectedGroup != value)
                {
                    this.cuiSelectedGroup = value;
                    if (value != ColorUISelectedGroup.None)
                    {
                        TabPage page = this.GetTabPageFromGroup(value);
                        if (page == null)
                            throw new ArgumentException("The ColorGroups property is not set for this selectedgroup.", value.ToString());
                        this.tabControl.SelectedTab = page;
                    }
                }
            }
        }

        private bool ShouldSerializeSelectedColorGroup()
        {
            if ((this.cuiSelectedGroup != ColorUISelectedGroup.None) &&
                (this.tabControl != null) && (this.tabControl.TabCount > 0))
            {
                TabPage lastpage = this.tabControl.TabPages[this.tabControl.TabCount - 1];
                return (lastpage != this.GetTabPageFromGroup(this.cuiSelectedGroup));
            }
            return (this.cuiSelectedGroup != ColorUISelectedGroup.None);
        }

        /// <summary>
        /// Resets the <see cref="ColorUIControl.SelectedColorGroup"/> property to its default value.
        /// </summary>
        public virtual void ResetSelectedColorGroup()
        {
            if ((this.tabControl != null) && (this.tabControl.TabCount > 0))
            {
                TabPage lastpage = this.tabControl.TabPages[this.tabControl.TabCount - 1];
                if (lastpage == m_userTabPage)
                    this.SelectedColorGroup = ColorUISelectedGroup.UserColors;
                if (lastpage == this.systemTabPage)
                    this.SelectedColorGroup = ColorUISelectedGroup.SystemColors;
                else if (lastpage == this.standardTabPage)
                    this.SelectedColorGroup = ColorUISelectedGroup.StandardColors;
                else
                    this.SelectedColorGroup = ColorUISelectedGroup.CustomColors;
            }
            else
                this.SelectedColorGroup = ColorUISelectedGroup.None;
        }

        /// <summary>
        /// Gets or sets the text displayed on the custom colors tab.
        /// </summary>
        /// <value>A String value.</value>
        [
        Category("Appearance"),
        Description("The text displayed on the custom colors tab."),
        ]
        public String CustomTabName
        {
            get { return this.strCustomTab; }
            set
            {
                if (this.strCustomTab != value)
                {
                    this.strCustomTab = value;
                    if (this.customTabPage != null)
                        this.customTabPage.Text = value;
                }
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeCustomTabName()
        {
            if (this.strCustomTab.Equals(SR.GetString(SR.ColorEditorPaletteTab, this)))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Resets the <see cref="ColorUIControl.CustomTabName"/> property to its default value.
        /// </summary>
        public virtual void ResetCustomTabName()
        {
            this.CustomTabName = SR.GetString(SR.ColorEditorPaletteTab, this);
        }

        /// <summary>
        /// Gets or sets the text displayed on the standard colors tab.
        /// </summary>
        /// <value>A String value.</value>
        [
        Category("Appearance"),
        Description("The text displayed on the standard colors tab.")
        ]
        public String StandardTabName
        {
            get { return this.strStandardTab; }
            set
            {
                if (this.strStandardTab != value)
                {
                    this.strStandardTab = value;
                    if (this.standardTabPage != null)
                        this.standardTabPage.Text = value;
                }
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeStandardTabName()
        {
            if (this.strStandardTab.Equals(SR.GetString(SR.ColorEditorStandardTab, this)))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Resets the <see cref="ColorUIControl.StandardTabName"/> property to its default value.
        /// </summary>
        public virtual void ResetStandardTabName()
        {
            this.StandardTabName = SR.GetString(SR.ColorEditorStandardTab, this);
        }

        /// <summary>
        /// Gets or sets the text displayed on the system colors tab.
        /// </summary>
        /// <value>A String value.</value>
        [
        Category("Appearance"),
        Description("The text displayed on the system colors tab.")
        ]
        public String SystemTabName
        {
            get { return this.strSystemTab; }
            set
            {
                if (this.strSystemTab != value)
                {
                    this.strSystemTab = value;
                    if (this.systemTabPage != null)
                        this.systemTabPage.Text = value;
                }
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeSystemTabName()
        {
            if (this.strSystemTab.Equals(SR.GetString(SR.ColorEditorSystemTab, this)))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Resets the <see cref="ColorUIControl.SystemTabName"/> property to its default value.
        /// </summary>
        public virtual void ResetSystemTabName()
        {
            this.SystemTabName = SR.GetString(SR.ColorEditorSystemTab, this);
        }

        private TabPage GetTabPageFromGroup(ColorUISelectedGroup group)
        {
            switch (group)
            {
                case ColorUISelectedGroup.CustomColors:
                    return this.customTabPage;
                case ColorUISelectedGroup.StandardColors:
                    return this.standardTabPage;
                case ColorUISelectedGroup.SystemColors:
                    return this.systemTabPage;
                case ColorUISelectedGroup.UserColors:
                    return m_userTabPage;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the current selected color.
        /// </summary>
        /// <value>A <see cref="System.Drawing.Color"/> value.</value>
        [
        Category("Color Selection"),
        Description("Specifies the current selected color.")
        ]
        public Color SelectedColor
        {
            get { return this.clrSelected; }
            set
            {
                if (this.clrSelected != value)
                {
                    this.clrSelected = value;
                    if ((this.tabControl != null) && (this.tabControl.TabCount > 0))
                        this.SetColorSelection(value);
                }
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeSelectedColor()
        {
            if (this.clrSelected == Color.Empty)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Resets the <see cref="ColorUIControl.SelectedColor"/> property to its default value.
        /// </summary>
        public virtual void ResetSelectedColor()
        {
            this.clrSelected = Color.Empty;
        }


        /// <summary>
        /// Gets or sets the border style of the control.
        /// </summary>
        /// <value>A <see cref="System.Windows.Forms.BorderStyle"/> value. The default is BorderStyle.Fixed3D.</value>
        [
        Category("Appearance"),
        DefaultValue(BorderStyle.Fixed3D),
        Description("Specifies the border style of the control.")
        ]
        public BorderStyle BorderStyle
        {
            get
            {
                return this.borderStyle;
            }
            set
            {
                if (this.borderStyle != value)
                {
                    if (!Enum.IsDefined(typeof(System.Windows.Forms.BorderStyle), value))
                        throw new InvalidEnumArgumentException("value", ((int)(value)), typeof(BorderStyle));
                    this.borderStyle = value;
                    UpdateStyles();
                }
            }
        }

        /// <summary>
        /// Overridden. See <see cref="M:System.Windows.Forms.Control.CreateParams"/>.
        /// </summary>
        protected override/*Control*/ CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
            get
            {
                System.Windows.Forms.CreateParams cp = base.CreateParams;
                switch (this.borderStyle)
                {
                    case BorderStyle.Fixed3D:
                        cp.ExStyle |= 0x200; // WS_EX_DLGFRAME
                        break;
                    case BorderStyle.FixedSingle:
                        cp.Style |= 0x800000; // WS_BORDER
                        break;
                }
                return cp;
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ColorUIControl"/> class.
        /// </summary>
        public ColorUIControl()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(ColorUIControl));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            this.InitializeComponent();
            this.AdjustListBoxItemHeight();
            if (this.clrSelected != Color.Empty)
                this.SetColorSelection(this.clrSelected);
            CTRLSIZE = new Size(163,177);
            TabControlItemSize = new Size(45, 17);
        }
        bool isScaling = false;
        /// <summary>
        /// Gets/Sets Control size before touch enabled
        /// </summary>
        [Browsable(false)]
        public Size BeforeTouchSize
        {
            get
            {
                return CTRLSIZE;
            }
            set
            {
                CTRLSIZE = value;
            }
        }
        bool _touchMode = false;
        /// <summary>
        /// Gets or sets value to enable or disable the Touchmode to the controls.
        /// </summary>
        /// <remarks>Scale factor will be updated automatically if scalefactor is equal to 1</remarks>
        [Browsable(true),DefaultValue(false),
        Category("Layout"), Description("Gets or sets value to enable or disable the Touchmode to the controls."),
    ]
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
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
        /// Scale the control based on the scale factor passed in the argument.
        /// </summary>
        /// <param name="scaleFactor">value to scale the factor based upon.</param>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            this.tabControl.ItemSize = new Size((int)(TabControlItemSize.Width * (scaleFactor + 5)), (int)(TabControlItemSize.Height * scaleFactor));
            this.AdjustListBoxItemHeight();
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            this.CustomColorsStretchOnResize = this.UserColorsStretchOnResize = true;
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }

        // Methods
        private void AdjustColorUIHeight()
        {
            if (this.tabControl.TabCount > 0)
            {
                TabPage tabpg = this.tabControl.TabPages[0];
                Size size = tabpg.Size;
                Rectangle rect = tabpg.ClientRectangle;
                int border = 0;
                this.tabControl.Size = new Size((size.Width + (2 * border)), ((size.Height + (2 * border)) + rect.Height));
                this.Size = this.tabControl.Size;
            }
        }

        private void AdjustListBoxItemHeight()
        {
            if (this.lbSystem != null)
                this.lbSystem.ItemHeight = (this.Font.Height + 2);
            if (this.lbStandard != null)
                this.lbStandard.ItemHeight = (this.Font.Height + 2);
            if (EnableTouchMode)
            {
                this.lbSystem.ItemHeight = (int)((this.Font.Height + 2) * (1.5F));
                this.lbStandard.ItemHeight = (int)((this.Font.Height + 2) * (1.5F));
            }
        }

        /// <summary>
        ///      Takes the given color and looks for an instance in the ColorValues table.
        /// </summary>
        private Color GetBestColor(Color color)
        {
            Color[] values = this.ColorValues;
            int argb = color.ToArgb();
            for (int n2 = 0; n2 < values.Length; n2++)
            {
                if (values[n2].ToArgb() == argb)
                    return values[n2];
            }
            return color;
        }

        /// <summary>
        ///      Retrieves an array of color constants for the given object.
        /// </summary>
        private Color[] GetConstants(Type enumType)
        {
            System.Reflection.PropertyInfo[] propertyInfos = enumType.GetProperties();
            System.Collections.ArrayList arrayList = new ArrayList();

            for (int n = 0; n < propertyInfos.Length; n++)
            {
                System.Reflection.PropertyInfo pi = propertyInfos[n];
                if (pi.PropertyType == typeof(System.Drawing.Color))
                {
                    if (pi.GetGetMethod() != null)
                        arrayList.Add(pi.GetValue(null, null));
                }
            }
            return (Color[])arrayList.ToArray(typeof(Color));
        }


        private Control GetCurrentComponent()
        {
            if (this.tabControl.SelectedTab == this.systemTabPage)
                return (Control)this.lbSystem;

            if (this.tabControl.SelectedTab == this.standardTabPage)
                return (Control)this.lbStandard;

            return (Control)this.pal;
        }

        private void InitializeComponent()
        {
            this.tabControl = new ColorEditorTabControl();

            this.InitializeTabPages();
            this.tabControl.SelectedIndexChanged += new EventHandler(this.OnSelectedTabChanged);

            this.tabControl.TabStop = false;
            this.tabControl.SelectedIndexChanged += new EventHandler(OnTabControlSelChange);
            this.tabControl.Dock = DockStyle.Fill;
            this.tabControl.Resize += new EventHandler(OnTabControlResize);
            this.Controls.Add(this.tabControl);
        }

        private void InitializeTabPages()
        {
            if (this.VisualStyle == ColorUIStyle.Metro)
                this.tabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            if (((this.cuiGroups & ColorUIGroups.CustomColors) == 0) && (this.customTabPage != null))
            {
                this.tabControl.TabPages.Remove(this.customTabPage);
                this.customTabPage.Controls.Remove(this.pal);
                this.pal.Dispose();
                this.pal = null;
                this.customTabPage.Dispose();
                this.customTabPage = null;
            }

            if (((this.cuiGroups & ColorUIGroups.StandardColors) == 0) && (this.standardTabPage != null))
            {
                this.tabControl.TabPages.Remove(this.standardTabPage);
                this.standardTabPage.Controls.Remove(this.lbStandard);
                this.lbStandard.Dispose();
                this.lbStandard = null;
                this.standardTabPage.Dispose();
                this.standardTabPage = null;
            }

            if (((this.cuiGroups & ColorUIGroups.SystemColors) == 0) && (this.systemTabPage != null))
            {
                this.tabControl.TabPages.Remove(this.systemTabPage);
                this.systemTabPage.Controls.Remove(this.lbSystem);
                this.lbSystem.Dispose();
                this.lbSystem = null;
                this.systemTabPage.Dispose();
                this.systemTabPage = null;
            }

            // UserColors
            if (((this.cuiGroups & ColorUIGroups.UserColors) == 0) && (this.m_userTabPage != null))
            {
                this.tabControl.TabPages.Remove(m_userTabPage);
                this.customTabPage.Controls.Remove(m_userColorPalette);
                m_userColorPalette.Dispose();
                m_userColorsCollection = null;
                m_userColorPalette = null;
                m_userTabPage.Dispose();
                m_userTabPage = null;
            }

            if ((this.customTabPage == null) && ((this.cuiGroups & ColorUIGroups.CustomColors) != 0))
            {
                this.customTabPage = new TabPage(this.strCustomTab);

                this.tabControl.TabPages.Add(this.customTabPage);
                if ((this.cuiSelectedGroup == ColorUISelectedGroup.None) ||
                            (this.cuiSelectedGroup == ColorUISelectedGroup.CustomColors))
                    this.tabControl.SelectedTab = this.customTabPage;
                this.customTabPage.BorderStyle = BorderStyle.FixedSingle;
                this.pal = new ColorPalette(this, this.CustomColors);
                this.pal.Dock = DockStyle.Fill;
                this.pal.StretchOnResize = this.CustomColorsStretchOnResize;
                this.pal.Picked += new EventHandler(OnPalettePick);

                this.customTabPage.Controls.Add(this.pal);
            }

            if ((this.standardTabPage == null) && ((this.cuiGroups & ColorUIGroups.StandardColors) != 0))
            {
                this.standardTabPage = new TabPage(this.strStandardTab);

                this.tabControl.TabPages.Add(this.standardTabPage);
                if ((this.cuiSelectedGroup == ColorUISelectedGroup.None) ||
                            (this.cuiSelectedGroup == ColorUISelectedGroup.StandardColors))
                    this.tabControl.SelectedTab = this.standardTabPage;

                this.lbStandard = new ColorEditorListBox();
                this.lbStandard.DrawMode = DrawMode.OwnerDrawFixed;
                this.lbStandard.BorderStyle = BorderStyle.FixedSingle;
                this.lbStandard.IntegralHeight = false;
                this.lbStandard.Sorted = false;
                this.lbStandard.Click += new EventHandler(OnListClick);
                this.lbStandard.DrawItem += new DrawItemEventHandler(OnListDrawItem);
                this.lbStandard.KeyDown += new KeyEventHandler(OnListKeyDown);
                this.lbStandard.Dock = DockStyle.Fill;

                Array.Sort((Array)this.ColorValues, (IComparer)new StandardColorComparer());
                this.lbStandard.Items.Clear();
                Color[] values = this.ColorValues;
                for (int n = 0; n < values.Length; n++)
                    this.lbStandard.Items.Add(values[n]);

                this.standardTabPage.Controls.Add(this.lbStandard);
            }

            if ((this.systemTabPage == null) && ((this.cuiGroups & ColorUIGroups.SystemColors) != 0))
            {
                this.systemTabPage = new TabPage(this.strSystemTab);

                this.tabControl.TabPages.Add(this.systemTabPage);
                if ((this.cuiSelectedGroup == ColorUISelectedGroup.None) ||
                            (this.cuiSelectedGroup == ColorUISelectedGroup.SystemColors))
                    this.tabControl.SelectedTab = this.systemTabPage;

                this.lbSystem = new ColorEditorListBox();
                this.lbSystem.DrawMode = DrawMode.OwnerDrawFixed;
                this.lbSystem.BorderStyle = BorderStyle.FixedSingle;
                this.lbSystem.IntegralHeight = false;
                this.lbSystem.Sorted = false;
                this.lbSystem.Click += new EventHandler(OnListClick);
                this.lbSystem.DrawItem += new DrawItemEventHandler(OnListDrawItem);
                this.lbSystem.KeyDown += new KeyEventHandler(OnListKeyDown);
                this.lbSystem.Dock = DockStyle.Fill;
                this.lbSystem.FontChanged += new EventHandler(OnFontChanged);

                Array.Sort((Array)this.SystemColorValues, (IComparer)new SystemColorComparer());
                this.lbSystem.Items.Clear();
                Color[] values = this.SystemColorValues;
                for (int n = 0; n < values.Length; n++)
                    this.lbSystem.Items.Add(values[n]);

                this.systemTabPage.Controls.Add(this.lbSystem);
            }

            // UserTabPage
            if ((this.m_userTabPage == null) && ((this.cuiGroups & ColorUIGroups.UserColors) != 0))
            {
                this.m_userTabPage = new TabPage(this.m_sUserTabText);

                if (this.cuiSelectedGroup == ColorUISelectedGroup.UserColors)
                {
                    this.tabControl.SelectedTab = this.m_userTabPage;
                }

                if (m_userCustomColors == null)
                {
                    m_userCustomColors = new Color[16];
                    for (int i = 0; i < m_userCustomColors.Length; i++)
                    {
                        m_userCustomColors[i] = Color.White;
                    }
                }

                m_userColorPalette = new ColorPalette(this, m_userCustomColors);

                m_userColorPalette.Dock = DockStyle.Fill;
                m_userColorPalette.StretchOnResize = this.UserColorsStretchOnResize;
                m_userColorPalette.Picked += new EventHandler(OnPalettePick);

                m_userTabPage.Controls.Add(m_userColorPalette);
                this.tabControl.TabPages.Add(this.m_userTabPage);
            }
        }

        private void OnSelectedTabChanged(Object sender, EventArgs e)
        {
            TabPage page = this.tabControl.SelectedTab;
            if (page != null)
            {
                if (page.Equals(this.customTabPage))
                    this.cuiSelectedGroup = ColorUISelectedGroup.CustomColors;
                else if (page.Equals(this.standardTabPage))
                    this.cuiSelectedGroup = ColorUISelectedGroup.StandardColors;
                else if (page.Equals(this.systemTabPage))
                    this.cuiSelectedGroup = ColorUISelectedGroup.SystemColors;
                else if (page.Equals(this.m_userTabPage))
                    this.cuiSelectedGroup = ColorUISelectedGroup.UserColors;
                else
                    this.cuiSelectedGroup = ColorUISelectedGroup.None;
                this.AdjustListBoxItemHeight();
            }
            else this.cuiSelectedGroup = ColorUISelectedGroup.None;
        }

        /// <summary>
        /// Overridden. See <see cref="M:System.Windows.Forms.Control.OnFontChanged"/>.
        /// </summary>
        protected override/*Control*/ void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            this.AdjustListBoxItemHeight();
            //this.AdjustColorUIHeight();
        }
        /// <summary></summary>
        /// <param name="e"/>
        protected override void OnSizeChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnSizeChanged(e);
            if (!EnableTouchMode && this.DesignMode)
            {
                CTRLSIZE = this.Size;
            }
        }

        private void OnFontChanged(object sender, EventArgs e)
        {
            this.systemHeightSet = false;
            this.commonHeightSet = false;
        }

        /// <summary>
        /// Overridden. See <see cref="M:System.Windows.Forms.Control.OnGotFocus"/>.
        /// </summary>
        protected override/*Control*/ void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            this.OnTabControlSelChange(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="ColorUIControl.ColorSelected"/> event.
        /// </summary>
        /// <param name="arg">A <see cref="System.EventArgs"/> value that contains the event data.</param>
        protected virtual void OnColorSelected(EventArgs arg)
        {
            if (this.ColorSelected != null)
            {
                this.ColorSelected(this, arg);
            }
        }

        private void OnListClick(object sender, EventArgs e)
        {
            System.Windows.Forms.ListBox listBox;
            listBox = (System.Windows.Forms.ListBox)sender;
            if (listBox.SelectedItem != null)
                this.clrSelected = (Color)listBox.SelectedItem;
            else
                this.clrSelected = Color.Empty;
            this.OnColorSelected(EventArgs.Empty);
        }

        private void OnListDrawItem(object sender, DrawItemEventArgs die)
        {
            System.Windows.Forms.ListBox listBox;
            object item;
            System.Drawing.Font font;
            System.Drawing.Graphics g;
            System.Drawing.Brush brush;
            System.Drawing.Rectangle rect;
            System.Drawing.Color color;

            if (die.Index == -1)
                return;

            listBox = (System.Windows.Forms.ListBox)sender;
            item = listBox.Items[die.Index];
            font = this.Font;

            if (listBox == this.lbStandard && !this.commonHeightSet)
            {
                if (this.VisualStyle == ColorUIStyle.Metro)
                    listBox.ItemHeight = listBox.Font.Height + 5;
                else
                    listBox.ItemHeight = listBox.Font.Height;
                this.commonHeightSet = true;
            }
            else if (listBox == this.lbSystem && !this.systemHeightSet)
            {
                if (this.VisualStyle == ColorUIStyle.Metro)
                    listBox.ItemHeight = listBox.Font.Height + 5;
                else
                    listBox.ItemHeight = listBox.Font.Height;
                this.systemHeightSet = true;
            }

            g = die.Graphics;
            die.DrawBackground();
            rect = die.Bounds;
            rect = die.Bounds;
            rect = die.Bounds;
            PaintValue(item, g, new Rectangle((rect.X + 2), (rect.Y + 2), 22, (rect.Height - 4)));
            rect = die.Bounds;
            rect = die.Bounds;
            rect = die.Bounds;
            g.DrawRectangle(SystemPens.WindowText, new Rectangle((rect.X + 2), (rect.Y + 2), 21, ((rect.Height - 4) - 1)));
            brush = (Brush)new SolidBrush(die.ForeColor);
            color = (Color)item;
            rect = die.Bounds;
            rect = die.Bounds;
            g.DrawString(SR.GetString(color.Name), font, brush, ((float)(rect.X + 26)), ((float)rect.Y));
            brush.Dispose();
        }


        /// <summary>
        /// Paints a representative value of the given object to the provided canvas. 		
        /// </summary>
        /// <param name="e">The <see cref="System.Drawing.Design.PaintValueEventArgs"/> instance containing the event data.</param>
        public virtual void PaintValue(PaintValueEventArgs e)
        {
            System.Drawing.Color color;
            System.Drawing.SolidBrush brush;
            if (e.Value is System.Drawing.Color)
            {
                color = (Color)e.Value;
                brush = new SolidBrush(color);
                e.Graphics.FillRectangle(brush, e.Bounds);
                brush.Dispose();
            }
        }

        /// <summary>
        /// Paints a representative value of the given object to the provided canvas. 		
        /// </summary>
        /// <param name="value">The value to paint.</param>
        /// <param name="canvas">Gets the System.Drawing.Graphics object with which painting should be done.</param>
        /// <param name="rectangle">Gets the rectangle that indicates the area in which the painting should be done.</param>
        public void PaintValue(object value, Graphics canvas, Rectangle rectangle)
        {
            this.PaintValue(new PaintValueEventArgs(null, value, canvas, rectangle));
        }

        private void OnListKeyDown(object sender, KeyEventArgs ke)
        {
            if (ke.KeyCode == Keys.Enter)
                this.OnListClick(sender, EventArgs.Empty);
        }

        private void OnPalettePick(object sender, EventArgs e)
        {
            ColorPalette cp = (ColorPalette)sender;
            this.clrSelected = this.GetBestColor(cp.SelectedColor);
            this.OnColorSelected(EventArgs.Empty);
        }

        private void OnTabControlResize(object sender, EventArgs e)
        {
            if (this.tabControl.TabCount > 0)
            {
                TabPage tabpg = this.tabControl.TabPages[0];
                System.Drawing.Size size = tabpg.Size;
                System.Drawing.Rectangle rect = tabpg.ClientRectangle;
                rect.Y = 0;
                rect.Height = (rect.Height - rect.Y);
                int border = 2;

                if (this.systemTabPage != null)
                    this.lbSystem.SetBounds(border, (rect.Y + (2 * border)), (rect.Width - border), ((size.Height - rect.Height) + (2 * border)));
                if (this.standardTabPage != null)
                    this.lbStandard.SetBounds(border, (rect.Y + (2 * border)), (rect.Width - border), ((size.Height - rect.Height) + (2 * border)));
            }
        }


        private void OnTabControlSelChange(object sender, EventArgs e)
        {
            System.Windows.Forms.TabPage page;
            page = this.tabControl.SelectedTab;
            if (page != null && page.Controls.Count > 0)
                page.Controls[0].Focus();
        }

        /// <summary>
        /// Overridden. See <see cref="M:System.Windows.Forms.Control.ProcessDialogKey"/>.
        /// </summary>
        protected override/*Control*/ bool ProcessDialogKey(Keys keyData)
        {
            bool shift;
            int index;
            int count;

            if ((keyData & Keys.Alt) == 0 &&
                (keyData & Keys.Control) == Keys.Control &&
                (keyData & Keys.KeyCode) == Keys.Tab)
            {
                shift = ((keyData & Keys.Shift) == 0);
                index = this.tabControl.SelectedIndex;
                if (index != -1)
                {
                    count = this.tabControl.TabPages.Count;
                    if (shift)
                        index = ((index + 1) % count);
                    else
                        index = (((index + count) - 1) % count);
                    this.tabControl.SelectedTab = this.tabControl.TabPages[index];
                    return true;
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        [
        Obsolete("Use void Start(Color clrselected) instead!"),
        Syncfusion.Documentation.DocumentationExclude()
        ]
        public void Start(object obj, Color clrselected)
        {
            Start(clrselected);
        }

        /// <summary>
        /// Displays the <see cref="ColorUIControl"/> control as a drop-down component. 
        /// </summary>
        /// <param name="clrselected"> The initially selected color. </param>
        public void Start(Color clrselected)
        {
            this.AdjustColorUIHeight();

            this.clrSelected = clrselected;
            if (clrselected != Color.Empty)
                this.SetColorSelection(clrselected);
        }


        /// <summary>
        /// Ends the color display drop-down.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void End()
        {
            this.clrSelected = Color.Empty;
        }

        private void SetColorSelection(Color clrselected)
        {
            Color[] values = this.ColorValues;
            System.Windows.Forms.TabPage page = this.customTabPage;
            if (this.standardTabPage != null)
            {
                for (int n = 0; n < values.Length; n++)
                {
                    if ((values[n].Equals((object)clrselected)))
                    {
                        this.lbStandard.SelectedItem = (object)clrselected;
                        page = this.standardTabPage;
                        break;
                    }
                }
            }
            if ((this.systemTabPage != null) && (page == this.customTabPage))
            {
                values = this.SystemColorValues;
                for (int n = 0; n < values.Length; n++)
                {
                    if (values[n].Equals((object)clrselected))
                    {
                        this.lbSystem.SelectedItem = (object)clrselected;
                        page = this.systemTabPage;
                        break;
                    }
                }
            }
            if (this.customTabPage != null)
            {
                values = this.pal.StandardColors;
                for (int n = 0; n < values.Length; n++)
                {
                    if (values[n].Equals((object)clrselected))
                    {
                        this.pal.SelectedColor = clrselected;
                        page = this.customTabPage;
                        break;
                    }
                }
            }
            if (page != null)
                this.tabControl.SelectedTab = page;
        }


        // Properties

        /// <summary>
        ///      Returns an array of standard colors.
        /// </summary>
        private Color[] ColorValues
        {
            get
            {
                if (this.standardColorConstants == null)
                    this.standardColorConstants = this.GetConstants(typeof(System.Drawing.Color));
                return this.standardColorConstants;
            }
        }

        /// <summary>
        ///      Retrieves the array of custom colors for our use.
        /// </summary>
        private Color[] CustomColors
        {
            get
            {
                if (this.customColors == null)
                {
                    this.customColors = new System.Drawing.Color[16];
                    for (int n = 0; n < 16; n++)
                        this.customColors[n] = Color.White;
                }
                return this.customColors;
            }
            set
            {
                this.customColors = value;
                this.pal = null;
            }
        }

        /// <summary>
        ///      Returns an array of system colors.
        /// </summary>
        private Color[] SystemColorValues
        {
            get
            {
                if (this.systemColorConstants == null)
                    this.systemColorConstants = this.GetConstants(typeof(System.Drawing.SystemColors));
                return this.systemColorConstants;
            }
        }

        [ToolboxItem(false)]
        class ColorEditorTabControl : TabControl
        {
            Font font;
            public ColorEditorTabControl()
            {
            }
            /// <summary>
            ///Events for horizontal scroll
            /// </summary>
            public event ScrollEventHandler HScroll;
            /// <summary>
            ///Assigns the value for Oldvalue
            /// </summary>
            private int oldValue = 0;
            /// <summary>
            ///
            /// </summary>
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);
                if (m.Msg == 0x114) // WM_HSCROLL
                {
                    this.OnHScroll(new ScrollEventArgs(((ScrollEventType)LoWord(m.WParam)), oldValue, HiWord(m.WParam), ScrollOrientation.HorizontalScroll));
                }
            }
            /// <summary>
            ///
            /// </summary>
            protected void OnHScroll(ScrollEventArgs sev)
            {
                this.Invalidate();
            }
            /// <summary>
            ///
            /// </summary>
            private int LoWord(IntPtr dWord)
            {
                return dWord.ToInt32() & 0xffff;
            }
            /// <summary>
            ///
            /// </summary>
            private int HiWord(IntPtr dWord)
            {
                if ((dWord.ToInt32() & 0x80000000) == 0x80000000)
                    return (dWord.ToInt32() >> 16);
                else
                    return (dWord.ToInt32() >> 16) & 0xffff;
            }
            /// <summary>
            ///
            /// </summary>
            public void setStyle(bool value)
            {
                this.SetStyle(ControlStyles.UseTextForAccessibility | ControlStyles.UserPaint, value);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (this.Parent is ColorUIControl)
                {
                    if ((this.Parent as ColorUIControl).VisualStyle == ColorUIStyle.Metro)
                    {
                        font = new Font(this.Font, this.Font.Style);
                        Brush back_brush = new SolidBrush(ColorTranslator.FromHtml("#EBEBEB"));
                        Brush fore_brush = new SolidBrush(this.Parent.ForeColor);
                        int index = this.SelectedIndex;
                        for (int i = 0; i < TabPages.Count; i++)
                        {

                            Rectangle tabRec = new Rectangle(new Point(GetTabRect(i).X + 2, GetTabRect(i).Y + 2), GetTabRect(i).Size);
                            Rectangle rect = new Rectangle(TabPages[i].Left - 1, TabPages[i].Top - 1, TabPages[i].Width + 1, TabPages[i].Height + 1);
                            if (i == this.SelectedIndex)
                            {
                                back_brush = new SolidBrush((this.Parent as ColorUIControl).MetroColor);// ColorTranslator.FromHtml("#FF119EDA");
                                fore_brush = new SolidBrush((this.Parent as ColorUIControl).MetroForeColor);
                            }
                            else
                            {
                                back_brush = new SolidBrush(ColorTranslator.FromHtml("#EBEBEB"));
                                fore_brush = new SolidBrush(this.Parent.ForeColor);
                            }
                            e.Graphics.FillRectangle(back_brush, tabRec);
                            e.Graphics.DrawRectangle(new Pen((this.Parent as ColorUIControl).m_metroColor), tabRec);
                            string tab_name = this.TabPages[i].Text;
                            StringFormat sf = new StringFormat();
                            sf.Alignment = StringAlignment.Center;
                            sf.LineAlignment = StringAlignment.Center;
                            e.Graphics.DrawString(tab_name, (this.Parent as ColorUIControl).Font, fore_brush, tabRec, sf);
                            sf.Dispose();
                        }
                        back_brush.Dispose();
                        fore_brush.Dispose();
                    }
                    else if ((this.Parent as ColorUIControl).VisualStyle == ColorUIStyle.Office2010)
                    {
                        font = new Font(this.Font, this.Font.Style);
                        Color backColor = Color.Silver;// ColorTranslator.FromHtml("#FF119EDA");
                        Brush fore_brush = new SolidBrush(this.Parent.ForeColor);
                        int index = this.SelectedIndex;
                        for (int i = 0; i < TabPages.Count; i++)
                        {

                            Rectangle tabRec = new Rectangle(new Point(GetTabRect(i).X + 2, GetTabRect(i).Y + 2), GetTabRect(i).Size);
                            Rectangle rect = new Rectangle(TabPages[i].Left - 1, TabPages[i].Top - 1, TabPages[i].Width + 1, TabPages[i].Height + 1);
                            if (i == this.SelectedIndex)
                            {
                                backColor = Color.FromArgb(240,246,253);
                                fore_brush = new SolidBrush(Color.FromArgb(84,92,139));
                            }
                            else
                            {
                                backColor = Color.Silver;
                                fore_brush = new SolidBrush(this.Parent.ForeColor);
                            }
                            e.Graphics.FillRectangle(new SolidBrush(backColor), tabRec);
                            string tab_name = this.TabPages[i].Text;
                            StringFormat sf = new StringFormat();
                            sf.Alignment = StringAlignment.Center;
                            sf.LineAlignment = StringAlignment.Center;
                            e.Graphics.DrawString(tab_name, font, fore_brush, tabRec, sf);
                            sf.Dispose();
                        }
                        fore_brush.Dispose();
                    }
                    else
                    {
                        this.DrawMode = System.Windows.Forms.TabDrawMode.Normal;
                        this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                        base.OnPaint(e);
                    }
                }
            }
            // Methods
            protected override/*Control*/ void OnGotFocus(EventArgs e)
            {
                System.Windows.Forms.TabPage tabPage;
                tabPage = this.SelectedTab;
                if (tabPage != null && tabPage.Controls.Count > 0)
                    tabPage.Controls[0].Focus();
            }
        }

        [ToolboxItem(false)]
        class ColorEditorListBox : ListBox
        {
            // Methods
            protected override/*ListControl*/ bool IsInputKey(Keys keyData)
            {
                System.Windows.Forms.Keys keys;
                keys = keyData;
                if (keys == Keys.Enter)
                    return true;
                return base.IsInputKey(keyData);
            }
        }

        class ColorPalette : Control
        {
            // Fields
            private Color[] staticColors;
            private Color selectedColor;
            private Point focus;
            private Color[] customColors;
            private ColorUIControl colorUI;

            private readonly static int[] staticCells;

            public const int CELLS_ACROSS = 8; // 0x0008 
            public const int CELLS_DOWN = 8; // 0x0008 
            public const int CELLS_CUSTOM = 16; // 0x0010 
            public const int CELLS = 64; // 0x0040 
            public const int CELL_SIZE = 16; // 0x0010 
            public const int MARGIN = 8; // 0x0008 

            // Events
            public event System.EventHandler Picked;

            // Constructors
            static ColorPalette()
            {
                int[] ns0 = new System.Int32[48];
                ns0[0] = 0xffffff;
                ns0[1] = 0xc0c0ff;
                ns0[2] = 0xc0e0ff;
                ns0[3] = 0xc0ffff;
                ns0[4] = 0xc0ffc0;
                ns0[5] = 0xffffc0;
                ns0[6] = 0xffc0c0;
                ns0[7] = 0xffc0ff;
                ns0[8] = 0xe0e0e0;
                ns0[9] = 0x8080ff;
                ns0[10] = 0x80c0ff;
                ns0[11] = 0x80ffff;
                ns0[12] = 0x80ff80;
                ns0[13] = 0xffff80;
                ns0[14] = 0xff8080;
                ns0[15] = 0xff80ff;
                ns0[16] = 0xc0c0c0;
                ns0[17] = 0xff;
                ns0[18] = 0x80ff;
                ns0[19] = 0xffff;
                ns0[20] = 0xff00;
                ns0[21] = 0xffff00;
                ns0[22] = 0xff0000;
                ns0[23] = 0xff00ff;
                ns0[24] = 0x808080;
                ns0[25] = 0xc0;
                ns0[26] = 0x40c0;
                ns0[27] = 0xc0c0;
                ns0[28] = 0xc000;
                ns0[29] = 0xc0c000;
                ns0[30] = 0xc00000;
                ns0[31] = 0xc000c0;
                ns0[32] = 0x404040;
                ns0[33] = 0x80;
                ns0[34] = 0x4080;
                ns0[35] = 0x8080;
                ns0[36] = 0x8000;
                ns0[37] = 0x808000;
                ns0[38] = 0x800000;
                ns0[39] = 0x800080;
                ns0[41] = 64;
                ns0[42] = 0x404080;
                ns0[43] = 0x4040;
                ns0[44] = 0x4000;
                ns0[45] = 0x404000;
                ns0[46] = 0x400000;
                ns0[47] = 0x400040;
                ColorPalette.staticCells = ns0;
            }


            public ColorPalette(ColorUIControl colorUI, Color[] customColors)
            {
                this.focus = new Point(0, 0);
                this.colorUI = colorUI;
                this.SetStyle(ControlStyles.Opaque | ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true);
                this.BackColor = SystemColors.Control;
                this.Size = new Size(0xca, 0xca);
                this.staticColors = new System.Drawing.Color[48];
                for (int n0 = 0; n0 < ColorPalette.staticCells.Length; n0++)
                    this.staticColors[n0] = ColorTranslator.FromOle(ColorPalette.staticCells[n0]);
                this.customColors = customColors;
            }

            /// <summary>
            /// Gets standard colors array;
            /// </summary>
            public Color[] StandardColors
            {
                get
                {
                    return staticColors;
                }
            }

            /// <summary>
            /// Stretch color cell on resize.
            /// </summary>
            private bool m_bStretchOnResize = false;

            /// <summary>
            /// Gets or sets stretch color cell on resize.
            /// </summary>
            public bool StretchOnResize
            {
                get
                {
                    return m_bStretchOnResize;
                }
                set
                {
                    if (m_bStretchOnResize != value)
                    {
                        m_bStretchOnResize = value;
                        this.Invalidate();
                    }
                }
            }

            // Methods
            protected override/*Control*/ AccessibleObject CreateAccessibilityInstance()
            {
                return new ColorPaletteAccessibleObject(this);
            }

            private void CalculateMultiplier(ref double multiplierX, ref double multiplierY)
            {
                if (m_bStretchOnResize)
                {
                    Rectangle bounds = this.Bounds;

                    double cellDefaultSize = CELL_SIZE + MARGIN;

                    multiplierX = (bounds.Width) / (cellDefaultSize * CELLS_ACROSS);
                    multiplierY = (bounds.Height) / (cellDefaultSize * CELLS_DOWN);
                }
                else
                {
                    multiplierX = 1.0;
                    multiplierY = 1.0;
                }
            }

            private const int c_iBorderIndent = 4;

            private void FillRectWithCellBounds(int across, int down, ref Rectangle rect)
            {
                int cellDefaultSize = CELL_SIZE + MARGIN;

                double multiplierX = 0.0;
                double multiplierY = 0.0;
                this.CalculateMultiplier(ref multiplierX, ref multiplierY);

                rect.X = (int)((across * cellDefaultSize + c_iBorderIndent) * multiplierX);
                rect.Y = (int)((down * cellDefaultSize + c_iBorderIndent) * multiplierY);
                rect.Width = (int)(CELL_SIZE * multiplierX);
                rect.Height = (int)(CELL_SIZE * multiplierY);
            }

            private int Get1DFrom2D(Point pt)
            {
                return this.Get1DFrom2D(pt.X, pt.Y);
            }

            private int Get1DFrom2D(int x, int y)
            {
                if (x == -1 || y == -1)
                    return -1;
                return (x + (8 * y));
            }

            internal Point Get2DFrom1D(int cell)
            {
                int x = (cell % 8);
                int y = (cell / 8);
                return new Point(x, y);
            }

            private Point GetCell2DFromLocationMouse(int x, int y)
            {
                double multiplierX = 0.0;
                double multiplierY = 0.0;
                this.CalculateMultiplier(ref multiplierX, ref multiplierY);

                double correctedBIX = c_iBorderIndent * multiplierX;
                double correctedBIY = c_iBorderIndent * multiplierY;
                int cellDefaultSize = CELL_SIZE + MARGIN;

                int across = (int)((x + correctedBIX) / multiplierX / cellDefaultSize);
                int down = (int)((y + correctedBIY) / multiplierY / cellDefaultSize);

                if (across < 0 || down < 0 || across >= 8 || down >= 8)
                    return new Point(-1, -1);

                if (((x + correctedBIX) - (cellDefaultSize * across * multiplierX)) < CELLS_ACROSS * multiplierX
                    || ((y + correctedBIY) - (cellDefaultSize * down * multiplierY)) < CELLS_DOWN * multiplierY)
                {
                    return new Point(-1, -1);
                }

                return new Point(across, down);
            }

            private Point GetCellFromColor(Color c)
            {
                for (int down = 0; down < 8; down++)
                {
                    for (int across = 0; across < 8; across++)
                    {
                        Color color = this.GetColorFromCell(across, down);
                        if (color.Equals(c))
                            return new Point(across, down);
                    }
                }
                return Point.Empty;
            }

            private int GetCellFromLocationMouse(int x, int y)
            {
                return this.Get1DFrom2D(this.GetCell2DFromLocationMouse(x, y));
            }

            private Color GetColorFromCell(int across, int down)
            {
                return this.GetColorFromCell(this.Get1DFrom2D(across, down));
            }

            private Color GetColorFromCell(int index)
            {
                if (index < 48)
                    return this.staticColors[index];
                return this.customColors[((index - 64) + 16)];
            }

            private Color GetColorFromCellMouse(int x, int y)
            {
                return this.GetColorFromCell(this.GetCellFromLocationMouse(x, y));
            }

            private void InvalidateFocus()
            {
                for (int down = 0; down < 8; down++)
                {
                    for (int across = 0; across < 8; across++)
                    {
                        if ((this.focus).X == across && (this.focus).Y == down)
                        {
                            Rectangle rect = new System.Drawing.Rectangle();
                            this.FillRectWithCellBounds(across, down, ref rect);
                            this.Invalidate(Rectangle.Inflate(rect, 5, 5));
                        }
                    }
                }
            }

            private void InvalidateSelection()
            {
                for (int down = 0; down < 8; down++)
                {
                    for (int across = 0; across < 8; across++)
                    {
                        Color color = this.SelectedColor;
                        if (color.Equals((object)this.GetColorFromCell(across, down)))
                        {
                            Rectangle rect = new System.Drawing.Rectangle();
                            this.FillRectWithCellBounds(across, down, ref rect);
                            this.Invalidate(Rectangle.Inflate(rect, 5, 5));
                        }
                    }
                }
            }

            protected override/*Control*/ bool IsInputKey(Keys keyData)
            {
                System.Windows.Forms.Keys keys = keyData;

                if (keys == Keys.Enter
                    || keys >= Keys.Left && keys <= Keys.Down)
                    return true;

                if (keys == Keys.F2)
                    return false;

                return base.IsInputKey(keyData);
            }

            protected virtual void LaunchDialog(int customIndex)
            {
                this.Invalidate();
                //				if (this.colorUI.DropDownService != null)
                //					this.colorUI.DropDownService.CloseDropDown();
                ColorDialog colorDialog = new CustomColorDialog();
                DialogResult dialogResult = colorDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    this.customColors[customIndex] = colorDialog.Color;
                    this.SelectedColor = this.customColors[customIndex];
                    //this.OnPicked(EventArgs.Empty);
                }
                colorDialog.Dispose();
            }

            protected override/*Control*/ void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);
                this.InvalidateFocus();
            }

            protected override/*Control*/ void OnKeyDown(KeyEventArgs e)
            {
                base.OnKeyDown(e);

                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        this.SelectedColor = this.GetColorFromCell((this.focus).X, (this.focus).Y);
                        this.InvalidateFocus();
                        this.OnPicked(EventArgs.Empty);
                        return;
                    case Keys.Space:
                        this.SelectedColor = this.GetColorFromCell((this.focus).X, (this.focus).Y);
                        this.InvalidateFocus();
                        return;
                    case Keys.Left:
                        this.InvalidateFocus();
                        if ((this.focus).X > 0)
                            (this.focus).X = ((this.focus).X - 1);
                        this.InvalidateFocus();
                        return;
                    case Keys.Right:
                        this.InvalidateFocus();
                        if ((this.focus).X < 7)
                            (this.focus).X = ((this.focus).X + 1);
                        this.InvalidateFocus();
                        return;
                    case Keys.Up:
                        this.InvalidateFocus();
                        if ((this.focus).Y > 0)
                            (this.focus).Y = ((this.focus).Y - 1);
                        this.InvalidateFocus();
                        return;
                    case Keys.Down:
                        this.InvalidateFocus();
                        if ((this.focus).Y < 7)
                            (this.focus).Y = ((this.focus).Y + 1);
                        this.InvalidateFocus();
                        return;
                }
            }

            protected override/*Control*/ void OnLostFocus(EventArgs e)
            {
                base.OnLostFocus(e);
                this.InvalidateFocus();
            }

            protected override/*Control*/ void OnMouseDown(MouseEventArgs me)
            {
                base.OnMouseDown(me);
                if (me.Button == MouseButtons.Left)
                {
                    Point point = this.GetCell2DFromLocationMouse(me.X, me.Y);
                    if (point.X != -1 && point.Y != -1 && point != this.focus)
                    {
                        this.InvalidateFocus();
                        this.focus = point;
                        this.InvalidateFocus();
                    }
                }
            }

            protected override/*Control*/ void OnMouseMove(MouseEventArgs me)
            {
                base.OnMouseMove(me);
                if (me.Button == MouseButtons.Left)
                {
                    Rectangle rect = this.Bounds;
                    if (rect.Contains(me.X, me.Y))
                    {
                        Point point = this.GetCell2DFromLocationMouse(me.X, me.Y);
                        if (point.X != -1 && point.Y != -1 && point != this.focus)
                        {
                            this.InvalidateFocus();
                            this.focus = point;
                            this.InvalidateFocus();
                        }
                    }
                }
            }

            protected override/*Control*/ void OnMouseUp(MouseEventArgs me)
            {
                base.OnMouseUp(me);
                if (me.Button == MouseButtons.Left)
                {
                    Point point = this.GetCell2DFromLocationMouse(me.X, me.Y);
                    if (point.X != -1 && point.Y != -1)
                    {
                        this.focus = point;
                        this.SelectedColor = this.GetColorFromCell((this.focus).X, (this.focus).Y);
                        this.InvalidateFocus();
                        this.OnPicked(EventArgs.Empty);
                    }
                }
                if (me.Button == MouseButtons.Right)
                {
                    int cell = this.GetCellFromLocationMouse(me.X, me.Y);
                    if (cell != -1 && cell >= 48 && cell < 64)
                        this.LaunchDialog(((cell - 64) + 16));
                }
            }

            protected override/*Control*/ void OnPaint(PaintEventArgs pe)
            {
                System.Drawing.Graphics g = pe.Graphics;
                int rownumbers = 0;
                using (Brush brush = new SolidBrush(this.BackColor))
                    g.FillRectangle(brush, this.ClientRectangle);
                Rectangle rect = new System.Drawing.Rectangle();
                bool found = false;
                rownumbers = colorUI.ShowUserSelectionColors ? 8 : 6;
                for (int y = 0; y < rownumbers; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        Color color = this.GetColorFromCell(this.Get1DFrom2D(x, y));
                        this.FillRectWithCellBounds(x, y, ref rect);
                        if (color.Equals(this.SelectedColor) && !found)
                        {
                            ControlPaint.DrawBorder(g, Rectangle.Inflate(rect, 3, 3), SystemColors.ControlText, ButtonBorderStyle.Solid);
                            found = true;
                        }
                        if ((this.focus).X == x && (this.focus).Y == y && this.Focused)
                            if (this.colorUI.VisualStyle != ColorUIStyle.Metro)
                                ControlPaint.DrawFocusRectangle(g, Rectangle.Inflate(rect, 5, 5), SystemColors.ControlText, SystemColors.Control);
                        if (this.colorUI.VisualStyle != ColorUIStyle.Metro)
                            ControlPaint.DrawBorder(g, Rectangle.Inflate(rect, 2, 2), SystemColors.Control, 2, ButtonBorderStyle.Inset, SystemColors.Control, 2, ButtonBorderStyle.Inset, SystemColors.Control, 2, ButtonBorderStyle.Inset, SystemColors.Control, 2, ButtonBorderStyle.Inset);
                        else
                            ControlPaint.DrawBorder(g, Rectangle.Inflate(rect, 1, 1), SystemColors.ControlText, ButtonBorderStyle.Solid);
                        this.PaintValue(color, g, rect);
                    }
                }
                base.OnPaint(pe);
            }

            protected void OnPicked(EventArgs e)
            {
                if (this.Picked != null)
                    this.Picked(this, e);
            }

            private void PaintValue(Color color, Graphics g, Rectangle rect)
            {
                using(Brush brush=new SolidBrush(color))
                    g.FillRectangle(brush, rect);
            }

            protected override/*Control*/ bool ProcessDialogKey(Keys keyData)
            {
                int cell;
                if (keyData == Keys.F2)
                {
                    cell = this.Get1DFrom2D((this.focus).X, (this.focus).Y);
                    if (cell >= 48 && cell < 64)
                        this.LaunchDialog(((cell - 64) + 16));
                    return true;
                }
                return base.ProcessDialogKey(keyData);
            }

            // Properties
            public Color[] CustomColors
            {
                get
                {
                    return this.customColors;
                }
            }

            public Color SelectedColor
            {
                get
                {
                    return this.selectedColor;
                }
                set
                {
                    if (!value.Equals(this.selectedColor))
                    {
                        this.InvalidateSelection();
                        this.selectedColor = value;
                        this.InvalidateFocus();
                        this.focus = this.GetCellFromColor(value);
                        this.InvalidateSelection();
                    }
                }
            }

            class ColorPaletteAccessibleObject : ControlAccessibleObject
            {
                // Fields
                private ColorCellAccessibleObject[] cells;

                // Constructors
                public ColorPaletteAccessibleObject(ColorPalette owner)
                    : base(owner)
                {
                    this.cells = new ColorCellAccessibleObject[64];
                }

                // Methods
                public override AccessibleObject GetChild(int id)
                {
                    if (id < 0 || id >= 64)
                        return null;

                    if (this.cells[id] == null)
                        this.cells[id] = new ColorCellAccessibleObject(this, this.ColorPalette.GetColorFromCell(id), id);
                    return (AccessibleObject)this.cells[id];
                }


                public override int GetChildCount()
                {
                    return 64;
                }


                public override AccessibleObject HitTest(int x, int y)
                {
                    NativeMethods.POINT point = new NativeMethods.POINT(x, y);
                    NativeMethods.ScreenToClient(this.ColorPalette.Handle, ref point);
                    int id = this.ColorPalette.GetCellFromLocationMouse(point.X, point.Y);
                    if (id != -1)
                        return this.GetChild(id);
                    return base.HitTest(x, y);
                }

                // Properties
                internal ColorPalette ColorPalette
                {
                    get
                    {
                        return (ColorPalette)this.Owner;
                    }
                }

                class ColorCellAccessibleObject : AccessibleObject
                {
                    // Fields
                    private Color color;
                    private ColorPaletteAccessibleObject parent;
                    private int cell;

                    // Constructors
                    public ColorCellAccessibleObject(ColorPaletteAccessibleObject parent, Color color, int cell)
                    {
                        this.parent = parent;
                        this.color = color;
                        this.cell = cell;
                    }

                    // Properties
                    public override/*AccessibleObject*/ Rectangle Bounds
                    {
                        get
                        {
                            System.Drawing.Point point = this.parent.ColorPalette.Get2DFrom1D(this.cell);
                            System.Drawing.Rectangle rect = new System.Drawing.Rectangle();
                            this.parent.ColorPalette.FillRectWithCellBounds(point.X, point.Y, ref rect);
                            NativeMethods.POINT nativePoint = new NativeMethods.POINT(rect.X, rect.Y);
                            NativeMethods.ClientToScreen(this.parent.ColorPalette.Handle, ref nativePoint);
                            return new Rectangle(nativePoint.X, nativePoint.Y, rect.Width, rect.Height);
                        }
                    }
                    public override/*AccessibleObject*/ string Name
                    {
                        get
                        {
                            return (this.color).ToString();
                        }
                    }
                    public override/*AccessibleObject*/ AccessibleObject Parent
                    {
                        get
                        {
                            return (AccessibleObject)this.parent;
                        }
                    }
                    public override/*AccessibleObject*/ AccessibleRole Role
                    {
                        get
                        {
                            return AccessibleRole.Cell;
                        }
                    }
                    public override/*AccessibleObject*/ string Value
                    {
                        get
                        {
                            return (this.color).ToString();
                        }
                    }
                } // ColorCellAccessibleObject 


            } // ColorPaletteAccessibleObject 
        }

        [ToolboxItem(false)]
        class CustomColorDialog : ColorDialog
        {
            // Fields
            private IntPtr hInstance;
            private const int COLOR_HUE = 703 /*0x02BF*/;
            private const int COLOR_SAT = 704 /*0x02C0*/;
            private const int COLOR_LUM = 705 /*0x02C1*/;
            private const int COLOR_RED = 706 /*0x02C2*/;
            private const int COLOR_GREEN = 707 /*0x02C3*/;
            private const int COLOR_BLUE = 708 /*0x02C4*/;
            private const int COLOR_ADD = 712 /*0x02C8*/;
            private const int COLOR_MIX = 719 /*0x02CF*/;

            public const int WM_INITDIALOG = 272 /*0x0110*/;
            public const int WM_COMMAND = 273 /*0x0111*/;


            // Constructors
            public CustomColorDialog()
            {
                System.IO.Stream stream = null;

                if (System.Globalization.CultureInfo.CurrentUICulture.Name == "en-US")
                {
                    stream = typeof(CustomColorDialog).Module.Assembly.GetManifestResourceStream(@"Syncfusion.Windows.Forms.ColorPicker.colordlg.data");
                }
                else
                {
                    try
                    {
                        System.Reflection.Assembly assembly = typeof(CustomColorDialog).Module.Assembly.GetSatelliteAssembly(System.Globalization.CultureInfo.CurrentUICulture);

                        if (assembly != null)
                        {
                            stream = assembly.GetManifestResourceStream(@"Syncfusion.Windows.Forms.ColorPicker.Resources.colordlg.data");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                }
                if (stream == null)
                {
                    stream = typeof(CustomColorDialog).Module.Assembly.GetManifestResourceStream(@"Syncfusion.Windows.Forms.ColorPicker.colordlg.data");
                }

                int count = ((int)(stream.Length - stream.Position));
                byte[] buffer = new byte[count];
                stream.Read(buffer, 0, count);
                this.hInstance = Marshal.AllocHGlobal(count);
                Marshal.Copy(buffer, 0, this.hInstance, count);
            }



            // Methods
            protected override/*Component*/ void Dispose(bool disposing)
            {
                if (this.hInstance != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(this.hInstance);
                    this.hInstance = IntPtr.Zero;
                }
                base.Dispose(disposing);
            }

            protected override/*CommonDialog*/ IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
            {
                IntPtr handle;
                byte red;
                byte green;
                byte blue;
                bool[] err;

                switch (msg)
                {
                    case WM_INITDIALOG:
                        NativeMethods.SendDlgItemMessage(hwnd, COLOR_HUE, 0xd3, (IntPtr)3, IntPtr.Zero);
                        NativeMethods.SendDlgItemMessage(hwnd, COLOR_SAT, 0xd3, (IntPtr)3, IntPtr.Zero);
                        NativeMethods.SendDlgItemMessage(hwnd, COLOR_LUM, 0xd3, (IntPtr)3, IntPtr.Zero);
                        NativeMethods.SendDlgItemMessage(hwnd, COLOR_RED, 0xd3, (IntPtr)3, IntPtr.Zero);
                        NativeMethods.SendDlgItemMessage(hwnd, COLOR_GREEN, 0xd3, (IntPtr)3, IntPtr.Zero);
                        NativeMethods.SendDlgItemMessage(hwnd, COLOR_BLUE, 0xd3, (IntPtr)3, IntPtr.Zero);
                        handle = NativeMethods.GetDlgItem(hwnd, COLOR_MIX);
                        NativeMethods.EnableWindow(handle, false);
                        NativeMethods.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, 0x80);
                        handle = NativeMethods.GetDlgItem(hwnd, 1);
                        NativeMethods.EnableWindow(handle, false);
                        NativeMethods.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, 0x80);
                        this.Color = Color.Empty;
                        break;
                    case WM_COMMAND:
                        if (NativeMethods.LOWORD((int)wParam) == COLOR_ADD)
                        {
                            err = new bool[1];
                            red = (byte)NativeMethods.GetDlgItemInt(hwnd, COLOR_RED, err, false);
                            green = (byte)NativeMethods.GetDlgItemInt(hwnd, COLOR_GREEN, err, false);
                            blue = (byte)NativeMethods.GetDlgItemInt(hwnd, COLOR_BLUE, err, false);
                            this.Color = Color.FromArgb((int)red, (int)green, (int)blue);
                            NativeMethods.PostMessage(hwnd, WM_COMMAND, (IntPtr)NativeMethods.MAKELONG(1, 0), NativeMethods.GetDlgItem(hwnd, 1));
                        }
                        break;
                }
                return base.HookProc(hwnd, msg, wParam, lParam);
            }



            // Properties
            protected override/*ColorDialog*/ IntPtr Instance
            {
                get
                {
                    return this.hInstance;
                }
            }
            protected override/*ColorDialog*/ int Options
            {
                get
                {
                    return 66;
                }
            }
        } // CustomColorDialog

        class StandardColorComparer : IComparer
        {
            // Methods
            public /*IComparer*/ int Compare(object x, object y)
            {
                System.Drawing.Color color0;
                System.Drawing.Color color1;
                color0 = (Color)x;
                color1 = (Color)y;
                if (color0.A < color1.A)
                    return -1;
                if (color0.A > color1.A)
                    return 1;
                if (((float)color0.GetHue()) < ((float)color1.GetHue()))
                    return -1;
                if (((float)color0.GetHue()) > ((float)color1.GetHue()))
                    return 1;
                if (((float)color0.GetSaturation()) < ((float)color1.GetSaturation()))
                    return -1;
                if (((float)color0.GetSaturation()) > ((float)color1.GetSaturation()))
                    return 1;
                if (((float)color0.GetBrightness()) < ((float)color1.GetBrightness()))
                    return -1;
                if (((float)color0.GetBrightness()) > ((float)color1.GetBrightness()))
                    return 1;
                return 0;
            }
        }

        class SystemColorComparer : IComparer
        {
            // Methods
            public /*IComparer*/ int Compare(object x, object y)
            {
                Color color0 = (Color)x;
                Color color1 = (Color)y;
                return String.Compare(color0.Name, color1.Name, false, System.Globalization.CultureInfo.CurrentUICulture);
            }
        }
    }
    /// <summary>
    /// ColorUI Style
    /// </summary>
    public enum ColorUIStyle
    {
        /// <summary>
        /// Classic appearance.
        /// </summary>
        Default,
        Office2010,
        /// <summary>
        /// Metro-like appearance.
        /// </summary>
        Metro
    }
    /// <summary>
    /// CheckBoxAdv Designer
    /// </summary>
    public class ColorUIDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        /// <summary>
        /// Designer ActionList collection
        /// </summary>
        private System.ComponentModel.Design.DesignerActionListCollection actionLists;

        /// <summary>
        ///  Initializes a new instance of the CheckBoxAdvDesigner class
        /// </summary>
        public ColorUIDesigner()
            : base()
        {
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

        /// <summary>
        /// Gets a value indication the designer action
        /// </summary>
        public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == this.actionLists)
                {
                    this.actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
                    this.actionLists.Add(new ColorUIActionList(this.Component));
                }

                return this.actionLists;
            }
        }

#endif
    }
}

