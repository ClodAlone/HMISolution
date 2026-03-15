#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region System dependencies
using System.Drawing;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Tools;
#endregion Syncfusion dependencies

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
    /// <summary>
    /// Renderer of Office2007Blue style.
    /// </summary>
    public class Office2007BlueRenderer : OfficeXPRenderer
    {
        #region Class construction
        /// <summary>
        /// Initializes a new instance of the Office2007BlueRenderer class. 
        /// No possibility to construct this class "normally"!
        /// Use FancyRenderer.GetInstance() property to retrieve instance pointer instead.
        /// </summary>
        protected Office2007BlueRenderer()
        {
            if (null == m_rendererInfo)
                m_rendererInfo = new BasicRendererInfo();

            // Initializing default values for this theme.
            m_defaultBackgroundColor = new BrushInfo(GradientStyle.Horizontal, Color.FromArgb(227, 239, 255), Color.FromArgb(101, 147, 207));
            m_defaultHotBackgroundColor = new BrushInfo(GradientStyle.Horizontal, Color.FromArgb(252, 151, 61), Color.FromArgb(255, 184, 94));
            m_defaultExpandFill = new BrushInfo(Color.FromArgb(101, 147, 207));
            m_defaultHotExpandFill = new BrushInfo(Color.FromArgb(101, 147, 207));
            m_defaultExpandLine = SystemColors.ControlDarkDark;
            m_defaultHotExpandLine = SystemColors.ControlDarkDark;
            m_defaultGripDark = new BrushInfo(SystemColors.ControlDarkDark);
            m_defaultHotGripDark = new BrushInfo(Color.FromArgb(101, 147, 207));
            m_defaultGripLight = new BrushInfo(Color.FromArgb(227, 239, 255));
            m_defaultHotGripLight = new BrushInfo(Color.FromArgb(227, 239, 255));
            m_bUseOrientation = true;
            SetDefaultSettings();
        }
        #endregion Class construction

        #region Methods
        /// <summary>
        /// Retrieves an instance of Office2007Renderer
        /// </summary>
        /// <returns>"new Office2007Renderer()"</returns>
        public static new Office2007BlueRenderer GetInstance()
        {
            return new Office2007BlueRenderer();
        }
        #endregion Methods
    }

    /// <summary>
    /// Renderer of Office2007Black style.
    /// </summary>
    public class Office2007BlackRenderer : OfficeXPRenderer
    {
        #region Class construction
        /// <summary>
        /// Initializes a new instance of the Office2007BlackRenderer class.
        /// No possibility to construct this class "normally"!
        /// Use FancyRenderer.GetInstance() property to retrieve instance pointer instead.
        /// </summary>
        protected Office2007BlackRenderer()
        {
            if (null == m_rendererInfo)
                m_rendererInfo = new BasicRendererInfo();

            // Initializing default values for this theme.
            m_defaultBackgroundColor = new BrushInfo(GradientStyle.Horizontal, Color.FromArgb(108, 108, 108), Color.FromArgb(82, 82, 82));
            m_defaultHotBackgroundColor = new BrushInfo(GradientStyle.Horizontal, Color.FromArgb(255, 240, 171), Color.FromArgb(255, 223, 103));
            m_defaultExpandFill = new BrushInfo(Color.FromArgb(151, 151, 151));
            m_defaultHotExpandFill = new BrushInfo(Color.FromArgb(101, 107, 207));
            m_defaultExpandLine = Color.FromArgb(106, 106, 106);
            m_defaultHotExpandLine = Color.FromArgb(101, 107, 207);
            m_defaultGripDark = new BrushInfo(Color.FromArgb(141, 141, 141));
            m_defaultHotGripDark = new BrushInfo(Color.FromArgb(101, 147, 207));
            m_defaultGripLight = new BrushInfo(Color.FromArgb(227, 239, 255));
            m_defaultHotGripLight = new BrushInfo(Color.FromArgb(255, 255, 255));
            m_bUseOrientation = true;
            SetDefaultSettings();
        }
        #endregion Class construction

        #region Methods
        /// <summary>
        /// Retrieves an instance of Office2007Renderer
        /// </summary>
        /// <returns>"new Office2007Renderer()"</returns>
        public static new Office2007BlackRenderer GetInstance()
        {
            return new Office2007BlackRenderer();
        }
        #endregion Methods
    }

    /// <summary>
    /// Renderer of Office2007Silver style.
    /// </summary>
    public class Office2007SilverRenderer : OfficeXPRenderer
    {
        #region Class construction
        /// <summary>
        /// Initializes a new instance of the Office2007SilverRenderer class.
        /// No possibility to construct this class "normally"!
        /// Use FancyRenderer.GetInstance() property to retrieve instance pointer instead.
        /// </summary>
        protected Office2007SilverRenderer()
        {
            if (null == m_rendererInfo)
                m_rendererInfo = new BasicRendererInfo();

            // Initializing default values for this theme.
            m_defaultBackgroundColor = new BrushInfo(GradientStyle.Horizontal, Color.FromArgb(232, 235, 240), Color.FromArgb(207, 211, 221));
            m_defaultHotBackgroundColor = new BrushInfo(GradientStyle.Horizontal, Color.FromArgb(255, 240, 171), Color.FromArgb(255, 223, 103));
            m_defaultExpandFill = new BrushInfo(Color.FromArgb(171, 171, 171));
            m_defaultHotExpandFill = new BrushInfo(Color.FromArgb(101, 107, 207));
            m_defaultExpandLine = Color.FromArgb(141, 141, 141);
            m_defaultHotExpandLine = Color.FromArgb(101, 107, 207);
            m_defaultGripDark = new BrushInfo(Color.FromArgb(161, 161, 161));
            m_defaultHotGripDark = new BrushInfo(Color.FromArgb(101, 147, 207));
            m_defaultGripLight = new BrushInfo(Color.FromArgb(227, 239, 255));
            m_defaultHotGripLight = new BrushInfo(Color.FromArgb(255, 255, 255));
            m_bUseOrientation = true;
            SetDefaultSettings();
        }
        #endregion Class construction

        #region Methods
        /// <summary>
        /// Retrieves an instance of Office2007Renderer
        /// </summary>
        /// <returns>"new Office2007Renderer()"</returns>
        public static new Office2007SilverRenderer GetInstance()
        {
            return new Office2007SilverRenderer();
        }
        #endregion Methods
    }
}
