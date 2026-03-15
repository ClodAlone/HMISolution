#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region Class using directives
using System;
using System.Collections;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
	/// <summary>
	/// Paints AH tab groups in VS2005 style.
	/// </summary>
	class TabGroupRendererVS2005
		: TabGroupRendererVS2005Beta
		, ITabGroupRenderer
	{
		#region Class members
		/// <summary>
		/// Stores calculated width of AH tab items.
		/// </summary>
		protected ArrayList m_itemsWidth = new ArrayList();
		#endregion

		#region Class properties
		/// <summary>
		/// Gets tab style name.
		/// </summary>
		new public static string TabStyleName
		{
			get
			{
				return "DockingTabsVS2005";
			}
		}
		#endregion

		#region Class constants
		/// <summary>
		/// Width correction to base tab item width.
		/// </summary>
		private const int DEF_WIDTH_CORRECT = 10;
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Registers class types.
		/// </summary>
		static TabGroupRendererVS2005()
		{
			m_tabPropertyExtender = new TabUIVS2005Properties();
			TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabGroupRendererVS2005), TabPanelPropertyExtender);
		}
		/// <summary>
		/// Creates an instance of the <see cref="TabGroupRendererVS2005"/>.
		/// </summary>
		/// <param name="parent">The <see cref="Syncfusion.Windows.Forms.Tools.ITabControl"/> instance.</param>
		/// <param name="panelRenderer">The parent <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> instance.</param>
		public TabGroupRendererVS2005(ITabControl parent, ITabPanelRenderer panelRenderer)
			: base(parent, panelRenderer)
		{
			itemBounds = new ArrayList();
			m_tabGroupBounds = new ArrayList();
			m_hitBounds = new ArrayList();
			VS2005Colors.UpdateStyleColors();			
		}

		#endregion

		#region Class overrides
		/// <summary>
		/// Overriden.
		/// </summary>
		/// <param name="tabSize"></param>
		/// <returns></returns>
		public override SizeF GetOverlapSize( SizeF tabSize )
		{
			SizeF size = SizeF.Empty;
			size.Width = DEF_WIDTH_CORRECT;
			return Size.Empty;
		}

		/// <summary>
		/// Returns preferred size for group items.
		/// </summary>
		/// <param name="g">Graphics to measure strings.</param>
		/// <returns>Preferred size</returns>
		public override SizeF GetPreferredSize( Graphics g )
		{
			SizeF preferredSize = SizeF.Empty;
			// Pass 1, find out the longest item
			int i = -1;
			TabGroupData tabGroupData = ( ( TabGroupData )this.TabData );
			SizeF itemSize = Size.Empty;
			m_itemsWidth.Clear();
			foreach( TabGroupItem item in tabGroupData.Items )
			{
				i++;
				tabGroupData.Text = item.Text;
				tabGroupData.ImageIndex = item.ImageIndex;

				itemSize = base.GetItemPreferredSize(g);
				itemSize -= new Size(2, 2);	// Size for the borders
				preferredSize.Height = itemSize.Height;
				float width = itemSize.Width + DEF_WIDTH_CORRECT/2;
				m_itemsWidth.Add( width );
				preferredSize.Width += width;
			}
			preferredSize.Width += DEF_WIDTH_CORRECT;
			preferredSize.Height += 2;
			
			return preferredSize;
		}

		protected override RectangleF CorrectBounds( RectangleF bounds )
		{
			bounds.Width -= DEF_WIDTH_CORRECT;
			return bounds;
		}
		/// <summary>
		/// Borders are not painted in this method// see PaintBorders.
		/// </summary>
		/// <param name="drawItemInfo">Info needed to draw tab item.</param>
		protected override void DrawBorders( DrawTabEventArgs drawItemInfo )
		{}
		/// <summary>
		/// Overriden. 
		/// </summary>
		/// <param name="drawItemInfo">Info needed to draw tab item.</param>
		protected override void DrawInterior( DrawTabEventArgs drawItemInfo )
		{
			Graphics g = drawItemInfo.Graphics;

			// Use Bounds here instead of BoundsInterior
			this.SetItemBounds(g, drawItemInfo.Bounds);
			m_hitBounds.Clear();

			for( int j = m_tabGroupBounds.Count - 1; j >= 0; j-- )
			{
				DrawTabEventArgs currentArgs = new DrawTabEventArgs(drawItemInfo);
				currentArgs.Bounds = Rectangle.Round(TabUtils.ApplyTransform(g, this.TabAlignment, ( RectangleF )m_tabGroupBounds[j], false));

				this.PaintBackground(currentArgs);
				this.PaintBorders(currentArgs);
			}

			// Transform g to horizontal co-ords
			this.ApplyTransform(g);

			TabGroupData tabGroupData = ( ( TabGroupData )this.TabData );
			int i = -1;
			foreach( RectangleF bounds in itemBounds )
			{
				tabGroupData.Text = "";
				i++;								
				RectangleF boundsTransformed = bounds;
				tabGroupData.ImageIndex = ( ( TabGroupItem )tabGroupData.Items[i] ).ImageIndex;

				tabGroupData.Text = ( ( TabGroupItem )tabGroupData.Items[i] ).Text;
				boundsTransformed.X = ( ( RectangleF )m_tabGroupBounds[i] ).Location.X;

				DrawTextAndImage(g, boundsTransformed, drawItemInfo);
			}


			g.ResetTransform();
		}
		/// <summary>
		/// No background in VS2005 style.
		/// </summary>
		/// <param name="drawItemInfo"></param>
		protected override void PaintBackground( DrawTabEventArgs drawItemInfo )
		{}
		/// <summary>
		/// Paints borders. Called from DrawInterior().
		/// </summary>
		/// <param name="drawItemInfo"></param>
		protected override void PaintBorders( DrawTabEventArgs drawItemInfo )
		{
			Graphics g = drawItemInfo.Graphics;
			Pen borderPen = new Pen(VS2005Colors.BorderColor);

			RectangleF curBounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);

			// Make g horizontal
			ApplyTransform(g);
			CalculateHitBound(curBounds);

			SaveGraphicsState(g, ref curBounds);

			SmoothingMode oldSM = g.SmoothingMode;
			g.SmoothingMode = SmoothingMode.None;
			
			Rectangle itemBounds = Rectangle.Round(curBounds);

			Point leftBottom = new Point(itemBounds.Left, itemBounds.Bottom);
			Point leftTop = new Point(itemBounds.Left, itemBounds.Top + 2);
			Point topLeft = new Point(itemBounds.Left + 2, itemBounds.Top);
			Point topRight = new Point(itemBounds.Right - 2, itemBounds.Top);
			Point rightTop = new Point(itemBounds.Right, itemBounds.Top + 2);
			Point rightBottom = new Point(itemBounds.Right, itemBounds.Bottom);

			GraphicsPath path = new GraphicsPath();
			path.AddLine(leftBottom, leftTop);
			path.AddLine(leftTop, topLeft);
			path.AddLine(topLeft, topRight);
			path.AddLine(topRight, rightTop);
			path.AddLine(rightTop, rightBottom);

			g.DrawPath(borderPen, path);
			
			g.SmoothingMode = oldSM;

			RestoreGraphicsState(g);

			g.ResetTransform();
		}
		/// <summary>
		/// Performs hit test for this renderer style.
		/// </summary>
		/// <param name="mousePosition">MousePosition for hit test.</param>
		/// <returns>if mouse is over rendered area.</returns>
		public override bool HitTest( PointF mousePosition )
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
							RectangleF rectMousePos = new RectangleF(mousePosition,
								SizeF.Empty);

							rectMousePos = TabUtils.ApplyTransform(g, this.TabAlignment,
								rectMousePos, false);

							mousePosition = rectMousePos.Location;
							GraphicsState savedState = g.Save();
							bounds = TabUtils.ApplyTransform(g, this.TabAlignment, bounds, false);
							g.Restore(savedState);
							g.ResetTransform();
						}
					}
				}
			}
			bool hit = false;
			foreach( RectangleF rect in m_tabGroupBounds )
			{
				if( rect.Contains(mousePosition) )
				{
					hit = true;
					break;
				}
			}
			return hit;
		}
		#endregion

		#region Class helper methods
		/// <summary>
		/// Sets bounds for each item.
		/// </summary>
		/// <param name="g">Graphics to use.</param>
		/// <param name="tabBounds">Bounds of tab items.</param>
		protected override void SetItemBounds( Graphics g, RectangleF tabBounds )
		{
			TabGroupData groupTabData = ( TabGroupData )this.TabData;

			if( boundsChanged || itemBounds.Count != groupTabData.Items.Count )
				boundsChanged = false;
			else
				return;

			if( groupTabData.Items.Count == 0 )
				return;

			tabBounds = TabUtils.ApplyTransform(g, this.TabAlignment, tabBounds, true);

			itemBounds.Clear();
			m_tabGroupBounds.Clear();

			TabControlAdv tcaParentBase = TabControl as TabControlAdv;
			Debug.Assert(null != tcaParentBase);

			float fTabLeft = tabBounds.Left + 1;

			for( int i = 0; i < groupTabData.Items.Count; i++ )
			{
				TabGroupItem item = ( TabGroupItem )groupTabData.Items[i];
				groupTabData.Text = "";
				groupTabData.Text = item.Text;
				SizeF itemPrefSize = new SizeF();
				itemPrefSize.Height = tabBounds.Height;
				itemPrefSize.Width = (float)m_itemsWidth[i];

				groupTabData.ImageIndex = item.ImageIndex;
				groupTabData.Text = item.Text;

				itemPrefSize.Height -= 2;

				itemBounds.Add(new RectangleF(new PointF(fTabLeft, tabBounds.Top + 1),
					new SizeF(itemPrefSize.Width , itemPrefSize.Height)));

				fTabLeft += itemPrefSize.Width;
			}

			// There is some empty space to the right, allocate it to the selected tab group item
			float freeSpace = tabBounds.Right - 1 - fTabLeft;

			if( freeSpace > 0 )
			{
				freeSpace = freeSpace / itemBounds.Count;

				for( int i = 0; i < itemBounds.Count; i++ )
				{
					RectangleF rect = ( RectangleF )itemBounds[i];
					rect.Width += freeSpace;
					itemBounds[i] = rect;
				}
			}

			foreach( RectangleF boards in itemBounds )
			{
				m_tabGroupBounds.Add(new RectangleF(
					boards.X
					, tabBounds.Y
					, boards.Width
					, tabBounds.Height));
			}
		}
		/// <summary>
		/// Gets AH caption to draw.
		/// </summary>
		/// <param name="caption">Default caption for item.</param>
		/// <returns>Caption to draw. Reformated if needed.</returns>
		protected string GetAutoHideCaption( string caption )
		{
			if( ( parent != null ) )
			{
				AHTabControl tabControl = parent as AHTabControl;
				if( tabControl != null )
				{
					DockingManager dockManager = tabControl.DockingManager;
					if( ( dockManager != null ) && ( dockManager.FullCaptionsInAutoHideMode == true ) )
					{
						return caption;
					}
				}
			}
			return caption.Substring(0, caption.Length < 3 ? caption.Length : 3);
		}
		#endregion
	}

    /// <summary>
    /// Paints AH tab groups in VS2010 style.
    /// </summary>
    class TabGroupRendererVS2010
        : TabGroupRendererVS2005Beta
        , ITabGroupRenderer
    {
        #region Class members
        /// <summary>
        /// Stores calculated width of AH tab items.
        /// </summary>
        protected ArrayList m_itemsWidth = new ArrayList();
        #endregion

        #region Class properties
        /// <summary>
        /// Gets tab style name.
        /// </summary>
        new public static string TabStyleName
        {
            get
            {
                return "DockingTabsVS2010";
            }
        }
        #endregion

        #region Class constants
        /// <summary>
        /// Width correction to base tab item width.
        /// </summary>
        private const int DEF_WIDTH_CORRECT = 10;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Registers class types.
        /// </summary>
        static TabGroupRendererVS2010()
        {
            m_tabProperty2010Extender = new TabUIVS2010Properties();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabGroupRendererVS2010), m_tabProperty2010Extender);
        }
        /// <summary>
        /// Creates an instance of the <see cref="TabGroupRendererVS2005"/>.
        /// </summary>
        /// <param name="parent">The <see cref="Syncfusion.Windows.Forms.Tools.ITabControl"/> instance.</param>
        /// <param name="panelRenderer">The parent <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> instance.</param>
        public TabGroupRendererVS2010(ITabControl parent, ITabPanelRenderer panelRenderer)
            : base(parent, panelRenderer)
        {
            itemBounds = new ArrayList();
            m_tabGroupBounds = new ArrayList();
            m_hitBounds = new ArrayList();
            VS2005Colors.UpdateStyleColors();
        }

        #endregion

        #region Class overrides
        /// <summary>
        /// Overriden.
        /// </summary>
        /// <param name="tabSize"></param>
        /// <returns></returns>
        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            SizeF size = SizeF.Empty;
            size.Width = DEF_WIDTH_CORRECT;
            return Size.Empty;
        }

        /// <summary>
        /// Returns preferred size for group items.
        /// </summary>
        /// <param name="g">Graphics to measure strings.</param>
        /// <returns>Preferred size</returns>
        public override SizeF GetPreferredSize(Graphics g)
        {
            SizeF preferredSize = SizeF.Empty;
            // Pass 1, find out the longest item
            int i = -1;
            TabGroupData tabGroupData = ((TabGroupData)this.TabData);
            SizeF itemSize = Size.Empty;
            m_itemsWidth.Clear();
            foreach (TabGroupItem item in tabGroupData.Items)
            {
                i++;
                tabGroupData.Text = item.Text;
                tabGroupData.ImageIndex = item.ImageIndex;

                itemSize = base.GetItemPreferredSize(g);
                itemSize -= new Size(2, 2);	// Size for the borders
                preferredSize.Height = itemSize.Height;
                float width = itemSize.Width + DEF_WIDTH_CORRECT / 2;
                m_itemsWidth.Add(width);
                preferredSize.Width += width;
            }
            preferredSize.Width += DEF_WIDTH_CORRECT;
            preferredSize.Height += 2;

            return preferredSize;
        }

        protected override RectangleF CorrectBounds(RectangleF bounds)
        {
            bounds.Width -= DEF_WIDTH_CORRECT;
            return bounds;
        }
        /// <summary>
        /// Borders are not painted in this method// see PaintBorders.
        /// </summary>
        /// <param name="drawItemInfo">Info needed to draw tab item.</param>
        protected override void DrawBorders(DrawTabEventArgs drawItemInfo)
        { }
        /// <summary>
        /// Overriden. 
        /// </summary>
        /// <param name="drawItemInfo">Info needed to draw tab item.</param>
        protected override void DrawInterior(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;

            // Use Bounds here instead of BoundsInterior
            this.SetItemBounds(g, drawItemInfo.Bounds);
            m_hitBounds.Clear();

            for (int j = m_tabGroupBounds.Count - 1; j >= 0; j--)
            {
                DrawTabEventArgs currentArgs = new DrawTabEventArgs(drawItemInfo);
                currentArgs.Bounds = Rectangle.Round(TabUtils.ApplyTransform(g, this.TabAlignment, (RectangleF)m_tabGroupBounds[j], false));

                this.PaintBackground(currentArgs);
                this.PaintBorders(currentArgs);
            }

            // Transform g to horizontal co-ords
            this.ApplyTransform(g);

            TabGroupData tabGroupData = ((TabGroupData)this.TabData);
            int i = -1;
            foreach (RectangleF bounds in itemBounds)
            {
                tabGroupData.Text = "";
                i++;
                RectangleF boundsTransformed = bounds;
                tabGroupData.ImageIndex = ((TabGroupItem)tabGroupData.Items[i]).ImageIndex;

                tabGroupData.Text = ((TabGroupItem)tabGroupData.Items[i]).Text;
                boundsTransformed.X = ((RectangleF)m_tabGroupBounds[i]).Location.X;
                drawItemInfo.ForeColor = Color.White;
                DrawTextAndImage(g, boundsTransformed, drawItemInfo);
            }


            g.ResetTransform();
        }
        /// <summary>
        /// No background in VS2005 style.
        /// </summary>
        /// <param name="drawItemInfo"></param>
        protected override void PaintBackground(DrawTabEventArgs drawItemInfo)
        { }
        /// <summary>
        /// Paints borders. Called from DrawInterior().
        /// </summary>
        /// <param name="drawItemInfo"></param>
        protected override void PaintBorders(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;
            Pen borderPen = new Pen(Color.FromArgb(155, 167, 183));

            RectangleF curBounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);

            // Make g horizontal
            ApplyTransform(g);
            CalculateHitBound(curBounds);

            SaveGraphicsState(g, ref curBounds);

            SmoothingMode oldSM = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            Rectangle itemBounds = Rectangle.Round(curBounds);

            Point leftBottom = new Point(itemBounds.Left, itemBounds.Bottom);
            Point leftTop = new Point(itemBounds.Left, itemBounds.Top + 2);
            Point topLeft = new Point(itemBounds.Left + 2, itemBounds.Top);
            Point topRight = new Point(itemBounds.Right - 2, itemBounds.Top);
            Point rightTop = new Point(itemBounds.Right, itemBounds.Top + 2);
            Point rightBottom = new Point(itemBounds.Right, itemBounds.Bottom);

            GraphicsPath path = new GraphicsPath();
            path.AddLine(leftBottom, leftTop);
            path.AddLine(leftTop, topLeft);
            path.AddLine(topLeft, topRight);
            path.AddLine(topRight, rightTop);
            path.AddLine(rightTop, rightBottom);

            g.DrawPath(borderPen, path);

            g.SmoothingMode = oldSM;

            RestoreGraphicsState(g);

            g.ResetTransform();
        }
        /// <summary>
        /// Performs hit test for this renderer style.
        /// </summary>
        /// <param name="mousePosition">MousePosition for hit test.</param>
        /// <returns>if mouse is over rendered area.</returns>
        public override bool HitTest(PointF mousePosition)
        {
            RectangleF bounds = this.GetCurrentBounds();

            if (this.NeedRotateTextWhenVertical)
            {
                if (this.TabControl != null)
                {
                    Control control = this.TabControl.GetControl();
                    if (control != null)
                    {
                        using (Graphics g = control.CreateGraphics())
                        {
                            RectangleF rectMousePos = new RectangleF(mousePosition,
                                SizeF.Empty);

                            rectMousePos = TabUtils.ApplyTransform(g, this.TabAlignment,
                                rectMousePos, false);

                            mousePosition = rectMousePos.Location;
                            GraphicsState savedState = g.Save();
                            bounds = TabUtils.ApplyTransform(g, this.TabAlignment, bounds, false);
                            g.Restore(savedState);
                            g.ResetTransform();
                        }
                    }
                }
            }
            bool hit = false;
            foreach (RectangleF rect in m_tabGroupBounds)
            {
                if (rect.Contains(mousePosition))
                {
                    hit = true;
                    break;
                }
            }
            return hit;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Sets bounds for each item.
        /// </summary>
        /// <param name="g">Graphics to use.</param>
        /// <param name="tabBounds">Bounds of tab items.</param>
        protected override void SetItemBounds(Graphics g, RectangleF tabBounds)
        {
            TabGroupData groupTabData = (TabGroupData)this.TabData;

            if (boundsChanged || itemBounds.Count != groupTabData.Items.Count)
                boundsChanged = false;
            else
                return;

            if (groupTabData.Items.Count == 0)
                return;

            tabBounds = TabUtils.ApplyTransform(g, this.TabAlignment, tabBounds, true);

            itemBounds.Clear();
            m_tabGroupBounds.Clear();

            TabControlAdv tcaParentBase = TabControl as TabControlAdv;
            Debug.Assert(null != tcaParentBase);

            float fTabLeft = tabBounds.Left + 1;

            for (int i = 0; i < groupTabData.Items.Count; i++)
            {
                TabGroupItem item = (TabGroupItem)groupTabData.Items[i];
                groupTabData.Text = "";
                groupTabData.Text = item.Text;
                SizeF itemPrefSize = new SizeF();
                itemPrefSize.Height = tabBounds.Height;
                itemPrefSize.Width = (float)m_itemsWidth[i];

                groupTabData.ImageIndex = item.ImageIndex;
                groupTabData.Text = item.Text;

                itemPrefSize.Height -= 2;

                itemBounds.Add(new RectangleF(new PointF(fTabLeft, tabBounds.Top + 1),
                    new SizeF(itemPrefSize.Width, itemPrefSize.Height)));

                fTabLeft += itemPrefSize.Width;
            }

            // There is some empty space to the right, allocate it to the selected tab group item
            float freeSpace = tabBounds.Right - 1 - fTabLeft;

            if (freeSpace > 0)
            {
                freeSpace = freeSpace / itemBounds.Count;

                for (int i = 0; i < itemBounds.Count; i++)
                {
                    RectangleF rect = (RectangleF)itemBounds[i];
                    rect.Width += freeSpace;
                    itemBounds[i] = rect;
                }
            }

            foreach (RectangleF boards in itemBounds)
            {
                m_tabGroupBounds.Add(new RectangleF(
                    boards.X
                    , tabBounds.Y
                    , boards.Width
                    , tabBounds.Height));
            }
        }
        /// <summary>
        /// Gets AH caption to draw.
        /// </summary>
        /// <param name="caption">Default caption for item.</param>
        /// <returns>Caption to draw. Reformated if needed.</returns>
        protected string GetAutoHideCaption(string caption)
        {
            if ((parent != null))
            {
                AHTabControl tabControl = parent as AHTabControl;
                if (tabControl != null)
                {
                    DockingManager dockManager = tabControl.DockingManager;
                    if ((dockManager != null) && (dockManager.FullCaptionsInAutoHideMode == true))
                    {
                        return caption;
                    }
                }
            }
            return caption.Substring(0, caption.Length < 3 ? caption.Length : 3);
        }
        #endregion
    }

    class TabGroupRendererVS2012
        : TabGroupRendererVS2005Beta
        , ITabGroupRenderer
    {
        #region Class members
        /// <summary>
        /// Stores calculated width of AH tab items.
        /// </summary>
        protected ArrayList m_itemsWidth = new ArrayList();
        #endregion

        #region Class properties
        /// <summary>
        /// Gets tab style name.
        /// </summary>
        new public static string TabStyleName
        {
            get
            {
                return "DockingTabsVS2005";
            }
        }
        #endregion

        #region Class constants
        /// <summary>
        /// Width correction to base tab item width.
        /// </summary>
        private const int DEF_WIDTH_CORRECT = 10;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Registers class types.
        /// </summary>
        static TabGroupRendererVS2012()
        {
            m_tabProperty2012Extender = new TabUIVS2012Properties();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabGroupRendererVS2012), TabPanel2012PropertyExtender);
        }
        /// <summary>
        /// Creates an instance of the <see cref="TabGroupRendererVS2005"/>.
        /// </summary>
        /// <param name="parent">The <see cref="Syncfusion.Windows.Forms.Tools.ITabControl"/> instance.</param>
        /// <param name="panelRenderer">The parent <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> instance.</param>
        public TabGroupRendererVS2012(ITabControl parent, ITabPanelRenderer panelRenderer)
            : base(parent, panelRenderer)
        {
            itemBounds = new ArrayList();
            m_tabGroupBounds = new ArrayList();
            m_hitBounds = new ArrayList();
            VS2005Colors.UpdateStyleColors();
        }

        #endregion

        #region Class overrides
        /// <summary>
        /// Overriden.
        /// </summary>
        /// <param name="tabSize"></param>
        /// <returns></returns>
        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            SizeF size = SizeF.Empty;
            size.Width = DEF_WIDTH_CORRECT;
            return Size.Empty;
        }

        /// <summary>
        /// Returns preferred size for group items.
        /// </summary>
        /// <param name="g">Graphics to measure strings.</param>
        /// <returns>Preferred size</returns>
        public override SizeF GetPreferredSize(Graphics g)
        {
            SizeF preferredSize = SizeF.Empty;
            // Pass 1, find out the longest item
            int i = -1;
            TabGroupData tabGroupData = ((TabGroupData)this.TabData);
            SizeF itemSize = Size.Empty;
            m_itemsWidth.Clear();
            foreach (TabGroupItem item in tabGroupData.Items)
            {
                i++;
                tabGroupData.Text = item.Text;
                tabGroupData.ImageIndex = item.ImageIndex;

                itemSize = base.GetItemPreferredSize(g);
                itemSize -= new Size(2, 2);	// Size for the borders
                preferredSize.Height = itemSize.Height;
                float width = itemSize.Width + DEF_WIDTH_CORRECT / 2;
                m_itemsWidth.Add(width);
                preferredSize.Width += width;
            }
            preferredSize.Width += DEF_WIDTH_CORRECT;
            preferredSize.Height += 2;

            return preferredSize;
        }

        protected override RectangleF CorrectBounds(RectangleF bounds)
        {
            bounds.Width -= DEF_WIDTH_CORRECT;
            return bounds;
        }
        /// <summary>
        /// Borders are not painted in this method// see PaintBorders.
        /// </summary>
        /// <param name="drawItemInfo">Info needed to draw tab item.</param>
        protected override void DrawBorders(DrawTabEventArgs drawItemInfo)
        { }
        /// <summary>
        /// Overriden. 
        /// </summary>
        /// <param name="drawItemInfo">Info needed to draw tab item.</param>
        protected override void DrawInterior(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;

            // Use Bounds here instead of BoundsInterior
            this.SetItemBounds(g, drawItemInfo.Bounds);
            m_hitBounds.Clear();

            for (int j = m_tabGroupBounds.Count - 1; j >= 0; j--)
            {
                DrawTabEventArgs currentArgs = new DrawTabEventArgs(drawItemInfo);
                currentArgs.Bounds = Rectangle.Round(TabUtils.ApplyTransform(g, this.TabAlignment, (RectangleF)m_tabGroupBounds[j], false));

                this.PaintBackground(currentArgs);
                currentArgs.Bounds = new Rectangle(currentArgs.Bounds.Location, new Size(currentArgs.Bounds.Width, currentArgs.Bounds.Height));
                this.PaintBorders(currentArgs);
                //RectangleF curBounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);

                //// Make g horizontal
                //ApplyTransform(g);
                //Rectangle itemBounds1 = Rectangle.Round(curBounds);
                //g.FillRectangle(new SolidBrush(Color.Red), itemBounds1.X, itemBounds1.Y + (itemBounds1.Height / 2), itemBounds1.Width, itemBounds1.Height / 3);
            }

            // Transform g to horizontal co-ords
            this.ApplyTransform(g);

            TabGroupData tabGroupData = ((TabGroupData)this.TabData);
            int i = -1;
            foreach (RectangleF bounds in itemBounds)
            {
                tabGroupData.Text = "";
                i++;
                RectangleF boundsTransformed = bounds;
                tabGroupData.ImageIndex = ((TabGroupItem)tabGroupData.Items[i]).ImageIndex;

                tabGroupData.Text = ((TabGroupItem)tabGroupData.Items[i]).Text;
                boundsTransformed.X = ((RectangleF)m_tabGroupBounds[i]).Location.X - 4;
                if(this.TabAlignment == System.Windows.Forms.TabAlignment.Left || this.TabAlignment == System.Windows.Forms.TabAlignment.Right )
                    boundsTransformed.Y = ((RectangleF)m_tabGroupBounds[i]).Location.Y + 4;
                else if (this.TabAlignment == System.Windows.Forms.TabAlignment.Top)
                    boundsTransformed.Y = ((RectangleF)m_tabGroupBounds[i]).Location.Y + 3;
                else if (this.TabAlignment == System.Windows.Forms.TabAlignment.Bottom)
                    boundsTransformed.Y = ((RectangleF)m_tabGroupBounds[i]).Location.Y + 5;
                int indexSelection = 0;

                if (this.parent is AHTabControl)
                {
                    indexSelection = (this.parent as AHTabControl).ClientIndex;
                }
                if (drawItemInfo != null && drawItemInfo.Index == indexSelection)
                    tabGroupData.ForeColor = (this.parent as AHTabControl).DockingManager.MetroColor;
                else
                    tabGroupData.ForeColor = Color.Empty;

                DrawTextAndImage(g, boundsTransformed, drawItemInfo);
            }


            g.ResetTransform();
        }
        /// <summary>
        /// No background in VS2005 style.
        /// </summary>
        /// <param name="drawItemInfo"></param>
        protected override void PaintBackground(DrawTabEventArgs drawItemInfo)
        {
            //Graphics g = drawItemInfo.Graphics;
            //RectangleF curBounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);
            //Rectangle itemBounds = Rectangle.Round(curBounds);


        }
        /// <summary>
        /// Paints borders. Called from DrawInterior().
        /// </summary>
        /// <param name="drawItemInfo"></param>
        protected override void PaintBorders(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;
            Pen borderPen = new Pen(Color.FromArgb(239, 239, 242));

            RectangleF curBounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);

            // Make g horizontal
            ApplyTransform(g);
            CalculateHitBound(curBounds);

            SaveGraphicsState(g, ref curBounds);

            SmoothingMode oldSM = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            Rectangle itemBounds = Rectangle.Round(curBounds);

            Point leftBottom = new Point(itemBounds.Left, itemBounds.Bottom);
            Point leftTop = new Point(itemBounds.Left, itemBounds.Top);
            Point topLeft = new Point(itemBounds.Left, itemBounds.Top);
            Point topRight = new Point(itemBounds.Right, itemBounds.Top);
            Point rightTop = new Point(itemBounds.Right, itemBounds.Top);
            Point rightBottom = new Point(itemBounds.Right, itemBounds.Bottom);

            GraphicsPath path = new GraphicsPath();
            path.AddLine(leftBottom, leftTop);
            path.AddLine(leftTop, topLeft);
            path.AddLine(topLeft, topRight);
            path.AddLine(topRight, rightTop);
            path.AddLine(rightTop, rightBottom);
            //this.CloseButtonBackColor 
            //g.DrawPath(borderPen, path);
            int indexSelection = 0;

            if (this.parent is AHTabControl)
            {
                indexSelection = (this.parent as AHTabControl).ClientIndex;
            }
            SolidBrush tabBackColor = new SolidBrush(Color.FromArgb(204, 206, 219));
            if (indexSelection == drawItemInfo.Index)
            {
                tabBackColor = new SolidBrush((this.parent as AHTabControl).DockingManager.MetroColor);
            }
            else
                tabBackColor = new SolidBrush(Color.FromArgb(204, 206, 219));
            g.FillRectangle(tabBackColor, itemBounds.X, itemBounds.Y + (itemBounds.Height / 2) + 4, itemBounds.Width - 6, 23);
            g.SmoothingMode = oldSM;

            RestoreGraphicsState(g);

            g.ResetTransform();
        }
        /// <summary>
        /// Performs hit test for this renderer style.
        /// </summary>
        /// <param name="mousePosition">MousePosition for hit test.</param>
        /// <returns>if mouse is over rendered area.</returns>
        public override bool HitTest(PointF mousePosition)
        {
            RectangleF bounds = this.GetCurrentBounds();

            if (this.NeedRotateTextWhenVertical)
            {
                if (this.TabControl != null)
                {
                    Control control = this.TabControl.GetControl();
                    if (control != null)
                    {
                        using (Graphics g = control.CreateGraphics())
                        {
                            RectangleF rectMousePos = new RectangleF(mousePosition,
                                SizeF.Empty);

                            rectMousePos = TabUtils.ApplyTransform(g, this.TabAlignment,
                                rectMousePos, false);

                            mousePosition = rectMousePos.Location;
                            GraphicsState savedState = g.Save();
                            bounds = TabUtils.ApplyTransform(g, this.TabAlignment, bounds, false);
                            g.Restore(savedState);
                            g.ResetTransform();
                        }
                    }
                }
            }
            bool hit = false;
            foreach (RectangleF rect in m_tabGroupBounds)
            {
                if (rect.Contains(mousePosition))
                {
                    hit = true;
                    break;
                }
            }
            return hit;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Sets bounds for each item.
        /// </summary>
        /// <param name="g">Graphics to use.</param>
        /// <param name="tabBounds">Bounds of tab items.</param>
        protected override void SetItemBounds(Graphics g, RectangleF tabBounds)
        {
            TabGroupData groupTabData = (TabGroupData)this.TabData;

            if (boundsChanged || itemBounds.Count != groupTabData.Items.Count)
                boundsChanged = false;
            else
                return;

            if (groupTabData.Items.Count == 0)
                return;

            tabBounds = TabUtils.ApplyTransform(g, this.TabAlignment, tabBounds, true);

            itemBounds.Clear();
            m_tabGroupBounds.Clear();

            TabControlAdv tcaParentBase = TabControl as TabControlAdv;
            Debug.Assert(null != tcaParentBase);

            float fTabLeft = tabBounds.Left + 1;

            for (int i = 0; i < groupTabData.Items.Count; i++)
            {
                TabGroupItem item = (TabGroupItem)groupTabData.Items[i];
                groupTabData.Text = "";
                groupTabData.Text = item.Text;
                SizeF itemPrefSize = new SizeF();
                itemPrefSize.Height = tabBounds.Height;
                itemPrefSize.Width = (float)m_itemsWidth[i];

                groupTabData.ImageIndex = item.ImageIndex;
                groupTabData.Text = item.Text;

                itemPrefSize.Height -= 2;

                itemBounds.Add(new RectangleF(new PointF(fTabLeft, tabBounds.Top + 1),
                    new SizeF(itemPrefSize.Width, itemPrefSize.Height)));

                fTabLeft += itemPrefSize.Width;
            }

            // There is some empty space to the right, allocate it to the selected tab group item
            float freeSpace = tabBounds.Right - 1 - fTabLeft;

            if (freeSpace > 0)
            {
                freeSpace = freeSpace / itemBounds.Count;

                for (int i = 0; i < itemBounds.Count; i++)
                {
                    RectangleF rect = (RectangleF)itemBounds[i];
                    rect.Width += freeSpace;
                    itemBounds[i] = rect;
                }
            }

            foreach (RectangleF boards in itemBounds)
            {
                m_tabGroupBounds.Add(new RectangleF(
                    boards.X
                    , tabBounds.Y
                    , boards.Width
                    , tabBounds.Height));
            }
        }
        /// <summary>
        /// Gets AH caption to draw.
        /// </summary>
        /// <param name="caption">Default caption for item.</param>
        /// <returns>Caption to draw. Reformated if needed.</returns>
        protected string GetAutoHideCaption(string caption)
        {
            if ((parent != null))
            {
                AHTabControl tabControl = parent as AHTabControl;
                if (tabControl != null)
                {
                    DockingManager dockManager = tabControl.DockingManager;
                    if ((dockManager != null) && (dockManager.FullCaptionsInAutoHideMode == true))
                    {
                        return caption;
                    }
                }
            }
            return caption.Substring(0, caption.Length < 3 ? caption.Length : 3);
        }
        #endregion
    }
}
