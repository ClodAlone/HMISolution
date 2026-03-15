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
using System.Text;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// 
	/// </summary>
	[System.Windows.Forms.Design.ToolStripItemDesignerAvailabilityAttribute(System.Windows.Forms.Design.ToolStripItemDesignerAvailability.None)]
	public abstract class CustomDropDownButtonBase
		: ToolStripDropDownButton
	{
		#region Fields
		/// <summary>
		/// Tool strip panel item.
		/// </summary>
		private ToolStripPanelItem m_panel;
		#endregion

		#region Properties
		/// <summary>
		/// Gets tool strip panel item.
		/// </summary>
		public ToolStripPanelItem Panel
		{
			get
			{
				if( m_panel == null )
				{
					m_panel = new ToolStripPanelItem(true);

					m_panel.AutoSize = false;
					m_panel.Transparent = false;

					this.DropDown.Items.Add(m_panel);
				}
				return m_panel;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override bool HasDropDownItems
		{
			get { return true; }
		}
		/// <summary>
		/// 
		/// </summary>
		internal abstract ToolStripEx ToolStrip { get;}
		#endregion

        #region Initialization
        /// <summary>
		/// Creates & initializes new instance of CustomDropDownButtonBase.
		/// </summary>
		public CustomDropDownButtonBase()
		{
			this.AutoSize = false;
			this.TextImageRelation = TextImageRelation.ImageAboveText;

			this.DropDown = new ToolStripDropDown();
			this.DropDown.Opening += new System.ComponentModel.CancelEventHandler(DropDown_Opening);

			this.DropDownDirection = ToolStripDropDownDirection.BelowRight;
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Reinits properties.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void DropDown_Opening( object sender, System.ComponentModel.CancelEventArgs e )
		{
			ToolStripEx ts = this.ToolStrip;

			if (ts != null)
			{
				this.DropDown.Renderer = ts.Renderer;

				this.Panel.AutoSize = false;
				
				this.Panel.RightToLeft = ts.RightToLeft;
				
				this.Panel.ToolStrip.LayoutStyle = ts.LayoutStyle;

				this.Panel.ToolStrip.Text = ts.Text;
				this.Panel.ToolStrip.Font = ts.Font;

				this.Panel.ToolStrip.ShowCaption = ts.ShowCaptionInternal;
				this.Panel.ToolStrip.CaptionFont = ts.CaptionFont;
				this.Panel.ToolStrip.CaptionStyle = ts.CaptionStyle;
				this.Panel.ToolStrip.CaptionTextStyle = ts.CaptionTextStyle;
				this.Panel.ToolStrip.CaptionAlignment = ts.CaptionAlignment;
				this.Panel.ToolStrip.CaptionTextStyle = ts.CaptionTextStyle;
				this.Panel.ToolStrip.LauncherStyle = ts.LauncherStyle;
				this.Panel.ToolStrip.ShowLauncher = ts.ShowLauncher;
				
				this.Panel.ToolStrip.GroupedButtons = ts.GroupedButtons;
				this.Panel.ToolStrip.ImageScalingSize = ts.ImageScalingSize;

                ToolStripDropDown dropDown = sender as ToolStripDropDown;

                if (dropDown != null && dropDown.OwnerItem is QuickToolstripReflectable)
                {
                    this.Panel.Size = this.Panel.GetPreferredSize(System.Drawing.Size.Empty);
                }
                else
                {
                    this.Panel.Size = this.Panel.GetPreferredSize(ts.ExpandedSize);
                }
			}
		}
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	class CollapsedDropDownButton
		: CustomDropDownButtonBase
	{
		#region Constants
		/// <summary>
		/// Horizontal padding.
		/// </summary>
		private const int DEF_HOR_PADDINGS = 2;
		/// <summary>
		/// Vertical padding.
		/// </summary>
		private const int DEF_VER_PADDINGS = 6;
		/// <summary>
		/// 
		/// </summary>
		const SetWindowPosFlags m_swpFlags =
			SetWindowPosFlags.SWP_NOZORDER |
			SetWindowPosFlags.SWP_NOSIZE |
			SetWindowPosFlags.SWP_NOMOVE |
			SetWindowPosFlags.SWP_NOACTIVATE |
			SetWindowPosFlags.SWP_FRAMECHANGED;
		#endregion

		#region Constructor/Destructor
		/// <summary>
		/// 
		/// </summary>
		internal CollapsedDropDownButton(ToolStripEx toolStrip)
		{
			this.Padding = new Padding( DEF_HOR_PADDINGS, DEF_VER_PADDINGS, DEF_HOR_PADDINGS, DEF_VER_PADDINGS );
			this.DropDownDirection = ToolStripDropDownDirection.BelowRight;

			m_toolStrip = toolStrip;
			if (m_toolStrip != null)
			{
				m_toolStrip.RendererChanged += new EventHandler(OnToolStripRendererChanged);
				OnToolStripRendererChanged(m_toolStrip, EventArgs.Empty);
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		internal override ToolStripEx ToolStrip
		{
			get { return this.Owner as ToolStripEx; }
		}
		#endregion

		#region Overrides

        protected override void Dispose(bool disposing)
        {
            if (m_toolStrip != null)
            {
                m_toolStrip.RendererChanged -= new EventHandler(OnToolStripRendererChanged);
                m_toolStrip = null;
            }
            base.Dispose(disposing);
        }

		/// <summary>
		/// Redraws non-client area when ToolStrip locates on the grouped RibbonPanel.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDropDownOpened( EventArgs e )
		{
			base.OnDropDownOpened( e );

			// Update frame only if RibbonControlAdv contain corresponding to RibbonPanel TabGroup.
			ToolStripEx tsEx = this.ToolStrip as ToolStripEx;

			if( tsEx != null )
			{
				RibbonPanel panel = tsEx.Parent as RibbonPanel;

				if( panel != null && panel.GetTabGroup() != null )
				{
					UpdateFrame();
				}
			}
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Redraws non-client area.
		/// </summary>
		private void UpdateFrame()
		{
			if( this.ToolStrip != null && this.ToolStrip.IsHandleCreated )
			{
				WindowsAPI.SetWindowPos( this.ToolStrip.Handle, IntPtr.Zero, 0, 0, 0, 0, m_swpFlags );
			}
		}
		#endregion

		#region Event handlers
		void OnToolStripRendererChanged(object sender, EventArgs e)
		{
			ToolStripEx ts = sender as ToolStripEx;

			if (ts != null)
			{
				if (ts.RenderMode == ToolStripRenderMode.ManagerRenderMode)
				{
					this.DropDown.Renderer = null;
				}
				else
				{
					this.DropDown.Renderer = ts.Renderer;
				}
			}
		}
		#endregion

		#region Fields
		ToolStripEx m_toolStrip;
		#endregion
	}
}
#endif
