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

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Diagnostics;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms.Tools
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public class TabGroupItem : IChangeNotifyingItem
	{
		public event SyncfusionPropertyChangedEventHandler PropertyChanged;

		private string text;
		private int imageIndex;
		private object tag;

		public TabGroupItem()
		{
		}

		public TabGroupItem(string text, int imageIndex, object tag)
		{
			this.Text = text;
			this.ImageIndex = imageIndex;
			this.tag = tag;
		}

		public string Text
		{
			get{return text;}
			set
			{
				if(text != value)
				{
					text = value;
					this.OnPropertyChanged(PropertyChangeEffect.NeedLayout);
				}
			}
		}
		public int ImageIndex
		{
			get{return imageIndex;}
			set
			{
				if(imageIndex != value)
				{
					imageIndex = value;
					this.OnPropertyChanged(PropertyChangeEffect.NeedRepaint);
				}
			}
		}
		public object Tag
		{
			get{return this.tag;}
			set{this.tag = value;}
		}

		protected virtual void OnPropertyChanged(PropertyChangeEffect propertyChangeEffect)
		{
			if(PropertyChanged != null)
			{
				this.PropertyChanged(this, new SyncfusionPropertyChangedEventArgs(propertyChangeEffect,
						"", null, null));
			}
		}
	}


	[Syncfusion.Documentation.DocumentationExclude()]
	public class TabGroupItemsList : ArrayListExt
	{
		public new TabGroupItem this[int index]
		{
			get
			{
				return (TabGroupItem)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}


	[Syncfusion.Documentation.DocumentationExclude()]
	public class TabGroupData : TabData
	{
		private TabGroupItemsList items;
		private Point padding = new Point(3, 3);
//		private AHTabPanelData tabPanelData;

		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		]
		public TabGroupItemsList Items
		{
			get{return items;}
		}

		public Point Padding
		{
			get{return this.padding;}
			set
			{
				if(this.padding != value)
				{
					this.padding = value;
					this.OnPropertyChanged();
				}
			}
		}

//		public AHTabPanelData TabPanelData
//		{
//			get{return this.tabPanelData;}
//			set{this.tabPanelData = value;}
//		}

		// Text and imageIndex overriden to prevent OnBoundsChanged from getting called
		public override string Text
		{
			get{return base.Text;}
			set
			{
				this.SetTextWithoutBoundsAffected(value);
			}
		}
		public override int ImageIndex
		{
			get{ return base.ImageIndex;}
			set
			{
				base.SetImageIndexWithoutBoundsAffected(value);
			}
		}

		public event EventHandler SelectedIndexChanged;
		private int selectedIndex = -1;
		public int SelectedIndex
		{
			get
			{
				// Adjust the selected index to reflect current settings
				if(selectedIndex >= this.Items.Count)
					ChangeSelectedIndex(this.Items.Count - 1);

				if((selectedIndex < 0 || selectedIndex >= this.Items.Count)
					&& this.Items.Count > 0)
					ChangeSelectedIndex(0);

				return selectedIndex;
			}
			set
			{
				ChangeSelectedIndex(value);
			}
		}

//		public override bool IsSelected()
//		{
//			return true;
//		}

		protected void ChangeSelectedIndex(int index)
		{
			if(this.Items.Count <= index)
				throw new ArgumentOutOfRangeException();

			int previousIndex = selectedIndex;
			this.selectedIndex = index;
			this.OnPropertyChanged();

			if(this.SelectedIndexChanged != null)
			{
				this.SelectedIndexChanged(this, EventArgs.Empty);
			}
		}

		public TabGroupData()
			: base()
		{
			items = new TabGroupItemsList();
			items.CollectionChanged += new CollectionChangeEventHandler(this.Items_CollectionChanged);
			items.ItemPropertyChanged += new SyncfusionPropertyChangedEventHandler(this.Items_PropertyChanged);
		}

		private void Items_CollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			//			this.UpdatePreferredHeight();
			this.OnBoundsAffected();
		}

		private void Items_PropertyChanged(object sender, SyncfusionPropertyChangedEventArgs e)
		{
			if(e.PropertyChangeEffect == PropertyChangeEffect.NeedLayout)
				this.OnBoundsAffected();
			else if(e.PropertyChangeEffect == PropertyChangeEffect.NeedRepaint)
				this.OnPropertyChanged();
		}
	}

