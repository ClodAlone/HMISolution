#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace Syncfusion.Windows.Forms
{
    class MetroTabsRenderer : TabsRendererBase
    {
          #region Class constants
        private const int c_defaultOverlapWidth = 16;
        private const int c_cornerCut = 2;
        #endregion

        #region Class members
  
        #endregion

        #region Class initialize

        public MetroTabsRenderer(InternalTab parent)
            : base( parent )
        {
           }

        #endregion

        #region Class properties
        /// </override>
        protected override Font ActiveTabFont
        {
            get
            {
                return new Font( base.ActiveTabFont, FontStyle.Bold );
            }
        }

        /// </override>
        protected override Color ForeColor
        {
            get
            {
                if( this.Parent != null )
                {
                    if (this.Parent.Pushed)
                        return Color.White;
                    else
                        return Color.Black;
                }
                else
                {
                    return base.ForeColor;
                }
            }
        }

        /// </override>

        #endregion
		/// <summary>
		///Gets metro highight color
		/// </summary>
        private Color metroHighlightColor = ColorTranslator.FromHtml("#16A5DC");
		/// <summary>
		/// Gets or sets metrohighlight color
		/// </summary>
        public Color MetroHighlightColor
        {
            get { return metroHighlightColor; }
            set { metroHighlightColor = value; }
        }
		/// <summary>
		/// Gets metronormal color.
		/// </summary>
        private Color metroNormalColor = ColorTranslator.FromHtml("#D1D3D4");
		/// <summary>
		/// Gets or sets metro normal color.
		/// </summary>
        public Color MetroNormalColor
        {
            get { return metroNormalColor; }
            set { metroNormalColor = value; }
        }





        #region Class overrides

        /// </override>
        public override void DrawBackground( Graphics g )
        {
            Rectangle rectFill = this.Bounds;

        
            if( this.Parent.Pushed )
            {
                if( this.Parent.Hovered )
                {
                }

                using (Brush br = new SolidBrush(MetroHighlightColor))
                {                   
                    g.FillRectangle ( br, rectFill );
                }

            }
            else
            {

                 using (Brush br = new SolidBrush(MetroNormalColor))
                {
                    g.FillRectangle( br, rectFill);
                }
           }
        }


        
        /// </override>
        public override void DrawBorders(Graphics g)
        {
        
        }

        protected override void DrawTextAndImage(Graphics g, Rectangle rectTextAndImage)
        {
            base.DrawTextAndImage(g, rectTextAndImage);
        }

        /// </override>
        public override Size GetItemPreferredSize()
        {
            Size size = base.GetItemPreferredSize();
            size.Width += c_defaultOverlapWidth;
            return size;
        }

        /// </override>
        public override int GetOverlappedWidth()
        {
            return 0;
        }
        #endregion

  
    }
}
