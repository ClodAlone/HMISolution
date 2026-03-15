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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Design;
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using Syncfusion.Windows.Forms.Design.Serialization;
#endif

#endregion

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary></summary>
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
    [Docking(DockingBehavior.Never)]
#endif
    [
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
	DesignerSerializer( typeof( SplitPanelAdvSerializer ), typeof( CodeDomSerializer ) ), 
#endif
ToolboxItem(false),
Designer(typeof(SplitPanelAdvDesigner))
]
    public class SplitPanelAdv : Panel
    {
        #region Class constants
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// Each split panel should have unique name in VS2003 designer.
		/// </summary>
		protected internal string m_strUniqueName = GenerateUniqueName();
#endif
        /// <summary>
        /// Used for unique panel names generation.
        /// </summary>
        private const string DEF_PANEL_NAME = "Panel";
        #endregion

        #region Class members
        /// <summary>
        /// Determines, is panel collapsed or not.
        /// </summary>
        private bool m_bCollapsed = false;
        /// <summary>
        /// Used for unique panel names generation.
        /// </summary>
        private static int m_panelUniqueIndex = 0;
        /// <summary>
        /// Parent contaiener, this splitter belongs to.
        /// </summary>
        private SplitContainerAdv m_splitContainer = null;
        /// <summary>
        /// Background brush.
        /// </summary>
        private BrushInfo m_bgBrush = BrushInfo.Empty;
        /// <summary></summary>
        private bool m_bThemesEnabled;
        /// <summary></summary>
        private ThemedControlDrawing m_tcd;
        /// <summary></summary>
        private bool m_bIgnoreThemeBackground = false;
        #endregion

        #region Class properties
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// Each split panel should have unique name in VS2003 designer.
		/// </summary>
		protected internal string UniqueName
		{
			get
			{
				return m_strUniqueName;
			}
		  set
		  {
        if( null != value )
        {
          m_strUniqueName = value;
        }
		  }
		}
#endif


        /// <summary></summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new bool TabStop
        {
            get
            {
                return base.TabStop;
            }
        }
        /// <summary></summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new int TabIndex
        {
            get
            {
                return base.TabIndex;
            }
        }

        /// <summary></summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override AnchorStyles Anchor
        {
            get
            {
                return base.Anchor;
            }
        }

        /// <summary></summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]

        public new string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }

        /// <summary></summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new Point Location
        {
            get
            {
                return base.Location;
            }
            set
            {
                base.Location = value;
            }
        }
        /// <summary></summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new BorderStyle BorderStyle
        {
            get
            {
                return base.BorderStyle;
            }
        }

        /// <summary>
        /// The background color, gradient and other styles can be set through 
        /// this property.
        /// </summary>
        /// <remarks>
        /// The GradientPanel control provides this property to enable specialized
        /// custom gradient backgrounds.
        /// </remarks>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Category("Appearance"),
        Description("Lets you set the background color, gradient, etc.")
        ]
        public BrushInfo BackgroundColor
        {
            get
            {
                return m_bgBrush;
            }

            set
            {
                if (m_bgBrush != value)
                {
                    m_bgBrush = value;
                    Invalidate();
                }
            }
        }
        /// <summary>
        /// Indicates whether the control is themed.
        /// </summary>
        [
        Description("Indicates if the control is themed."),
        Category("Appearance"),
        DefaultValue(false)
        ]
        public bool ThemesEnabled
        {
            get
            {
                return m_bThemesEnabled;
            }
            set
            {
                if (m_bThemesEnabled != value)
                {
                    m_bThemesEnabled = value;
                    UpdateStyles();
                    InvalidateWindow();
                    OnThemeChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Indicates whether the control will ignore the theme's background color and draw the backcolor instead.
        /// </summary>
        [
        Description("Indicates if the control will ignore the theme's background color and draw the backcolor instead."),
        Category("Appearance"),
        DefaultValue(false)
        ]
        public bool IgnoreThemeBackground
        {
            get
            {
                return m_bIgnoreThemeBackground;
            }
            set
            {
                if (m_bIgnoreThemeBackground != value)
                {
                    m_bIgnoreThemeBackground = value;
                    Invalidate();
                }
            }
        }

        /// <summary></summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new Size Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
            }
        }

        /// <summary></summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new bool Visible
        {
            get
            {
                return !m_bCollapsed;
            }
        }

        /// <summary></summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override DockStyle Dock
        {
            get
            {
                return base.Dock;
            }
        }

        /// <summary>
        /// Indicates, draw panel selected or not at design-time.
        /// </summary>
        protected internal bool DrawSelected
        {
            get
            {
                bool bDrawSelected = (m_splitContainer != null &&
                    m_splitContainer.SelectedPanel == this);

                return bDrawSelected;
            }
        }
        /// <summary>
        /// Determines, is panel collapsed or not.
        /// </summary>
        protected internal bool Collapsed
        {
            get
            {
                return m_bCollapsed;
            }
            set
            {
                if (value != m_bCollapsed)
                {
                    base.Visible = !value;

                    m_bCollapsed = value;
                }
            }
        }
        /// <summary></summary>
        protected bool IsVerticalGradient
        {
            get
            {
                return (m_bgBrush != null
                    && m_bgBrush.Style == BrushStyle.Gradient
                    && m_bgBrush.GradientStyle == GradientStyle.Vertical);
            }
        }
        /// <summary></summary>
        protected bool IsHorizontalGradient
        {
            get
            {
                return (m_bgBrush != null
                    && m_bgBrush.Style == BrushStyle.Gradient
                    && m_bgBrush.GradientStyle == GradientStyle.Horizontal);
            }
        }
        #endregion

        #region Class events
        /// <summary></summary>
        [Category("Property Changed")]
        public event EventHandler ThemeChanged;
        #endregion Events

        #region Required Designer Variables
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="container"/>
        public SplitPanelAdv(SplitContainerAdv container)
        {
            if (container == null)
            {
                throw new NullReferenceException("container");
            }

            // allow using transparent background
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            // set repaint on resize
            base.SetStyle(ControlStyles.ResizeRedraw, true);

            // optimize painting( reduce flicker )
            base.SetStyle(ControlStyles.AllPaintingInWmPaint |
                WhidbeyCompatibleControlStyles.DoubleBuffer, true);

            //PatternStyle style = PatternStyle.None;

            /*
              if( container.Orientation == Orientation.Horizontal )
              {
                  style = PatternStyle.Horizontal;
              }
              else
              {
                  style = PatternStyle.Vertical;
              }
        */

            //m_bgBrush = new BrushInfo( style, DefaultForeColor, DefaultBackColor );

            if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed)
            {
                m_tcd = new ThemedControlDrawing(ThemedControls.EDIT, this);
            }

            base.Visible = true;
            m_splitContainer = container;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();

                if (m_tcd != null)
                {
                    m_tcd.Dispose();
                    m_tcd = null;
                }
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion

        #region Class codedom serialization
        /// <summary></summary>
        protected void ResetBackgroundColor()
        {
            m_bgBrush = BrushInfo.Empty;
        }

        /// <summary></summary>
        /// <returns></returns>
        protected bool ShouldSerializeBackgroundColor()
        {
            return !m_bgBrush.Equals(BrushInfo.Empty);
        }
        #endregion

        #region Painting
        /// <summary></summary>
        /// <param name="e"/>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            RaisePaintEvent(this, e);
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            if (Height <= 0 || Width <= 0)
            {
                return;
            }

            if (null == this.BackgroundImage)
            {
                if (!(ThemesEnabled && XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed) ||
          IgnoreThemeBackground)
                {
                    if (BackgroundColor != BrushInfo.Empty)
                    {
                        BrushPaint.FillRectangle(e.Graphics, ClientRectangle, BackgroundColor);
                    }
                }
                else
                {
                    if (Enabled)
                    {
                        if (m_tcd != null)
                        {
                            m_tcd.DrawThemeBackground(e.Graphics, 1, 1, new Rectangle(-2, -2, Width + 4, Height + 4));
                        }
                    }
                    else
                    {
                        Brush br = new SolidBrush((BackColor == SystemColors.Window) ? SystemColors.Control : BackColor);
                        e.Graphics.FillRectangle(br, ClientRectangle);
                        br.Dispose();
                    }
                }
            }
        }

        /// <summary></summary>
        private void InvalidateWindow()
        {
            Invalidate();
        }

        /// <summary></summary>
        /// <param name="e"/>
        protected virtual void OnThemeChanged(EventArgs e)
        {
            if (ThemeChanged != null)
            {
                ThemeChanged(this, e);
            }
        }
        #endregion Painting

        #region Class utility methods
        /// <summary></summary>
        /// <param name="style"/>
        protected internal void SetBorderStyle(BorderStyle style)
        {
            base.BorderStyle = style;
        }

        /// <summary></summary>
        /// <returns></returns>
        private static string GenerateUniqueName()
        {
            return DEF_PANEL_NAME + (++m_panelUniqueIndex).ToString();
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClick(EventArgs e)
        {
            SplitContainerAdv container = this.Parent as SplitContainerAdv;

            if (container != null)
            {
                container.OnPanelClick(this);
            }

            base.OnClick(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDoubleClick(EventArgs e)
        {
            SplitContainerAdv container = this.Parent as SplitContainerAdv;

            if (null != container)
            {
                container.OnPanelDoubleClick(this);
            }

            base.OnDoubleClick(e);
        }
        #endregion
    }
}