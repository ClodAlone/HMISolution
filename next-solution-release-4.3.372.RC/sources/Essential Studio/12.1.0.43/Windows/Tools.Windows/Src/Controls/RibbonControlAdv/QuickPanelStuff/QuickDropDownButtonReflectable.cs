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
using System.Reflection;

using Syncfusion.Windows.Forms.Tools.Win32API;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Represents DropDownButton able to reflect functionality of referenced tool strip DropDownButton.
	/// </summary>
	[System.Windows.Forms.Design.ToolStripItemDesignerAvailabilityAttribute(
		System.Windows.Forms.Design.ToolStripItemDesignerAvailability.None)]
	public class QuickDropDownButtonReflectable
		: ToolStripDropDownButton
		, IQuickItem
	{
		#region Constructors
		/// <summary>
		/// Creates new instance of QuickDropDownButtonReflectable.
		/// </summary>
		public QuickDropDownButtonReflectable()
		{
			this.DisplayStyle = ToolStripItemDisplayStyle.Image;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reflectedItem"></param>
		public QuickDropDownButtonReflectable(ToolStripDropDownButton reflectedItem) : this(reflectedItem, false) 
		{ }
		/// <summary>
		/// Creates and initializes new instance of QuickDropDownButtonReflectable
		/// </summary>
		/// <param name="reflectedItem">ToolStripDropDownButton to reflect.</param>
		internal QuickDropDownButtonReflectable(ToolStripDropDownButton reflectedItem, bool bExactCopy):this()
		{
			m_bExactCopy = bExactCopy;
			this.ReflectedDropDownButton = reflectedItem;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets DropDownButton that is reflected by current QuickDropDownButtonReflectable.
		/// </summary>
		public ToolStripDropDownButton ReflectedDropDownButton
		{
			get
			{
				return m_reflectedDropDownButton;
			}
			set
			{
				if (m_reflectedDropDownButton != null)
				{
					m_reflectedDropDownButton.EnabledChanged -= new EventHandler(OnReflectedDropDownButtonEnabledChanged);
				}

				m_reflectedDropDownButton = value;

				if (m_reflectedDropDownButton != null)
				{
					m_reflectedDropDownButton.EnabledChanged += new EventHandler(OnReflectedDropDownButtonEnabledChanged);
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
				if (m_reflectedDropDownButton != null)
				{
					return m_reflectedDropDownButton.Enabled;
				}
				return false;
			}
			set
			{
				if (m_reflectedDropDownButton != null)
				{
					m_reflectedDropDownButton.Enabled = value;
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
				if (m_reflectedDropDownButton != null)
				{
					return m_reflectedDropDownButton.RightToLeft;
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
				if (m_bExactCopy && m_reflectedDropDownButton != null && !this.AutoSize)
				{
					return m_reflectedDropDownButton.Size;
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
			FieldInfo eventsField = typeof(Component).GetField("events", BindingFlags.NonPublic | BindingFlags.Instance);
			eventsField.SetValue(this, null);
			base.Dispose(disposing);
		}
		/// <summary>
		/// Detaches DropDown if quick item was removed
		/// </summary>
		/// <param name="e"></param>
		protected override void OnOwnerChanged(EventArgs e)
		{
			if (this.Owner == null && this.DropDown.OwnerItem==this)
			{
				this.DropDown.OwnerItem = m_reflectedDropDownButton;
			}
			base.OnOwnerChanged(e);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="constrainingSize"></param>
		/// <returns></returns>
		public override Size GetPreferredSize(Size constrainingSize)
		{
			if (m_bExactCopy && m_reflectedDropDownButton != null)
			{
				return m_reflectedDropDownButton.GetPreferredSize(constrainingSize);
			}
			return base.GetPreferredSize(constrainingSize);
		}
        protected override void OnClick(EventArgs e)
        {
            m_reflectedDropDownButton.PerformClick();
        }
		#endregion

		#region Event handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnReflectedDropDownButtonEnabledChanged(object sender, EventArgs e)
		{
			Invalidate();
		}


        protected override void OnMouseHover(EventArgs e)
        {
            if (m_reflectedDropDownButton != null)
            {
                this.ToolTipText = m_reflectedDropDownButton.ToolTipText;
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
			if (m_reflectedDropDownButton != null)
			{
				this.DropDown = m_reflectedDropDownButton.DropDown;
				this.Image = m_reflectedDropDownButton.Image;
				this.Text = m_reflectedDropDownButton.Text;
                this.ToolTipText = m_reflectedDropDownButton.ToolTipText;
				if (m_bExactCopy)
				{
					this.AutoSize = m_reflectedDropDownButton.AutoSize;
					this.DisplayStyle = m_reflectedDropDownButton.DisplayStyle;

					this.ShowDropDownArrow = m_reflectedDropDownButton.ShowDropDownArrow;

					this.ImageAlign = m_reflectedDropDownButton.ImageAlign;
					this.ImageScaling = m_reflectedDropDownButton.ImageScaling;

					this.TextAlign = m_reflectedDropDownButton.TextAlign;
					this.TextImageRelation = m_reflectedDropDownButton.TextImageRelation;
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
		public bool Reflects(IComponent c)
		{
			return (c == m_reflectedDropDownButton);
		}
		/// <summary>
		/// Gets component that is reflected by current QuickDropDownButtonReflectable.
		/// </summary>
		public Component ReflectedComponent
		{
			get
			{
				return m_reflectedDropDownButton;
			}
		}
		#endregion

		#region Fields
		/// <summary>
		/// DropDownButton that is reflected by current QuickDropDownButtonReflectable.
		/// </summary>
		private ToolStripDropDownButton m_reflectedDropDownButton;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bExactCopy;
		#endregion
	}
}
#endif
