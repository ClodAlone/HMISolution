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
	/// Represents Textbox able to reflect functionality of referenced tool strip Textbox.
	/// </summary>
	[System.Windows.Forms.Design.ToolStripItemDesignerAvailabilityAttribute(System.Windows.Forms.Design.ToolStripItemDesignerAvailability.None )]
	public class QuickTextboxReflectable
		: ToolStripTextBox
		, IQuickItem
	{
		#region Constructors
		/// <summary>
		/// Creates new instance of QuickTextboxReflectable.
		/// </summary>
		public QuickTextboxReflectable()
		{ }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reflectedItem"></param>
		public QuickTextboxReflectable(ToolStripTextBox reflectedItem) : this(reflectedItem, false) 
		{ }
		/// <summary>
		/// Creates & initializes new instance of QuickTextboxReflectable.
		/// </summary>
		/// <param name="reflectedButton">ToolStripComboBox to reflect.</param>
		public QuickTextboxReflectable(ToolStripTextBox reflectedItem, bool bExactCopy)
		{
			m_bExactCopy = bExactCopy;
			this.ReflectedTextBox = reflectedItem;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets Textbox that is reflected by current QuickTextboxReflectable.
		/// </summary>
		public ToolStripTextBox ReflectedTextBox
		{
			get
			{
				return m_reflectedTextBox;
			}
			set
			{
				if( m_reflectedTextBox != null )
				{
					m_reflectedTextBox.TextChanged -= new EventHandler(OnReflectedTextBoxTextChanged);
					m_reflectedTextBox.EnabledChanged -= new EventHandler(OnReflectedTextBoxEnabledChanged);
				}
				
				m_reflectedTextBox = value;
				
				if (m_reflectedTextBox != null)
				{
					if (m_bExactCopy)
					{
						this.AutoSize = m_reflectedTextBox.AutoSize;
					}
					m_reflectedTextBox.TextChanged += new EventHandler(OnReflectedTextBoxTextChanged);
					m_reflectedTextBox.EnabledChanged -= new EventHandler(OnReflectedTextBoxEnabledChanged);

					OnReflectedTextBoxTextChanged(m_reflectedTextBox, EventArgs.Empty);
				}
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
				if (m_reflectedTextBox != null)
				{
					return m_reflectedTextBox.Enabled;
				}
				return false;
			}
			set
			{
				if (m_reflectedTextBox != null)
				{
					m_reflectedTextBox.Enabled = value;
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
				if (m_reflectedTextBox != null)
				{
					return m_reflectedTextBox.RightToLeft;
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
				if (m_bExactCopy && m_reflectedTextBox != null && !this.AutoSize)
				{
					return m_reflectedTextBox.Size;
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
			if (m_bExactCopy && m_reflectedTextBox != null)
			{
				return m_reflectedTextBox.GetPreferredSize(constrainingSize);
			}
			return base.GetPreferredSize(constrainingSize);
		}
		/// <summary>
		/// Updates text of reflected text box.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnTextChanged( EventArgs e )
		{
			base.OnTextChanged( e );

			if (!m_bUpdatingValue)
			{
				m_bUpdatingValue = true;
				
				m_reflectedTextBox.Text = this.Text;
				
				m_bUpdatingValue = false;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose( bool disposing )
		{
			if( m_reflectedTextBox != null )
			{
				m_reflectedTextBox.TextChanged -= new EventHandler( OnReflectedTextBoxTextChanged );
			}

			base.Dispose( disposing );
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Updates current text.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnReflectedTextBoxTextChanged( object sender, EventArgs e )
		{
			if (!m_bUpdatingValue)
			{
				m_bUpdatingValue = true;
				
				this.Text = m_reflectedTextBox.Text;
				
				m_bUpdatingValue = false;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnReflectedTextBoxEnabledChanged(object sender, EventArgs e)
		{
			Invalidate();
		}

        protected override void OnMouseHover(EventArgs e)
        {
            if(m_reflectedTextBox!=null)
            {
                this.ToolTipText = m_reflectedTextBox.ToolTipText;
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
			Invalidate();
		}
		/// <summary>
		/// Checks whether current class instance reflects given component.
		/// </summary>
		/// <param name="�"></param>
		/// <returns></returns>
		public bool Reflects( IComponent c )
		{
			return ( c == m_reflectedTextBox );
		}
		/// <summary>
		/// Gets component that is reflected by current QuickTextboxReflectable.
		/// </summary>
		public Component ReflectedComponent
		{
			get
			{
				return m_reflectedTextBox;
			}
		}
		#endregion

		#region Fields
		/// <summary>
		/// Textbox that is reflected by current QuickTextboxReflectable.
		/// </summary>
		private ToolStripTextBox m_reflectedTextBox;
		private bool m_bUpdatingValue = false;
		private bool m_bExactCopy = false;
		#endregion
	}
}
#endif