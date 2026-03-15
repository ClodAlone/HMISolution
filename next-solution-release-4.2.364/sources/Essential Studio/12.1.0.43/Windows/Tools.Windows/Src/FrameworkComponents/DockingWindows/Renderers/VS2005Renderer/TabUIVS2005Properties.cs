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

#region Class using directives
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
#endregion

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
	/// <summary>
	/// TabUIVS2005Properties describes properties for painting AH tab panel in
	/// VS2005 style.
	/// </summary>
	public class TabUIVS2005Properties
		: TabUIDefaultProperties
	{
		#region Class properties
		/// <summary>
		///Indicates whether to draw from left to right.
		/// </summary>
		public override bool DrawLeftToRight
		{
			get
			{
				return false;
			}
		}
		/// </override>
		public override Color DefaultTabPanelBackgroundColor(ITabPanelData panelData, ITabControl tabControl)
		{
			return VS2005Colors.PanelColor;
		}
		/// </override>
		public override Color DefaultInactiveTabColor( ITabPanelData panelData, ITabControl tabControl )
		{
			return VS2005Colors.TabItemColor;
		}
		/// </override>
		public override Color DefaultActiveTabColor(ITabPanelData panelData, ITabControl tabControl)
		{
			return VS2005Colors.TabItemColor;
		}
		#endregion

		#region Class constants
		/// <summary>
		/// Space between top border and panel.
		/// </summary>
		private const int DEF_OVERLAP_HEIGHT = 10;
		#endregion

		#region Class public methods
		/// <summary>
		/// Returns the size by which the selected tab overlaps the inactive tabs.
		/// </summary>
		public override SizeF GetOverlapSize( SizeF tabSize )
		{
			return new SizeF( DEF_OVERLAP_HEIGHT, 0 );
		}
		/// <summary>
		/// Paints autohide tab panel.
		/// </summary>
		/// <param name="tabControl"></param>
		/// <param name="g"></param>
		/// <param name="bgColor"></param>
		/// <param name="bounds"></param>
		public override void OnPaintPanelBackground( ITabControl tabControl, Graphics g, Color bgColor, Rectangle bounds )
		{
			Brush brush;
			TabAlignment alignment = tabControl.Renderer.TabPanelData.Alignment;

			switch( alignment )
			{ 
				case TabAlignment.Top:
				case TabAlignment.Bottom:
					brush = new LinearGradientBrush(bounds, VS2005Colors.LeftAHPanelColor
						, VS2005Colors.RightAHPanelColor, LinearGradientMode.Horizontal);
					break;

				case TabAlignment.Left:
					brush = new SolidBrush(VS2005Colors.RightAHPanelColor);
					break;

				case TabAlignment.Right:
					brush = new SolidBrush(VS2005Colors.LeftAHPanelColor);
					break;

				default:
					brush = new SolidBrush(VS2005Colors.LeftAHPanelColor);
					break;
			}

			g.FillRectangle(brush, bounds);
		}
		#endregion
	}

    /// <summary>
    /// TabUIVS2010Properties describes properties for painting AH tab panel in
    /// VS2010 style.
    /// </summary>
    public class TabUIVS2010Properties
        : TabUIDefaultProperties
    {
        #region Class properties
        /// <summary>
        ///Indicates whether to draw from left to right.
        /// </summary>
        public override bool DrawLeftToRight
        {
            get
            {
                return false;
            }
        }
        /// </override>
        public override Color DefaultTabPanelBackgroundColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return VS2005Colors.PanelColor;
        }
        /// </override>
        public override Color DefaultInactiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return VS2005Colors.TabItemColor;
        }
        /// </override>
        public override Color DefaultActiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return VS2005Colors.TabItemColor;
        }
        #endregion

        #region Class constants
        /// <summary>
        /// Space between top border and panel.
        /// </summary>
        private const int DEF_OVERLAP_HEIGHT = 10;
        #endregion

        #region Class public methods
        /// <summary>
        /// Returns the size by which the selected tab overlaps the inactive tabs.
        /// </summary>
        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            return new SizeF(DEF_OVERLAP_HEIGHT, 0);
        }
        /// <summary>
        /// Paints autohide tab panel.
        /// </summary>
        /// <param name="tabControl"></param>
        /// <param name="g"></param>
        /// <param name="bgColor"></param>
        /// <param name="bounds"></param>
        public override void OnPaintPanelBackground(ITabControl tabControl, Graphics g, Color bgColor, Rectangle bounds)
        {
            Brush brush;
            TabAlignment alignment = tabControl.Renderer.TabPanelData.Alignment;

            switch (alignment)
            {
                case TabAlignment.Top:
                case TabAlignment.Bottom:
                    brush = new LinearGradientBrush(bounds, VS2005Colors.LeftAHPanelColor
                        , VS2005Colors.RightAHPanelColor, LinearGradientMode.Horizontal);
                    break;

                case TabAlignment.Left:
                    brush = new SolidBrush(VS2005Colors.RightAHPanelColor);
                    break;

                case TabAlignment.Right:
                    brush = new SolidBrush(VS2005Colors.LeftAHPanelColor);
                    break;

                default:
                    brush = new SolidBrush(VS2005Colors.LeftAHPanelColor);
                    break;
            }
            brush = new SolidBrush(Color.FromArgb(41, 57, 85));//FromArgb(239, 239, 242) );
            g.FillRectangle(brush, bounds);
        }
        #endregion
    }

    public class TabUIVS2012Properties
        : TabUIDefaultProperties
    {
        #region Class properties
        /// <summary>
        ///Indicates whether to draw from left to right.
        /// </summary>
        public override bool DrawLeftToRight
        {
            get
            {
                return false;
            }
        }
        /// </override>
        public override Color DefaultTabPanelBackgroundColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return Color.FromArgb(239, 239, 242);
        }
        /// </override>
        public override Color DefaultInactiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return Color.FromArgb(239, 239, 242); ;
        }
        /// </override>
        public override Color DefaultActiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return Color.FromArgb(104, 113, 118); ;
        }
        #endregion

        #region Class constants
        /// <summary>
        /// Space between top border and panel.
        /// </summary>
        private const int DEF_OVERLAP_HEIGHT = 10;
        #endregion

        #region Class public methods
        /// <summary>
        /// Returns the size by which the selected tab overlaps the inactive tabs.
        /// </summary>
        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            return new SizeF(DEF_OVERLAP_HEIGHT, 0);
        }
        /// <summary>
        /// Paints autohide tab panel.
        /// </summary>
        /// <param name="tabControl"></param>
        /// <param name="g"></param>
        /// <param name="bgColor"></param>
        /// <param name="bounds"></param>
        public override void OnPaintPanelBackground(ITabControl tabControl, Graphics g, Color bgColor, Rectangle bounds)
        {
            Brush brush;
            TabAlignment alignment = tabControl.Renderer.TabPanelData.Alignment;

            switch (alignment)
            {
                case TabAlignment.Top:
                case TabAlignment.Bottom:
                    brush = new LinearGradientBrush(bounds, VS2005Colors.LeftAHPanelColor
                        , VS2005Colors.RightAHPanelColor, LinearGradientMode.Horizontal);
                    break;

                case TabAlignment.Left:
                    brush = new SolidBrush(VS2005Colors.RightAHPanelColor);
                    break;

                case TabAlignment.Right:
                    brush = new SolidBrush(VS2005Colors.LeftAHPanelColor);
                    break;

                default:
                    brush = new SolidBrush(VS2005Colors.LeftAHPanelColor);
                    break;
            }
            brush = new SolidBrush(Color.White);//FromArgb(239, 239, 242) );
            g.FillRectangle(brush, bounds);
        }
        #endregion
    }
}
