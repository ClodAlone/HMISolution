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
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;

using Syncfusion.Windows.Forms.Tools;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Syncfusion.Runtime.InteropServices;
using System.ComponentModel;
#endregion

namespace Syncfusion.Windows.Forms
{	/// <summary>
    /// Render button in metro style
    /// </summary>
   internal class MetroButtonRenderer:ButtonRenderer 
    {	
       #region Class Constants
		/// <summary>
		/// Default radius truncation coreners of the control.
		/// </summary>
		private const int DEF_BORDERS_RADIUS = 0;
		/// <summary>
		/// Angle for vertical gradient brush.
		/// </summary>
		private const int DEF_VERTICAL_BRUSH_ANGLE = 1;
		/// <summary>
		/// Width for brush.
		/// </summary>
		private const int DEF_WIDTH_BRUSH = 1;
		/// <summary>
		/// 
		/// </summary>
		private const ButtonAdvState HIGHTLIGHTED = ButtonAdvState.MouseOver | ButtonAdvState.Pressed;
		#endregion

		#region Class Members
		/// <summary>
		/// Blend for selected control.
		/// </summary>
		private Blend m_blButtonSelected = null;
		/// <summary>
		/// Blend for control.
		/// </summary>
		private Blend m_blButtonDefault = null;
		/// <summary>
		/// Blend for pressed control.
		/// </summary>
		private Blend m_blButtonPressed = null;
		/// <summary>
		/// Blend for disabled control.
		/// </summary>
		private Blend m_blButtonDisabled = null;
        ///Metro Color
        ///</summary>
        private Color m_metroColor;
        /// <summary>
        /// Metro ForeColor
        /// </summary>
        private Color m_metroForeColor;
		#endregion
        #region Property
        private Color MetroForeColor
        {
            get
            {
                return m_metroForeColor;
            }
            set
            {
                m_metroForeColor = Button.ForeColor;
                value = m_metroForeColor;
            }
        }
        #endregion
        #region Class Initialize/Finalize methods
        /// <summary></summary>
        /// <param name="button"/>
        /// 
       public MetroButtonRenderer(ButtonAdv button)
           : base(button)
        {
            m_blButtonSelected = new Blend();
            m_blButtonSelected.Positions = new float[] { 0.0F, 0.45F, 0.5F, 1.0F };
            m_blButtonSelected.Factors = new float[] { 0.0F, 0.4F, 0.8F, 0.2F };

            m_blButtonDefault = new Blend();
            m_blButtonDefault.Positions = new float[] { 0.0F, 0.45F, 0.45F, 1.0F };
            m_blButtonDefault.Factors = new float[] { 0.0F, 0.4F, 1.1F, 0.0F };

            m_blButtonPressed = new Blend();
            m_blButtonPressed.Positions = new float[] { 0.0F, 0.50F, 0.55F, 1.0F };
            m_blButtonPressed.Factors = new float[] { 0.0F, 0.6F, 1.0F, 0.4F };

            m_blButtonDisabled = new Blend();
            m_blButtonDisabled.Positions = new float[] { 0.0F, 0.45F, 0.5F, 1.0F };
            m_blButtonDisabled.Factors = new float[] { 0.0F, 0.4F, 1.0F, 0.6F };
           
        }
        /// <summary>Make class cleanup</summary>
       /// <param name="disposing"></param>
       protected override void OnDispose(bool disposing)
       {
           if (disposing)
           {
               m_blButtonSelected = null;
               m_blButtonDefault = null;
               m_blButtonPressed = null;
               m_blButtonDisabled = null;
           }

           base.OnDispose(disposing);
       }
        #endregion

       #region Class Utility Methods
       ///<summary>
       ///Sets metro color      
       ///</summary>
       public override void SetMetroColor(Color metroColor)
       {
           this.m_metroColor = metroColor;
           this.Button.ForeColor = Color.White;
           this.Button.BackColor = Color.FromArgb(22, 165, 220);
       }

       /// <summary>
       /// Gets vertical gradient brush.
       /// </summary>
       private LinearGradientBrush GetVerticalBrush(ref Rectangle rc, Color cl1, Color cl2)
       {
           Rectangle rcBrush = new Rectangle(rc.Left, rc.Top, DEF_WIDTH_BRUSH, rc.Height);

           return new LinearGradientBrush(rcBrush, cl1, cl2, DEF_VERTICAL_BRUSH_ANGLE);
       }