//
	[Syncfusion.Documentation.DocumentationExclude()]
	public class TabPanelPropertyTabGroup : TabUIDefaultProperties
	{
		public override Color DefaultTabPanelBackgroundColor(ITabPanelData panelData, ITabControl tabControl)
		{
			Color activeTabColor = panelData.ActiveTabColor;
			if(activeTabColor == Color.Empty)
				activeTabColor = this.DefaultActiveTabColor(panelData, tabControl);
			return ControlPaint.Light(activeTabColor);
		}
		public override SizeF GetOverlapSize(SizeF tabSize)
		{
			return new SizeF(0, 0);
		}
		public override Color DefaultInactiveTabColor(ITabPanelData panelData, ITabControl tabControl)
		{
			return this.DefaultActiveTabColor(panelData, tabControl);
		}
		public override bool ShowInDesignMode
		{
			get{return false;}
		}
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class TabGroupRenderer : TabRendererBase, ITabGroupRenderer
	{
		private ArrayList m_itemBounds;
		protected ArrayList itemBounds
		{
			get
			{
				return m_itemBounds;
			}
			set
			{
				if( m_itemBounds != value )
				{
					m_itemBounds = value;
				}
			}
		}
		private bool boundsChanged = false;

		public static string TabStyleName { get{return "DockingTabs";}
		}

		public static readonly int OVERLAPX = 2;

		protected override RectangleF CorrectBounds( RectangleF bounds )
		{
			bounds.Width -= 5;
			return base.CorrectBounds(bounds);
		}

		static TabPanelPropertyTabGroup tabPanelPropertyExtender;
		public static TabPanelPropertyTabGroup TabPanelPropertyExtender
		{
			get{return tabPanelPropertyExtender;}
		}
		static TabGroupRenderer()
		{
			tabPanelPropertyExtender = new TabPanelPropertyTabGroup();
			TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabGroupRenderer), TabPanelPropertyExtender);
		}

		public TabGroupRenderer(ITabControl parent, ITabPanelRenderer panelRenderer)
			: base(parent, panelRenderer)
		{
			itemBounds = new ArrayList();
		}

		public override ITabData TabData
		{
			get
			{
				return base.TabData;
			}
			set
			{
				base.TabData = value;
				((TabGroupData)this.TabData).Items.CollectionChanged +=
					new CollectionChangeEventHandler(this.ItemsCollection_Changed);

				((TabGroupData)this.TabData).SelectedIndexChanged +=
					new EventHandler(this.ItemBounds_Changed);
			}
		}

		private void ItemsCollection_Changed(object sender, CollectionChangeEventArgs args)
		{
			boundsChanged = true;
		}

		private void ItemBounds_Changed(object sender, EventArgs e)
		{
			// A better way is to maintain a NeedLayout flag and later compute the bounds
			// in a Layout method.
//			SetItemBounds(this.parent.GetGraphics());
			boundsChanged = true;

		}

		public RectangleF GetGroupItemBounds(int i)
		{
			if(i >= this.itemBounds.Count)
				return RectangleF.Empty;
            using (Graphics g = this.TabControl.GetGraphics())
            {
                return TabUtils.ApplyTransform(g,
                    this.TabAlignment, (RectangleF)itemBounds[i], false);
            }
		}

		public override RectangleF Bounds
		{
			get{return base.Bounds;}
			set
			{
				if(base.Bounds != value)
				{
					base.Bounds = value;
					this.boundsChanged = true;
				}
			}
		}

		protected virtual void SetItemBounds(Graphics g, RectangleF tabBounds)
		{
			bool needLayout = false;

			if( tabBounds.Width != this.Bounds.Width ||
				tabBounds.Height != this.Bounds.Height )
				needLayout = false;

			TabGroupData groupTabData = (TabGroupData)this.TabData;			

			if(boundsChanged || itemBounds.Count != groupTabData.Items.Count)
				boundsChanged = false;
			else
				return;

			if(groupTabData.Items.Count == 0)
				return;

			tabBounds = TabUtils.ApplyTransform(g, this.TabAlignment, tabBounds, true);

			itemBounds.Clear();

			TabControlAdv tcaParentBase = TabControl as TabControlAdv;
			Debug.Assert( null != tcaParentBase );

			bool bIsMirrored = tcaParentBase.IsMirrored;

			float fTabLeft = tabBounds.Left + 1;

			for(int i = 0; i < groupTabData.Items.Count; i++)
			{
				fTabLeft += groupTabData.Padding.X;

				TabGroupItem item = (TabGroupItem)groupTabData.Items[i];
				groupTabData.Text = "";

				if(groupTabData.SelectedIndex == i)
					groupTabData.Text = item.Text;

				groupTabData.ImageIndex = item.ImageIndex;

				if(i != groupTabData.SelectedIndex)
				{
					string text = item.Text;
					groupTabData.Text = text;
				}

				SizeF itemPrefSize = base.GetPreferredSize(g);

				itemPrefSize -= new SizeF(2, 2);
				itemPrefSize -= new SizeF(groupTabData.Padding.X * 2, 0);

				itemBounds.Add(new RectangleF(new PointF(fTabLeft, tabBounds.Top + 1),
					new SizeF(itemPrefSize.Width,tabBounds.Height)));

				fTabLeft += itemPrefSize.Width + 1 /*separator*/ + groupTabData.Padding.X;
			}

			// There is some empty space to the right, allocate it to the selected tab group item
			bool bNonEmptySideArea = false;
			bNonEmptySideArea = (fTabLeft < tabBounds.Right - 1);

			if (bNonEmptySideArea)
			{
				for(int i = 0; i < this.itemBounds.Count; i++)
				{
					RectangleF newBounds = (RectangleF)this.itemBounds[i];
					if(i == groupTabData.SelectedIndex)
					{
						float fWidthExt = 0.0F;
						fWidthExt = (tabBounds.Right - 1 - fTabLeft);

						newBounds.Width += fWidthExt;
					}
					else if(i > groupTabData.SelectedIndex)
					{
						float fOffset = (tabBounds.Right - 1 - fTabLeft);
						newBounds.Offset( fOffset, 0 );
					}

					itemBounds[i] = newBounds;
				}
			}
			
			if( needLayout )
				this.panelRenderer.Layout(g, false);
		}

		protected override void DrawBackground(DrawTabEventArgs drawiteminfo)
		{
			Graphics gph = drawiteminfo.Graphics;
			AHTabControl tabctrl = this.TabControl.GetControl() as AHTabControl;
			if(XPThemes.IsThemedOS && XPThemes.IsThemeActive && tabctrl.DockingManager.ThemesEnabled)
			{
				// Draw the interior using the themes API
				Rectangle rctab = drawiteminfo.Bounds;
				gph.SetClip(rctab);
				tabctrl.ThemeDraw.DrawThemeBackground(gph, ThemeParts.TABP_TABITEM, ThemeStates.TIS_NORMAL, Rectangle.Inflate(rctab,2,2));
				gph.ResetClip();
			}
			else
			{
                using (Brush brush = new SolidBrush(drawiteminfo.BackColor))
                    gph.FillRectangle(brush, drawiteminfo.Bounds);
			}
		}
		protected override void DrawBorders(DrawTabEventArgs drawiteminfo)
		{
			Graphics gph = drawiteminfo.Graphics;
			AHTabControl tabctrl = this.TabControl.GetControl() as AHTabControl;
            int width = drawiteminfo.Bounds.Width;
            if (tabctrl != null)
                width = (tabctrl.Edge == DockingStyle.Bottom || tabctrl.Edge == DockingStyle.Top) ? drawiteminfo.Bounds.Width + 5 : drawiteminfo.Bounds.Width;
            Rectangle calbounds = new Rectangle(drawiteminfo.Bounds.X, drawiteminfo.Bounds.Y, width, drawiteminfo.Bounds.Height);
			// Transformed to horizontal alignment
            RectangleF currentBounds = TabUtils.ApplyTransform(gph, this.TabAlignment, calbounds, true);
			// This could instead be imp. as a no-transform version with ControlPaint
			this.ApplyTransform(gph);

			Pen borderpen = null;
			if(XPThemes.IsThemedOS && XPThemes.IsThemeActive && tabctrl.DockingManager.ThemesEnabled)
				borderpen = new Pen(SystemColors.ControlDark);
			else
				borderpen = new Pen(drawiteminfo.ForeColor);

			// Always draw like the tab is selected
			gph.DrawLine(borderpen, new PointF(currentBounds.Left, currentBounds.Bottom-1), new PointF(currentBounds.Left, currentBounds.Top)); // Draw left line
			gph.DrawLine(borderpen, new PointF(currentBounds.Left, currentBounds.Top), new PointF(currentBounds.Right-1, currentBounds.Top)); // Top line
			gph.DrawLine(borderpen, new PointF(currentBounds.Right-1, currentBounds.Top), new PointF(currentBounds.Right-1, currentBounds.Bottom-1)); // Right line

			borderpen.Dispose();
			gph.ResetTransform();
		}
		public override SizeF GetPreferredSize(Graphics g)
		{
			SizeF preferredSize = SizeF.Empty;
			// Pass 1, find out the longest item
			int i = -1;
			TabGroupData tabGroupData = ((TabGroupData)this.TabData);
			foreach(TabGroupItem item in tabGroupData.Items)
			{
				i++;
				tabGroupData.Text = item.Text;
				tabGroupData.ImageIndex = item.ImageIndex;

				SizeF itemSize = base.GetPreferredSize(g);
				itemSize -= new Size(2,2);	// Size for the borders

				if(preferredSize.Width < itemSize.Width)
				{
					preferredSize.Width = itemSize.Width;
				}
				if(preferredSize.Height < itemSize.Height)
					preferredSize.Height = itemSize.Height;
			}

			// Pass 2: Add the size of the other items in the group without the text
			tabGroupData.Text = "";
			i = -1;
			foreach(TabGroupItem item in tabGroupData.Items)
			{
				i++;
                if (tabGroupData.SelectedIndex == i)
                {
                    //if (tabGroupData.Items.Count > 1&& item.ImageIndex != -1 && this.parent.Renderer.TabPanelData.ImageList != null && this.parent.Renderer.TabPanelData != null && this.parent.Renderer != null)
                    //    preferredSize.Width += (this.parent.Renderer.TabPanelData.ImageList.ImageSize.Width + 4) ;
                    continue;
                }

				tabGroupData.ImageIndex = item.ImageIndex;				
				string text = item.Text;
                tabGroupData.Text = text;				

				SizeF itemSize = base.GetPreferredSize(g);
				itemSize -= new Size(2,2);	// Size for the borders

				preferredSize.Width += itemSize.Width;
			}

			// For borders
			preferredSize += new Size(2,2);

			// For Separator between items
			preferredSize.Width += tabGroupData.Items.Count - 1;

			return preferredSize;
		}
		protected virtual void CorrectTabBounds( ref RectangleF bounds )
		{ }
		//
		protected override void DrawInterior(DrawTabEventArgs drawItemInfo)
		{
			Graphics g = drawItemInfo.Graphics;

			// Use Bounds here instead of BoundsInterior
			this.SetItemBounds(g, drawItemInfo.Bounds);

			RectangleF tabBounds = TabUtils.ApplyTransform(g, this.TabAlignment,
				drawItemInfo.Bounds, true);

			CorrectTabBounds( ref tabBounds);

			// Transform g to horizontal co-ords
			this.ApplyTransform(g);

			TabGroupData tabGroupData = ((TabGroupData)this.TabData);
			int i = -1;
			foreach(RectangleF bounds in itemBounds)
			{
				tabGroupData.Text = "";
				i++;
				// Convert to horizontal co-ords
//				RectangleF boundsTransformed = TabPanelRenderer.ApplyTransform(g, this.TabAlignment, bounds, true);
				RectangleF boundsTransformed = bounds;

				if(i == tabGroupData.SelectedIndex)
					tabGroupData.Text = ((TabGroupItem)tabGroupData.Items[i]).Text;

				tabGroupData.ImageIndex = ((TabGroupItem)tabGroupData.Items[i]).ImageIndex;

				// If no image, then show a portion of the text.
				if(i != tabGroupData.SelectedIndex)
				{
					string text = ((TabGroupItem)tabGroupData.Items[i]).Text;
                    tabGroupData.Text = text;
				}

				DrawTextAndImage(g, boundsTransformed, drawItemInfo);

				if(i != itemBounds.Count - 1)
					g.DrawLine(new Pen(Color.FromArgb(128, drawItemInfo.ForeColor)), new PointF(boundsTransformed.Right + this.panelRenderer.TabPanelData.Padding.X, tabBounds.Top),
						new PointF(boundsTransformed.Right + this.panelRenderer.TabPanelData.Padding.X, tabBounds.Bottom + 1));
			}

			g.ResetTransform();
		}

		protected string GetAutoHideCaption(string caption)
		{
			if( (parent != null) )
			{
				AHTabControl tabControl = parent as AHTabControl;
				if( tabControl != null ) 
				{
					DockingManager dockManager = tabControl.DockingManager;
					if( (dockManager != null) && (dockManager.FullCaptionsInAutoHideMode == true) )
					{
						return caption;
					}
				}
			}
			return caption.Substring(0, caption.Length < 3 ? caption.Length : 3);
		}
	}


	[Syncfusion.Documentation.DocumentationExclude()]
	public class AHTabRenderer : SingleLineTabPanelRenderer
	{
		public AHTabRenderer(ITabControl parent)
			: base(parent)
		{
		}
		public RectangleF GetGroupItemRect(int tabIndex, int groupIndex)
		{
			if(tabIndex >= this.tabRenderers.Count)
				return RectangleF.Empty;
			RectangleF rectangle = Rectangle.Empty;
			
			ITabGroupRenderer currentRenderer = this.tabRenderers[tabIndex] as ITabGroupRenderer;

			if( currentRenderer != null )
				rectangle = currentRenderer.GetGroupItemBounds( groupIndex );
			
			return rectangle;
		}
	}


	[
		ToolboxItem(false),
		DesignTimeVisible(false)
	]
	[Syncfusion.Documentation.DocumentationExclude()]
	public class AHTabControlBase : TabControlAdv
	{
		public AHTabControlBase()
			: base()
		{
		}

		protected override void Init()
		{
			this.TabPanelData = new AHTabPanelData(this);
			this.FocusOnTabClick = false;
			base.Init();
		}

		public TabDataCollection TabsData
		{
			get{return this.TabPanelData.TabsData;}
		}

		protected override void RendererChanged(TabPanelRenderer rendererNew)
		{
			if(this.TabPanelData.Multiline == false)
				base.RendererChanged(new AHTabRenderer(this));
			else
				base.RendererChanged(rendererNew);
		}

		public Rectangle GetGroupItemRect(int tabIndex, int groupIndex)
		{
			AHTabRenderer renderer = this.Renderer as AHTabRenderer;
			if(renderer == null)
				return Rectangle.Empty;

			return Rectangle.Round(renderer.GetGroupItemRect(tabIndex, groupIndex));
		}

	}


	[Syncfusion.Documentation.DocumentationExclude()]
	public class AHTabPanelData : TabPanelData
	{
		public AHTabPanelData(Control parent)
			:base(parent)
		{
		}

		public override ITabData CreateNewTabData()
		{
			TabGroupData tabGroupData = new TabGroupData();
			tabGroupData.Padding = new Point(3, 3);
			return tabGroupData;
		}
	}

}
