#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region System dependencies

using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Tools;
#endregion Syncfusion dependencies

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
    /// <summary>
    /// VS2005 style renderer.
    /// </summary>
    public class VS2005Renderer
      : BasicRenderer
    {
        #region Overridables
        /// <summary>
        /// Draws a background.
        /// </summary>
        /// <param name="e">Paint context.</param>
        /// <param name="ri">An instance of RendererInfo, which should be used as datasource for painting.</param>
        /// <param name="bounds">Bounds within which we should paint.</param>
        public override void DrawBackground(PaintEventArgs e, IRendererInfo ri, Rectangle bounds)
        {
            BasicRendererInfo fri = ri as BasicRendererInfo;

            if (fri != null)
            {
                BrushPaint.FillRectangle(e.Graphics, bounds, fri.BackgroundColor);
            }
        }
        #endregion Constants

        #region initialize\finalize methods
        /// <summary>
        /// Initializes a new instance of the VS2005Renderer class.
        /// </summary>
        protected VS2005Renderer()
            : base()
        {
            m_defaultBackgroundColor = new BrushInfo(Color.FromArgb(244, 242, 232));
            m_defaultHotBackgroundColor = new BrushInfo(Color.FromArgb(244, 242, 232));
            SetDefaultSettings();
        }
        #endregion

        #region class Methods
        /// <summary>
        /// Retrieves an instance of DefaultRenderer
        /// </summary>
        /// <returns>"new DefaultRenderer()"</returns>
        public static new VS2005Renderer GetInstance()
        {
            return new VS2005Renderer();
        }
        #endregion Methods
    }
}
