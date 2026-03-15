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

namespace Syncfusion.Windows.Forms.Tools
{
	class QuickGallery : ToolStripGallery, IQuickItem
	{
		#region Constructors
		public QuickGallery(ToolStripGallery gallery) : base(gallery.Items, true)
		{
			m_reflectedItem = gallery;

			this.AutoSize = false;

			Reset();
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		protected override void OnGalleryItemClicked(ToolStripGalleryItemEventArgs args)
		{
			base.OnGalleryItemClicked(args);

			if (m_reflectedItem != null)
			{
				m_reflectedItem.PerformClick(args.GalleryItem);
			}
		}
		#endregion

		#region IQuickItem Members
		public void Reset()
		{
			if (m_reflectedItem != null)
			{
				this.Size = m_reflectedItem.Size;
				this.ScrollerType = m_reflectedItem.ScrollerType;
				this.Dimensions = m_reflectedItem.Dimensions;
                this.ToolTipText = m_reflectedItem.ToolTipText;
				this.DropDownDimensions = m_reflectedItem.DropDownDimensions;
				this.DropDownMinimumSize = m_reflectedItem.DropDownMinimumSize;
				
				this.ItemSize = m_reflectedItem.ItemSize;
				this.ItemImageSize = m_reflectedItem.ItemImageSize;
				
				this.ItemMargin = m_reflectedItem.ItemMargin;
				this.ItemPadding = m_reflectedItem.ItemPadding;
				this.ItemDisplayStyle = m_reflectedItem.ItemDisplayStyle;
				this.ItemTextImageRelation = m_reflectedItem.ItemTextImageRelation;

				this.ShowCaption = m_reflectedItem.ShowCaption;
				this.CaptionText = m_reflectedItem.CaptionText;
				this.ImageList = m_reflectedItem.ImageList;
				this.CheckedItem = m_reflectedItem.CheckedItem;
				this.CheckOnClick = m_reflectedItem.CheckOnClick;


			}
		}
		public bool Reflects(IComponent c)
		{
			return m_reflectedItem!=null && m_reflectedItem == c;
		}
		public Component ReflectedComponent
		{
			get { return m_reflectedItem; }
		}
		#endregion

		#region Fields
		/// <summary>
		/// 
		/// </summary>
		ToolStripGallery m_reflectedItem;
		#endregion

	}
}
#endif
