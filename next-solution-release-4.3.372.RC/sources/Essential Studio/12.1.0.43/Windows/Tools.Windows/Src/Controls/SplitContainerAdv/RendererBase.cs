#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region System dependencies

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Windows.Forms.Tools.Enums;
#endregion Syncfusion dependencies

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
    /// <summary>
    /// A class in which all renderer per-instance settings are stored.
    /// </summary>
    public class RendererInfo : IRendererInfo
    {
        #region Members

        /// <summary>
        /// Default for this theme value for background brush.
        /// </summary>
        protected BrushInfo m_bgBrush;

        /// <summary>
        /// Current renderer drawing orientation. Usually reflects container orientation.
        /// </summary>
        private Orientation m_orientation;

        /// <summary>
        /// Remembers the original orientation of the container.
        /// </summary>
        protected bool m_bOrientationChanged;

        /// <summary>
        /// Current control draw state;
        /// </summary>
        private DrawState m_eDrawState;

        /// <summary>
        /// Indicates whether the control themes are enabled.
        /// </summary>
        private bool m_bThemesEnabled;

        /// <summary>
        /// Indicates whether the control is enabled.
        /// </summary>
        private bool m_bEnabled;

        /// <summary>
        /// Indicates whether the control's theme background should be ignored.
        /// </summary>
        private bool m_bIgnoreThemeBackground;

        /// <summary>
        /// Themes control drawing.
        /// </summary>
        private ThemedControlDrawing m_tcd;

        #endregion Members

        #region Properties
        /// <summary>
        /// Gets or sets Background color
        /// The background color, gradient and other styles can be set through 
        /// this property.
        /// </summary>
        /// <remarks>
        /// The SplitContainerAdv control provides this property to enable specialized
        /// custom gradient backgrounds.
        /// </remarks>
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
                }
            }
        }

        /// <summary>
        /// Gets or sets current renderer instance orientation.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return m_orientation;
            }
            set
            {
                if (value != m_orientation)
                {
                    m_orientation = value;

                    OnOrientationChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether theming is enabled for the control.
        /// </summary>
        public bool ThemesEnabled
        {
            get
            {
                return m_bThemesEnabled;
            }
            set
            {
                if (value != m_bThemesEnabled)
                {
                    m_bThemesEnabled = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if control is enabled.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return m_bEnabled;
            }
            set
            {
                if (value != m_bEnabled)
                {
                    m_bEnabled = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control will ignore the theme's background color and draw the backcolor instead.
        /// </summary>
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
                }
            }
        }

        /// <summary>
        /// Gets or sets a themed control.
        /// </summary>
        public ThemedControlDrawing ThemedControl
        {
            get
            {
                return m_tcd;
            }
            set
            {
                if (value != m_tcd)
                {
                    m_tcd = value;
                }
            }
        }

        #endregion Properties

        #region Events
        /// <summary>
        ///  Raised when orientation changed
        /// </summary>
        public event EventHandler OrientationChanged;
        #endregion

        #region Raisers
        /// <summary>
        /// Raiser for event OrientationChanged
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected virtual void OnOrientationChanged(EventArgs e)
        {
            if (OrientationChanged != null)
            {
                OrientationChanged(this, e);
            }
        }
        #endregion

        #region IRendererInfo Members

        /// <summary>
        /// Gets or sets current control state.
        /// </summary>
        public DrawState DrawState
        {
            get
            {
                return m_eDrawState;
            }
            set
            {
                if (m_eDrawState != value)
                {
                    m_eDrawState = value;
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// A class a renderer should be derived from.
    /// </summary>
    public abstract class Renderer :
        IRenderer
    {
        #region IRenderer members
        /// <summary>
        /// Override this method to provide correct update renderer-specified information from control.
        /// </summary>
        /// <param name="instance">An instance of holding container.</param>
        /// <param name="iri">An instance of RendererInfo, which should be updated.</param>
        /// <returns>Modified RendererInfo instance.</returns>
        public virtual IRendererInfo UpdateRendererInfo(SplitContainerAdv instance, IRendererInfo iri)
        {
            RendererInfo ri = iri as RendererInfo;

            if (null == iri)
            {
                ri = new RendererInfo();
            }

            ri.Orientation = instance.Orientation;
            ri.DrawState = instance.DrawState;
            ri.Enabled = instance.Enabled;
            ri.IgnoreThemeBackground = instance.IgnoreThemeBackground;
            ri.ThemedControl = instance.ThemedControl;
            ri.ThemesEnabled = instance.ThemesEnabled;

            return ri;
        }

        /// <summary>
        /// Override this to draw a background.
        /// </summary>
        /// <param name="e">Paint context.</param>
        /// <param name="ri">An instance of RendererInfo, which should be used as datasource for painting.</param>
        /// <param name="bounds">Bounds within which we should paint.</param>
        public abstract void DrawBackground(PaintEventArgs e, IRendererInfo ri, Rectangle bounds);

        /// <summary>
        /// Override this to provide hot background capabilities to your control's theme.
        /// Hot background means that control can change its background color when under mouse cursor.
        /// </summary>
        /// <param name="e">Paint context.</param>
        /// <param name="ri">An instance of RendererInfo, which should be used as datasource for painting.</param>
        /// <param name="bounds">Bounds within which we should paint.</param>
        public abstract void DrawHotBackground(PaintEventArgs e, IRendererInfo ri, Rectangle bounds);

        /// <summary>
        /// Override this to provide thumbnail drawing according to your theme.
        /// </summary>
        /// <param name="e">Paint context.</param>
        /// <param name="ri">An instance of RendererInfo, which should be used as datasource for painting.</param>
        /// <param name="bounds">Bounds within which we should paint.</param>
        public abstract void DrawThumbnail(PaintEventArgs e, IRendererInfo ri, Rectangle bounds);

        /// <summary>
        /// Override this to provide hot thumbnail drawing according to your theme.
        /// Hot thumbnail means that control can change its thumbnail color when under mouse cursor.
        /// </summary>
        /// <param name="e">Paint context.</param>
        /// <param name="ri">An instance of RendererInfo, which should be used as datasource for painting.</param>
        /// <param name="bounds">Bounds within which we should paint.</param>
        public abstract void DrawHotThumbnail(PaintEventArgs e, IRendererInfo ri, Rectangle bounds);

        /// <summary>
        /// Override this to provide foreground drawing.
        /// </summary>
        /// <param name="e">Paint context.</param>
        /// <param name="ri">An instance of RendererInfo, which should be used as datasource for painting.</param>
        /// <param name="bounds">Bounds within which we should paint.</param>
        public abstract void DrawForeground(PaintEventArgs e, IRendererInfo ri, Rectangle bounds);

        /// <summary>
        /// Override this to provide hot foreground.
        /// Hot foreground means that control can change its hot foreground color when under mouse cursor.
        /// </summary>
        /// <param name="e">Paint context.</param>
        /// <param name="ri">An instance of RendererInfo, which should be used as datasource for painting.</param>
        /// <param name="bounds">Bounds within which we should paint.</param>
        public abstract void DrawHotForeground(PaintEventArgs e, IRendererInfo ri, Rectangle bounds);

        /// <summary>
        /// For usage only within SplitContainerAdv class. Just point "this" to this method, so
        /// control properties will became appropriate for this theme.
        /// These properties are NOT used while drawing "default" theme. We're just blanking them.
        /// </summary>
        /// <remarks>
        /// Override this to provide custom settings saving. In overriden method initialize container
        /// properties you want to modify, with appropriate values from RendererInfo class.
        /// </remarks>
        /// <param name="instance">Spliter container</param>
        /// <param name="bInit">Bool value</param>
        /// <returns>Modified RenderInfo.</returns>
        public virtual IRendererInfo GetAppropriateThemeSettings(SplitContainerAdv instance, bool bInit)
        {
            return UpdateRendererInfo(instance, null);
        }

        /// <summary>
        /// Override this to compare with default renderer settings.
        /// </summary>
        /// <param name="property"> Property identifier. </param>
        /// <param name="value"> Compared value. </param>
        /// <returns> True if value is equal and false in other case.</returns>
        public virtual bool CompareWithDefaultValue(RendererProperty property, object value)
        {
            if (null == value)
                return false;

            return value.Equals(GetDefaultValue(property));
        }

        /// <summary>
        /// Gets default value for some property.
        /// </summary>
        /// <param name="property"> Property identifier. </param>
        /// <returns> Default value. </returns>
        public abstract object GetDefaultValue(RendererProperty property);

        /// <summary>
        /// Override this to draw splitter according to renderer settings.
        /// </summary>
        /// <param name="e">Paint context.</param>
        /// <param name="ri">An instance of RendererInfo, which should be used as datasource for painting.</param>
        /// <param name="bounds">Bounds within which we should paint.</param>
        public virtual void Draw(PaintEventArgs e, IRendererInfo ri, Rectangle bounds)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (ri.DrawState == DrawState.Normal)
            {
                DrawBackground(e, ri, bounds);
                DrawForeground(e, ri, bounds);
                DrawThumbnail(e, ri, bounds);
            }
            else if (ri.DrawState == DrawState.Hovered)
            {
                DrawHotBackground(e, ri, bounds);
                DrawHotForeground(e, ri, bounds);
                DrawHotThumbnail(e, ri, bounds);
            }
        }

        #endregion IRenderer Members

        #region Methods
        /// <summary>
        /// Provides with an appropriate renderer instance.
        /// </summary>
        /// <param name="style">Style by which an appropriate renderer is to be selected.</param>
        /// <returns>An instance of Render's inheritor class, regarding to provided style.</returns>
        public static Renderer GetRenderer(Style style)
        {
            Renderer renderer = BasicRenderer.GetInstance();
            switch (style)
            {
                case Style.Office2007Blue:
                    renderer = Office2007BlueRenderer.GetInstance();
                    break;
                case Style.Office2007Black:
                    renderer = Office2007BlackRenderer.GetInstance();
                    break;
                case Style.Office2007Silver:
                    renderer = Office2007SilverRenderer.GetInstance();
                    break;
                case Style.Office2003:
                    renderer = Office2003Renderer.GetInstance();
                    break;
                case Style.OfficeXP:
                    renderer = OfficeXPRenderer.GetInstance();
                    break;
                case Style.VS2005:
                    renderer = VS2005Renderer.GetInstance();
                    break;
                case Style.Mozilla:
                    renderer = MozillaRenderer.GetInstance();
                    break;
                case Style.Default:
                    renderer = DefaultRenderer.GetInstance();
                    break;
                default:
                    break;
            }
            return renderer;
        }

        #endregion Methods
    }
}
