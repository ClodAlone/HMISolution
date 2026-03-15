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
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using Syncfusion.ComponentModel;
using Syncfusion.Collections;

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IMergedContainer
	{
		bool IsItemHiddenByMerge(BarItem item);
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class MergedStateTracker : IDisposable
	{
		// Key: MergeOrder+Text; Value: ArrayList of BarItemIDs of matching BarItems
		// The first item in the list will always be based on the current ActiveForm.
		private Hashtable htMergeKeyVsItemIdsList = new Hashtable();
		private IBarItemContainer container;

		public MergedStateTracker(IBarItemContainer container)
		{
			this.container = container;
		}

		public virtual void Dispose()
		{
			this.container = null;
			this.htMergeKeyVsItemIdsList.Clear();
		}

		MainFrameBarManager Manager
		{
			get{return container.Manager as MainFrameBarManager;}
		}

		protected Form ActiveForm
		{
			get{return this.Manager.ActiveForm;}
		}

		public virtual void UpdateMergeHash()
		{
			string formTypeName = BarManager.GetFormTypeName(this.ActiveForm);
			foreach(string mergeKeys in this.htMergeKeyVsItemIdsList.Keys)
			{
				ArrayList list = this.htMergeKeyVsItemIdsList[mergeKeys] as ArrayList;
				if(list == null || list.Count <= 1)
					continue;
				
				this.BringItemIdToFront(list, formTypeName);
			}
		}

		public virtual void InitMergedItemsHash()
		{
			this.htMergeKeyVsItemIdsList.Clear();
			foreach(BarItem item in this.container.Items)
			{
				if(this.IsMergeable(item))
				{
					// Add the item to the merged items hash.
					string mergeId = GetMergeId(item);
					ArrayList list = this.GetMergeListForKey(mergeId);

					BarItemID barItemID = BarItemID.FromBarItem(item);
					if(!list.Contains(barItemID))
						list.Add(barItemID);
				}
			}
		}

		public virtual void OnItemsCollectionChanged(CollectionChangeEventArgs e)
		{
			// Update the merged items hash with the added/removed BarItem.
			BarItem barItem = e.Element as BarItem;
			// A separator might get inserted during runtime.
			if(barItem is StandAloneBarItem)
				return;
			this.UpdateMergeHashForItem(barItem, e.Action);
		}

		public virtual void OnItemPropertyChanged(BarItem item, SyncfusionPropertyChangedEventArgs e)
		{
			if(e.PropertyName == "MergeOrder" || e.PropertyName == "MergeType")
			{
				this.UpdateMergeHashForItem(item, CollectionChangeAction.Add);
			}
			else if(e.PropertyName == "Text")
			{
				// First remove old ID...
				string mergeId = item.MergeOrder.ToString() + (string)e.OldValue;
				if(this.htMergeKeyVsItemIdsList.Contains(mergeId))
				{
					ArrayList list = this.htMergeKeyVsItemIdsList[mergeId] as ArrayList;
					list.Remove(BarItemID.FromBarItem(item));
					this.BringItemIdToFront(list, BarManager.GetFormTypeName(this.ActiveForm));
				}
				// Then add new ID...
				this.UpdateMergeHashForItem(item, CollectionChangeAction.Add);
			}
		}

		public static string GetMergeId(BarItem item)
		{
			return item.MergeOrder.ToString() + item.Text;
		}

		protected virtual bool IsMergeable(BarItem barItem)
		{
			// Main items are mergebale only if not MenuMerge.Add.
			// Child items are all mergeable.
			if(barItem.MergeType != MenuMerge.Add
				|| barItem.Manager != this.Manager)
				return true;
			else
				return false;
		}

		private BarItem GetMainItem(ArrayList list)
		{
			string formTypeName = BarManager.GetFormTypeName(this.Manager);
			foreach(BarItemID id in list)
			{
				if(BarItemID.IsItemContainedInManager(id, formTypeName))
					return this.Manager.GetBarItemFromBarItemID(id);
			}
			return null;
		}
		public virtual bool ShouldDrawVisible(BarItem item, out bool validValueReturned)
		{
			validValueReturned = false;
			if(this.IsMergeable(item))
			{
				// If this itemID is on top of the list, then draw, otherwise don't.
				string mergeId = GetMergeId(item);
				ArrayList list = this.htMergeKeyVsItemIdsList[mergeId] as ArrayList;
				if(list != null && list.Count > 0)
				{
					BarItemID barItemId = (BarItemID)list[0];
					BarItem topItem = this.Manager.SearchBarItem(barItemId);

					validValueReturned = true;
					return (barItemId == BarItemID.FromBarItem(item)
						&& (item.MergeType != MenuMerge.Remove || item.Manager == this.Manager))
						// Hide child item when child item = MenuMerge.MergeItems && Main item = MenuMerge.Replace
						&& (item.Manager == this.Manager || item.MergeType != MenuMerge.MergeItems || this.GetMainItem(list) == null || this.GetMainItem(list).MergeType != MenuMerge.Replace)
						// Some exceptions:
						|| (item.Manager == this.Manager && item.MergeType != MenuMerge.Remove && topItem.MergeType == MenuMerge.Add)
						|| (item.Manager == this.Manager && item.MergeType == MenuMerge.Replace && (topItem.MergeType == MenuMerge.Remove || topItem.MergeType == MenuMerge.MergeItems));
				}
			}
			return true;
		}

		protected ArrayList GetMergeListForKey(string mergeId)
		{
			ArrayList list = null;
			if(this.htMergeKeyVsItemIdsList.Contains(mergeId))
				list = this.htMergeKeyVsItemIdsList[mergeId] as ArrayList;
			else
			{
				list = new ArrayList();
				this.htMergeKeyVsItemIdsList[mergeId] = list;
			}
			return list;
		}

		protected void BringItemIdToFront(ArrayList list, string formTypeName)
		{
			if(list.Count <= 1)
				return;
			// Get the item belonging to the active manager to front.
			int i = -1;
			foreach(BarItemID barItemID in list)
			{
				i++;
				if(BarItemID.IsItemContainedInManager(barItemID, formTypeName))
				{
					if(i != 0)
					{
						list.RemoveAt(i);
						list.Insert(0, barItemID);
					}
					break;
				}
			}
		}
		protected void UpdateMergeHashForItem(BarItem barItem, CollectionChangeAction action)
		{
			if(barItem != null)
			{
				string mergeId = GetMergeId(barItem);
				bool isMergeable = this.IsMergeable(barItem);
				ArrayList list = null;
			
				if(action == CollectionChangeAction.Add)
				{
					// Added...
					if(isMergeable)
					{
						list = this.GetMergeListForKey(mergeId);
						BarItemID itemID = BarItemID.FromBarItem(barItem);
						// Add to list if mergeable
						if(!list.Contains(itemID))
							list.Add(itemID);
					}
				}
				else
				{
					// Removed...
					if(this.htMergeKeyVsItemIdsList.Contains(mergeId))
					{
						list = this.GetMergeListForKey(mergeId);
						list.Remove(BarItemID.FromBarItem(barItem));
					}
				}
				if(list != null && list.Count > 1)
					this.BringItemIdToFront(list, BarManager.GetFormTypeName(this.ActiveForm));
			}
		}
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class MergedParentBarItem : ParentBarItem, IMergedContainer
	{
		protected internal ArrayListExt mergedItems = new ArrayListExt();
		private bool visible = true;
		private bool mergeOfItemsInChildManagersOnly = true;
		protected Hashtable m_hrForms = null;

		private MergedStateTracker mergedState = null;

		#region INIT
		public MergedParentBarItem(ParentBarItem[] itemsToMerge, BarManager manager)
			: base()
		{
			if(itemsToMerge.Length < 2) throw new ArgumentException("The merged Items should have at least 2 items to merge.", "mergedItems");

			AdwiseEvents();

			this.mergedItems.AddRange( itemsToMerge );

			SetupForms();

			// Referring to this index in BarManager.
			this.CategoryIndex = 1001; // Special index for merged items.
			this.Manager = manager;

			this.mergedState = this.CreateMergedStateTracker();
			
			if(itemsToMerge[0].Manager is MainFrameBarManager)
				this.mergeOfItemsInChildManagersOnly = false;

			this.MergeParentBarItems(this.mergedItems);
			if(!this.mergeOfItemsInChildManagersOnly)
			{
				this.mergedState.InitMergedItemsHash();

				// Listen to ActiveFormChanged.
				if( null != this.Manager )
				{	
					this.Manager.MainFrameBarManager.ActiveFormChanged += new EventHandler(this.ActiveFormChanged);
					this.Items.ItemPropertyChanged += new SyncfusionPropertyChangedEventHandler(this.ItemPropertyChanged);
				}
			}
		}

		private void AdwiseEvents()
		{
			this.mergedItems.CollectionChanged += new CollectionChangeEventHandler( mergedItems_CollectionChanged );
		}

		private void UnadwiseEvents()
		{
			this.mergedItems.CollectionChanged -= new CollectionChangeEventHandler( mergedItems_CollectionChanged );

			foreach( ParentBarItem item in mergedItems )
			{
				UnadwiseItemEvents( item );
			}
		}

		private void mergedItems_CollectionChanged( object sender, CollectionChangeEventArgs e )
		{
			BarItem item = (BarItem)e.Element;

			if( item != null )
			{
				if( e.Action == CollectionChangeAction.Add )
				{
					AdwiseItemEvents( item );
				}
				else if( e.Action == CollectionChangeAction.Remove )
				{
					UnadwiseItemEvents( item );
				}
			}
		}

		private void AdwiseItemEvents( BarItem barItem )
		{
			barItem.PropertyChanged += new SyncfusionPropertyChangedEventHandler( barItem_PropertyChanged );
		}

		private void UnadwiseItemEvents( BarItem barItem )
		{
			barItem.PropertyChanged -= new SyncfusionPropertyChangedEventHandler( barItem_PropertyChanged );
		}

		private void barItem_PropertyChanged( object sender, SyncfusionPropertyChangedEventArgs e )
		{
			ParentBarItem pbi = (ParentBarItem)sender;

			if( (e.PropertyName == "Text" ) && pbi == GetActiveMergedItem() )
			{
				OnPropertyChanged( e );

				MergeItems( pbi );
			}
		}

		private void SetupForms()
		{
			if( null != m_hrForms )
			{
				m_hrForms.Clear();
			}
			else
			{
				m_hrForms = new Hashtable();
			}

			int iMergedItem = 0;

			foreach( ParentBarItem pbi in this.mergedItems )
			{
				if( null != pbi.Manager )
				{
					string sFormTypeName = BarManager.GetFormTypeName(pbi.Manager);

					m_hrForms.Add( sFormTypeName, iMergedItem++ );
				}
			}
		}

		protected MergedStateTracker CreateMergedStateTracker()
		{
			return new MergedStateTracker(this);
		}

		public bool IsAnyChildItemManagerActive()
		{
			bool bResult = false;

			if( this.mergeOfItemsInChildManagersOnly )
			{
				Form activeForm = null;
				MainFrameBarManager manager = this.Manager as MainFrameBarManager;

				if( null != manager )
				{
					activeForm = manager.ActiveForm;
				}

				if( activeForm != null )
				{
					string activeFormTypeName = BarManager.GetFormTypeName( activeForm );

					foreach( ParentBarItem item in this.mergedItems )
					{
						if( BarManager.GetFormTypeName( item.Manager ) == activeFormTypeName )
						{
							bResult = true;
							break;
						}
					}
				}
			}
			else
			{
				bResult = true;
			}

			return bResult;
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				UnadwiseEvents();

				if( null != this.mergedState )
				{
					this.mergedState.Dispose();
					this.mergedState = null;
					this.UnsubscribePropertyChangedEvents();
				}				
			}
			base.Dispose(disposing);
		}
		private void UnsubscribePropertyChangedEvents()
		{
			if(!this.mergeOfItemsInChildManagersOnly)
			{
				if( null != this.Manager )
				{	
					this.Manager.MainFrameBarManager.ActiveFormChanged -= new EventHandler(this.ActiveFormChanged);
					this.Items.ItemPropertyChanged -= new SyncfusionPropertyChangedEventHandler(this.ItemPropertyChanged);
				}
			}
		}
		#endregion INIT


		#region MERGE_LIST_HASH
		protected override void OnItemsCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			base.OnItemsCollectionChanged(sender, e);
			if(this.mergedState != null)
				this.mergedState.OnItemsCollectionChanged(e);
		}

		private void ItemPropertyChanged(object sender, SyncfusionPropertyChangedEventArgs e)
		{
			if(this.mergedState != null)
				this.mergedState.OnItemPropertyChanged(sender as BarItem, e);
		}

		public override bool ShouldDrawVisible(BarItem item)
		{
			if(this.mergeOfItemsInChildManagersOnly)
				return true;
		
			if(this.mergedState != null)
			{
				bool validValueReturned = false;
				bool shouldDrawVisible = this.mergedState.ShouldDrawVisible(item, out validValueReturned);
				if(validValueReturned)
					return shouldDrawVisible;
			}

			return base.ShouldDrawVisible(item);
		}
		bool IMergedContainer.IsItemHiddenByMerge(BarItem item)
		{
			bool validValueReturned = false;
			bool shouldDrawVisible = this.mergedState.ShouldDrawVisible(item, out validValueReturned);
			if(validValueReturned)
				return !shouldDrawVisible;
			else
				return false;
		}

		#endregion MERGE_LIST_HASH

		public override void RemoveItem(BarItem item)
		{
			base.RemoveItem(item);

			foreach( ParentBarItem parentBarItem in this.mergedItems )
			{
				parentBarItem.RemoveItem(item);
			}

			if( this.mergedItems.Contains( item ) )
			{
				this.mergedItems.Remove( item );
			}
		}

		private void InsertIntoHash(Hashtable hash, string key, ParentBarItem value,
			ref bool isNewEntry)
		{
			isNewEntry = false;
			if(hash[key] == null)
			{
				hash[key] = new BarItems();
				isNewEntry = true;
			}

			((BarItems)hash[key]).Add(value);
		}

		public void MergeParentBarItems( ArrayListExt parentItems )
		{
			int itemPositions = 0;

			this.Items.SuspendEvents();
			this.Items.Clear();

			Hashtable itemsOrderOfOccurance = new Hashtable();
			Hashtable itemsByNameAndMergeOrder = new Hashtable();

			// Non ParentBarItem type items
			ArrayList mergedList = new ArrayList();
			Hashtable sepeartedItems = new Hashtable();

			// First pass, separate the items into mergeable and non-mergeable items
			foreach(ParentBarItem parentItem in parentItems)
			{
				foreach(BarItem item in parentItem.Items)
				{
					if(parentItem.IsGroupBeginning(item))
						sepeartedItems.Add(item, 1);

					string barItemIDString = BarItemID.FromBarItem(item).ToString();
					string itemNameAndMergeOrder = item.Text + item.ID + item.MergeOrder.ToString();

					bool isNewEntry = true;
					if(item is ParentBarItem)
					{
						this.InsertIntoHash(itemsByNameAndMergeOrder, itemNameAndMergeOrder, item as ParentBarItem, ref isNewEntry);
					}
					else
					{
						mergedList.Add(item);
					}

					// Keep track of the order in which this item was added.
					if(isNewEntry)
					{
						if( !itemsOrderOfOccurance.ContainsKey(barItemIDString) )
						{
							itemsOrderOfOccurance[barItemIDString] = itemPositions++;
						}
					}
				}
			}

			// Second pass, deal with the mergeable items list (merge them if necessary)
			foreach(BarItems mergeList in itemsByNameAndMergeOrder.Values)
			{
				if(mergeList.Count == 1)
					mergedList.Add(mergeList[0]);
				else
				{
					// There are items to merge!
					ParentBarItem[] parentItemsToMerge = new ParentBarItem[mergeList.Count];
					for(int i = 0; i < mergeList.Count; i++)
						parentItemsToMerge[i] = mergeList[i] as ParentBarItem;

					MergedParentBarItem mergedItem 
						= new MergedParentBarItem(parentItemsToMerge, this.Manager);
					if(sepeartedItems[parentItemsToMerge[0]] != null)
						sepeartedItems[mergedItem] = 1;
					mergedList.Add(mergedItem);
					this.InsertPositionalInfo(mergedItem, parentItemsToMerge[0] as ParentBarItem, itemsOrderOfOccurance);
				}
			}

			// Reorder the items in mergedList based on the order of their occurance.
			ArrayList unorderedList = new ArrayList(itemPositions);
			// Make space for the maximum possible no. of entries.
			for(int i = 0; i < itemPositions; i++)
				unorderedList.Add(null);
			foreach(BarItem item in mergedList)
			{
				// Add this item to the position based on its order of occurance.
				string barItemIDString = BarItemID.FromBarItem(item).ToString();
				unorderedList[(int)itemsOrderOfOccurance[barItemIDString]] = item;
			}

			itemsByNameAndMergeOrder.Clear();
			// Now order the items based on the MergeOrder
			foreach(BarItem item in unorderedList)
			{
				string itemNameAndMergeOrder = item.Text + item.ID + item.MergeOrder.ToString();

				bool inserted = false;
				BarItem prevItem = itemsByNameAndMergeOrder[itemNameAndMergeOrder] as BarItem;
				if(prevItem != null)
				{
					if(prevItem.MergeOrder == item.MergeOrder)
					{
						// Then add the item beside the previous item
						int prevItemIndex = this.Items.IndexOf(prevItem);
						this.Items.SuspendMergeRecurstion();
						this.Items.Insert(prevItemIndex + 1, item);
						this.Items.ResumeMergeRecurstion();
						inserted = true;
					}
				}
				else
				{
					itemsByNameAndMergeOrder[itemNameAndMergeOrder] = item;
				}
				if(!inserted)
				{
					int insertPos = this.Items.FindMergePosition(item.MergeOrder);					
					this.Items.SuspendMergeRecurstion();
					this.Items.Insert(insertPos, item);
					this.Items.ResumeMergeRecurstion();
				}
				if(sepeartedItems[item] != null)
					this.BeginGroupAt(item);
			}
			this.Items.ResumeEvents(true);
		}

		private void InsertPositionalInfo(ParentBarItem parentItem1, ParentBarItem parentItem2,
			Hashtable itemsOrderOfOccurance)
		{
			string sourceParentString = BarItemID.FromBarItem(parentItem2).ToString();
			string destParentString = BarItemID.FromBarItem(parentItem1).ToString();

			int sourcePosition = (int)itemsOrderOfOccurance[sourceParentString];
			itemsOrderOfOccurance[destParentString] = sourcePosition;
		}

		private void ActiveFormChanged(object sender, EventArgs e)
		{
			this.ActiveManagerChanged(this.Manager.MainFrameBarManager.ActiveChildBarManager);
		}

		public void ActiveManagerChanged(BarManager activeManager)
		{
			if(!this.mergeOfItemsInChildManagersOnly)
			{
				this.visible = true;
				if(this.mergedState != null)
					this.mergedState.UpdateMergeHash();
			}
			else
			{
				bool oldValue = this.visible;
				this.visible = false;

				// If any of the children needs to be visible, make this visible. 
				foreach(ParentBarItem parentItem in this.mergedItems)
				{
					if(parentItem.Manager.Form.GetType() == activeManager.Form.GetType())
					{
						this.visible = true;
						break;
					}
				}

				// Firing event here (since we don't change the base Visible property).
				if(oldValue != this.visible)
					this.OnPropertyChanged(
						new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "Visible", oldValue, this.visible));
			}
		}

		#region OVERRIDES
		protected internal override void OnUpdateUI(EventArgs args)
		{
			// Pass the event to all the merged items.
			if(this.mergedItems != null)
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				if( null != pbiActiveMerged )
				{
					pbiActiveMerged.OnUpdateUI(args);
				}
			}
			base.OnUpdateUI(args);
		}
		public override void OnPopup(EventArgs args)
		{
			// Pass the event to all the merged items.
			if(this.mergedItems != null)
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				if( null != pbiActiveMerged )
				{
					pbiActiveMerged.OnPopup(args);
				}
			}
			base.OnPopup(args);
		}

		public override void OnPopupClosed(EventArgs args)
		{
			// Pass the event to all the merged items.
			if(this.mergedItems != null)
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				if( null != pbiActiveMerged )
				{
					pbiActiveMerged.OnPopupClosed(args);
				}
			}
			base.OnPopupClosed(args);
		}
		protected override void OnItemClicked(EventArgs args)
		{
			base.OnItemClicked(args);

			BarItem activeItem = GetActiveMergedItem() as ParentBarItem;

			if( null != activeItem )
			{
				activeItem.PerformClick();
			}
		}
		#endregion OVERRIDES

		#region OVERRIDEN_PROPERTIES
		public override bool UpdateUIMFCStyle
		{
			get
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				return ( null != pbiActiveMerged ) ? pbiActiveMerged.UpdateUIMFCStyle : base.UpdateUIMFCStyle;
			}
			set
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				if( null != pbiActiveMerged )
				{
					pbiActiveMerged.UpdateUIMFCStyle = value;
				}
				else
				{
					base.UpdateUIMFCStyle = value;
				}
			}
		}

		public override BarManager Manager
		{
			get{return base.Manager;}
			set
			{
				if(value == null)
					this.UnsubscribePropertyChangedEvents();

				base.Manager = value;
			}
		}
		public override bool Enabled 
		{
			get
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				return ( null != pbiActiveMerged ) ? pbiActiveMerged.Enabled : base.Enabled;
			}
			set
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				if( null != pbiActiveMerged )
				{
					pbiActiveMerged.Enabled = value;
				}
				else
				{
					base.Enabled = value;
				}
			}
		}
		public override bool Checked 
		{
			get
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				return ( null != pbiActiveMerged ) ? pbiActiveMerged.Checked : base.Checked;
			}
			set
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				if( null != pbiActiveMerged )
				{
					pbiActiveMerged.Checked = value;
				}
				else
				{
					base.Checked = value;
				}
			}
		}
		public override bool CloseOnClick
		{
			get
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				return ( null != pbiActiveMerged ) ? pbiActiveMerged.CloseOnClick : base.CloseOnClick;
			}
			set
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				if( null != pbiActiveMerged )
				{
					pbiActiveMerged.CloseOnClick = value;
				}
				else
				{
					base.CloseOnClick = value;
				}
			}
		}

		public override int ImageIndex 
		{
			get
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				return ( null != pbiActiveMerged ) ? pbiActiveMerged.ImageIndex : base.ImageIndex;
			}
			set
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				if( null != pbiActiveMerged )
				{
					pbiActiveMerged.ImageIndex = value;
                    this.OnPropertyChanged( new SyncfusionPropertyChangedEventArgs( PropertyChangeEffect.NeedRepaint, "ImageIndex", null, null ));
				}
				else
				{
					base.ImageIndex = value;
				}
			}
		}

		#region ImageList
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageList ImageList
		{
			get
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				return ( null != pbiActiveMerged ) ? pbiActiveMerged.ImageList : base.ImageList;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageListAdv ImageListAdv
		{
			get
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				return (null != pbiActiveMerged) ? pbiActiveMerged.ImageListAdv : base.ImageListAdv;
			}
		}
		#endregion

		#region LargeImageList
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageList LargeImageList
		{
			get
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				return ( null != pbiActiveMerged ) ? pbiActiveMerged.LargeImageList : base.LargeImageList;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageListAdv LargeImageListAdv
		{
			get
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				return (null != pbiActiveMerged) ? pbiActiveMerged.LargeImageListAdv : base.LargeImageListAdv;
			}
		}
		#endregion

		public override int MergeOrder 
		{
			get
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				return ( null != pbiActiveMerged ) ? pbiActiveMerged.MergeOrder : base.MergeOrder;
			}
			set
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				if( null != pbiActiveMerged )
				{
					pbiActiveMerged.MergeOrder = value;
				}
				else
				{
					base.MergeOrder = value;
				}
			}
		}
		public override MenuMerge MergeType
		{
			get
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				return ( null != pbiActiveMerged ) ? pbiActiveMerged.MergeType : base.MergeType;
			}
			set
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				if( null != pbiActiveMerged )
				{
					pbiActiveMerged.MergeType = value;
				}
				else
				{
					base.MergeType = value;
				}
			}
		}
		public override PaintStyle PaintStyle
		{
			get
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				return ( null != pbiActiveMerged ) ? pbiActiveMerged.PaintStyle : base.PaintStyle;
			}
			set
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				if( null != pbiActiveMerged )
				{
					pbiActiveMerged.PaintStyle = value;
				}
				else
				{
					base.PaintStyle = value;
				}
			}
		}
		public override bool Visible 
		{
			get{return this.visible;}
			set{
				if(this.visible != value)
				{
					this.visible = value;
					this.OnPropertyChanged(
						new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "Visible", !this.visible, this.visible));
				}
			}
		}
		public override string Text
		{
			get
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				return ( null != pbiActiveMerged ) ? pbiActiveMerged.Text : base.Text;
			}
			set
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				if( null != pbiActiveMerged )
				{
					pbiActiveMerged.Text = value;
				}
				else
				{
					base.Text = value;
				}
			}
		}

        public override ImageExt Image
        {
            get
            {
                ParentBarItem pbiActiveMerged = GetActiveMergedItem();

                return (null != pbiActiveMerged) ? pbiActiveMerged.Image: base.Image;
            }
            set
            {
                ParentBarItem pbiActiveMerged = GetActiveMergedItem();

                if (null != pbiActiveMerged)
                {
                    pbiActiveMerged.Image = value;
                }
                else
                {
                    base.Image = value;
                }
            }
        }
		public override Shortcut Shortcut 
		{
			get
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				return ( null != pbiActiveMerged ) ? pbiActiveMerged.Shortcut : base.Shortcut;
			}
			set
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				if( null != pbiActiveMerged )
				{
					pbiActiveMerged.Shortcut = value;
				}
				else
				{
					base.Shortcut = value;
				}
			}
		}
		public override string Tooltip
		{
			get
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				return ( null != pbiActiveMerged ) ? pbiActiveMerged.Tooltip : base.Tooltip;
			}
			set
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				if( null != pbiActiveMerged )
				{
					pbiActiveMerged.Tooltip = value;
				}
				else
				{
					base.Tooltip = value;
				}
			}
		}
		public override ParentBarItemStyle ParentStyle
		{
			get
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				return ( null != pbiActiveMerged ) ? pbiActiveMerged.ParentStyle : base.ParentStyle;
			}
			set
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				if( null != pbiActiveMerged )
				{
					pbiActiveMerged.ParentStyle = value;
				}
				else
				{
					base.ParentStyle = value;
				}
			}
		}
		public override bool Customizable
		{
			get
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				return ( null != pbiActiveMerged ) ? pbiActiveMerged.Customizable : base.Customizable;
			}
			set
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				if( null != pbiActiveMerged )
				{
					pbiActiveMerged.Customizable = value;
				}
				else
				{
					base.Customizable = value;
				}
			}
		}
		public override bool IsRecentlyUsedItem
		{
			get
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				return ( null != pbiActiveMerged ) ? pbiActiveMerged.IsRecentlyUsedItem : base.IsRecentlyUsedItem;
			}
			set
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				if( null != pbiActiveMerged )
				{
					pbiActiveMerged.IsRecentlyUsedItem = value;
				}
				else
				{
					base.IsRecentlyUsedItem = value;
				}
			}
		}
		public override bool UsePartialMenus
		{
			get
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				return ( null != pbiActiveMerged ) ? pbiActiveMerged.UsePartialMenus : base.UsePartialMenus;
			}
			set
			{
				ParentBarItem pbiActiveMerged = GetActiveMergedItem();

				if( null != pbiActiveMerged )
				{
					pbiActiveMerged.UsePartialMenus = value;
				}
				else
				{
					base.UsePartialMenus = value;
				}
			}
		}
		#endregion OVERRIDEN_PROPERTIES
		#region Misc
		protected ParentBarItem GetActiveMergedItem()
		{
			ParentBarItem pbiActiveMergedItem = null;

			if( null != this.Manager )
			{
				MainFrameBarManager mainFrameBarManager = this.Manager.MainFrameBarManager;

				if( null != mainFrameBarManager && null != mainFrameBarManager.ActiveChildBarManager )
				{
					object index = m_hrForms[BarManager.GetFormTypeName(mainFrameBarManager.ActiveForm)];

					if( null == index )
					{
						index = m_hrForms[BarManager.GetFormTypeName(mainFrameBarManager.Form)];
					}

					if( null != index )
					{
						int nItem = (int)index;
						int nMergedItems = this.mergedItems.Count;

						if( nItem >= 0 && nItem < nMergedItems )
						{
							ParentBarItem mergedItem = (ParentBarItem)this.mergedItems[nItem];

							pbiActiveMergedItem =
								mainFrameBarManager.ActiveChildBarManager.Items.FindItem(mergedItem.ID) as ParentBarItem;

							if( null == pbiActiveMergedItem )
							{
								pbiActiveMergedItem = mergedItem;
							}
						}
					}
				}
				else
				{
					pbiActiveMergedItem = (ParentBarItem)this.mergedItems[0];
				}
			}
			
			return pbiActiveMergedItem;
		}
		#endregion Misc
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class MergedBar : Bar, IMergedContainer
	{
		protected internal Bar[] mergedBars;

		private MergedStateTracker mergedState = null;

		private bool mergeOfItemsInChildManagersOnly = true;

		private bool currentVisibleStateDueToMerge = true;

		/// <summary>
		/// State of changes propagation.
		/// </summary>
		private bool m_bPropagateChanges = false;
		
		#region INIT
		public MergedBar(Bar[] barsToMerge, BarManager manager)
			: base(manager)
		{
			this.mergedBars = barsToMerge;
			this.mergedState = this.CreateMergedStateTracker();
			this.MergeBars(this.mergedBars);

			this.mergedState.InitMergedItemsHash();

			// Listen to ActiveFormChanged.
			if( null != this.Manager )
			{	
				this.Manager.MainFrameBarManager.ActiveFormChanged += new EventHandler(this.ActiveFormChanged);
				this.Items.ItemPropertyChanged += new SyncfusionPropertyChangedEventHandler(this.ItemPropertyChanged);
			}

			// Replace original with merged.
			bool insertedThisInManager = false;
			foreach(Bar bar in barsToMerge)
			{
				if(bar.Manager == manager)
				{
					mergeOfItemsInChildManagersOnly = false;
					// Replace...
					manager.ReplaceBarsWithMergedBar(this);
					//manager.ReplaceBars(bar, this);
					insertedThisInManager = true;
				}
				else
				{
					//bar.Manager.Bars.Remove(bar);
					MainFrameBarManager mainManager = this.Manager as MainFrameBarManager;
					mainManager.ReplaceChildBarsWithMergedBar(this, bar);
				}
			}
			// ... or Add.
			if(insertedThisInManager == false)
				//manager.Bars.Add(this);
				manager.ReplaceBarsWithMergedBar(this);

			this.CurrentVisibleStateDueToMerge = this.IsAnyChildBarManagerActive();
		}

		public bool CurrentVisibleStateDueToMerge
		{
			get{return this.currentVisibleStateDueToMerge;}
			set	
			{
				if(this.currentVisibleStateDueToMerge != value)
				{
					bool oldvalue = this.currentVisibleStateDueToMerge;
					this.currentVisibleStateDueToMerge = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "BarStyle", oldvalue, value));
				}
			}
		}

		private bool IsAnyChildBarManagerActive()
		{
			// If this is a merge of bars from the child forms, then show this only
			// if any one of those child form types is active.
			if(!this.mergeOfItemsInChildManagersOnly)
				return true;

			Form activeForm = ((MainFrameBarManager)this.Manager).ActiveForm;
			if(activeForm == null)
				return false;

			string activeFormTypeName = BarManager.GetFormTypeName(activeForm);
			foreach(Bar bar in this.mergedBars)
			{
				if(BarManager.GetFormTypeName(bar.Manager) == activeFormTypeName)
					return true;
			}
			return false;
		}

		protected MergedStateTracker CreateMergedStateTracker()
		{
			return new MergedStateTracker(this);
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if( mergedState != null )
				{
					this.mergedState.Dispose();
					this.mergedState = null;

					this.UnsubscribePropertyChangedEvents();	
					this.LoseMergedItems();
				}
			}
			base.Dispose(disposing);
		}
		public override BarManager Manager
		{
			get{return base.Manager;}
			set
			{
				if(value == null)
					this.UnsubscribePropertyChangedEvents();
				this.manager = value;
			}
		}
		private void UnsubscribePropertyChangedEvents()
		{
			if( null != this.Manager )
			{	
				this.Manager.MainFrameBarManager.ActiveFormChanged -= new EventHandler(this.ActiveFormChanged);
			}

			if( null != this.Items )
			{
				this.Items.ItemPropertyChanged -= new SyncfusionPropertyChangedEventHandler(this.ItemPropertyChanged);
			}
		}
		#endregion INIT

		#region MERGED_STATE
		private void ItemPropertyChanged(object sender, SyncfusionPropertyChangedEventArgs e)
		{
			if(this.mergedState != null)
				this.mergedState.OnItemPropertyChanged(sender as BarItem, e);
		}
		protected override void OnItemsCollectionChanged(object sender, CollectionChangeEventArgs args)
		{
			base.OnItemsCollectionChanged(sender, args);
			if(this.mergedState != null)
				this.mergedState.OnItemsCollectionChanged(args);
		}

		private void LoseMergedItems()
		{
			if(this.Manager == null || this.Manager.Form == null)
				return;

			for(int i=this.Items.Count - 1; i >= 0; i--)
			{
				BarItem item = this.Items[i];
				if(item is MergedParentBarItem)
				{
					this.Manager.RemoveReferencesToBarItem(item);
					item.Dispose();
				}
			}

			// Replace merged with original.
			bool removedThisFromManager = false;
			for(int i = 0; i < this.mergedBars.Length; i++)
			//foreach(Bar bar in this.mergedBars)
			{
				Bar bar = this.mergedBars[i];
				if(bar.Manager == this.Manager)
				{
					// Replace or...
					//this.Manager.ReplaceBars(this, bar);
					this.Manager.RestoreOriginalBar(this);
					removedThisFromManager = true;
				}
				else
				{
					//bar.Manager.ReplaceBars(this, bar);
					MainFrameBarManager mainManager = this.Manager as MainFrameBarManager;
					mainManager.RestoreOriginalChildBars(this, BarManager.GetFormTypeName(bar.Manager));
				}
			}

			// ... remove.
			if(removedThisFromManager == false)
				//this.Manager.Bars.Remove(this);
				this.Manager.RestoreOriginalBar(this);
		}
		public override void RemoveItem(BarItem item)
		{
			base.RemoveItem(item);
			foreach(Bar bar in this.mergedBars)
				bar.RemoveItem(item);
		}
		public override bool ShouldDrawVisible(BarItem item)
		{
			bool validValueReturned = false;
			if(base.ShouldDrawVisible(item))
			{
				if(this.mergedState != null)
				{
					bool shouldDrawVisible = this.mergedState.ShouldDrawVisible(item, out validValueReturned);
					if(validValueReturned)
						return shouldDrawVisible;
				}
				return true;
			}
			return false;
		}

		bool IMergedContainer.IsItemHiddenByMerge(BarItem item)
		{
			bool validValueReturned = false;
			bool shouldDrawVisible = this.mergedState.ShouldDrawVisible(item, out validValueReturned);
			if(validValueReturned)
				return !shouldDrawVisible;
			else
				return false;
		}

		protected override bool ShouldDrawInactiveItems()
		{
			return false;
		}

		private void ActiveFormChanged(object sender, EventArgs e)
		{
			this.ActiveManagerChanged(
				BarManager.GetManagerFromForm(((MainFrameBarManager)this.Manager).ActiveForm));

			this.CurrentVisibleStateDueToMerge = this.IsAnyChildBarManagerActive();
		}

		public void ActiveManagerChanged(BarManager activeManager)
		{
			if(this.mergedState != null)
				this.mergedState.UpdateMergeHash();
		}
		#endregion MERGED_STATE
		
		private void InsertIntoHash(Hashtable hash, string key, ParentBarItem value,
			ref bool isNewEntry)
		{
			isNewEntry = false;
			if(hash[key] == null)
			{
				hash[key] = new BarItems();
				isNewEntry = true;
			}

			((BarItems)hash[key]).Add(value);
		}

		public void MergeBars(Bar[] bars)
		{
			int itemPositions = 0;

			this.Items.SuspendEvents();
			this.Items.Clear();

			Hashtable itemsOrderOfOccurance = new Hashtable();
			Hashtable itemsByNameAndMergeOrder = new Hashtable();
			// Non ParentBarItem type items
			ArrayList mergedList = new ArrayList();
			Hashtable sepeartedItems = new Hashtable();

			// First pass, separate the items into mergeable and non-mergeable items
			foreach(Bar bar in bars)
			{
				foreach(BarItem item in bar.Items)
				{
					if(bar.IsGroupBeginning(item))
						sepeartedItems.Add(item, 1);

					string barItemIDString = BarItemID.FromBarItem(item).ToString();
					string itemNameAndMergeOrder = item.Text + item.ID + item.MergeOrder.ToString();

					bool isNewEntry = true;
					if(item is ParentBarItem && item.MergeType != MenuMerge.Remove )
					{
						this.InsertIntoHash(itemsByNameAndMergeOrder, itemNameAndMergeOrder, item as ParentBarItem, ref isNewEntry);
					}
					else
					{
						mergedList.Add(item);
					}

					// Keep track of the order in which this item was added.
					if(isNewEntry)
					{
						itemsOrderOfOccurance[barItemIDString] = itemPositions++;
					}
				}
			}

			// Second pass, deal with the mergeable items list (merge them if necessary)
			foreach(BarItems mergeList in itemsByNameAndMergeOrder.Values)
			{
				if(mergeList.Count == 1)
					mergedList.Add(mergeList[0]);
				else
				{
					// There are items to merge!
					ParentBarItem[] parentItemsToMerge = new ParentBarItem[mergeList.Count];
					for(int i = 0; i < mergeList.Count; i++)
						parentItemsToMerge[i] = mergeList[i] as ParentBarItem;

					MergedParentBarItem mergedItem = new MergedParentBarItem(parentItemsToMerge, this.Manager);
					if(sepeartedItems[parentItemsToMerge[0]] != null)
						sepeartedItems[mergedItem] = 1;
					mergedList.Add(mergedItem);

					this.InsertPositionalInfo(mergedItem, parentItemsToMerge[0] as ParentBarItem, itemsOrderOfOccurance);
				}
			}

			// Reorder the items in mergedList based on their order of their occurance.
			ArrayList unorderedList = new ArrayList(itemPositions);
			// Make space for the maximum possible no. of entries.
			for(int i = 0; i < itemPositions; i++)
				unorderedList.Add(null);
			foreach(BarItem item in mergedList)
			{
				// Add this item to the position based on its order of occurance.
				string barItemIDString = BarItemID.FromBarItem(item).ToString();
				unorderedList[(int)itemsOrderOfOccurance[barItemIDString]] = item;
			}

			itemsByNameAndMergeOrder.Clear();
			// Now order the items based on the MergeOrder
			foreach(BarItem item in unorderedList)
			{
                if(item != null)
                {
				string itemNameAndMergeOrder = item.Text + item.ID + item.MergeOrder.ToString();

				bool inserted = false;
				BarItem prevItem = itemsByNameAndMergeOrder[itemNameAndMergeOrder] as BarItem;
				if(prevItem != null)
				{
					if(prevItem.MergeOrder == item.MergeOrder)
					{
						// Then add the item beside the previous item
						int prevItemIndex = this.Items.IndexOf(prevItem);
						this.Items.Insert(prevItemIndex + 1, item);
						inserted = true;
					}
				}
				else
				{
					itemsByNameAndMergeOrder[itemNameAndMergeOrder] = item;
				}
				if(!inserted)
				{
					int insertPos = this.Items.FindMergePosition(item.MergeOrder);
					this.Items.Insert(insertPos, item);
				}
				if(sepeartedItems[item] != null)
					this.BeginGroupAt(item);
            }
			}
			this.Items.ResumeEvents(true);
		}

		private void InsertPositionalInfo(ParentBarItem parentItem1, ParentBarItem parentItem2,
			Hashtable itemsOrderOfOccurance)
		{
			string sourceParentString = BarItemID.FromBarItem(parentItem2).ToString();
			string destParentString = BarItemID.FromBarItem(parentItem1).ToString();

			int sourcePosition = (int)itemsOrderOfOccurance[sourceParentString];
			itemsOrderOfOccurance[destParentString] = sourcePosition;
		}

		
		internal void PropagateAdd( BarItem barItem )
		{
			Debug.Assert( this.CanPropagateChanges && mergedBars.Length == 1, "Propagation of changes is not allowed!" );

			Bar propagatedBar = this.mergedBars[0];
			BarItems propagatedItems = propagatedBar.Items;

			propagatedItems.SuspendMergeRecurstion();
			propagatedItems.Add(barItem);
			propagatedItems.ResumeMergeRecurstion();
		}

		#region Properties
		/// <summary>
		/// Allows propagation of changes to <see cref="Items"/> collection from <see cref="MergedBar"/> to underlying <see cref="Bar"/>.
		/// </summary>
		/// <remarks>
		/// Works only for SDI forms and MDI forms without MDI children forms.
		/// </remarks>
		public bool PropagateChanges
		{
			get
			{
				return m_bPropagateChanges;
			}
			set
			{
				m_bPropagateChanges = value;
			}
		}

		internal bool CanPropagateChanges
		{
			get
			{
				bool bPropagate = false;
				MainFrameBarManager mfbm = this.Manager.MainFrameBarManager;

				if( null != mfbm )
				{
					Form mainForm = mfbm.Form;

					bPropagate = !( null == mainForm || (mainForm.IsMdiContainer && mainForm.MdiChildren.Length > 0) );
				}

				return bPropagate && this.PropagateChanges;
			}
		}

		/// <summary>
		/// Returns the array of bars used to create this MergedBar.
		/// The MDI parent form's <see cref="Bar"/> is always at index zero.
		/// </summary>
		public Bar[] MergedBarsCollection
		{
			get
			{
				return this.mergedBars.Clone() as Bar[];
			}
		}
		#endregion Properties

		#region OVERRIDEN_PROPERTIES
		public override int MenuItemMergeOrder
		{
			get{return mergedBars[0].MenuItemMergeOrder;}
			set{}
		}
		public override bool AllowCustomizing
		{
			get{return mergedBars[0].AllowCustomizing;}
			set{}
		}
		public override bool AllowHiding
		{
			get{return mergedBars[0].AllowHiding;}
			set{}
		}
		public override bool AllowItemsReorderOnShrunk
		{
			get{return mergedBars[0].AllowItemsReorderOnShrunk;}
			set{}
		}
		public override string BarName
		{
			//get{return SR.GetString(SR.MergedBarNamePrefix) + " " + mergedBars[0].BarName;}
			get{return mergedBars[0].BarName;}
			set{
			}
		}
		public override BarStyle BarStyle 
		{
			get
			{
				return mergedBars[0].BarStyle;
			}
			set
			{
				if( value != mergedBars[ 0 ].BarStyle )
				{
					mergedBars[ 0 ].BarStyle = value;
					this.OnPropertyChanged( new SyncfusionPropertyChangedEventArgs( 
						PropertyChangeEffect.NeedLayout, "BarStyle", value, value ) );
				}
			}
		}

		/// <summary>
		/// Gets or sets user friendly bar's caption string, which appears when the tool bar floats and in the customization dialog.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// This will appear as the caption when the tool bar floats and in the customization dialog as the bar's identity.
		/// </remarks>
		public override string Caption
		{
			get
			{
				return mergedBars[0].Caption;
			}
			set
			{
			}
		}

		#endregion OVERRIDEN_PROPERTIES
	}

}