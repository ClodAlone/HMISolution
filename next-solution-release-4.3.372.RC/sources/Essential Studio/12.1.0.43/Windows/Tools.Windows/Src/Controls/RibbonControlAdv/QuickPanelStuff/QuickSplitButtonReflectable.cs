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
using System.Drawing;

using Syncfusion.Windows.Forms.Tools.Win32API;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Represents SplitButton able to reflect functionality of referenced tool strip SplitButton.
	/// </summary>
	[System.Windows.Forms.Design.ToolStripItemDesignerAvailabilityAttribute(
		System.Windows.Forms.Design.ToolStripItemDesignerAvailability.None )]
	public class QuickSplitButtonReflectable
		: ToolStripSplitButton
		, IQuickItem
	{
		#region Constructors
		/// <summary>
		/// Creates new instance of QuickSplitButtonReflectable.
		/// </summary>
		public QuickSplitButtonReflectable()
		{
			this.DisplayStyle = ToolStripItemDisplayStyle.Image;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reflectedItem"></param>
		public QuickSplitButtonReflectable(ToolStripSplitButton reflectedItem):this(reflectedItem,false)
		{ }
		/// <summary>
		/// Creates and initializes new instance of QuickSplitButtonReflectable.
		/// </summary>
		/// <param name="reflectedItem">ToolStripSplitButton to reflect.</param>
		public QuickSplitButtonReflectable(ToolStripSplitButton reflectedItem, bool bExactCopy):this()
		{
			m_bExactCopy = bExactCopy;
			this.ReflectedSplitButton = reflectedItem;
            m_reflectedSplitButton.Paint += new PaintEventHandler(m_reflectedSplitButton_Paint);
		}

        void m_reflectedSplitButton_Paint(object sender, PaintEventArgs e)
        {
            if (this.Image != m_reflectedSplitButton.Image)
                this.Image = m_reflectedSplitButton.Image;
        }
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets SplitButton that is reflected by current QuickSplitButtonReflectable.
		/// </summary>
		public ToolStripSplitButton ReflectedSplitButton
		{
			get
			{
				return m_reflectedSplitButton;
			}
			set
			{
				if (m_reflectedSplitButton != null)
				{
					m_reflectedSplitButton.EnabledChanged -= new EventHandler(OnReflectedSplitButtonEnabledChanged);
				}

				m_reflectedSplitButton = value;

				if (m_reflectedSplitButton != null)
				{
					m_reflectedSplitButton.EnabledChanged += new EventHandler(OnReflectedSplitButtonEnabledChanged);
				}

				Reset();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Enabled
		{
			get
			{
				if (m_reflectedSplitButton != null)
				{
					return m_reflectedSplitButton.Enabled;
				}
				return false;
			}
			set
			{
				if (m_reflectedSplitButton != null)
				{
					m_reflectedSplitButton.Enabled = value;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override RightToLeft RightToLeft
		{
			get
			{
				if (m_reflectedSplitButton != null)
				{
					return m_reflectedSplitButton.RightToLeft;
				}
				return base.RightToLeft;
			}
			set
			{
				base.RightToLeft = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override Size Size
		{
			get
			{
				if (m_bExactCopy && m_reflectedSplitButton != null && !this.AutoSize)
				{
					return m_reflectedSplitButton.Size;
				}
				return base.Size;
			}
			set
			{
				base.Size = value;
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Prevents disposing of Reflected button's dropdown
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			this.DropDown = null;
			base.Dispose(disposing);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="constrainingSize"></param>
		/// <returns></returns>
		public override Size GetPreferredSize(Size constrainingSize)
		{
			if (m_bExactCopy && m_reflectedSplitButton != null)
			{
				return m_reflectedSplitButton.GetPreferredSize(constrainingSize);
			}
			return base.GetPreferredSize(constrainingSize);
		}
		/// <summary>
		/// Detaches reflected button's dropdown if item was removed
		/// </summary>
		/// <param name="e"></param>
		protected override void OnOwnerChanged(EventArgs e)
		{
			if (this.Owner == null && this.DropDown.OwnerItem == this)
			{
				this.DropDown.OwnerItem = m_reflectedSplitButton;
			}
			base.OnOwnerChanged(e);
		}
		/// <summary>
		/// Redirects mouse click to reflected button.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnButtonClick(EventArgs e)
		{
            m_reflectedSplitButton.PerformButtonClick();
		}

        protected override void OnClick(EventArgs e)
        {
            if ((this is ToolStripSplitButton) && (this.DropDownButtonPressed))
                m_reflectedSplitButton.PerformClick();
        }
		#endregion

		#region Event handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnReflectedSplitButtonEnabledChanged(object sender, EventArgs e)
		{
			Invalidate();
		}

        protected override void OnMouseHover(EventArgs e)
        {
            if (m_reflectedSplitButton != null)
            {
                this.ToolTipText = m_reflectedSplitButton.ToolTipText;
            }
            base.OnMouseHover(e);
        }
		#endregion

		#region IReflectable Implementation
		/// <summary>
		/// Update properties from reflected button
		/// </summary>
		public void Reset()
		{
			if (m_reflectedSplitButton != null)
			{
				this.DropDown = m_reflectedSplitButton.DropDown;

				this.Image = m_reflectedSplitButton.Image;
				this.Text = m_reflectedSplitButton.Text;
                this.ToolTipText = m_reflectedSplitButton.ToolTipText;
				if (m_bExactCopy)
				{
					this.AutoSize = m_reflectedSplitButton.AutoSize;
					this.DisplayStyle = m_reflectedSplitButton.DisplayStyle;

					this.ImageAlign = m_reflectedSplitButton.ImageAlign;
					this.ImageScaling = m_reflectedSplitButton.ImageScaling;

					this.TextAlign = m_reflectedSplitButton.TextAlign;
					this.TextImageRelation = m_reflectedSplitButton.TextImageRelation;
				}
				else
				{
					this.DisplayStyle = this.Image != null ? ToolStripItemDisplayStyle.Image : ToolStripItemDisplayStyle.Text;
				}

				Invalidate();
			}
		}
		/// <summary>
		/// Checks whether current class instance reflects given component.
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public bool Reflects( IComponent c )
		{
			return ( c == m_reflectedSplitButton );
		}
		/// <summary>
		/// Gets component that is reflected by current QuickSplitButtonReflectable.
		/// </summary>
		public Component ReflectedComponent
		{
			get
			{
				return m_reflectedSplitButton;
			}
		}
		#endregion

		#region Fields
		/// <summary>
		/// SplitButton that is reflected by current QuickSplitButtonReflectable.
		/// </summary>
		private ToolStripSplitButton m_reflectedSplitButton;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bExactCopy;
		#endregion
	}
}
#endif
