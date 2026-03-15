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

#region Class using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
	/// <summary>
	/// TabGroupRendererVS2005 paints tabs in group mode Visual studio 2005 Beta style.
	/// </summary>
	public class TabGroupRendererVS2005Beta
		: OneNoteStyleRenderer
		, ITabGroupRenderer
	{
		#region Class members
		/// <summary>
		/// Properties for panel painting for current style.
		/// </summary>
		protected static TabUIVS2005Properties m_tabPropertyExtender;
        protected static TabUIVS2010Properties m_tabProperty2010Extender;
        protected static TabUIVS2012Properties m_tabProperty2012Extender;
		/// <summary>
		/// Stores bounds of tab items.
		/// </summary>
		protected ArrayList itemBounds;
		/// <summary>
		/// Defines if bounds changed.
		/// </summary>
		protected bool boundsChanged = false;
		/// <summary>
		/// Stores tab group bounds.
		/// </summary>
		protected ArrayList m_tabGroupBounds;
		/// <summary>
		/// Collection to store hit rectangles of items.
		/// </summary>
		protected ArrayList m_hitBounds;
		/// <summary>
		/// Offset for hit region.
		/// </summary>
		private float m_hitOffset = 0;
		/// <summary>
		/// Inner shadowed border path.
		/// </summary>
		protected GraphicsPath m_innerPath;
		#endregion

		#region Class constants
		/// <summary>
		/// Offset for text in curved borders.
		/// </summary>
		private int DEF_CURVE_OFFSET = 13;
		/// <summary>
		/// Offset between tab items.
		/// </summary>
		private const int DEF_ITEM_OFFSET = -2;
		/// <summary>
		/// Width correction.
		/// </summary>
		private const int DEF_WIDTH_CORRECT = 10;
		#endregion

		#region Class properties
		/// <summary>
		/// Gets tab style name.
		/// </summary>
		new public static string TabStyleName 
		{
			get
			{
				return "DockingTabsVS2005Beta";
			}
		}
		/// <summary>
		/// Returns the <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties"/>
		/// instance that provides default properties for this renderer.
		/// </summary>
		new public static TabUIVS2005Properties TabPanelPropertyExtender
		{
			get
			{
				return m_tabPropertyExtender;
			}
		}
        /// <summary>
        /// Returns the <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties"/>
        /// instance that provides default properties for this renderer.
        /// </summary>
        public static TabUIVS2010Properties TabPanel2010PropertyExtender
        {
            get
            {
                return m_tabProperty2010Extender;
            }
        }
        /// <summary>
        /// Returns the <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties"/>
        /// instance that provides default properties for this renderer.
        /// </summary>
        public static TabUIVS2012Properties TabPanel2012PropertyExtender
        {
            get
            {
                return m_tabProperty2012Extender;
            }
        }
		/// <summary>
		/// Border Color.
		/// </summary>
		protected override Color BorderColor
		{
			get
			{
				return VS2005Colors.BorderColor;
			}
		}
		/// <summary>
		/// Gets/sets bounds.
		/// </summary>
		public override RectangleF Bounds
		{
			get
			{
				return base.Bounds;
			}
			set
			{
				if(base.Bounds != value)
				{
					base.Bounds = value;
					this.boundsChanged = true;
				}
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Registers class types.
		/// </summary>
		static TabGroupRendererVS2005Beta()
		{
			m_tabPropertyExtender = new TabUIVS2005Properties();
			TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabGroupRendererVS2005Beta), TabPanelPropertyExtender);
		}
		/// <summary>
		/// Creates an instance of the <see cref="TabGroupRendererVS2005Beta"/>.
		/// </summary>
		/// <param name="parent">The <see cref="Syncfusion.Windows.Forms.Tools.ITabControl"/> instance.</param>
		/// <param name="panelRenderer">The parent <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> instance.</param>
		public TabGroupRendererVS2005Beta(ITabControl parent, ITabPanelRenderer panelRenderer)
			: base(parent, panelRenderer)
		{
			itemBounds = new ArrayList();
			m_tabGroupBounds = new ArrayList();
			m_hitBounds = new ArrayList();
			VS2005Colors.UpdateStyleColors();
		}

		#endregion

		#region Class public methods
		/// <summary>
		/// Gets group item bounds.
		/// </summary>
		/// <param name="i">Number of item.</param>
		/// <returns>Bounds.</returns>
		public RectangleF GetGroupItemBounds(int i)
		{
			if(i >= this.itemBounds.Count)
				return RectangleF.Empty;
			int number = m_hitBounds.Count - 1 - i;

			return TabUtils.ApplyTransform(this.TabControl.GetGraphics(),
				this.TabAlignment, (RectangleF)m_hitBounds[number], false);
		}
		#endregion

		#region Class overrides
		/// <summary>
		/// Gets outer and inner border path from bounds in VS2005 style.
		/// </summary>
		/// <param name="bounds">Bounds in which to draw border.</param>
		/// <returns>Border path.</returns>
		protected override GraphicsPath GetBorderPathFromBounds( RectangleF bounds )
		{
			GraphicsPath path = new GraphicsPath();
			float height = bounds.Height;
			float slopeSpan = (height * 0.7f);
			float topSlopeSpanY = (height / 4f);
			float topSlopeSpanX = (height / 7f);

			slopeSpan -= (4 + topSlopeSpanY);

			PointF[] aptCurve = 
				new PointF[]
				{
					new PointF(bounds.Left, bounds.Bottom-1),
					new PointF(bounds.Left + (4 + slopeSpan), bounds.Top + topSlopeSpanX),
					new PointF(bounds.Left + (7 + slopeSpan + topSlopeSpanY), bounds.Top -1)
				};
			
			path.AddCurve( aptCurve, 0.5f );

			PointF[] aptLines = 
				new PointF[]
				{
					new PointF(bounds.Right - (7 + slopeSpan + topSlopeSpanY), bounds.Top -1),
					new PointF(bounds.Right - (4 + slopeSpan), bounds.Top + topSlopeSpanX),
					new PointF(bounds.Right, bounds.Bottom-1)
				};
			path.AddLine( aptCurve[2], aptLines[0] );
			path.AddCurve( aptLines, 0.5f );
			path.AddLine( aptLines[2], aptCurve[0] );

			m_innerPath = new GraphicsPath();
			m_innerPath.AddLine( aptCurve[2].X-1, aptCurve[2].Y+1, aptLines[0].X, aptLines[0].Y+1 );
			aptLines[0].Y++;
			aptLines[1].Y++;
			aptLines[2].X--;
			aptLines[2].Y++;
			m_innerPath.AddCurve( aptLines, 0.5f );

			return path;
		}
		/// <summary>
		/// Override.
		/// </summary>
		/// <param name="drawItemInfo">See the <see cref="Syncfusion.Windows.Forms.Tools.DrawTabEventArgs"/></param>
		protected override void DrawBackground(DrawTabEventArgs drawItemInfo)
		{
			return;
		}
		/// <summary>
		/// Override.
		/// </summary>
		/// <param name="drawItemInfo">See the <see cref="Syncfusion.Windows.Forms.Tools.DrawTabEventArgs"/></param>
		protected override void DrawBorders(DrawTabEventArgs drawItemInfo)
		{
			return;
		}
		/// <summary>
		/// Performs hit test.
		/// </summary>
		/// <param name="mousePosition">Mouse position.</param>
		/// <returns>If any tab item is hit.</returns>
		public override bool HitTest(PointF mousePosition)
		{
			RectangleF bounds = this.GetCurrentBounds();
			
			if( this.NeedRotateTextWhenVertical )
			{
				if( this.TabControl != null )
				{
					Control control = this.TabControl.GetControl();
					if( control != null )
					{					
						using( Graphics g = control.CreateGraphics() )
						{
							RectangleF rectMousePos = new RectangleF( mousePosition, 
								SizeF.Empty );

							rectMousePos = TabUtils.ApplyTransform( g, this.TabAlignment,
								rectMousePos, false );

							mousePosition = rectMousePos.Location;
							GraphicsState savedState = g.Save();
							bounds = TabUtils.ApplyTransform( g, this.TabAlignment, bounds, false );
							g.Restore( savedState );
							g.ResetTransform();
						}
					}
				}
			}
      bool hit = false;
			foreach( RectangleF rect in m_tabGroupBounds )
			{
				GraphicsPath path = this.GetBorderPathFromBounds( rect );
				path.CloseFigure();
				Region region = new Region( path );
				if(region.IsVisible(mousePosition))
					hit = true;
			}
			return hit;
		}
		/// <summary>
		/// Gets preferred size for tab item.
		/// </summary>
		/// <param name="g">Graphics for string measuring.</param>
		/// <returns>preferred size.</returns>
		public override SizeF GetPreferredSize(Graphics g)
		{
			SizeF preferredSize = SizeF.Empty;
			// Pass 1, find out the longest item
			int i = -1;
			TabGroupData tabGroupData = ((TabGroupData)this.TabData);
			SizeF itemSize = Size.Empty;
			foreach(TabGroupItem item in tabGroupData.Items)
			{
				i++;
				tabGroupData.Text = item.Text;
				tabGroupData.ImageIndex = item.ImageIndex;

				itemSize = base.GetPreferredSize(g);
				itemSize -= new Size(2,2);	// Size for the borders
				preferredSize.Height = itemSize.Height;
				preferredSize.Width += itemSize.Width+DEF_WIDTH_CORRECT;
			}

			// For borders
			preferredSize += new Size(2,2);
			return preferredSize;
		}
		/// <summary>
		/// Gets/sets tab (group) data.
		/// </summary>
		public override ITabData TabData
		{
			get
			{
				ITabData data = base.TabData;
				return data;
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
		/// <summary>
		/// Calculates overlap size.
		/// </summary>
		/// <param name="tabSize">tab size.</param>
		/// <returns>Size.</returns>
		public override SizeF GetOverlapSize(SizeF tabSize)
		{
			return Size.Empty;
		}
		/// <summary>
		/// Draws interior.
		/// </summary>
		/// <param name="drawItemInfo">Arguments for draw interior action.</param>
		protected override void DrawInterior(DrawTabEventArgs drawItemInfo)
		{
			Graphics g = drawItemInfo.Graphics;

			// Use Bounds here instead of BoundsInterior
			this.SetItemBounds(g, drawItemInfo.Bounds);
			m_hitBounds.Clear();
			
			for( int j = m_tabGroupBounds.Count - 1; j >= 0; j-- )
			{
				DrawTabEventArgs currentArgs = new DrawTabEventArgs( drawItemInfo);
				currentArgs.Bounds = Rectangle.Round( TabUtils.ApplyTransform( g, this.TabAlignment, (RectangleF)m_tabGroupBounds[j], false) );
				
				this.PaintBackground( currentArgs );
				this.PaintBorders( currentArgs );
			}

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
				tabGroupData.ImageIndex = ((TabGroupItem)tabGroupData.Items[i]).ImageIndex;
				
				tabGroupData.Text = ((TabGroupItem)tabGroupData.Items[i]).Text;
				boundsTransformed.X = ((RectangleF)m_tabGroupBounds[i]).Location.X;

				DrawTextAndImage(g, boundsTransformed, drawItemInfo);
			}
			

			g.ResetTransform();
		}
		/// <summary>
		/// Corrects 
		/// </summary>
		/// <param name="bounds"></param>
		/// <returns></returns>
		protected override RectangleF CorrectBounds( RectangleF bounds )
		{
			bounds.Width -= DEF_WIDTH_CORRECT;
			return bounds;
		}
		/// <summary>
		/// Gets current bounds.
		/// </summary>
		/// <returns>Current bounds.</returns>
		public override RectangleF GetCurrentBounds()
		{
			bool tabSelected = this.panelRenderer.TabPanelData.SelectedIndex == -1 ?
				false :
				this.panelRenderer.TabPanelData.TabsData[this.panelRenderer.TabPanelData.SelectedIndex] == this.TabData;
		
			SizeF overlappedSize = this.GetOverlapSize(this.panelRenderer.TabPanelData.TabSize);
			RectangleF overlappedRect = this.Bounds;
			overlappedRect.Inflate((float)Math.Ceiling(overlappedSize.Width/2), 0);
			overlappedRect.Offset(0, -overlappedSize.Height);
			overlappedRect.Height += overlappedSize.Height;

			return overlappedRect;
		}
		/// <summary>
		/// Gets interior(separator of tabs)bounds.
		/// </summary>
		/// <param name="currentBounds">Current bounds.</param>
		/// <param name="selectedTab">If tab is selected.</param>
		/// <returns>Interior bounds.</returns>
		protected override RectangleF GetInteriorBounds(RectangleF currentBounds, bool selectedTab)
		{
			return base.GetInteriorBounds( currentBounds, selectedTab );
		}
		#endregion

		#region Class private methods
		/// <summary>
		/// Calculates hit bounds for current item.
		/// </summary>
		/// <param name="bounds">Bounds of item.</param>
		protected virtual void CalculateHitBound( RectangleF bounds )
		{
			PointF location = new PointF( bounds.Location.X + m_hitOffset
				, bounds.Location.Y);
			float width = bounds.Width - m_hitOffset*2;
			m_hitBounds.Add( new RectangleF( location, new SizeF( width, bounds.Height ) ) );
		}
		/// <summary>
		/// Draws background.
		/// </summary>
		/// <param name="drawItemInfo">Painting arguments.</param>
		protected virtual void PaintBackground(DrawTabEventArgs drawItemInfo)
		{
			if(drawItemInfo.Bounds.Width == 0 || drawItemInfo.Bounds.Height == 0)
				return;

			Graphics gph = drawItemInfo.Graphics;
			RectangleF curBounds = TabUtils.ApplyTransform(gph, this.TabAlignment, drawItemInfo.Bounds, true);

			// Make g horizontal
			ApplyTransform(gph);
			CalculateHitBound( curBounds );

			SaveGraphicsState( gph, ref curBounds );

			Color bgColor = drawItemInfo.BackColor;
			Color backColorTop = ControlPaint.LightLight( bgColor );

			// Get the border path and fill it.
			GraphicsPath path = null;
			
			path = this.GetBorderPathFromBounds(curBounds);
			path.CloseFigure();

			Brush brs = new LinearGradientBrush(curBounds, backColorTop,
				bgColor, LinearGradientMode.Vertical);
			gph.FillPath(brs, path);

			RestoreGraphicsState( gph );

			gph.ResetTransform();
		}
		/// <summary>
		/// Draws borders.
		/// </summary>
		/// <param name="drawItemInfo">Draw bounds action arguments.</param>
		protected virtual void PaintBorders(DrawTabEventArgs drawItemInfo)
		{
			Graphics g = drawItemInfo.Graphics;

			RectangleF curBounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);

			// Make g horizontal
			ApplyTransform(g);

			SaveGraphicsState( g, ref curBounds );
		
			SmoothingMode oldSM = g.SmoothingMode;
			g.SmoothingMode = SmoothingMode.None;

			bool bIsMirrored = panelRenderer.IsMirrored;

			drawItemInfo.Graphics.DrawPath(new Pen( this.BorderColor, 1),
				this.GetBorderPathFromBounds(curBounds));
			g.DrawPath( new Pen( VS2005Colors.InnerBorderColor ), m_innerPath );

			g.SmoothingMode = oldSM;

			RestoreGraphicsState( g );

			g.ResetTransform();
		}
		/// <summary>
		/// Event hangler for Items changed event.
		/// </summary>
		/// <param name="sender">object sender.</param>
		/// <param name="args">Change collection event arguments.</param>
		private void ItemsCollection_Changed(object sender, CollectionChangeEventArgs args)
		{
			boundsChanged = true;
		}
		/// <summary>
		/// Event hangler for item bounds changed event.
		/// </summary>
		/// <param name="sender">object sender.</param>
		private void ItemBounds_Changed(object sender, EventArgs e)
		{
			boundsChanged = true;
		}
		/// <summary>
		/// Sets item bounds.
		/// </summary>
		/// <param name="g">Graphics to use.</param>
		/// <param name="tabBounds">Bounds of tab items.</param>
		protected virtual void SetItemBounds(Graphics g, RectangleF tabBounds)
		{
			TabGroupData groupTabData = (TabGroupData)this.TabData;

			if(boundsChanged || itemBounds.Count != groupTabData.Items.Count)
				boundsChanged = false;
			else
				return;

			if(groupTabData.Items.Count == 0)
				return;

			tabBounds = TabUtils.ApplyTransform(g, this.TabAlignment, tabBounds, true);

			itemBounds.Clear();
			m_tabGroupBounds.Clear();

			TabControlAdv tcaParentBase = TabControl as TabControlAdv;
			Debug.Assert( null != tcaParentBase );

			float fTabLeft = tabBounds.Left + 1;

			for(int i = 0; i < groupTabData.Items.Count; i++)
			{
				fTabLeft += groupTabData.Padding.X;

				TabGroupItem item = (TabGroupItem)groupTabData.Items[i];
				groupTabData.Text = "";
				groupTabData.Text = item.Text;

				groupTabData.ImageIndex = item.ImageIndex;
				groupTabData.Text = item.Text;

				SizeF itemPrefSize = base.GetPreferredSize(g);

				itemPrefSize -= new SizeF(2, 2);
				itemPrefSize -= new SizeF(groupTabData.Padding.X * 2, 0);

				if( i != 0 )
					fTabLeft -= DEF_ITEM_OFFSET;

				itemBounds.Add(new RectangleF(new PointF(fTabLeft , tabBounds.Top + 1),
					new SizeF(itemPrefSize.Width+DEF_CURVE_OFFSET,itemPrefSize.Height)));

				fTabLeft += itemPrefSize.Width + 1 + groupTabData.Padding.X;
			}

			// There is some empty space to the right, allocate it to the selected tab group item
			float freeSpace = tabBounds.Right - 1 - fTabLeft;

			if( freeSpace > 0 )
			{
				freeSpace = freeSpace / itemBounds.Count;

				for( int i = 0; i < itemBounds.Count; i++ )
				{
					RectangleF rect = (RectangleF)itemBounds[i];
					rect.Width += freeSpace;
					itemBounds[i] = rect;
				}
			}
			
			foreach( RectangleF boards in itemBounds )
			{
				m_tabGroupBounds.Add( new RectangleF( 
					boards.X
					, tabBounds.Y
					, boards.Width
					, tabBounds.Height ) );
			}
			if( m_tabGroupBounds.Count == 1 )
			{
				RectangleF rect = (RectangleF)m_tabGroupBounds[0];
				rect.X -= 5;
				m_tabGroupBounds[0] = rect;
			}	
				
		}
		#endregion
	}
}
