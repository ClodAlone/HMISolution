#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.HTMLUI;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;B&gt; Tag.
    /// </summary>
    [ElementTag(TagName.Sub)]
    public class SUBElementImpl : SUPElementImpl
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Sub;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the SUBElementImpl class
        /// </summary>
        /// <param name="parent">Parent element for this object.</param>
        public SUBElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
        }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Overridden. Draws text in the element.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="rect">Rectangle for output.</param>
        /// <param name="text">Text object.</param>
        /// <param name="parent">Parent block for the text.</param>
        protected override void DrawText(Graphics g, Rectangle rect, Text text, Block parent)
        {
            //// Shift  location to the bottom.
            int fontHeight = this.Format.Font.Height;
            int y = rect.Y + (int)((fontHeight / DEF_FONT_SIZE_MULTIPLIER) - fontHeight);
            rect.Y = y;

            base.DrawText(g, rect, text, parent);
        }
        #endregion
    }
}