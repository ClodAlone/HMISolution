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
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Represents quick button able to reflect functionality of referenced tool strip button.
	/// </summary>
	[System.Windows.Forms.Design.ToolStripItemDesignerAvailabilityAttribute(System.Windows.Forms.Design.ToolStripItemDesignerAvailability.None)]
	public class QuickButtonReflectable
		: ToolStripButton
		, IQuickItem
	{
		#region Constructors
		/// <summary>
		/// Creates new instance of QuickItemButton.
		/// </summary>
		public QuickButtonReflectable()
		{
			this.DisplayStyle = ToolStripItemDisplayStyle.Image;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reflectedButton"></param>
		public QuickButtonReflectable(ToolStripButton reflectedButton) : this(reflectedButton, false)
		{ }
		/// <summary>
		/// Creates & initializes new instance of QuickItemButton
		/// </summary>
		/// <param name="reflectedButton">Toolstrip button to reflect.</param>
		internal QuickButtonReflectable( ToolStripButton reflectedButton, bool bExactCopy ):this()
		{
			m_bExactCopy = bExactCopy;
			this.ReflectedButton = reflectedButton;
            m_reflectedButton.Paint += new PaintEventHandler(m_reflectedButton_Paint);
		}

        void m_reflectedButton_Paint(object sender, PaintEventArgs e)
        {
            if (m_reflectedButton.Image != this.Image)
            {
                this.Image = m_reflectedButton.Image;
            }
        }

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets button that is reflected by current QuickItemButton.
		/// </summary>
		public ToolStripButton ReflectedButton
		{
			get
			{
				return m_reflectedButton;
			}
			set
			{
				if (m_reflectedButton != null)
				{
					m_reflectedButton.EnabledChanged -= new EventHandler(OnReflectedButtonEnabledChanged);
					m_reflectedButton.CheckedChanged -= new EventHandler(OnReflectedButtonCheckedChanged);
				}

				m_reflectedButton = value;

				if (m_reflectedButton != null)
				{
					m_reflectedButton.EnabledChanged += new EventHandler(OnReflectedButtonEnabledChanged);
					m_reflectedButton.CheckedChanged += new EventHandler(OnReflectedButtonCheckedChanged);
				}

				Reset();
			}
		}
		///// <summary>
		///// 
		///// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Enabled
		{
			get
			{
				if (m_reflectedButton != null)
				{
					return m_reflectedButton.Enabled;
				}
				return false;
			}
			set
			{
				if (m_reflectedButton != null)
				{
					m_reflectedButton.Enabled = value;
				}
			}
		}
		///// <summary>
		///// 
		///// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override RightToLeft RightToLeft
		{
			get
			{
				if (m_reflectedButton != null)
				{
					return m_reflectedButton.RightToLeft;
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
				if (m_bExactCopy && m_reflectedButton != null && !this.AutoSize)
				{
					return m_reflectedButton.Size;
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
        static Dictionary<ToolStripButton, bool> m_hashQuickbuttons = new Dictionary<ToolStripButton, bool>();
        protected override void OnPaint(PaintEventArgs e)
        {
           
            if ((this.Parent is RibbonControlAdvHeader)&&(this.Parent as RibbonControlAdvHeader).BackStageView != null)
            {
                if ((this.Parent as RibbonControlAdvHeader).RibbonStyle == RibbonStyle.Office2010)
                {
                    if ((this.Parent as RibbonControlAdvHeader).BackStageView.BackStage.Visible)
                    {
                        this.ReflectedButton.Enabled = false;
                    }
                    else
                    {
                        if (!m_hashQuickbuttons.ContainsKey(this.ReflectedButton))
                            m_hashQuickbuttons.Add(this.ReflectedButton, this.ReflectedButton.Enabled);
                        else
                        {
                            bool buttonEnable = true;
                            m_hashQuickbuttons.TryGetValue(this.ReflectedButton, out buttonEnable);
                            this.ReflectedButton.Enabled = buttonEnable;
                        }
                    }
                }
            }

            base.OnPaint(e);
        }

       

		/// <summary>
		/// 
		/// </summary>
		/// <param name="constrainingSize"></param>
		/// <returns></returns>
		public override Size GetPreferredSize(Size constrainingSize)
		{
			if (m_bExactCopy && m_reflectedButton != null)
			{
				return m_reflectedButton.GetPreferredSize(constrainingSize);
			}
			return base.GetPreferredSize(constrainingSize);
		}
		/// <summary>
		/// Redirects mouse click to reflected button.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClick( EventArgs e )
		{
            m_reflectedButton.PerformClick();
		}
		#endregion

		#region Event handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnReflectedButtonEnabledChanged(object sender, EventArgs e)
		{
            if ((this.Parent is RibbonControlAdvHeader) && (this.Parent as RibbonControlAdvHeader).BackStageView != null)
            {
                if ((this.Parent as RibbonControlAdvHeader).RibbonStyle == RibbonStyle.Office2010)
                {
                    if (!(this.Parent as RibbonControlAdvHeader).BackStageView.BackStage.Visible)
                    {
                        if (!m_hashQuickbuttons.ContainsKey(this.ReflectedButton))
                            m_hashQuickbuttons.Add(this.ReflectedButton, this.ReflectedButton.Enabled);
                        else
                            m_hashQuickbuttons[this.ReflectedButton] = this.ReflectedButton.Enabled;
                    }
                }
            }
			Invalidate();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnReflectedButtonCheckedChanged(object sender, EventArgs e)
		{
			this.Checked = m_reflectedButton.Checked;
		}

        protected override void OnMouseHover(EventArgs e)
        {
            if (m_reflectedButton != null)
            {
                this.ToolTipText = m_reflectedButton.ToolTipText;
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
			if (m_reflectedButton != null)
			{
                this.AutoToolTip = m_reflectedButton.AutoToolTip;
				this.Image = m_reflectedButton.Image;
				this.Text = m_reflectedButton.Text;
                this.ToolTipText = m_reflectedButton.ToolTipText;
				this.Checked = m_reflectedButton.Checked;

				if (m_bExactCopy)
				{
					this.AutoSize = m_reflectedButton.AutoSize;
					this.DisplayStyle = m_reflectedButton.DisplayStyle;

					this.ImageAlign = m_reflectedButton.ImageAlign;
					this.ImageScaling = m_reflectedButton.ImageScaling;

					this.TextAlign = m_reflectedButton.TextAlign;
					this.TextImageRelation = m_reflectedButton.TextImageRelation;
				}
				else
				{
					this.DisplayStyle = this.Image != null ? ToolStripItemDisplayStyle.Image : ToolStripItemDisplayStyle.Text;
				}
			}
		}
		/// <summary>
		/// Checks whether current class instance reflects given component.
		/// </summary>
		/// <param name='�'></param>
		/// <returns></returns>
		public bool Reflects( IComponent c )
		{
			return ( c == m_reflectedButton );
		}
		/// <summary>
		/// Gets component that is reflected by current QuickItemButton.
		/// </summary>
		public Component ReflectedComponent
		{
			get
			{
				return m_reflectedButton;
			}
		}
		#endregion

		#region Fields
		/// <summary>
		/// Button that is reflected by current QuickItemButton.
		/// </summary>
		private ToolStripButton m_reflectedButton;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bExactCopy;
		#endregion
	}
}
#endif
