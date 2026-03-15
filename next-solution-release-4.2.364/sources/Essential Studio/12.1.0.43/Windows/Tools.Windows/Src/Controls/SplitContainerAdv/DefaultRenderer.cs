using System;
#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Drawing;
using System.Text;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
    /// <summary>
    /// Default renderer style.
    /// </summary>
    public class DefaultRenderer
      : BasicRenderer
    {
        #region Class construction
        /// <summary>
        /// Initializes a new instance of the DefaultRenderer class.
        /// No possibility to construct this class "normally"!
        /// Use MozillaRenderer.GetInstance() method to retrieve instance pointer instead.
        /// </summary>
        protected DefaultRenderer()
        {
            if (null != m_rendererInfo)
            {
                m_defaultBackgroundColor = new BrushInfo(SystemColors.Control);
                m_defaultHotBackgroundColor = new BrushInfo(SystemColors.Control);
                SetDefaultSettings();
            }
        }
        #endregion Class construction

        #region Methods
        /// <summary>
        /// Retrieves an instance of DefaultRenderer
        /// </summary>
        /// <returns>"new DefaultRenderer"</returns>
        public static new DefaultRenderer GetInstance()
        {
            return new DefaultRenderer();
        }
        #endregion Methods
    }
}
