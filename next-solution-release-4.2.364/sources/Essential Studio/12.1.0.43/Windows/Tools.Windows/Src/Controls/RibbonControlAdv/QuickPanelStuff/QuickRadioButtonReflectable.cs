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
	/// Represents ToolStripRadioButton able to reflect functionality of referenced tool strip ToolStripRadioButton.
	/// </summary>
	[System.Windows.Forms.Design.ToolStripItemDesignerAvailabilityAttribute(
		System.Windows.Forms.Design.ToolStripItemDesignerAvailability.None )]
	public class QuickRadioButtonReflectable
		: ToolStripRadioButton
		, IQuickItem
	{
		#region Constructors
		/// <summary>
		/// Creates new instance of QuickRadioButtonReflectable.
		/// </summary>
		public QuickRadioButtonReflectable()
		{
			this.DisplayStyle = ToolStripItemDisplayStyle.Image;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reflectedItem"></param>
		public QuickRadioButtonReflectable( ToolStripRadioButton reflectedItem )
			: this( reflectedItem, false )
		{ }
		/// <summary>
		/// Creates and initializes new instance of QuickRadioButtonReflectable.
		/// </summary>
		/// <param name="reflectedItem">ToolStripRadioButton to reflect.</param>
		internal QuickRadioButtonReflectable( ToolStripRadioButton reflectedItem, bool bExactCopy )
			: this()
		{
			m_bExactCopy = bExactCopy;
			this.ReflectedRadioButton = reflectedItem;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets RadioButton that is reflected by current QuickRadioButtonReflectable.
		/// </summary>
		public ToolStripRadioButton ReflectedRadioButton
		{
			get
			{
				return m_reflectedRadioButton;
			}
			set
			{
				if( m_reflectedRadioButton != null )
				{
					m_reflectedRadioButton.EnabledChanged -= new EventHandler( OnReflectedRadioButtonEnabledChanged );
					m_reflectedRadioButton.CheckedChanged -= new EventHandler( OnReflectedRadioButtonCheckedChanged );
				}

				m_reflectedRadioButton = value;

				if( m_reflectedRadioButton != null )
				{
					m_reflectedRadioButton.EnabledChanged += new EventHandler( OnReflectedRadioButtonEnabledChanged );
					m_reflectedRadioButton.CheckedChanged += new EventHandler( OnReflectedRadioButtonCheckedChanged );
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
				if( m_reflectedRadioButton != null )
				{
					return m_reflectedRadioButton.Enabled;
				}
				return false;
			}
			set
			{
				if( m_reflectedRadioButton != null )
				{
					m_reflectedRadioButton.Enabled = value;
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
				if( m_reflectedRadioButton != null )
				{
					return m_reflectedRadioButton.RightToLeft;
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
				if( m_bExactCopy && m_reflectedRadioButton != null && !this.AutoSize )
				{
					return m_reflectedRadioButton.Size;
				}
				return base.Size;
			}
			set
			{
				base.Size = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override int GroupID
		{
			get	{	return m_reflectedRadioButton.GroupID; }
			set {	m_reflectedRadioButton.GroupID = value;	}
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
			if( m_bExactCopy && m_reflectedRadioButton != null )
			{
				return m_reflectedRadioButton.GetPreferredSize( constrainingSize );
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

			m_reflectedRadioButton.PerformClick();
		}
		/// <summary>
		/// 
		/// </summary>
		protected internal override void CheckCurrentButton() { }
		/// <summary>
		/// 
		/// </summary>
		protected internal override void CheckCurrentUncheckedButton() { }
		/// <summary>
		/// 
		/// </summary>
		protected internal override void CheckRemainingUncheckedButtons() { }
		#endregion

		#region Event handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnReflectedRadioButtonEnabledChanged( object sender, EventArgs e )
		{
			Invalidate();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnReflectedRadioButtonCheckedChanged( object sender, EventArgs e )
		{
			this.Checked = m_reflectedRadioButton.Checked;
		}

        protected override void OnMouseHover(EventArgs e)
        {
            if (m_reflectedRadioButton != null)
            {
                this.ToolTipText = m_reflectedRadioButton.ToolTipText;
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
			if( m_reflectedRadioButton != null )
			{
				this.Image = m_reflectedRadioButton.Image;
				this.GroupID = m_reflectedRadioButton.GroupID;
				this.Text = m_reflectedRadioButton.Text;
                this.ToolTipText = m_reflectedRadioButton.ToolTipText;
				this.Checked = m_reflectedRadioButton.Checked;

				if( m_bExactCopy )
				{
					this.AutoSize = m_reflectedRadioButton.AutoSize;
					this.TextAlign = m_reflectedRadioButton.TextAlign;
					this.CheckAlign = m_reflectedRadioButton.CheckAlign;
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
			return ( c == m_reflectedRadioButton );
		}
		/// <summary>
		/// Gets component that is reflected by current QuickRadioButtonReflectable.
		/// </summary>
		public Component ReflectedComponent
		{
			get
			{
				return m_reflectedRadioButton;
			}
		}
		#endregion

		#region Fields
		/// <summary>
		/// RadioButton that is reflected by current QuickRadioButtonReflectable.
		/// </summary>
		private ToolStripRadioButton m_reflectedRadioButton;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bExactCopy;
		#endregion
	}
}
#endif
