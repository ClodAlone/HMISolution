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
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Tools
{
	class QuickSplitButtonEx : ToolStripSplitButtonEx, IQuickItem
	{
		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		public QuickSplitButtonEx(ToolStripSplitButtonEx item) : this(item, false) 
		{ }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="comp"></param>
		/// <param name="bExactCopy"></param>
		public QuickSplitButtonEx(ToolStripSplitButtonEx item, bool bExactCopy) : base()
		{
			m_bExactCopy = bExactCopy;
			this.ReflectedItem = item;
            m_reflectedItem.Paint += new PaintEventHandler(m_reflectedItem_Paint);
		}

        void m_reflectedItem_Paint(object sender, PaintEventArgs e)
        {
            if (m_reflectedItem.Image != this.Image)
                this.Image = m_reflectedItem.Image;
        }
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ToolStripSplitButtonEx ReflectedItem
		{
			get
			{
				return m_reflectedItem;
			}
			set
			{
				if (m_reflectedItem != null)
				{
					m_reflectedItem.EnabledChanged -= new EventHandler(OnEnabledChanged);
				}

				m_reflectedItem = value;

				if (m_reflectedItem != null)
				{
					m_reflectedItem.EnabledChanged += new EventHandler(OnEnabledChanged);
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
				if (m_reflectedItem != null)
				{
					return m_reflectedItem.Enabled;
				}
				return false;
			}
			set
			{
				if (m_reflectedItem != null)
				{
					m_reflectedItem.Enabled = value;
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
				if (m_reflectedItem != null)
				{
					return m_reflectedItem.RightToLeft;
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
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Size Size
		{
			get
			{
				if (m_bExactCopy && m_reflectedItem != null && !this.AutoSize)
				{
					return m_reflectedItem.Size;
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
			if (m_bExactCopy && m_reflectedItem != null)
			{
				return m_reflectedItem.GetPreferredSize(constrainingSize);
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
				this.DropDown.OwnerItem = m_reflectedItem;
			}
			base.OnOwnerChanged(e);
		}
		/// <summary>
		/// Redirects mouse click to reflected button.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			m_reflectedItem.PerformClick();
		}
		#endregion

		#region IQuickItem Members
		/// <summary>
		/// 
		/// </summary>
		public void Reset()
		{
			if (m_reflectedItem != null)
			{
				this.DropDown = m_reflectedItem.DropDown;

				this.Image = m_reflectedItem.Image;
				this.Text = m_reflectedItem.Text;
                this.ToolTipText = m_reflectedItem.ToolTipText;
				if (m_bExactCopy)
				{
					this.AutoSize = m_reflectedItem.AutoSize;
					this.DisplayStyle = m_reflectedItem.DisplayStyle;

					this.ImageAlign = m_reflectedItem.ImageAlign;
					this.ImageScaling = m_reflectedItem.ImageScaling;

					this.TextAlign = m_reflectedItem.TextAlign;
					this.TextImageRelation = m_reflectedItem.TextImageRelation;
				}
				else
				{
					this.DisplayStyle = this.Image != null ? ToolStripItemDisplayStyle.Image : ToolStripItemDisplayStyle.Text;
				}

				Invalidate();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public bool Reflects(IComponent c)
		{
			return (c == m_reflectedItem);
		}
		/// <summary>
		/// 
		/// </summary>
		public Component ReflectedComponent
		{
			get { return m_reflectedItem; }
		}
		#endregion

		#region Event handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnEnabledChanged(object sender, EventArgs e)
		{
			Invalidate();
		}
		#endregion

		#region Fields
		/// <summary>
		/// 
		/// </summary>
		ToolStripSplitButtonEx m_reflectedItem;
		/// <summary>
		/// 
		/// </summary>
		bool m_bExactCopy = false;
		#endregion

	}
}
#endif