       /// <summary>
       /// Draws background.
       /// </summary>
       private void DrawBackground(Graphics g, Rectangle rc, ButtonAdvState state)
       {
           if (rc.Width > 0 && rc.Height > 0)
           {
               GraphicsState gState = g.Save();
               g.SmoothingMode = SmoothingMode.Default;

               DrawBorder(g, rc, state, this.Button.Enabled);


               g.Restore(gState);
           }
       }

       /// <summary>
       /// Draws border.
       /// </summary>
       private void DrawBorder(Graphics g, Rectangle rc, ButtonAdvState state, bool enabled)
       {
           rc.X  = -1;
           if (rc.Width > 0 && rc.Height > 0)
           {
               GraphicsState gState = g.Save();
               g.SmoothingMode = SmoothingMode.AntiAlias;
               
               if (enabled)
               {
                   Color buttonColor = Button.BackColor;
                   switch (state)
                   {
                       case ButtonAdvState.Default:
                           {
                               buttonColor = Button.BackColor;
                               break;
                           }
                       case ButtonAdvState.MouseOver:
                           {
                               if (!Button.IsBackStageButton)
                                   buttonColor = ControlPaint.LightLight(buttonColor);
                               else
                                   buttonColor = Button.BackColor;
                               break;
                           }
                       case ButtonAdvState.MouseOver | ButtonAdvState.Pressed:
                           {
                               buttonColor = Button.BackColor;
                               break;
                           }
                   }
                   SolidBrush brush = new SolidBrush(buttonColor);
                   g.FillRectangle(brush, rc);
                   brush.Dispose();
               }
               else
               {
                   if (!Button.IsBackStageButton)
                   {
                       SolidBrush brush = new SolidBrush(Color.Gray);
                       g.FillRectangle(brush, rc);
                       brush.Dispose();
                   }
                   else
                   {
                       SolidBrush brush = new SolidBrush(Button.BackColor);
                       g.FillRectangle(brush, rc);
                       brush.Dispose();
                   }
               }

               g.Restore(gState);
           }
       }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="baseColor"></param>
       /// <param name="blendColor"></param>
       /// <returns></returns>
       internal static Color MergeColors(Color baseColor, Color blendColor)
       {
           int r = MergeChannels(baseColor.R, blendColor.R);
           int g = MergeChannels(baseColor.G, blendColor.G);
           int b = MergeChannels(baseColor.B, blendColor.B);

           return Color.FromArgb(r, g, b);
       }
       /// <summary>
       /// 
       /// </summary>
       /// <param name="baseChannel"></param>
       /// <param name="blendChannel"></param>
       /// <returns></returns>
       private static int MergeChannels(int baseChannel, int blendChannel)
       {
           const int MAX = 255;

           int min = (baseChannel * blendChannel) / MAX;
           int max = MAX - ((MAX - baseChannel) * (MAX - blendChannel)) / MAX;

           return (byte)(min + (baseChannel * (max - min)) / MAX);
       }
 
       /// <summary>
       /// Fill rectangle with gradient for disabled control.
       /// </summary>
       private void PaintGradientDisabled(Graphics g, Rectangle rc, Color clBegin, Color clEnd)
       {
           if (rc.Width > 0 && rc.Height > 0)
           {
               using (LinearGradientBrush brush = GetVerticalBrush(ref rc, clBegin, clEnd))
               {
                   brush.Blend = m_blButtonDisabled;
                   brush.WrapMode = WrapMode.TileFlipXY;
                   g.FillRectangle(brush, rc);
               }
           }
       }
       #endregion

       #region Class Overrides
       /// <summary></summary>
       /// <param name="g"></param>        
       public override void Render(Graphics g)
       {
           DrawBackground(g, this.bounds, this.Button.State);
           DrawTextAndImage(g);
       }

       /// <summary>
       /// Specifies region for drawing
       /// </summary>
       public override Region GetRegion(Rectangle bounds)
       {
           bounds.X = bounds.X -1;
           bounds.Y = bounds.Y ;        
                     
           Region r = new Region(bounds); 
           return r;
       }      
       #endregion        
    }
}