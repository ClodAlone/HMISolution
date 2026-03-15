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
using System.Drawing;

namespace Syncfusion.Windows.Forms.Tools
{
    public class TabRendererMetro : TabRenderer2D
    {

        protected override void DrawFocusRect(Graphics g, RectangleF focusRect, Color fore, Color back)
        {
            //base.DrawFocusRect(g, focusRect, fore, back);
        }

        protected override void DrawText(Graphics g, RectangleF rectText, string text, StringFormat format, DrawTabEventArgs e)
        {
            
            if (base.IsSelectedState(e.State) && e.Font.Bold)
                e.ForeColor = Color.White;
            base.DrawText(g, rectText, text, format, e);
           // SolidBrush b = new SolidBrush (Color.White );
           // g.DrawString (text,this
        }
        protected override void DrawBackground(DrawTabEventArgs drawItemInfo)
        {
            // Take a look at TabControlAdv.Init for notes on why we need this check.
            if (this.panelRenderer.TabPanelBackColor != drawItemInfo.BackColor
                || !this.panelRenderer.IsBackgroundSolid())
            {
                Graphics g = drawItemInfo.Graphics;

                g.FillRectangle(new SolidBrush(drawItemInfo.BackColor), drawItemInfo.Bounds);
                RectangleF rect = new RectangleF(0, g.ClipBounds.Top + (g.ClipBounds.Height - 2), g.ClipBounds.Width, 2);
                if(this.panelRenderer.TabPanelData.Alignment == System.Windows.Forms.TabAlignment.Top)
                    g.FillRectangle(new SolidBrush(drawItemInfo.BackColor), rect);
            }
        }

		public new static string TabStyleName { 
            get
            {
                return "Metro";
            }
		}

		static FlatTabPanelProperty tabPanelPropertyExtender;

	
		public new static FlatTabPanelProperty TabPanelPropertyExtender
		{
			get{return tabPanelPropertyExtender;}
		}
		static TabRendererMetro()
		{
			
			tabPanelPropertyExtender = new FlatTabPanelProperty();
			 TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererMetro), TabPanelPropertyExtender);

		
			TabRenderer2D.buttonBorderColors[0] = SystemColors.ControlDarkDark;
		}

        public TabRendererMetro(ITabControl parent, ITabPanelRenderer panelRenderer)
			: base(parent, panelRenderer)
		{
		}
        new public static void RegisterTabType()
        {
            tabPanelPropertyExtender = new FlatTabPanelProperty();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererMetro), TabPanelPropertyExtender);
        }

        protected override void DrawBorders(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;


            RectangleF currentBounds = TabUtils.ApplyTransform(g, this.TabAlignment,
                drawItemInfo.Bounds, true);

            this.ApplyTransform(g);


            g.ResetTransform();
        }
    }
    public class FlatTabPanelProperty : TabPanelProperty2D
    {
		/// <summary>
		///
		/// </summary>
        public override Color DefaultInactiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return ColorTranslator.FromHtml("#D1D3D4");
        }
		/// <summary>
		///
		/// </summary>
        public override Color DefaultActiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return ColorTranslator.FromHtml("#16A5DC");
        }
		/// <summary>
		///
		/// </summary>
        public override Color DefaultFixedSingleBorderColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return ColorTranslator.FromHtml("#16A5DC");
        }
       
      
    }
}
