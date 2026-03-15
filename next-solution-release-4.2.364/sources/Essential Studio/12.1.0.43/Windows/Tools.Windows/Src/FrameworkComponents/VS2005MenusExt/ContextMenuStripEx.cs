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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// ContextMenuStrip extended with title.
	/// </summary>
	[Description("Represents Office 2007 Style context menuStrip with title.")]
	[ToolboxBitmap(typeof(ContextMenuStripEx), "ToolboxIcons.ContextMenuStripEx.bmp")]
	public class ContextMenuStripEx
		: ContextMenuStrip
	{
		#region Initialization
		/// <summary>
		/// Creates new instance of ContextMenuStripEx.
		/// </summary>
        public enum ContextMenuStyle
        {
            Default,
            Metro
        }
            
		public ContextMenuStripEx()
			: this( string.Empty )
		{
		}
		/// <summary>
		/// Creates & initializes new instance of ContextMenuStripEx.
		/// </summary>
        public ContextMenuStripEx(string title)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(ContextMenuStripEx));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            this.Renderer = new Office12ToolStripRenderer(new OfficeBlue());

            if (title != null)
            {
                this.Text = title;
            }
            CTRLSIZE = this.Size;
            FONTSTYLE = this.Font;
            USERFONTSTYLE = FONTSTYLE;
        }
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

        /// <summary>
        /// Default font style of the control
        /// </summary>
        private static Font FONTSTYLE = default(Font);

        /// <summary>
        /// Font which stored after changed in design
        /// </summary>
        private static Font USERFONTSTYLE = default(Font);
		/// <summary>
		///MetroColor of Contextmenu
		/// </summary>
        private Color metroColor = ColorTranslator.FromHtml("#CCECF9");
		/// <summary>
		///Gets or Set the Metrocolor
		/// </summary>
        public Color MetroColor
        {
            get { return metroColor; }
            set
            {
                metroColor = value;

                if (style == ContextMenuStyle.Metro)
                {
                    this.Renderer = new newMetroToolStripRenderer(metroColor);
                    this.Invalidate();
                }
            }
        }

        private ContextMenuStyle style;
        public ContextMenuStyle Style
        {
            get { return style; }
            set
            {
                style = value;

                if (style == ContextMenuStyle.Default)
                {

                    this.Renderer = new Office12ToolStripRenderer(new OfficeBlue());

                }
                else if (style == ContextMenuStyle.Metro)
                {
                    this.Renderer = new newMetroToolStripRenderer(metroColor);
                    this.DropShadowEnabled = false;
                }
            }
        }
             
		#endregion

        #region For Touch

        bool isScaling = false;

        bool _touchMode = false;
        /// <summary>
        ///Gets or Sets the TouchMode
        /// </summary>
		[DefaultValue(false)]
        public bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                        ApplyScaleToControl(1.5F);
                    else
                        ApplyScaleToControl(1);
                }
            }
        }

        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }
        /// <summary>
        ///Applies scale factor
        /// </summary>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            if (FONTSTYLE.Name == USERFONTSTYLE.Name)
                this.Font = new Font(FONTSTYLE.FontFamily, FONTSTYLE.Size * scaleFactor);
            else
                this.Font = new Font(USERFONTSTYLE.FontFamily, FONTSTYLE.Size * scaleFactor);
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }

        #endregion

		#region Overrides
		/// <summary>
		/// Extends height for title.
		/// </summary>
		/// <param name="proposedSize"></param>
		/// <returns></returns>
		public override Size GetPreferredSize( Size proposedSize )
		{
			Size szResult = base.GetPreferredSize( proposedSize );
			szResult.Height += this.TitleHeight;

			szResult.Width = Math.Max(szResult.Width, this.TitleSize.Width + CONTEXTMENUSTRIP_MARGIN*2);
			return szResult;
		}
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.Renderer is Office12ToolStripRenderer && !string.IsNullOrEmpty(Text))
            {
                Office12ToolStripRenderer renderer = this.Renderer as Office12ToolStripRenderer;
                SolidBrush titleBgBrush = new SolidBrush(renderer.OfficeColorTable.ContextMenuTitle);
                SolidBrush titleTextBrush = new SolidBrush(renderer.OfficeColorTable.RibbonTabText);
                e.Graphics.FillRectangle(titleBgBrush, 1, 1, this.Width - 2, this.TitleHeight + 2);
                Rectangle rect = new Rectangle(0, 0, this.Width, this.TitleHeight);
                Rectangle textRect = Rectangle.Inflate(rect, -ContextMenuStripEx.CONTEXTMENUSTRIP_MARGIN, -ContextMenuStripEx.CONTEXTMENUSTRIP_MARGIN);
                e.Graphics.DrawString(Text, this.Font, titleTextBrush, textRect);
                titleBgBrush.Dispose();
                titleTextBrush.Dispose();
            }
        }
		protected override void Dispose(bool disposing)
		{
			if (disposing)
            {
                //this.Text = null;
                
                this.Renderer = null;

                if (this.m_font != null)
                {
                    this.m_font.Dispose();
                    this.m_font = null;
                }
            }
            base.Close();
            //base.DestroyHandle();    
			base.Dispose(disposing);
            GC.Collect();
            
		}
		/// <summary>
		/// Resets title height.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnTextChanged( EventArgs e )
		{
			m_szTitle = Size.Empty;
			base.OnTextChanged( e );

			PerformLayout();
		}
		/// <summary>
		/// Resets title height.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFontChanged( EventArgs e )
		{
			m_font = null;
			m_szTitle = Size.Empty;
            if (!isScaling)
            {
                if (USERFONTSTYLE != this.Font)
                    USERFONTSTYLE = this.Font;
            }
			base.OnFontChanged( e );

			PerformLayout();
		}
		/// <summary>
		/// Disables right mouse button
		/// </summary>
		/// <param name="mea"></param>
		protected override void OnMouseDown(MouseEventArgs mea)
		{
			if (mea.Button == MouseButtons.Left)
			{
				base.OnMouseDown(mea);
				base.RightToLeft = this.RightToLeft;
			}
		}
		/// <summary>
		/// Disables right mouse button
		/// </summary>
		/// <param name="mea"></param>
		protected override void OnMouseUp(MouseEventArgs mea)
		{
			if (mea.Button == MouseButtons.Left)
			{
				base.OnMouseUp(mea);
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the bounds of the display rectangle.
		/// </summary>
		public override Rectangle DisplayRectangle
		{
			get
			{
				Rectangle rc = base.DisplayRectangle;
				rc.Y += this.TitleHeight;
				rc.Height -= this.TitleHeight;
           
                rc.Width = this.Width;
				return rc;
			}
		}
		/// <summary>
		/// Gets or sets title height.
		/// </summary>
		internal int TitleHeight
		{
			get
			{
				return Math.Max(m_iTitleHeight, this.TitleSize.Height);
			}
			set
			{
				m_iTitleHeight = value;
			}
		}
		/// <summary>
		/// Gets or sets Title Size
		/// </summary>
		internal Size TitleSize
		{
			get
			{
				if (m_szTitle.IsEmpty)
				{
					if (!string.IsNullOrEmpty(this.Text))
					{
						m_szTitle = TextRenderer.MeasureText(this.Text, this.HeaderFont);
						m_szTitle.Height += CONTEXTMENUSTRIP_MARGIN * 2;
					}
				}
				return m_szTitle;
			}
		}
		/// <summary>
		/// Gets or sets Title font.
		/// </summary>
		internal Font HeaderFont
		{
			get
			{
				if( m_font == null )
				{
					m_font = new Font( base.Font, FontStyle.Bold );
				}

				return m_font;
			}
		}
		#endregion

		#region Constants
		/// <summary>
		/// Margin of ContextMenuStrip before and after title caption.
		/// </summary>
		internal const int CONTEXTMENUSTRIP_MARGIN = 6;
		#endregion

		#region Fields
		/// <summary>
		/// Title height.
		/// </summary>
		private int m_iTitleHeight = -1;
		/// <summary>
		/// Title size.
		/// </summary>
		private Size m_szTitle = Size.Empty;
		/// <summary>
		/// Font for title.
		/// </summary>
		private Font m_font;
		#endregion
	}
}

#endif

