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
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using Syncfusion.Windows.Forms.Grid;
using Microsoft.Win32;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public enum MoveHint
	{
		moveNext,
		movePrevious,
		moveFirst,
		moveLast
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class XPMenuGridFactory
	{
		[ThreadStatic()]
		static ArrayList _standByGrids;
		[ThreadStatic()]
		static ArrayList _deployedGrids;
		static Type menuGridType;
		static System.Reflection.ConstructorInfo menuGridConstructorInfo = null;

		internal static ArrayList StandByGrids
		{
			get
			{
				if(_standByGrids == null)
				{
					_standByGrids = new ArrayList(2);
					// 1 Stand by grid
					_standByGrids.Add(CreateMenuGridInstance());
				}
				return _standByGrids;
			}
		}
		static ArrayList DeployedGrids
		{
			get
			{
				if(_deployedGrids == null)
				{
					_deployedGrids = new ArrayList(2);
				}
				return _deployedGrids;
			}
		}
		private static bool menusJited = false;

		/// <summary>
		/// Initializes menus for faster dropdown-the first time.
		/// </summary>
		[Documentation.DocumentationExclude()]
		public static void InitMenus()
		{

#if SINGLE_DLL_BUILD
#else
			// Trying to load the menus in a separate thread. But, this code has the following bugs:
			// 1) Creating a new instance of the MenuGrid, throws the "ole drag-drop" exception.
			// 2) The created menus should be destroyed as they cannot be used by the main thread
			// 3) The InitMenusInternal code is not thread-safe.
//			Syncfusion.Windows.EnclosureInfo ei = new Syncfusion.Windows.EnclosureInfo();
//			if(ei.CaseType == Syncfusion.Windows.EnclosureInfo.Type.Laptop
//				|| ei.CaseType == Syncfusion.Windows.EnclosureInfo.Type.Notebook
//				|| ei.CaseType == Syncfusion.Windows.EnclosureInfo.Type.Portable
//				|| ei.CaseType == Syncfusion.Windows.EnclosureInfo.Type.SubNotebook)
//			{
//				// Don't load the grid right here. Do it in a separate thread:
//				MethodInvoker mi = new MethodInvoker(InitMenusInternal);
//				mi.BeginInvoke(null, null);
//
//				return;
//			}
#endif			

			InitMenusInternal();
		}

		private static void InitMenusInternal()
		{
			if(menusJited == false)
			{
				menusJited = true;
				// Create a MenuGrid.
				MenuGrid menuGrid = XPMenuGridFactory.GetMenuGridToDeploy();

				menuGrid.JitCode();

				// Call this manually, since we are not actually showing the menu.
				XPMenuGridFactory.ReleaseMenuGrid(menuGrid);
				
				// This JITs more code and shows the menus faster, the first time.
				// But we want to avoid showing a menu for now, as this logic will 
				// not work in dual monitors.

				// Show it somewhere to kick-in Jiting.
//				ParentBarItem item = new ParentBarItem();
//				item.Items.Add(new BarItem("SF"));
//
//				//Show the menu
//				MenuGridHost host = menuGrid.Host;
//				host.preventAdjustLocation = true;
//				Rectangle screen = Screen.PrimaryScreen.Bounds;
//				Point pt = new Point(screen.Right - 3, screen.Bottom - 3);
//				menuGrid.Show(item, pt, null, false);
//				menuGrid.HidePopup(PopupCloseType.Canceled);
//				host.preventAdjustLocation = false;
			}
		}
		/// <summary>
		/// Gets / sets a custom type representing the drop-down menus.
		/// </summary>
		/// <value>A type deriving from <b>MenuGrid</b>. Default is null.</value>
		/// <remarks>Specify null to make the framework use the default type.</remarks>
		public static Type MenuGridType
		{
			get{return menuGridType;}
			set
			{
				if(menuGridType != value)
				{
					menuGridType = value;

					if(menuGridType != null)
					{
						// Check if the new Type derives from MenuGrid
						Type baseType = typeof(MenuGrid);
						if(!baseType.IsAssignableFrom(value))
							throw new ArgumentException("Type: " + value.FullName + " should derive from " + baseType.FullName);

						// Check if the new Type has a default constructor
						System.Reflection.ConstructorInfo constructorInfo;
						constructorInfo = value.GetConstructor(new System.Type[]{});
						if(constructorInfo == null)
							throw new ArgumentException("Type: " + value.FullName + " should have a default constructor.");

						menuGridConstructorInfo = constructorInfo;

						menuGridType = value;
					}
					else
						menuGridConstructorInfo = null;

					ReleaseAllGrids();
				}
			}
		}

		static XPMenuGridFactory()
		{
			SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(System_UserPreferenceChanged);
		}

		static void System_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
		{
			// The menu's size might have changed, for example.
			ReleaseAllGrids();
		}

		static MenuGrid CreateMenuGridInstance()
		{
			if(menuGridConstructorInfo != null)
				return menuGridConstructorInfo.Invoke(new object[]{}) as MenuGrid;
			else
				return new MenuGrid();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public static MenuGrid GetMenuGridToDeploy()
		{
			VerifyMenuStates();

			if(XPMenuGridFactory.StandByGrids.Count == 0)
				StandByGrids.Add(CreateMenuGridInstance());

			MenuGrid gridToDeploy = (MenuGrid)StandByGrids[StandByGrids.Count - 1];
			StandByGrids.RemoveAt(StandByGrids.Count - 1);
			DeployedGrids.Add(gridToDeploy);

			//Trace.WriteLine("No. of Grids deployed: " + deployedGrids.Count);
			return gridToDeploy;
	    }	

		internal static void VerifyMenuStates()
		{
			if(XPMenuGridFactory._standByGrids != null && XPMenuGridFactory._standByGrids.Count > 0)
			{
				MenuGrid gridToDeploy = (MenuGrid)_standByGrids[_standByGrids.Count - 1];
				// Sometimes the handle gets destroyed in the designer when deleting a component!
				// in which case give up and recreate menus.
				if(!gridToDeploy.IsHandleCreated)
					ReleaseAllGrids();
			}
		}

		internal static void ReleaseMenuGrid(MenuGrid menuGrid)
		{
			if(menuGrid != null)
			{
				menuGrid.CurrentCell.Deactivate(true);
				menuGrid.Model.Clear(true);
				menuGrid.Model.RowCount = 0;
				DeployedGrids.Remove(menuGrid);
				StandByGrids.Add(menuGrid);
			}
		}
		public static void ReleaseAllGrids()
		{
			StandByGrids.Clear();
		}
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IDndTrackingControl
	{
		Control GetControl();
		// Special case: If this returns null and beforeOrAfter == true, then will insert at the beginning of the list
		BarItem HitTestBarItem(Point mousePosition, ref bool beforeOrAfter);
		Rectangle GetCueRect(BarItem barItem, bool beforeOrAfter);
		bool DesignMode{get;}
		bool CanDropItem(BarItem item);
	}

	/// <summary>
	/// The data passed to the drag drop handlers while dragging a BarItem during 
	/// User Customization.
	/// </summary>
	public class BarItemDndData
	{
		private BarItem itemDragged;
		private IBarItemContainer itemParent;
		private IBarItemContainer destination = null;
		public BarItemDndData(BarItem itemDragged, IBarItemContainer itemParent)
		{
			this.itemDragged = itemDragged;
			this.itemParent = itemParent;
		}
		public BarItem ItemDragged{get{return this.itemDragged;}}
		public IBarItemContainer ItemParent{get{return this.itemParent;}}
		public IBarItemContainer ItemDestination
		{
			get
			{
				return this.destination;
			}
			set
			{
				this.destination = value;
			}
		}
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class CustomizationDndHelper : IDisposable
	{
		static readonly bool Before = true;
//		static readonly bool After = false;
		private bool dragging = false;
		private bool allowRecord = true;
		IBarItemContainer parentItem;
		IDndTrackingControl control;

		public CustomizationDndHelper(IDndTrackingControl control)
		{
			this.control = control;
		}

		~CustomizationDndHelper()
		{
			if(this.control != null)
			{
				this.Reset();
				this.control = null;
			}
		}

        public void Dispose()
        {
            if (this.control != null)
            {
                this.Reset();
                this.control = null;
            }
        }
        
		public IBarItemContainer ParentItem
		{
			get{return this.parentItem;}
			set{this.parentItem = value;}
		}

		public bool AllowRecord
		{
			get{return this.allowRecord;}
			set{this.allowRecord = value;}
		}

        public void Reset()
		{
			this.CueRect = Rectangle.Empty;
			this.barItemLastHit = null;
			this.latestBeforeOrAfterState = false;
		}

		public bool Dragging
		{
			get{return this.dragging;}
			set
			{
				this.dragging = value;
				if(value == false)
					this.Reset();
			}
		}

		#region DNDOPS
		internal void ComputeDropEffect(DragEventArgs drgevent, BarItem draggedItem,
			ref BarItem hitItem, ref bool beforeOrAfter)
		{
			if(!this.control.CanDropItem(draggedItem))
			{
				drgevent.Effect = DragDropEffects.None;
				return;
			}

			// Make sure the hit item is not a separator
			hitItem = this.control.HitTestBarItem(
				this.control.GetControl().PointToClient(new Point(drgevent.X, drgevent.Y)), ref beforeOrAfter);

			this.GetUpdatedHitItem(ref hitItem, ref beforeOrAfter);

			// Special Case: hitItem == null and beforeOrAfter == true means, insert at the top of the list
			if(draggedItem != null && this.CanDropItem(draggedItem, drgevent)
				&& ((hitItem == null && beforeOrAfter) || (hitItem != null && hitItem.Text != "-")))
				drgevent.Effect = this.GetDropEffect(drgevent);
			else
				drgevent.Effect = DragDropEffects.None;
		}

		public void OnDragOver(DragEventArgs drgevent)
		{
			BarItemDndData dndData = drgevent.Data.GetData(typeof(BarItem).FullName) as BarItemDndData;
			BarItem draggedItem = dndData.ItemDragged;

			BarItem hitItem = null;
			bool beforeOrAfter = false;
			this.ComputeDropEffect(drgevent, draggedItem, ref hitItem, ref beforeOrAfter);

			this.UpdateInsertionCue(drgevent.Effect, hitItem, beforeOrAfter);
		}

		protected virtual void GetUpdatedHitItem(ref BarItem hitItem, ref bool beforeOrAfter)
		{
			// If no item hit, then use the previous hit item
			if(hitItem == null && beforeOrAfter == false)
			{
				// If the previous hit state is a valid state, then use that instead
				if(this.barItemLastHit != null || this.latestBeforeOrAfterState == true)
				{
					hitItem = this.barItemLastHit;
					beforeOrAfter = this.latestBeforeOrAfterState;
				}
			}
		}

		private DragDropEffects GetDropEffect(DragEventArgs drgevent)
		{
			//Trace.WriteLine("KeyState: " + drgevent.KeyState.ToString() + " Keys.Control: " + Keys.Control.ToString());

			int controlKey = 0x0008; //MK_CONTROL
			if((drgevent.AllowedEffect & DragDropEffects.Move) == 0
				|| (drgevent.KeyState & controlKey) > 0)
				return DragDropEffects.Copy;
			else
				return DragDropEffects.Move;
		}

		public void OnDragDrop(System.Windows.Forms.DragEventArgs e)
		{
			BarItemDndData dndData = e.Data.GetData(typeof(BarItem).FullName) as BarItemDndData;
			BarItem droppedItem = null != dndData ? dndData.ItemDragged : null;

			BarItem hitItem = null;
			bool beforeOrAfter = false;
			this.ComputeDropEffect(e, droppedItem, ref hitItem, ref beforeOrAfter);

			if(droppedItem != null && e.Effect != DragDropEffects.None)//this.CanDropItem(droppedItem, e))
			{
				if(droppedItem is NewMenuItem)
				{
					this.InsertNewItem(droppedItem as NewMenuItem, e);
					return;
				}

				dndData.ItemDestination = this.parentItem;

                IPopupParent popupParent = this.control as IPopupParent;
                if (null != this.control && popupParent != null && popupParent.IsRightToLeft)
				{
					beforeOrAfter = !beforeOrAfter;
				}

				this.InsertItem(hitItem, droppedItem, beforeOrAfter);
			}
		}

		public virtual void InsertNewItem(NewMenuItem newMenuItem, DragEventArgs e)
		{
			// Compose a new name for the new item.
			string newName = SR.GetString(SR.NewMenu, this.parentItem.Manager.MainFrameBarManager);
			string id = newName;

			while(!newMenuItem.Manager.Items.IsValidItemID(null, id))
				id = IDGenerator.GetNextID(id);

			if(this.AllowRecord && !this.control.DesignMode)
			{
				MainFrameBarManager mainManager = this.parentItem.Manager.MainFrameBarManager;
				// Record creation of this new parent menu item
				mainManager.RecordAddCustomParentMenu(id);

				// Insert this new item into this parentItem
				BarItemID barID = new BarItemID( id, BarManager.GetFormTypeName(mainManager) );
				ParentBarItem droppedItem = mainManager.GetBarItemFromBarItemID(barID) as ParentBarItem;

				if(droppedItem != null)
				{
					bool beforeOrAfter = Before;
					BarItem hitItem = this.control.HitTestBarItem(
						this.control.GetControl().PointToClient(new Point(e.X, e.Y)), ref beforeOrAfter);

					BarItemDndData dndData = e.Data.GetData(typeof(BarItem).FullName) as BarItemDndData;
					dndData.ItemDestination = this.parentItem;
					this.InsertItem(hitItem, droppedItem, beforeOrAfter);
				}
			}
		}

		public void ResetItem(BarItem itemToReset, bool messageboxshow)
		{
			if(this.parentItem.Manager.MainFrameBarManager != null)
			{
				messageboxshow &= !this.parentItem.Manager.MainFrameBarManager.ResetBarItem(itemToReset);

				this.parentItem.Manager.MainFrameBarManager.OnResetBarItem( itemToReset );
				if(messageboxshow)
					MessageBox.Show(SR.GetString(SR.NotifyCustomizationReset, this), SR.GetString(SR.NotifyCustomizationResetTitle, this), MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		public virtual void RemoveItem( BarItem itemToRemove )
		{
			BarManager manager = itemToRemove.Manager;

			if( this.ParentItem != null )
			{
				manager = this.ParentItem.Manager;
			}

			bool bRecord = false;

			if( manager != null )
			{
				MainFrameBarManager mainMgr = manager.MainFrameBarManager;
				bRecord = mainMgr != null && !mainMgr.DesignMode && mainMgr.NeedSaveCustomData;
			}

			this.RemoveItem( itemToRemove, bRecord );
		}

		public virtual void RemoveItem( BarItem itemToRemove, bool record )
		{
			int curIndex = this.parentItem.Items.IndexOf( itemToRemove );
			if( curIndex == -1 )
				return;

			if( this.AllowRecord && record && ((this.control != null && !this.control.DesignMode) || !this.parentItem.Manager.DesignMode) )
			{
				if( itemToRemove is CustomParentMenuItem )
				{
					CustomParentMenuItem customItem = ((CustomParentMenuItem)itemToRemove);
					customItem.RefrenceCount--;
					// If reference count is 0 then just Remove it from the recorded list.
					if( customItem.RefrenceCount == 0 )
					{
						record = false;
						this.parentItem.Manager.MainFrameBarManager.RemoveCustomParentMenu( customItem );
					}
				}

				if( record )
				{
					this.parentItem.Manager.MainFrameBarManager.RecordRemove( itemToRemove, this.parentItem );
					return;
				}
			}

			if( parentItem != null )
			{
				// Is this a group Beginner?
				bool groupBeginner = parentItem.IsGroupBeginning( itemToRemove );
				if( groupBeginner )
				{
					parentItem.RemoveGroupAt( itemToRemove );
					// Get next item
					BarItem nextItem = null;
					curIndex = this.parentItem.Items.IndexOf( itemToRemove );
					for( int i = curIndex + 1; i < parentItem.Items.Count; i++ )
					{
						BarItem item = this.parentItem.Items[i] as BarItem;
						if( item.Text != "-" )
						{
							nextItem = item;
							break;
						}
					}
					// Begin group at the adjacent item
                    if (nextItem != null&& this.parentItem.IsGroupBeginning(nextItem ))
                        this.parentItem.BeginGroupAt(nextItem);
				}

				parentItem.RemoveItem( itemToRemove );
			}
		}

        internal void RemoveItem(BarItem hitItem, BarItem itemToRemove, bool record)
        {
            int curIndex = this.parentItem.Items.IndexOf(itemToRemove);
            if (curIndex == -1)
                return;

            if (hitItem == null)
            {
                this.RemoveItem(itemToRemove, record);
                return;
            }

            int hitIndex = this.parentItem.Items.IndexOf(hitItem);

            if( hitIndex == curIndex - 1 ||
				(hitIndex == curIndex - 2 && this.parentItem.Items[ curIndex - 1 ] is SeparatorItem) )
            {
                this.RemoveItem(itemToRemove, record);
            }
        }

		public virtual void InsertItem(BarItem hitItem, BarItem droppedItem, bool beforeOrAfter)
		{
			this.InsertItem(hitItem, droppedItem, beforeOrAfter, true);
		}

		public virtual void InsertItem(BarItem hitItem, BarItem droppedItem, bool beforeOrAfter, bool record)
		{
			if(hitItem == droppedItem)
				return;

			if(this.AllowRecord && record && !this.control.DesignMode)
			{
				this.parentItem.Manager.MainFrameBarManager.RecordInsert(
					droppedItem, hitItem, this.parentItem, beforeOrAfter);
				return;
			}

			if(droppedItem is CustomParentMenuItem)
				((CustomParentMenuItem)droppedItem).RefrenceCount++;

			int hitIndex = -1;
			if(hitItem != null)
			{
				hitIndex = this.parentItem.Items.IndexOf(hitItem);
				if(hitIndex == -1)
					// The adjacent item is not available now so don't know where to insert.
					return;
			}

			this.parentItem.Items.SuspendEvents();

			bool alreadyInList = this.parentItem.Items.IndexOf(droppedItem) != -1;

			//need this to save parentItem, which is removed after deleting droppedItem. 
            IBarItemContainer savedParent = this.parentItem;

			bool group_begining = alreadyInList && savedParent.IsGroupBeginning(droppedItem);

            // If the droppedItem is already present elsewhere, remove it
            this.RemoveItem( droppedItem, alreadyInList && droppedItem.Manager.Customizing ? true : false );

            this.parentItem = savedParent;

			if( droppedItem.Manager.Customizing )
			{
				int itemIdx = savedParent.Items.IndexOf( droppedItem );

				if( itemIdx >= 0 )
				{
					bool groupAt = savedParent.IsGroupBeginning( droppedItem );

					if( groupAt )
					{
						savedParent.RemoveGroupAt( droppedItem );

						--itemIdx;
						savedParent.Items.RemoveAt( itemIdx );

						if( itemIdx < savedParent.Items.Count )
						{
							savedParent.BeginGroupAt( savedParent.Items[itemIdx] );
						}
					}
					else
					{
						savedParent.Items.RemoveAt( itemIdx );
					} 
				}
			}

			// Recalculate the hitIndex after removing item above.
			hitIndex = this.parentItem.Items.IndexOf(hitItem);

			if(hitItem == null && beforeOrAfter)
				// Insert at top of the list
				this.parentItem.Items.Insert(0, droppedItem);
			else if(beforeOrAfter == Before)
			{
				bool groupBeginner = false;
				// If the hitItem is the beginning of a group, not anymore.
				if(this.parentItem.IsGroupBeginning(hitItem))
				{
					this.parentItem.RemoveGroupAt(hitItem);
					// Instead begin the group before the dropped item
					groupBeginner = true;

                    if (hitIndex >= this.parentItem.Items.Count)
					    --hitIndex;
				}

				hitIndex = this.SkipPastMergedItems(hitIndex, beforeOrAfter);
				
				// Insert it now
				this.parentItem.Items.Insert(hitIndex, droppedItem);

				if (groupBeginner)
				{
					this.parentItem.BeginGroupAt(droppedItem);
				}
			}
			else
			{
				// Insert after hit item.
				if (hitItem != null)
				{
					hitIndex = this.SkipPastMergedItems(hitIndex, beforeOrAfter);
					hitIndex++;
				}
				else
				{
                    hitIndex = this.parentItem.Items.FindMergePosition(droppedItem.MergeOrder);
				}

				this.parentItem.Items.Insert(hitIndex, droppedItem);

				if (group_begining)
				{
					this.parentItem.BeginGroupAt(droppedItem);
				}
			}
			this.parentItem.Items.ResumeEvents(true);

			// This is necessary because users sometimes listen to what items get added to the bar by the
			// user during customization
			if(!alreadyInList)
				this.parentItem.Items.RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, droppedItem));
		}

		private int SkipPastMergedItems(int index, bool beforeOrAfter)
		{
			if(index == -1)
				return -1;
			int newIndex = index;
			BarItem item = this.parentItem.Items[index] as BarItem;

			string sourceMergeId = MergedStateTracker.GetMergeId(item);

			if(beforeOrAfter == Before)
			{
				for(int i = index-1; i >= 0; i--)
				{
					BarItem nextItem = this.parentItem.Items[i] as BarItem;
					if(MergedStateTracker.GetMergeId(nextItem) == sourceMergeId)
					{
						newIndex = i;
					}
				}				
			}
			else
			{
				for(int i = index+1; i < this.parentItem.Items.Count; i++)
				{
					BarItem nextItem = this.parentItem.Items[i] as BarItem;
					if(MergedStateTracker.GetMergeId(nextItem) == sourceMergeId)
					{
						newIndex = i;
					}
				}
			}
			return newIndex;
		}

		protected virtual bool CanDropItem(BarItem item, DragEventArgs e)
		{
			if(item != this.parentItem)
			{
				CanDragDropEventArgs args = new CanDragDropEventArgs(this.parentItem, false);
				item.OnCanDragDrop(args);
				if(!args.Cancel)
				{
					// If dragging within the source, then Move should be allowed
					if(this.dragging)
					{
						if((e.AllowedEffect & DragDropEffects.Move) > 0)
							return true;
					}
					else
						return true;
				}
			}
			return false;
		}
		#endregion DNDOPS

		#region DRAWING
		private Rectangle cueRect = Rectangle.Empty;
		private BarItem barItemLastHit;
		private bool latestBeforeOrAfterState;

		protected Rectangle CueRect
		{
			get{return this.cueRect;}
			set
			{
				if(this.cueRect != value)
				{
					if(this.cueRect != Rectangle.Empty)
						this.control.GetControl().Invalidate(this.cueRect, false);

					this.cueRect = value;

					if(cueRect != Rectangle.Empty)
						this.control.GetControl().Invalidate(this.cueRect, false);
				}
			}
		}
		protected void UpdateInsertionCue(DragDropEffects effect, BarItem barItem, bool beforeOrAfter)
		{
			if(effect == DragDropEffects.None)
				this.CueRect = Rectangle.Empty;
			else
			{
				if(barItem != this.barItemLastHit
					|| beforeOrAfter != this.latestBeforeOrAfterState)
				{
					this.CueRect = this.control.GetCueRect(barItem, beforeOrAfter);
				}
				this.barItemLastHit = barItem;
				this.latestBeforeOrAfterState = beforeOrAfter;
			}
		}

		public void PaintCueCursor(Graphics g)
		{
			if(this.control == null
				|| this.parentItem == null)
				return;

			if(this.cueRect != Rectangle.Empty 
				&& g.ClipBounds.IntersectsWith(this.cueRect))
			{
				bool horizontal = this.cueRect.Width > this.cueRect.Height ? true : false;
				if(horizontal)
					DrawLeftAndRightCorners(g);
				else
					DrawTopAndBottomCorners(g);

				// Fill interior line
				Rectangle interiorLine = Rectangle.Empty;
				if(horizontal)
					interiorLine = new Rectangle(this.cueRect.Left + 3, this.cueRect.Top + 2,
						this.cueRect.Width - 6, 3);
				else
					interiorLine = new Rectangle(this.cueRect.Left + 2, this.cueRect.Top + 3,
						3, this.cueRect.Height - 6);
                using (Brush vspen = new SolidBrush(SystemColors.ControlText))
                {
                    g.FillRectangle(vspen,interiorLine);
                }
			}
		}
		private void DrawTopAndBottomCorners(Graphics g)
		{
			Pen pen = new Pen(SystemColors.ControlText);

			// Top corner
			int top = this.cueRect.Top;
			int left = this.cueRect.Left;
			g.DrawLine(pen, new Point(left, top), new Point(left + 6, top));
			g.DrawLine(pen, new Point(left+1, top + 1), new Point(left+5, top + 1));
			g.DrawLine(pen, new Point(left+2, top + 2), new Point(left+4, top + 2));

			// bottom corner
			int bottom = this.cueRect.Bottom - 1;
			g.DrawLine(pen, new Point(left, bottom), new Point(left + 6, bottom));
			g.DrawLine(pen, new Point(left+1, bottom - 1), new Point(left+5, bottom - 1));
			g.DrawLine(pen, new Point(left+2, bottom - 2), new Point(left+4, bottom - 2));
            pen.Dispose();
		}

		private void DrawLeftAndRightCorners(Graphics g)
		{
			Pen pen = new Pen(SystemColors.ControlText, 1);

			// Left corner
			int top = this.cueRect.Top;
			int left = this.cueRect.Left;
			g.DrawLine(pen, new Point(left, top), new Point(left, top + 7));
			g.DrawLine(pen, new Point(left+1, top + 1), new Point(left+1, top + 6));
			g.DrawLine(pen, new Point(left+2, top + 2), new Point(left+2, top + 5));

			// Right corner
			int right = this.cueRect.Right - 1;
			g.DrawLine(pen, new Point(right, top), new Point(right, top + 7));
			g.DrawLine(pen, new Point(right-1, top + 1), new Point(right-1, top + 6));
			g.DrawLine(pen, new Point(right-2, top + 2), new Point(right-2, top + 5));
            pen.Dispose();
		}
		#endregion DRAWING
	}

	internal class MergeHelper
	{
		public static void MergeItems(IBarItemContainer dest, IBarItemContainer src)
		{
			if(src == dest)
				throw new ArgumentException("Trying to Merge 2 same instances", "src");
			for(int i = 0; i < src.Items.Count; i++)
			{
				BarItem sourceItem = src.Items[i];
				MenuMerge sourceMergeType = sourceItem.MergeType;
				bool applyGroupings = false;
				switch(sourceMergeType)
				{
					case MenuMerge.Add:
						dest.Items.Insert(dest.Items.FindMergePosition(sourceItem.MergeOrder),sourceItem);
						applyGroupings = true;
						break;
					case MenuMerge.Replace:
					case MenuMerge.MergeItems:
					{
						int sourceMergeOrder = sourceItem.MergeOrder;
						int mergePos = dest.Items.FindMergePosition(sourceMergeOrder - 1);
						while(mergePos < dest.Items.Count)
						{
							BarItem destItem = dest.Items[mergePos];
							if(destItem.MergeOrder != sourceMergeOrder
								|| destItem.Text != sourceItem.Text)
							{
								dest.Items.Insert(mergePos, sourceItem);
								applyGroupings = true;
								break;
							}
							else
							{
								if(sourceItem.MergeType != MenuMerge.MergeItems
									|| destItem.MergeType != MenuMerge.MergeItems)
								{
									dest.RemoveGroupAt(destItem);
									//destItem.Dispose();
									dest.Items[mergePos] = sourceItem;
									applyGroupings = true;
									break;
								}
								else
								{
									if(destItem is ParentBarItem
										&& sourceItem is ParentBarItem)
									{
										((ParentBarItem)destItem).MergeItems(sourceItem as ParentBarItem);
									}
									break;
								}
							}
						}
						if(mergePos == dest.Items.Count)
						{
							dest.Items.Insert(mergePos, sourceItem);
							applyGroupings = true;
						}
					}
					break;
					case MenuMerge.Remove:
					{
						int sourceMergeOrder = sourceItem.MergeOrder;
						int mergePos = dest.Items.FindMergePosition(sourceMergeOrder - 1);
						while(mergePos < dest.Items.Count)
						{
							// There could be more than 1 destItem matching this criteria; if unmerging a previously merged item.
							BarItem destItem = dest.Items[mergePos];
							if(destItem.MergeOrder == sourceMergeOrder
								&& destItem.Text == sourceItem.Text)
							{
								if(destItem.MergeType != MenuMerge.Replace
									&& destItem.MergeType != MenuMerge.Add)
								{
									dest.RemoveItem(destItem);
									break;
								}
							}
							if(destItem.MergeOrder > sourceMergeOrder)
								break;
							mergePos++;
						}	
					}
					break;
				}
				if(applyGroupings)
				{
					if(src.IsGroupBeginning(sourceItem))
						dest.BeginGroupAt(sourceItem);
				}
			}
		}
	}
}
