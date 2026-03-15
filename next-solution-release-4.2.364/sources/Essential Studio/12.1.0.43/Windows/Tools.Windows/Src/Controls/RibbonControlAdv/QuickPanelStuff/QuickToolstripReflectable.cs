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
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Tools

{
	/// <summary>
	/// Represents dropdown button situated in quick items panel and reflecting existing toolstrip.
	/// </summary>
	[Designer("System.Windows.Forms.Design.ToolStripItemDesigner, System.Design")]
	[System.Windows.Forms.Design.ToolStripItemDesignerAvailabilityAttribute(System.Windows.Forms.Design.ToolStripItemDesignerAvailability.None)]
	public class QuickToolstripReflectable : CustomDropDownButtonBase, IQuickItem
	{
		#region Constructors
		/// <summary>
		/// Creates new instance of QuickToolStripDropDownButton.
		/// </summary>
		public QuickToolstripReflectable()
		{
			this.AutoSize = true;
		}
		/// <summary>
		/// Creates & initializes new instance of QuickToolStripDropDownButton.
		/// </summary>
		/// <param name="reflectedToolstrip">Toolstrip to reflect.</param>
		public QuickToolstripReflectable(ToolStripEx reflectedToolStrip) : this()
		{
			this.ReflectedToolStrip = reflectedToolStrip;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets toolstrip reflected by QuickToolStripDropDownButton.
		/// </summary>
		/// 
		/// </summary>
		internal override ToolStripEx ToolStrip
		{
			get
			{
				return m_reflectedToolStrip;
			}
		}
		/// <summary>
		/// Gets or sets reflected toolstrip.
		/// </summary>
		public ToolStripEx ReflectedToolStrip
		{
			get
			{
				return m_reflectedToolStrip;
			}
			set
			{
				if( m_reflectedToolStrip != null )
				{
					m_reflectedToolStrip.ImageChanged -= new EventHandler( OnReflectedToolStripImageChanged );
					this.Panel.ToolStrip.LauncherClick -= new EventHandler( OnPanelToolStripLauncherClick );

					this.Panel.Items.Clear();
				}

				m_reflectedToolStrip = value;

				if( m_reflectedToolStrip != null )
				{
					m_reflectedToolStrip.ImageChanged += new EventHandler(OnReflectedToolStripImageChanged);
					this.Panel.ToolStrip.LauncherClick += new EventHandler( OnPanelToolStripLauncherClick );
				}

				Reset();
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDropDownShow(EventArgs e)
		{
			base.OnDropDownShow(e);

			this.Panel.Items.Clear();

			if (m_reflectedToolStrip != null)
			{
				List<ToolStripItem> items = GetReflectableItems(m_reflectedToolStrip, true);
				this.Panel.Items.AddRange(items.ToArray());

				this.Panel.RightToLeft = m_reflectedToolStrip.RightToLeft;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose( bool disposing )
		{
			this.Panel.Items.Clear();
			
			base.Dispose( disposing );
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Updates current image.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnReflectedToolStripImageChanged( object sender, EventArgs e )
		{
			if (m_reflectedToolStrip != null)
			{
				this.Image = m_reflectedToolStrip.Image;
				Invalidate();
			}
		}
		/// <summary>
		/// Performs launcher click of reflected toolstrip.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnPanelToolStripLauncherClick( object sender, EventArgs e )
		{
			if (m_reflectedToolStrip != null)
			{
				m_reflectedToolStrip.PerformLauncherClick();
			}
		}
		#endregion

		#region Static Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="comp"></param>
		/// <returns></returns>
		public static ToolStripItem GetItemToReflect(Component comp)
		{
			return GetItemToReflect(comp, false);
		}
		/// <summary>
		/// Creates an item able to reflect given component.
		/// </summary>
		/// <param name="comp">Component that should be reflected.</param>
		/// <returns>ToolStripItem reflecting given component
		/// or null if it can't be created because of actual type of component.</returns>
		public static ToolStripItem GetItemToReflect(Component comp, bool bExactCopy)
		{
			ToolStripItem result = null;

			if( comp is ToolStripCheckBox )
			{
				result = new QuickCheckBoxReflectable( ( ToolStripCheckBox ) comp, bExactCopy );
			}
			else if( comp is ToolStripRadioButton )
			{
				result = new QuickRadioButtonReflectable( ( ToolStripRadioButton ) comp, bExactCopy );
			}
			else if( comp is ToolStripButton )
			{
				result = new QuickButtonReflectable((ToolStripButton)comp, bExactCopy);
			}
			else if( comp is ToolStripDropDownButton )
			{
				if (comp is CustomDropDownButtonBase)
				{
					result = new QuickToolstripReflectable(((CustomDropDownButtonBase)comp).ToolStrip);
				}
				else
				{
					result = new QuickDropDownButtonReflectable((ToolStripDropDownButton)comp, bExactCopy);
				}
			}
			else if( comp is ToolStripSplitButton )
			{
				result = new QuickSplitButtonReflectable((ToolStripSplitButton)comp, bExactCopy);
			}
			else if (comp is ToolStripSplitButtonEx)
			{
				result = new QuickSplitButtonEx((ToolStripSplitButtonEx)comp, bExactCopy);
			}
			else if (comp is ToolStripTextBox)
			{
				result = new QuickTextboxReflectable((ToolStripTextBox)comp, bExactCopy);
			}
			else if (comp is ToolStripComboBox)
			{
				result = new QuickComboboxReflectable((ToolStripComboBox)comp, bExactCopy);
			}
			else if (comp is ToolStripComboBoxEx)
			{
				result = new QuickComboBoxEx((ToolStripComboBoxEx)comp, bExactCopy);
			}
			else if (comp is ToolStripProgressBar)
			{
				result = new ProgressbarReflectable((ToolStripProgressBar)comp, bExactCopy);
			}
			else if (comp is ToolStripSeparator)
			{
				result = new ToolStripSeparator();
			}
			else if (comp is ToolStripLabel)
			{
				ToolStripLabel label = (ToolStripLabel)comp;
				ToolStripLabel newLabel = new ToolStripLabel(label.Text, label.Image, label.IsLink);
				newLabel.Font = label.Font;
				newLabel.ForeColor = label.ForeColor;
				result = newLabel;
			}
			else if (comp is ToolStripPanelItem)
			{
				ToolStripPanelItem srcPanel = comp as ToolStripPanelItem;

				ToolStripPanelItem panelItem = new ToolStripPanelItem();

				panelItem.Font = srcPanel.Font;
				panelItem.RowCount = srcPanel.RowCount;
				panelItem.GroupedButtons = srcPanel.GroupedButtons;

				panelItem.ToolStrip.Items.AddRange(GetReflectableItems(srcPanel.ToolStrip, true).ToArray());

				result = panelItem;
			}
			else if (comp is ToolStripGallery)
			{
				result = new QuickGallery((ToolStripGallery)comp);
			}
			else if (comp is ToolStripEx)
			{
				result = new QuickToolstripReflectable((ToolStripEx)comp);
			}

			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reflectedToolStrip"></param>
		/// <returns></returns>
		private static List<ToolStripItem> GetReflectableItems(ToolStripEx reflectedToolStrip)
		{
			return GetReflectableItems(reflectedToolStrip, false);
		}
		/// <summary>
		/// Gets list of items that reflect items of given toolstrip.
		/// </summary>
		/// <param name="reflectedToolStrip">Toolstrip to extract items from.</param>
		/// <returns>List of items that reflect items of given toolstrip.</returns>
		private static List<ToolStripItem> GetReflectableItems(ToolStripEx reflectedToolStrip, bool bExactCopy)
		{
			List<ToolStripItem> result = new List<ToolStripItem>();

			ToolStripItemCollection items = reflectedToolStrip.GetItems();

			foreach( ToolStripItem item in items )
			{
				ToolStripItem newItem = GetItemToReflect(item, bExactCopy);

				if( newItem != null )
				{
					result.Add( newItem );
				}
			}

			return result;
		}
		#endregion

		#region IReflectable Members
		/// <summary>
		/// 
		/// </summary>
		public void Reset()
		{
			if (m_reflectedToolStrip != null)
			{
				this.Image = m_reflectedToolStrip.Image;
			}
		}
		/// <summary>
		/// Checks whether current class instance reflects given component.
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public bool Reflects( IComponent c )
		{
			return ( c == m_reflectedToolStrip );
		}
		/// <summary>
		/// Gets component that is reflected by current QuickToolstripReflectable.
		/// </summary>
		public Component ReflectedComponent
		{
			get
			{
				return m_reflectedToolStrip;				 
			}
		}
		#endregion

		#region Fields
		/// <summary>
		/// Toolstrip reflected by QuickToolStripDropDownButton.
		/// </summary>
		private ToolStripEx m_reflectedToolStrip;
		#endregion
	}
}
#endif
