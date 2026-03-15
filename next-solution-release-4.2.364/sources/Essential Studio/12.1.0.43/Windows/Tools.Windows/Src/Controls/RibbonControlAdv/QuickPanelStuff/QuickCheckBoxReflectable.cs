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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Represents ToolStripCheckBox able to reflect functionality of referenced tool strip ToolStripCheckBox.
	/// </summary>
	[System.Windows.Forms.Design.ToolStripItemDesignerAvailabilityAttribute(
		System.Windows.Forms.Design.ToolStripItemDesignerAvailability.None )]
	public class QuickCheckBoxReflectable
		: ToolStripCheckBox
		, IQuickItem
	{
		#region Constructors
		/// <summary>
		/// Creates new instance of QuickCheckBoxReflectable.
		/// </summary>
		public QuickCheckBoxReflectable()
		{
			this.DisplayStyle = ToolStripItemDisplayStyle.Image;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reflectedItem"></param>
		public QuickCheckBoxReflectable( ToolStripCheckBox reflectedItem )
			: this( reflectedItem, false )
		{ }
		/// <summary>
		/// Creates and initializes new instance of QuickCheckBoxReflectable.
		/// </summary>
		/// <param name="reflectedItem">ToolStripCheckBox to reflect.</param>
		internal QuickCheckBoxReflectable( ToolStripCheckBox reflectedItem, bool bExactCopy )
			: this()
		{
			m_bExactCopy = bExactCopy;
			this.ReflectedCheckBox = reflectedItem;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets CheckBox that is reflected by current QuickCheckBoxReflectable.
		/// </summary>
		public ToolStripCheckBox ReflectedCheckBox
		{
			get
			{
				return m_reflectedCheckBox;
			}
			set
			{
				if( m_reflectedCheckBox != null )
				{
					m_reflectedCheckBox.EnabledChanged -= new EventHandler( OnReflectedCheckBoxEnabledChanged );
					m_reflectedCheckBox.CheckedChanged -= new EventHandler( OnReflectedCheckBoxCheckedChanged );
					m_reflectedCheckBox.CheckStateChanged -= new EventHandler( OnReflectedCheckBoxCheckStateChanged );
				}

				m_reflectedCheckBox = value;

				if( m_reflectedCheckBox != null )
				{
					m_reflectedCheckBox.EnabledChanged += new EventHandler( OnReflectedCheckBoxEnabledChanged );
					m_reflectedCheckBox.CheckedChanged += new EventHandler( OnReflectedCheckBoxCheckedChanged );
					m_reflectedCheckBox.CheckStateChanged += new EventHandler( OnReflectedCheckBoxCheckStateChanged );
				}

				Reset();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public override bool Enabled
		{
			get
			{
				if( m_reflectedCheckBox != null )
				{
					return m_reflectedCheckBox.Enabled;
				}
				return false;
			}
			set
			{
				if( m_reflectedCheckBox != null )
				{
					m_reflectedCheckBox.Enabled = value;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public override RightToLeft RightToLeft
		{
			get
			{
				if( m_reflectedCheckBox != null )
				{
					return m_reflectedCheckBox.RightToLeft;
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
				if( m_bExactCopy && m_reflectedCheckBox != null && !this.AutoSize )
				{
					return m_reflectedCheckBox.Size;
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
		public override Size GetPreferredSize( Size constrainingSize )
		{
			if( m_bExactCopy && m_reflectedCheckBox != null )
			{
				return m_reflectedCheckBox.GetPreferredSize( constrainingSize );
			}
			return base.GetPreferredSize( constrainingSize );
		}
		/// <summary>
		/// Redirects mouse click to reflected button.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClick( EventArgs e )
		{
			base.OnClick( e );

			m_reflectedCheckBox.PerformClick();
		}
		#endregion

		#region Event handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnReflectedCheckBoxEnabledChanged( object sender, EventArgs e )
		{
			Invalidate();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnReflectedCheckBoxCheckedChanged( object sender, EventArgs e )
		{
			this.Checked = m_reflectedCheckBox.Checked;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnReflectedCheckBoxCheckStateChanged( object sender, EventArgs e )
		{
			this.CheckState = m_reflectedCheckBox.CheckState;
		}

        protected override void OnMouseHover(EventArgs e)
        {
            if (m_reflectedCheckBox != null)
            {
                this.ToolTipText = m_reflectedCheckBox.ToolTipText;
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
			if( m_reflectedCheckBox != null )
			{
				this.Image = m_reflectedCheckBox.Image;
				this.Text = m_reflectedCheckBox.Text;
				this.ThreeState = m_reflectedCheckBox.ThreeState;
				this.CheckState = m_reflectedCheckBox.CheckState;
				this.Checked = m_reflectedCheckBox.Checked;
                this.ToolTipText = m_reflectedCheckBox.ToolTipText;
				if( m_bExactCopy )
				{
					this.AutoSize = m_reflectedCheckBox.AutoSize;
					this.TextAlign = m_reflectedCheckBox.TextAlign;
					this.CheckAlign = m_reflectedCheckBox.CheckAlign;
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
			return ( c == m_reflectedCheckBox );
		}
		/// <summary>
		/// Gets component that is reflected by current QuickCheckBoxReflectable.
		/// </summary>
		public Component ReflectedComponent
		{
			get
			{
				return m_reflectedCheckBox;
			}
		}
		#endregion

		#region Fields
		/// <summary>
		/// CheckBox that is reflected by current QuickCheckBoxReflectable.
		/// </summary>
		private ToolStripCheckBox m_reflectedCheckBox;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bExactCopy;
		#endregion
	}
}
#endif
