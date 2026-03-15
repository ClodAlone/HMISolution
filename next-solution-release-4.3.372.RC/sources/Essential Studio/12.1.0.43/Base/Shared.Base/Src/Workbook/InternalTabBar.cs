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
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Security.Permissions;
using System.Security;

using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// An interface for hosting <see cref="InternalTabBar"/> objects.
	/// </summary>
	interface IInternalTabBarParent
	{
		/// <summary>
		/// Indicates whether the specified tab is being dragged.
		/// </summary>
		/// <param name="nTab">The tab index.</param>
		/// <param name="nTarget">Current target.</param>
		/// <returns>True if tab can be dropped; False otherwise.</returns>
		bool OnDraggingTab(int nTab, int nTarget);

		/// <summary>
		/// Indicates that a tab has been dragged.
		/// </summary>
		/// <param name="nTab">The tab index.</param>
		/// <param name="nTarget">Current target</param>
		void OnDraggedTab(int nTab, int nTarget);

		/// <summary>
		/// Indicates whether the scroll button has been pressed.
		/// </summary>
		/// <param name="arrowKind">Indicates which type of arrow button.</param>
		/// <param name="pixels">The number of pixels to scroll.</param>
		/// <returns>True if scrolled; False otherwise.</returns>
		bool OnScroll(ArrowType arrowKind, int pixels);

		/// <summary>
		/// Scrolling finished.
		/// </summary>
		void OnScrolled();


		/// <summary>
		/// Gets / sets the cursor to display.
		/// </summary>
		Cursor OverrideCursor 
		{
			get; 
			set;
		}
	}


	/// <summary>
	/// Provides data about a <see cref="InternalTabBar.TabMoved"/> or <see cref="InternalTabBar.TabMoving"/> events of a <see cref="InternalTabBar"/>.
	/// </summary>
	public class TabMovedEventArgs : CancelEventArgs
	{
		int tab;
		int destTab;

		/// <summary>
		/// Initializes a <see cref="TabMovedEventArgs"/>.
		/// </summary>
		/// <param name="tab">The original tab index.</param>
		/// <param name="destTab">The destination tab index.</param>
		public TabMovedEventArgs(int tab, int destTab)
		{
			this.tab = tab;
			this.destTab = destTab;
		}

		/// <summary>
		/// Gets / sets the original tab index.
		/// </summary>
		public int Tab
		{
			get
			{
				return tab;
			}
			set
			{
				tab = value;
			}
		}

		/// <summary>
		/// Gets / sets the destination tab index.
		/// </summary>
		public int DestTab
		{
			get
			{
				return destTab;
			}
			set
			{
				destTab = value;
			}
		}
	}

	/// <summary>
	/// Handles the <see cref="InternalTabBar.TabMoved"/> or <see cref="InternalTabBar.TabMoving"/> events of an <see cref="InternalTabBar"/>
	/// </summary>
	public delegate void TabMovedEventHandler(object sender, TabMovedEventArgs e);

	/// <summary>
	/// Specifies scroll behavior for a tab bar.
	/// </summary>
	public enum InternalTabBarScrollBehavior
    {
		/// <summary>
		/// Scroll pixels.
		/// </summary>
		ScrollPixels,

		/// <summary>
		/// Scroll tabs.
		/// </summary>
		ScrollTabs
    }

	/// <summary>
	///    Helper class for <see cref="ButtonBar"/>. Manages <see cref="InternalButton"/> items.
	/// </summary>
	public class InternalTabBar: Disposable
    {
		// Events
		/// <summary>
		/// Occurs when a tab has been moved.
		/// </summary>
		public event TabMovedEventHandler TabMoved;

		/// <summary>
		/// Occurs before a tab is moved.
		/// </summary>
		public event TabMovedEventHandler TabMoving;

		// Fields
		private static object startstopSemaphor = true;
		private static object elapsedSemaphor = true;
		private InternalTabCollection tabs = null;
		private Control parent = null;
		private Rectangle bounds;
		private int currentTab = -1;
		private bool dirty = true;
		private bool flatLook = false;
		private Timer repeatClickEventTimer = null;
		private int logicalWidth = 0;
		private int scrollPos = 0;
		private InternalTabBarScrollBehavior tabScrollBehavior;
		private bool isDragTabMode = false;
        private int cachedTabFolderDelta = 0;
        private DragHelper dragHelper = new DragHelper();

		/// <summary>
		/// Initializes an <see cref="InternalTabBar"/> and attaches it to a control.
		/// </summary>
		/// <param name="parent">The parent control.</param>
		public InternalTabBar(Control parent)
        {
			if (parent == null)
				throw new ArgumentNullException("parent", "Must pass valid window control.");
			this.parent = parent;
			this.tabs = new InternalTabCollection(parent as TabBar);
			WireParent();
		}
		
		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				CancelMode();
				if (this.tabs != null)
				{
					foreach (InternalButton button in this.tabs)
						if (button != null)
							button.Dispose();
                    this.tabs.Clear();
					this.tabs = null;
				}

			    UnwireParent();
			    
				if (this.repeatClickEventTimer != null)
					this.repeatClickEventTimer.Dispose();

				this.repeatClickEventTimer = null;

                if (dragHelper != null)
                {
                    dragHelper.Dispose();
                    dragHelper = null;
                }
			}

			base.Dispose( disposing );
		}

		void WireParent()
		{
			if (this.parent != null)
			{
				this.parent.MouseDown += new MouseEventHandler(OnMouseDownEvent);
				this.parent.MouseMove += new MouseEventHandler(OnMouseMoveEvent);
				this.parent.MouseUp += new MouseEventHandler(OnMouseUpEvent);
				this.parent.MouseLeave += new EventHandler(OnMouseLeaveEvent);

				ButtonBar bar = this.parent as ButtonBar;
				if (bar != null)
					bar.CancelMode += new EventHandler(OnCancelModeEvent);
			}
		}

		void UnwireParent()
		{
			if (this.parent != null)
			{
				this.parent.MouseDown -= new MouseEventHandler(OnMouseDownEvent);
				this.parent.MouseMove -= new MouseEventHandler(OnMouseMoveEvent);
				this.parent.MouseUp -= new MouseEventHandler(OnMouseUpEvent);
				this.parent.MouseLeave -= new EventHandler(OnMouseLeaveEvent);

				ButtonBar bar = this.parent as ButtonBar;
				if (bar != null)
					bar.CancelMode -= new EventHandler(OnCancelModeEvent);
        	    		
				this.parent = null;
			}
		}

		/// <summary>
		/// Called from parent control to draw this bar.
		/// </summary>
		/// <param name="g">A Graphics object.</param>
		/// <param name="delta">The delta in pixels between tabs.</param>
		public void Paint(Graphics g, int delta)
		{
			if (g == null)
				throw new ArgumentNullException("g", "Must pass valid graphics.");
			try
			{
				if (this.bounds.IsEmpty)
				{
					Debug.WriteLine("InternalButtonPar.Paint: Bounds is empty. Nothing to draw.");
					return;
				}
				if (!g.ClipBounds.IntersectsWith(this.bounds))
					return; // nothing to draw
				
				if (LogicalWidth <= 0)
					AdjustSize(true, false);

                TabBarSplitterStyle style = TabBarSplitterStyle.Default;
                if( parent is TabBar )
                {
                    style = ( parent as TabBar ).Style;
                }

                if( style == TabBarSplitterStyle.Office2007 )
                {
                    TabBar tabBar = parent as TabBar;

                    Color startColor = tabBar.Office2007ColorTable.TabBarSplitterTabBarStartColor;
                    Color endColor = tabBar.Office2007ColorTable.TabBarSplitterTabBarEndColor;
                    using( LinearGradientBrush linearBr = new LinearGradientBrush( this.bounds, startColor, endColor, LinearGradientMode.Vertical ) )
                    {
                        Blend blend = new Blend();
                        blend.Positions = new float[] { 0f, 0.12f, 0.12f, 0.25f, 0.2f, 1f };
                        blend.Factors = new float[] { 1f, 0.7f, 0.5f, 0.2f, 0f, 0.5f };
                        
                        linearBr.Blend = blend;
                        g.FillRectangle( linearBr, this.bounds );
                    }
                }

				if (this.parent.RightToLeft == RightToLeft.Yes)
				{
					for (int i = this.tabs.Count-1; i >= 0; i--)
					{
						if (!this.Tabs[i].Visible)
							continue;

						if (i == CurrentTab)
							continue; // keep this one to draw in the end 
					
						if (this.tabs[i].Bounds.IntersectsWith(this.bounds))
						{	
							Rectangle r = this.tabs[i].Bounds;
							
							this.tabs[i].Paint(g, r, this.flatLook, this.bounds);
						
							if (CurrentTab != -1 && 
								CurrentTab < this.tabs.Count && 
								this.tabs[i].Bounds.IntersectsWith(this.tabs[CurrentTab].Bounds))
								this.tabs[CurrentTab].Dirty = true;
						}
					}
				}
				else
				{
					for (int i = this.tabs.Count-1; i >= 0; i--)
					{
						if (!this.Tabs[i].Visible)
							continue;

						if (i == CurrentTab)
							continue; // keep this one to draw in the end 
					
						if (this.tabs[i].Bounds.IntersectsWith(this.bounds))
						{	
							Rectangle r = this.tabs[i].Bounds;
							this.tabs[i].Paint(g, r, this.flatLook, this.bounds);
						
							if (CurrentTab != -1 && 
								CurrentTab < this.tabs.Count && 
								this.tabs[i].Bounds.IntersectsWith(this.tabs[CurrentTab].Bounds))
								this.tabs[CurrentTab].Dirty = true;
						}
					}
				}

				if (CurrentTab != -1 && 
                    CurrentTab < this.tabs.Count &&
                    (this.dirty || this.tabs[CurrentTab].Dirty))
					this.tabs[CurrentTab].Paint(g, this.tabs[CurrentTab].Bounds, this.flatLook, this.bounds);
			}
			finally 
			{
				this.dirty = false;
			}
		}

		/// <summary>
		/// Called when a button is clicked.
		/// </summary>
		/// <param name="button">The <see cref="InternalButton"/> that was clicked.</param>
		/// <remarks>
		/// Called by OnMouseDownEvent.
		/// </remarks>
		protected virtual void OnClickedButton(InternalButton button)
		{
#if DEBUG
			if (Switches.Workbook.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(button);
#else
			;
#endif

			try
			{			
				IInternalButtonParent target = this.parent as IInternalButtonParent;
				if (target != null)
					target.OnClickedButton(button);
			}
			catch (Exception ex)
			{
				TraceUtil.TraceExceptionCatched(ex);
				if (!ExceptionManager.RaiseExceptionCatched(null, ex))
					throw;
				// something happened - let's cancel any pending actions.
				CancelMode();
				Debug.Write("Exception catched in OnClickedButton: " + ex.ToString());
			}
		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void DragTabMouseMove(int tab, int destTab)
		{
			try
			{			
				IInternalTabBarParent tabBarParent = this.parent as IInternalTabBarParent;
				if (tabBarParent != null)
                {
                    if (!tabBarParent.OnDraggingTab(tab, destTab))
                        return;
                }
                InternalTab internalTab = Tabs[tab];
                if (this.dragHelper.IsDragging)
                {
                    this.dragHelper.DoDrag(Control.MousePosition, DragDropEffects.Copy);
                    // suche: GetMousePos ...
                }
                else
                    this.dragHelper.StartDrag(internalTab.CreateBitmap(internalTab.Bounds.Size, this.FlatLook), 
                        Control.MousePosition, 
                        DragDropEffects.Copy);

				DragTarget = destTab;
			}
			catch (Exception ex)
			{
				TraceUtil.TraceExceptionCatched(ex);
				if (!ExceptionManager.RaiseExceptionCatched(null, ex))
					throw;
				// something happened - let's cancel any pending actions.
				CancelMode();
				Debug.Write("Exception catched in OnClickedButton: " + ex.ToString());
			}
		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal virtual void DragTabMouseUp(int tab, int destTab)
		{
			try
			{			
				StopTimer();
				IInternalTabBarParent tabBarParent = this.parent as IInternalTabBarParent;

				ISupportUpdating su = this.parent as ISupportUpdating;
				if (su != null)
					su.BeginUpdate();
				this.parent.SuspendLayout();
				this.dragHelper.EndDrag();
                ResetDragTarget();

				if (tabBarParent != null)
					tabBarParent.OnDraggedTab(tab, destTab);

				this.parent.ResumeLayout(false);
				if (su != null)
					su.EndUpdate();
				if (OnTabMoving(tab, destTab))
				{
					if (su != null)
						su.BeginUpdate();
					this.parent.SuspendLayout();
					MoveTab(tab, destTab);
					OnTabMoved(tab, destTab);
					RefreshCurrentTab(true);
				}
				this.parent.ResumeLayout(false);
				if (su != null)
					su.EndUpdate();
				this.parent.PerformLayout();

			}
			catch (Exception ex)
			{
				TraceUtil.TraceExceptionCatched(ex);
				if (!ExceptionManager.RaiseExceptionCatched(null, ex))
					throw;
				// something happened - let's cancel any pending actions.
				CancelMode();
				Debug.Write("Exception catched in OnClickedButton: " + ex.ToString());
			}
		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual bool OnTabMoving(int tab, int destTab)
		{
			if (TabMoving != null)
			{
				TabMovedEventArgs e = new TabMovedEventArgs(tab, destTab);
				TabMoving(this, e);
				return !e.Cancel;
			}
			return true;
		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void MoveTab(int tab, int destTab)
		{
			InternalTab sourceTab = this.tabs[tab];
			if (destTab == -1)
			{
				this.tabs.RemoveAt(tab);
				this.tabs.Add(sourceTab);
				this.currentTab = this.tabs.Count-1;
				this.AdjustSize(false, false);
			}
			else if (destTab < tab)
			{
				this.tabs.RemoveAt(tab);
				this.tabs.Insert(destTab, sourceTab);
				this.currentTab = destTab;
				this.AdjustSize(false, false);
			}
			else if (destTab > tab+1)
			{
				this.tabs.Insert(destTab, sourceTab);
				this.tabs.RemoveAt(tab);
				this.currentTab = destTab-1;
				this.AdjustSize(false, false);
			}
		}

		void OnTabMoved(int tab, int destTab)
		{
			if (TabMoved != null)
				TabMoved(this, new TabMovedEventArgs(tab, destTab));
		}

		/// <summary>
		/// Checks if mouse is over a button and returns the zero-based button index or -1.
		/// </summary>
		/// <param name="x">X-coordinate of mouse pointer.</param>
		/// <param name="y">Y-coordinate of mouse pointer.</param>
		/// <returns>Zero-based button index; -1 if not over a button.</returns>
		public int HitTest( int x, int y )
		{
            if( this.bounds.Contains( x, y ) )
            {
                if( currentTab != -1 && this.tabs[ currentTab ] != null )
                {
                    InternalTab tab = this.tabs[ currentTab ];
                    tab.Renderer.Bounds = tab.Bounds;

                    if( tab.Style == TabBarSplitterStyle.Office2007 && tab.GetTabRegion.IsVisible( x, y ) )
                    {
                        return this.currentTab;
                    }
                    if (tab.Style == TabBarSplitterStyle.Metro  && tab.GetTabRegion.IsVisible(x, y))
                    {
                        return this.currentTab;
                    }
                }
                
                for( int i = 0; i < this.tabs.Count; i++ )
                {
                    InternalTab tab = this.tabs[ i ];
                    tab.Renderer.Bounds = tab.Bounds;

                    if( tab != null && tab.Visible &&  ( tab.Style == TabBarSplitterStyle.Office2007 && tab.GetTabRegion.IsVisible( x, y ) 
                        || tab.Style == TabBarSplitterStyle.Default && tab.Bounds.Contains(x, y) || tab.Style == TabBarSplitterStyle.Metro  && tab.Bounds.Contains(x, y)))
                    {
                        return i;
                    }
                }
            }
			return -1;			
		}
	
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void AdjustSize(bool measureTabs, bool initToolTip)
		{
			int logWidth = 0;
					
			IInternalTabParent tabParent = this.parent as IInternalTabParent;
			if (tabParent != null)
			{
				Graphics g;
				Font font;
				
				tabParent.GetMeasureTools(out g, out font, out cachedTabFolderDelta);
			
				try
				{
					Rectangle rect = new Rectangle(this.bounds.Left-this.scrollPos, this.bounds.Top, 0, this.bounds.Height);
					if (parent.RightToLeft == RightToLeft.Yes)
					{
						for (int i = this.tabs.Count-1; i >= 0; i--)
						{
							InternalTab tab = Tabs[i];

							if (!tab.Visible)
								continue;

							if (measureTabs)
								tab.AdjustSize(g, font, cachedTabFolderDelta);
						
							rect.Width = tab.Size.Width;
							this.tabs[i].Bounds = rect;

							if (initToolTip)
								tab.InitToolTip(Rectangle.Intersect(rect, this.bounds));

							rect.Offset(rect.Width-cachedTabFolderDelta, 0);
							logWidth += Tabs[i].Size.Width-cachedTabFolderDelta;
						}
					}
					else
					{
						for (int i = 0; i < this.tabs.Count; i++)
						{
							InternalTab tab = Tabs[i];

							if (!tab.Visible)
								continue;

							if (measureTabs)
								tab.AdjustSize(g, font, cachedTabFolderDelta);
						
							rect.Width = tab.Size.Width;
							this.tabs[i].Bounds = rect;

							if (initToolTip)
								tab.InitToolTip(Rectangle.Intersect(rect, this.bounds));

							rect.Offset(rect.Width-cachedTabFolderDelta, 0);
							logWidth += Tabs[i].Size.Width-cachedTabFolderDelta;
						}
					}
			
					if (logWidth > 0)
						logWidth += cachedTabFolderDelta + 2;

					LogicalWidth = logWidth;
				}
				finally
				{
					tabParent.DisposeMeasureTools();
				}
			}
		}

		/// <summary>
		/// Repaints only if marked dirty.
		/// </summary>
		public void InvalidateIfDirty()
		{
			if (this.Dirty)
            {
				this.parent.Invalidate(this.bounds);
            }
		}
		
		/// <summary>
		/// Returns the index for the specified button.
		/// </summary>
		/// <param name="button">The button to search.</param>
		public int FindButton(InternalButton button)
		{
			for (int i = 0; i < this.tabs.Count; i++)
			{
				if (this.tabs[i] == button)
					return i;
			}
			
			return -1;
		}
				
		
		/// <summary>
		/// Returns the index for a button with the specified cookie.
		/// </summary>
		/// <param name="cookie">The cookie to search for.</param>
		public int FindTab(object cookie)
		{
			for (int i = 0; i < this.tabs.Count; i++)
			{
				if (this.tabs[i].Cookie == cookie)
					return i;
			}
			return -1;
		}
				
		/// <summary>
		/// Cancels current action.
		/// </summary>
		public void CancelMode()
		{
			StopTimer();
			if (dragHelper != null)
				this.dragHelper.CancelDrag();
            ResetDragTarget();
			IsDragTabMode = false;
			if (parent != null)
				this.parent.Capture = false;
			InvalidateIfDirty();
		}		
		
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void OnCancelModeEvent(object sender, EventArgs e) 
        {
            CancelMode();
        }
        
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void OnMouseDownEvent(object sender, MouseEventArgs e) 
        {
            // Cancel if another mouse button was pressed.
			if (Capture)
            {
                CancelMode();
                return;
            }
			int index = HitTest(e.X, e.Y);
			bool bControlKey = ((Control.ModifierKeys & Keys.Control) > 0);
			if (index != -1)
			{
				InternalButton button = this.tabs[index];
				if (button.Enabled)
				{
					int savedTab = CurrentTab;
					if (bControlKey)
					{
						if (CurrentTab != -1 && index != CurrentTab && CurrentTab < this.tabs.Count)
							this.tabs[CurrentTab].Checked = true;
						this.tabs[index].Checked = !this.tabs[index].Checked;
						CancelMode();
						return;
					}
					else
					{
						ResetChecked();
						this.parent.Capture = true;
					}
					OnClickedButton(button);
					InvalidateIfDirty();

					// make sure operation was not canceled ...
					if (CurrentTab != -1 && this.parent.Capture && e.Button == MouseButtons.Left)
						StartTimer();
					else
						CancelMode();
				}
			}
        }
        
		// Parent might check this to see if we process mouse messages.       
		/// <summary>
		/// Indicates whether a button is currently pressed.
		/// </summary>
		public bool Capture 
		{
			get 
			{ 
				return this.repeatClickEventTimer != null; 
			}
		}
	
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void OnMouseUpEvent(object sender, MouseEventArgs e) 
        {
			Debug.Assert(sender == this.parent, "OnMouseDownEvent: sender must be same as this.parent.");
			Debug.Assert(this.tabs != null, "Tabs are not initialized.");

			lock (elapsedSemaphor)
			{
				// Check if this.parent is processing mouse messages.
				if (this.bounds.Contains(e.X, e.Y))
				{
					int index = HitTest(e.X, e.Y);
				
					if (Capture && this.currentTab != -1)
					{
						if (index != CurrentTab)
						{
							// Mouse was dragged, draw arrow.
							DragTabMouseUp(CurrentTab, index);
						}
					}
				    
					InvalidateIfDirty();
				}
				CancelMode();
			}
        }

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void OnMouseMoveEvent(object sender, MouseEventArgs e) 
        {
			Debug.Assert(sender == this.parent, "OnMouseDownEvent: sender must be same as this.parent.");
			Debug.Assert(this.tabs != null, "Tabs are not initialized.");

			// Check if this.parent is processing mouse messages.
			int index = HitTest(e.X, e.Y);
				
			if (Capture && this.currentTab != -1)
			{
				if (index != CurrentTab)
				{
					// Mouse was dragged, draw arrow.
					DragTabMouseMove(CurrentTab, index);
				}
			}
			else if (!this.parent.Capture)
			{
				for (int i = 0; i < this.tabs.Count; i++)
					if (i != this.currentTab)
						this.tabs[i].Hovered = (i == index);
			}
					
			InvalidateIfDirty();
        }

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void OnMouseLeaveEvent(object sender, EventArgs e) 
        {
			Debug.Assert(sender == this.parent, "OnMouseDownEvent: sender must be same as this.parent.");
			Debug.Assert(this.tabs != null, "Tabs are not initialized.");
				
			ResetHovered();
			InvalidateIfDirty();
        }

        private void StartTimer()
        {
			lock(startstopSemaphor)
			{
				Debug.Assert(this.repeatClickEventTimer == null, "Oops - StartTimer called twice.");
#if DEBUG
				if (Switches.Timers.TraceVerbose)
				    TraceUtil.TraceCurrentMethodInfo("InternalTabBar.StartTimer");
#else
				;
#endif

				this.repeatClickEventTimer = new Timer();
				this.repeatClickEventTimer.Tick += new EventHandler(OnTimerElapsed);
				this.repeatClickEventTimer.Interval = 500; // 500 milliseconds
				this.repeatClickEventTimer.Enabled = true;
			}
        }

		private void StopTimer()
		{
			lock(startstopSemaphor)
			{
				if (this.repeatClickEventTimer != null)
				{
#if DEBUG
					if (Switches.Timers.TraceVerbose)
					    TraceUtil.TraceCurrentMethodInfo("InternalTabBar.StopTimer");
#else
					;
#endif

					this.repeatClickEventTimer.Tick -= new EventHandler(OnTimerElapsed);
					this.repeatClickEventTimer.Dispose();
					this.repeatClickEventTimer = null;
				}
			}
		}
		
		private void OnTimerElapsed(object source, EventArgs e)
	    {
			if (this.repeatClickEventTimer == null)
				return; // This is just the completion call - we don't want to handle that.

			if (this.parent == null)
			{
				CancelMode();
				return;
			}
				
			if (this.repeatClickEventTimer.Interval > 200)
				this.repeatClickEventTimer.Interval -= 200; // accelerate
			else if (this.repeatClickEventTimer.Interval > 25)
				this.repeatClickEventTimer.Interval -= 5; // accelerate
			
			if (this.currentTab == -1)
			{
				Debug.WriteLine("InternalTabBar.OnTimerElapsed: Unnecessary TimerEvent was fired. Timer has been stopped.");
				// invalid state
				StopTimer();
			}
			else
			{
				Point p = this.parent.PointToClient(Control.MousePosition);
				int destTab = HitTest(p.X, p.Y);
				if (destTab != -1)
					DragTabMouseMove(this.currentTab, destTab);

				if (p.Y > this.bounds.Top-15 && p.Y < this.bounds.Bottom+15)
				{
					if (p.X > this.bounds.Right-15 && p.X < this.bounds.Right+15)
					{
						Scroll(ArrowType.Next, 10);
					}
					else if (p.X > this.bounds.Left-15 && p.X < this.bounds.Left+15)
					{
						Scroll(ArrowType.Previous, 10);
					}
				}
			}
	    }

		// Scrolling Functions

		/// <overload>
		/// Scrolls a specified tab into view.
		/// </overload>
		/// <summary>
		/// Scrolls a specified tab into view.
		/// </summary>
		/// <param name="tab">The tab that should be made visible.</param>
		public void ScrollInView(InternalTab tab)
		{
			int i = FindButton(tab);
			if (i != -1)
				ScrollInView(i);
		}
		
		/// <summary>
		/// Scrolls a specified tab into view.
		/// </summary>
		/// <param name="tab">The index of the tab that should be made visible.</param>
		public void ScrollInView(int tab)
		{
			Rectangle intersectRect = Bounds;
			Rectangle tabBounds = this.tabs[tab].Bounds;
			tabBounds.Width = Math.Min(tabBounds.Width, Bounds.Width);
			intersectRect.Intersect(tabBounds);
			if (intersectRect != tabBounds)
			{
				int offset = tabBounds.Right - Bounds.Right + this.cachedTabFolderDelta;
				if (offset > 0)
				{
					if (parent.RightToLeft == RightToLeft.Yes)
						Scroll(ArrowType.Previous, offset+this.cachedTabFolderDelta);
					else
						Scroll(ArrowType.Next, offset+this.cachedTabFolderDelta);
					if (this.ScrollBehavior == InternalTabBarScrollBehavior.ScrollTabs)
                        AlignNextTab();
				}
				else
				{
					offset = Bounds.Left - tabBounds.Left;
					if (offset > 0)
					{
						if (parent.RightToLeft == RightToLeft.Yes)
							Scroll(ArrowType.Next, offset);
						else
							Scroll(ArrowType.Previous, offset);
					}
				}
			}
		}				

		/// <summary>
		/// Returns a value that indicates which buttons to show enabled. Other buttons are disabled.
		/// </summary>
        public ArrowType EnableButtonFlags
        {
			get 
			{ 
				ArrowType flags = ArrowType.None;
				
				if (parent.RightToLeft == RightToLeft.Yes)
				{
					if (ScrollPos > 0)
						flags |=  ArrowType.Next|ArrowType.Last; 
					
					if (ScrollPos < LogicalWidth-this.bounds.Width)
						flags |= ArrowType.First|ArrowType.Previous;
				}
				else
				{
					if (ScrollPos > 0)
						flags |= ArrowType.First|ArrowType.Previous;
					
					if (ScrollPos < LogicalWidth-this.bounds.Width)
						flags |=  ArrowType.Next|ArrowType.Last; 
				}	
				return flags;
			}
		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void AlignFirstTab()
		{
			int tab = HitTest(Bounds.Left+cachedTabFolderDelta, Bounds.Top+1);
			if (tab != -1)
			{
				int offset = Bounds.Left - Tabs[tab].Bounds.Left;
				if (offset > 0)
    				Scroll(ArrowType.Previous, offset, true);
			} 
		}
						
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void AlignNextTab()
		{
			int tab = HitTest(Bounds.Left+cachedTabFolderDelta, Bounds.Top+1);
			if (tab != -1 && tab < Tabs.Count)
			{
				int offset = Tabs[tab+1].Bounds.Left - Bounds.Left;
				if (offset > 0)
    				Scroll(ArrowType.Next, offset, true);
			} 
		}

		/// <overload>
		/// Scrolls the tabs in the specified direction.
		/// </overload>
		/// <summary>
		/// Scrolls the tabs in the specified direction.
		/// </summary>
		/// <param name="arrowKind">The direction to scroll.</param>
		public void ScrollTab(ArrowType arrowKind)
		{
			if (parent.RightToLeft == RightToLeft.Yes)
			{
				switch (arrowKind)
				{
					case ArrowType.First:
						arrowKind = ArrowType.Last;
						break;
					case ArrowType.Last:
						arrowKind = ArrowType.First;
						break;
					case ArrowType.Next:
						arrowKind = ArrowType.Previous;
						break;
					case ArrowType.Previous:
						arrowKind = ArrowType.Next;
						break;
				}
			}
			switch (arrowKind)
			{
				case ArrowType.First:
					if (ScrollPos != 0)
						Scroll(ArrowType.Previous, ScrollPos);
					break;
				case ArrowType.Last:
					int pos = LogicalWidth - Bounds.Width;
					if (pos > ScrollPos)
						Scroll(ArrowType.Next, pos - ScrollPos, true);
					AlignNextTab();
					break;
				case ArrowType.Next:
					AlignFirstTab();
					if (ScrollPos < LogicalWidth-Bounds.Width)
					{
						int tab = HitTest(Bounds.Left+cachedTabFolderDelta, Bounds.Top+1);
						if (tab != -1 && tab < Tabs.Count)
    						Scroll(ArrowType.Next, Tabs[tab].Bounds.Width-cachedTabFolderDelta, true);
					}
					break;
				case ArrowType.Previous:
					AlignFirstTab();
					if (ScrollPos > 0)
					{
						int tab = HitTest(Bounds.Left+this.cachedTabFolderDelta, Bounds.Top+1);
						if (tab != -1 && tab > 0)
    						Scroll(ArrowType.Previous, Tabs[tab-1].Bounds.Width-cachedTabFolderDelta);
					}
					break;
			}
		
            InvalidateIfDirty();
		}
		
		/// <summary>
		/// Overloaded. Scrolls the tabs in the specified direction with the specified number of pixels.
		/// </summary>
		/// <param name="arrowKind">The direction to scroll.</param>
		/// <param name="pixels">The pixels to scroll.</param>
		public void Scroll(ArrowType arrowKind, int pixels)
        {
            Scroll(arrowKind, pixels, false);
        }

		/// <summary>
		/// Scrolls the tabs in the specified direction with the number of pixels.
		/// </summary>
		/// <param name="arrowKind">The direction to scroll.</param>
		/// <param name="pixels">The pixels to scroll.</param>
		/// <param name="ignoreLast">Indicates whether scrolling should abort when last button is visible and you scroll further.</param>
		public void Scroll(ArrowType arrowKind, int pixels, bool ignoreLast)
		{
			if (parent.RightToLeft == RightToLeft.Yes)
			{
				switch (arrowKind)
				{
					case ArrowType.First:
						arrowKind = ArrowType.Last;
						break;
					case ArrowType.Last:
						arrowKind = ArrowType.First;
						break;
					case ArrowType.Next:
						arrowKind = ArrowType.Previous;
						break;
					case ArrowType.Previous:
						arrowKind = ArrowType.Next;
						break;
				}
			}
			int newpos = ScrollPos;
			switch (arrowKind)
			{
			case ArrowType.First:
				newpos = 0;
				break;
			case ArrowType.Last:
				int pos = LogicalWidth-Bounds.Width;
				if (ScrollPos < pos)
					newpos = pos;
				break;
			case ArrowType.Next:
                int n = (ignoreLast?0:Bounds.Width);
				if (ScrollPos < LogicalWidth-n)
					newpos = Math.Min(ScrollPos+pixels, LogicalWidth-n);
				break;
			case ArrowType.Previous:
				if (ScrollPos > 0)
					newpos = Math.Max(0, ScrollPos-pixels);
				break;
			}
			
			if (ScrollPos != newpos)
			{
				int offset = ScrollPos-newpos;

				IInternalTabBarParent tabBarParent = this.parent as IInternalTabBarParent;
				if (tabBarParent == null
                    || tabBarParent.OnScroll(arrowKind, pixels))
                {
				    try
				    {
				    	// REVIEW: Unmanaged code access optimization
				    	Rectangle clientArea = this.bounds;
				    	NativeMethods.RECT scrollArea = new NativeMethods.RECT(clientArea); //this.bounds.Left+offset, this.bounds.Top, offset, this.bounds.Height);
				    	NativeMethods.RECT clipArea = new NativeMethods.RECT(clientArea); 
				    	this.parent.Update();
				    	this.dirty = false;
				    	ScrollPos = newpos;
				    	bool b = NativeMethods.ScrollWindow(this.parent.Handle, offset, 0, ref scrollArea, ref clipArea);
				    }
				    catch (SecurityException)
				    {
				    	// Unsafe code could not execute, so let's just redraw.
				    	ScrollPos = newpos;
				    	this.parent.Invalidate(this.bounds);
				    }

				    this.dirty = true;
				    this.parent.Update();

				    if (tabBarParent != null)
                        tabBarParent.OnScrolled();

                    // Reinitialize tooltips
                    ResetToolTips();
                    InitToolTips();
                }
            }
		}

		// The button array.        

		/// <summary>
		/// Gets / sets the button list.
		/// </summary>
		public InternalTabCollection Tabs 
		{
			get 
			{ 
				return this.tabs; 
			}
			set 
			{ 
				CancelMode();
				this.tabs = value; 
				this.dirty = true;
			}
		}

		/// <summary>
		/// Gets / sets the boundaries of this bar.
		/// </summary>
		public Rectangle Bounds 
		{
			get 
			{ 
				return this.bounds; 
			}
			set 
			{ 
				if (value.Width >= 0 && value.Height >= 0 && this.bounds != value)
				{
					this.bounds = value; 
					this.dirty = true; 
					if (parent.RightToLeft == RightToLeft.Yes)
					{
						ScrollPos = LogicalWidth-Bounds.Width;
						this.AdjustSize(true, true);
						this.RefreshCurrentTab(true);
					}
				}
			}
		}
        

		/// <summary>
		/// Indicates the flat look status for buttons.
		/// </summary>
		public bool FlatLook 
		{
			get 
			{ 
				return this.flatLook; 
			}
			set 
			{ 
				this.flatLook = value; 
				this.dirty = true; 
			}
		}

		/// <summary>
		/// Initializes ToolTips boundaries.
		/// </summary>
		public void InitToolTips()
        {
            AdjustSize(false, true);
        }

		/// <summary>
		/// Reinitializes and hides ToolTips.
		/// </summary>
		public void ResetToolTips()
		{
			if (this.tabs != null) 
                foreach (InternalButton button in this.tabs)
                    if (button != null)
                        button.ResetToolTip();
		}

		/// <summary>
		/// Indicates whether any button is dirty or sets all buttons dirty.
		/// </summary>
		public bool Dirty
        {
			// Check if this bar or any button is this.dirty.
			get
			{
				if (this.dirty)
					return true;
				else
				{
					if (this.tabs != null) 
                        foreach (InternalButton button in this.tabs)
                        {
                            if (button != null && button.Dirty)
                                return true;
                        }
					return false;
				}
			}
			// Mark this bar this.dirty or reset all this.tabs.
			set
			{
				this.dirty = value;
                if (this.tabs != null) 
                    foreach (InternalButton button in this.tabs)
                        if (button != null)
                            button.Dirty = value;
            }
		}

		/// <summary>
		/// Indicates whether any button is enabled or sets all buttons enabled / disabled.
		/// </summary>
		public bool Enabled
        {
			// Check if any button is Enabled.
			get
			{
				if (this.tabs != null) 
                    foreach (InternalButton button in this.tabs)
                    {
                        if (button != null && button.Enabled)
                            return true;
                    }
				return false;
			}
			// Apply value to all this.tabs.
			set
			{
				if (this.tabs != null) 
                    foreach (InternalButton button in this.tabs)
                        if (button != null)
                            button.Enabled = value;
					
				if (Dirty)
					CancelMode();
			}
		}
		
		/// <summary>
		/// Indicates whether any button is in hovered state.
		/// </summary>
		public bool Hovered
        {
			// Check if any button is Hovered.
			get
			{
				if (this.tabs != null) 
                    foreach (InternalButton button in this.tabs)
                    {
                        if (button != null && button.Hovered)
                            return true;
                    }
				return false;
			}
		}

		/// <summary>
		/// Resets hovered state for all buttons.
		/// </summary>
		public void ResetHovered()
		{
			if (this.tabs != null) 
                foreach (InternalButton button in this.tabs)
                    if (button != null)
                        button.Hovered = false;
		}

		/// <summary>
		/// Indicates whether any button is in pushed state.
		/// </summary>
		public bool Pushed
        {
			// Check if any button is pushed.
			get
			{
				if (this.tabs != null) 
                    foreach (InternalButton button in this.tabs)
                    {
                        if (button != null && button.Pushed)
                            return true;
                    }
				return false;
			}
		}

		/// <summary>
		/// Resets pushed state for all buttons.
		/// </summary>
		public void ResetPushed()
		{
			if (this.tabs != null) 
                foreach (InternalButton button in this.tabs)
                    if (button != null)
                        button.Pushed = false;
		}

		/// <summary>
		/// Indicates whether any button is in checked state.
		/// </summary>
		public bool Checked
        {
			// Check if any button is checked.
			get
			{
				if (this.tabs != null) 
                    foreach (InternalButton button in this.tabs)
                    {
                        if (button != null && button.Checked)
                            return true;
                    }
				return false;
			}
		}

		/// <summary>
		/// Resets checked state for all buttons.
		/// </summary>
		public void ResetChecked()
		{
			if (this.tabs != null) 
                foreach (InternalButton button in this.tabs)
                    if (button != null)
                        button.Checked = false;
		}

		/// <summary>
		/// Indicates whether any button is in DragTarget state or sets DragTarget state for the specified index.
		/// </summary>
		public int DragTarget
        {
			// Check if any button is DragTarget.
			get
			{
				if (this.tabs != null)
                {
                    for (int i = 0; i < this.tabs.Count; i++)
                    {
                        if (this.tabs[i] != null && this.tabs[i].DragTarget)
                            return i;
                    }
                }
				return -1;
			}
			set
			{
                if (this.tabs != null && value != DragTarget)
                {
                    for (int i = 0; i < this.tabs.Count; i++)
                    {
                        if (this.tabs[i] != null)
                            this.tabs[i].DragTarget = (i == value);
                    }
                    InvalidateIfDirty();
                }
			}
		}

		/// <summary>
		/// Resets DragTarget state for all buttons.
		/// </summary>
		public void ResetDragTarget()
		{
			if (this.tabs != null) 
                foreach (InternalButton button in this.tabs)
                    if (button != null)
                        button.DragTarget = false;
		}

		/// <summary>
		/// Gets / sets the scroll behavior of this tab bar: pixel or tabs.
		/// </summary>
		public InternalTabBarScrollBehavior ScrollBehavior 
		{
			get 
			{ 
				return this.tabScrollBehavior; 
			}
			set 
			{ 
				this.tabScrollBehavior = value; 
				AlignFirstTab();
			}
		}
		
		/// <summary>
		/// Gets / sets the current tab.
		/// </summary>
		public int CurrentTab
		{
			get 
			{
				return this.currentTab; 
			}
			set 
			{
				if (this.currentTab != value)
				{
					this.currentTab = value;
					RefreshCurrentTab(true);
				}
			}
		}

		/// <summary>
		/// Refreshes the current tab and optionally scrolls it into view.
		/// </summary>
		/// <param name="scroll">True if current tab should be scrolled into view.</param>
		public void RefreshCurrentTab(bool scroll)
		{
			ResetPushed();
			if (this.currentTab >= 0 && this.currentTab < this.tabs.Count)
			{
				this.tabs[this.currentTab].Pushed = true;
				if (scroll)
					ScrollInView(this.tabs[this.currentTab]);
			}
		}

		/// <summary>
		/// Gets / sets the total logical width of this button bar including all buttons. If the logical
		/// width is greater than the actual width, the tab bar lets the user scroll it.
		/// </summary>
		public int LogicalWidth 
        {
            get 
            { 
                return this.logicalWidth; 
            }
            set 
            { 
                this.logicalWidth = value; 
                this.dirty = true; 
            }
        }
		
		/// <summary>
		/// Gets / sets the current scroll position.
		/// </summary>
        public int ScrollPos 
        {
            get 
            { 
                return this.scrollPos; 
            }
            set 
            { 
                this.scrollPos = value; 
                this.dirty = true; 
            }
		}
		
		/// <summary>
		/// Indicates whether the user is in the process of dragging a tab.
		/// </summary>
		public bool IsDragTabMode 
		{
			get 
			{
				return this.isDragTabMode; 
			}
			set 
			{
				if (this.isDragTabMode != value)
				{
					this.isDragTabMode = value; 
					IInternalTabBarParent tabBarParent = this.parent as IInternalTabBarParent;
					if (tabBarParent != null)
					{
						if (this.isDragTabMode )
							tabBarParent.OverrideCursor = Cursors.Hand;
						else
							tabBarParent.OverrideCursor = null;
					}
				}
			}
		}
	}
}
