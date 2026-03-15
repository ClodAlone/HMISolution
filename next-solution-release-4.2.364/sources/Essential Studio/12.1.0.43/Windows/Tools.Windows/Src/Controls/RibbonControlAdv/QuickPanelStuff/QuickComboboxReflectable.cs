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
	/// Represents ComboBox able to reflect functionality of referenced tool strip ComboBox.
	/// </summary>
	[System.Windows.Forms.Design.ToolStripItemDesignerAvailabilityAttribute(
		System.Windows.Forms.Design.ToolStripItemDesignerAvailability.None )]
	public class QuickComboboxReflectable
		: ToolStripComboBox
		, IQuickItem
	{
		#region Constructors
		/// <summary>
		/// Creates new instance of QuickComboboxReflectable.
		/// </summary>
		public QuickComboboxReflectable()
		{ }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reflectedItem"></param>
		public QuickComboboxReflectable(ToolStripComboBox reflectedItem) : this(reflectedItem, false)
		{ }
		/// <summary>
		/// Creates and initializes new instance of QuickComboboxReflectable.
		/// </summary>
		/// <param name="reflectedItem">ToolStripComboBox to reflect.</param>
		public QuickComboboxReflectable( ToolStripComboBox reflectedItem, bool bExactCopy )
		{
			m_bExactCopy = bExactCopy;
			this.ReflectedComboBox = reflectedItem;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets ComboBox that is reflected by current QuickComboboxReflectable.
		/// </summary>
		public ToolStripComboBox ReflectedComboBox
		{
			get
			{
				return m_reflectedCombobox;
			}
			set
			{
				Detach();

				m_reflectedCombobox = value;

				Attach();
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
				if (m_reflectedCombobox != null)
				{
					return m_reflectedCombobox.Enabled;
				}
				return false;
			}
			set
			{
				if (m_reflectedCombobox != null)
				{
					m_reflectedCombobox.Enabled = value;
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
				if (m_reflectedCombobox != null)
				{
					return m_reflectedCombobox.RightToLeft;
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
				if (m_bExactCopy && m_reflectedCombobox != null && !this.AutoSize)
				{
					return m_reflectedCombobox.Size;
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
		/// 
		/// </summary>
		/// <param name="constrainingSize"></param>
		/// <returns></returns>
		public override Size GetPreferredSize(Size constrainingSize)
		{
			if (m_bExactCopy && m_reflectedCombobox != null)
			{
				return m_reflectedCombobox.GetPreferredSize(constrainingSize);
			}
			return base.GetPreferredSize(constrainingSize);
		}
		/// <summary>
		/// Updates text of reflected combobox.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnTextChanged( EventArgs e )
		{

			if (!m_bUpdatingValue)
			{
				m_bUpdatingValue = true;
				
				m_reflectedCombobox.Text = this.Text;
				
				m_bUpdatingValue = false;
			}
            base.OnTextChanged(e);
        }
		/// <summary>
		/// Updates selected index of reflected combobox.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSelectedIndexChanged( EventArgs e )
		{
			base.OnSelectedIndexChanged( e );

			if (!m_bUpdatingValue)
			{
				m_bUpdatingValue = true;
				
				m_reflectedCombobox.SelectedIndex = this.SelectedIndex;
				
				m_bUpdatingValue = false;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose( bool disposing )
		{
			Detach();
			base.Dispose( disposing );
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Updates text.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnReflectedComboBoxTextChanged( object sender, EventArgs e )
		{
			if(!m_bUpdatingValue)
			{
				m_bUpdatingValue = true;
				
				this.Text = m_reflectedCombobox.Text;
				
				m_bUpdatingValue = false;
			}
		}
		/// <summary>
		/// Updates selected index.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnReflectedComboboxSelectedIndexChanged( object sender, EventArgs e )
		{
			if (!m_bUpdatingValue)
			{
				m_bUpdatingValue = true;

				this.SelectedIndex = m_reflectedCombobox.SelectedIndex;

				m_bUpdatingValue = false;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnReflectedComboboxEnabledChanged(object sender, EventArgs e)
		{
			Invalidate();
		}

        protected override void OnMouseHover(EventArgs e)
        {
            if (m_reflectedCombobox != null)
            {
                this.ToolTipText = m_reflectedCombobox.ToolTipText;
            }
            base.OnMouseHover(e);
        }
		#endregion

		#region IReflectable Implementation
		/// <summary>
		/// 
		/// </summary>
		public void Reset()
		{
			if (m_reflectedCombobox != null)
			{
				this.Text = m_reflectedCombobox.Text;
                this.ToolTipText = m_reflectedCombobox.ToolTipText;
				object[] items = new object[m_reflectedCombobox.Items.Count];
				m_reflectedCombobox.Items.CopyTo(items, 0);

				this.Items.Clear();
				this.Items.AddRange(items);

				this.DropDownStyle = m_reflectedCombobox.DropDownStyle;
				this.SelectedIndex = m_reflectedCombobox.SelectedIndex;

				if (m_bExactCopy)
				{
					this.AutoSize = m_reflectedCombobox.AutoSize;
				}
			}
		}
		/// <summary>
		/// Checks whether current class instance reflects given component.
		/// </summary>
		/// <param name="�"></param>
		/// <returns></returns>
		public bool Reflects( IComponent c )
		{
			return ( c == m_reflectedCombobox );
		}
		/// <summary>
		/// Gets component that is reflected by current QuickComboboxReflectable.
		/// </summary>
		public Component ReflectedComponent
		{
			get
			{
				return m_reflectedCombobox;
			}
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		void Detach()
		{
			if (m_reflectedCombobox != null)
			{
				m_reflectedCombobox.TextChanged -= new EventHandler(OnReflectedComboBoxTextChanged);
				m_reflectedCombobox.EnabledChanged -= new EventHandler(OnReflectedComboboxEnabledChanged);
				m_reflectedCombobox.SelectedIndexChanged -= new EventHandler(OnReflectedComboboxSelectedIndexChanged);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		void Attach()
		{
			if (m_reflectedCombobox != null)
			{
				m_reflectedCombobox.TextChanged += new EventHandler(OnReflectedComboBoxTextChanged);
				m_reflectedCombobox.EnabledChanged += new EventHandler(OnReflectedComboboxEnabledChanged);
				m_reflectedCombobox.SelectedIndexChanged += new EventHandler(OnReflectedComboboxSelectedIndexChanged);
			}
		}
		#endregion

		#region Fields
		/// <summary>
		/// ComboBox that is reflected by current QuickComboboxReflectable.
		/// </summary>
		private ToolStripComboBox m_reflectedCombobox;
		private bool m_bUpdatingValue = false;
		private bool m_bExactCopy = false;
		#endregion
	}
}
#endif