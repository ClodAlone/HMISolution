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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Collections;

namespace Syncfusion.Windows.Forms.Tools
{
	#region RibbonItemsEventArgs
	/// <summary>
	/// Provides data for RibbonItemsEventHandler.
	/// Event arguments contains extra information 
	/// about <see cref="T:System.Windows.Forms.ToolStripItem"></see> item.
	/// </summary>
	[Description( "Provides data for RibbonItemsEventHandler. Event arguments contains extra information about ToolStrip item." )]
	public class RibbonItemEventArgs : CancelEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Forms.Tools.RibbonItemEventArgs"></see> class.
		/// </summary>
		/// <param name="item"> The toolstrip item that is the source of the event. </param>
		public RibbonItemEventArgs(ToolStripItem item)
			: base(false)
		{
			m_item = item;
		}
		/// <summary>
		/// Gets the toolstrip item associated with the event.
		/// </summary>
		/// <returns> The toolstrip item associated with the event. </returns>
		public ToolStripItem Item
		{
			get { return m_item; }
		}
		/// <summary>
		/// <see cref="T:System.Windows.Forms.ToolStripItem"></see> instance.
		/// </summary>
		private ToolStripItem m_item;
	}
	#endregion

	#region RibbonItemsEventHandler
	/// <summary>
	/// Represents the method that will handle the <see cref="E:Syncfusion.Windows.Forms.Tools.BeforeAddItem"></see> and
	/// <see cref="E:Syncfusion.Windows.Forms.Tools.BeforeRemoveItem"></see> events for a <see cref="T:Syncfusion.Windows.Forms.Tools.RibbonControlAdvHeader"></see> class.
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="args"></param>
	public delegate void RibbonItemsEventHandler(object obj, RibbonItemEventArgs args);
	#endregion

	#region IRibbonItems
	/// <summary>
	/// Supports events which are raised before <see cref="T:System.Windows.Forms.ToolStripItem"></see> item
	/// will be added to collection and before <see cref="T:System.Windows.Forms.ToolStripItem"></see> item
	/// will be removed from collection. Also method for removing <see cref="T:System.Windows.Forms.ToolStripItem"></see> item
	/// at the specific index is represented.
	/// </summary>
	[Description( "Supports events which are raised before ToolStrip item will be added to collection and before" + 
	"ToolStrip item will be removed from collection. Also method for removing ToolStrip item at the specific index is represented." )]
	public interface IRibbonItems : ICollection
	{
		/// <summary>
		/// Removes the <see cref="T:System.Windows.Forms.ToolStripItem"></see> item at the specified index.
		/// </summary>
		/// <param name="idx"> The zero-based index of the item to remove. </param>
		void RemoveAt(int idx);

		/// <summary>
		/// Gets the item at the specified index. 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		ToolStripItem this[int index] { get; }

		/// <summary>
		/// Occurs before <see cref="T:System.Windows.Forms.ToolStripItem"></see> item
		/// will be added to collection.
		/// </summary>
		event RibbonItemsEventHandler BeforeAddItem;
		/// <summary>
		/// Occurs before <see cref="T:System.Windows.Forms.ToolStripItem"></see> item
		/// will be removed from collection.
		/// </summary>
		event RibbonItemsEventHandler BeforeRemoveItem;
	}
	#endregion

	#region RibbonItemsCollection
	/// <summary>
	/// Header of RibbonControlAdv.
	/// </summary>
	partial class RibbonControlAdvHeader
	{
		/// <summary>
		/// Collection of <see cref="T:System.Windows.Forms.ToolStripItem"></see> items,
		/// which is also supports <see cref="I:Syncfusion.Windows.Forms.Tools.IRibbonItems"></see> interface
		/// to manipulate adding and removing operations.
		/// </summary>
		[Description( "Collection of ToolStrip items, which is also supports IRibbonItems interface to manipulate adding and removing operations." )]
		internal class RibbonItemsCollection : ObservableList<ToolStripItem>, IRibbonItems
		{
			#region IRibbonItems Members
			/// <summary>
			/// Removes the <see cref="T:System.Windows.Forms.ToolStripItem"></see> item at the specified index.
			/// </summary>
			/// <param name="index"> The zero-based index of the item to remove. </param>
			void IRibbonItems.RemoveAt(int index)
			{
				this.List.RemoveAt(index);
			}

			/// <summary>
			/// Gets the item at the specified index.
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			ToolStripItem IRibbonItems.this[int index]
			{
				get
				{
					if(index<0 && index>=this.List.Count)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					return (ToolStripItem)this.List[index];
				}
			}

			/// <summary>
			/// Occurs before <see cref="T:System.Windows.Forms.ToolStripItem"></see> item
			/// will be added to collection.
			/// </summary>
			event RibbonItemsEventHandler IRibbonItems.BeforeAddItem
			{
				add
				{
					this.BeforeAddItem += value;
				}
				remove
				{
					this.BeforeAddItem -= value;
				}
			}
			/// <summary>
			/// Occurs before <see cref="T:System.Windows.Forms.ToolStripItem"></see> item
			/// will be removed from collection.
			/// </summary>
			event RibbonItemsEventHandler IRibbonItems.BeforeRemoveItem
			{
				add
				{
					this.BeforeRemoveItem += value;
				}
				remove
				{
					this.BeforeRemoveItem -= value;
				}
			}
			#endregion

			#region Overrides
			/// <summary>
			/// Raises <see cref="E:Syncfusion.Windows.Forms.Tools.BeforeAddItem"></see> event and
			/// according to <see cref="P:Syncfusion.Windows.Forms.Tools.RibbonItemEventArgs.Cancel"></see> property
			/// of <see cref="T:Syncfusion.Windows.Forms.Tools.RibbonItemEventArgs"></see> does base implementation.
			/// </summary>
			/// <param name="index"> The zero-based index of the item to add. </param>
			/// <param name="value"> <see cref="T:System.Windows.Forms.ToolStripItem"></see> item to add. </param>
			/// <returns></returns>
			protected override bool OnInsert(int index, object value)
			{
				if (value is ToolStripItem && OnBeforeAddItem(value as ToolStripItem))
				{
					return base.OnInsert(index, value);
				}
				return false;
			}
			/// <summary>
			/// Raises <see cref="E:Syncfusion.Windows.Forms.Tools.BeforeRemoveItem"></see> event and
			/// according to <see cref="P:Syncfusion.Windows.Forms.Tools.RibbonItemEventArgs.Cancel"></see> property
			/// of <see cref="T:Syncfusion.Windows.Forms.Tools.RibbonItemEventArgs"></see> does base implementation.
			/// </summary>
			/// <param name="index"> The zero-based index of the item to remove. </param>
			/// <param name="value"> <see cref="T:System.Windows.Forms.ToolStripItem"></see> item to remove. </param>
			/// <returns></returns>
			protected override bool OnRemove(int index, object value)
			{
				if (value is ToolStripItem && OnBeforeRemove(value as ToolStripItem))
				{
					return base.OnRemove(index, value);
				}
				return false;
			}
			/// <summary>
			/// Changes owner of <see cref="T:System.Windows.Forms.ToolStripItem"></see> item
			/// if object sent to this method is of <see cref="T:System.Windows.Forms.ToolStripItem"></see> type.
			/// </summary>
			/// <param name="index"> The zero-based index of the item to remove. </param>
			/// <param name="value"> Object which has been removed. </param>
			protected override void OnRemoveComplete(int index, object value)
			{
				ToolStripItem item = value as ToolStripItem;

				if (item is ToolStripItem)
				{
					item.Owner = null;
				}

				base.OnRemoveComplete(index, value);
			}
			#endregion

			#region Implementation
			/// <summary>
			/// Raises <see cref="E:Syncfusion.Windows.Forms.Tools.BeforeAddItem"></see> event
			/// and checks of <see cref="T:System.Windows.Forms.ToolStripItem"></see> item can be added.
			/// </summary>
			/// <param name="item"> <see cref="T:System.Windows.Forms.ToolStripItem"></see> item to be added. </param>
			/// <returns> True if item can be added, otherwise - false. </returns>
			bool OnBeforeAddItem(ToolStripItem item)
			{
				bool bResult = true;
				
				if (BeforeAddItem != null)
				{
					RibbonItemEventArgs ea = new RibbonItemEventArgs(item);

					BeforeAddItem(this, ea);

					bResult = !ea.Cancel;
				}
				return bResult;
			}
			/// <summary>
			/// Raises <see cref="E:Syncfusion.Windows.Forms.Tools.BeforeRemoveItem"></see> event
			/// and checks of <see cref="T:System.Windows.Forms.ToolStripItem"></see> item can be added.
			/// <param name="item"> <see cref="T:System.Windows.Forms.ToolStripItem"></see> item to be removed. </param>
			/// <returns> True if item can be removed, otherwise - false. </returns>
			bool OnBeforeRemove(ToolStripItem item)
			{
				bool bResult = true;

				if (BeforeRemoveItem != null)
				{
					RibbonItemEventArgs ea = new RibbonItemEventArgs(item);

					BeforeRemoveItem(this, ea);

					bResult = !ea.Cancel;
				}
				return bResult;
			}
			#endregion

			#region Events
			/// <summary>
			/// Occurs before <see cref="T:System.Windows.Forms.ToolStripItem"></see> item
			/// will be added to collection.
			/// </summary>
			event RibbonItemsEventHandler BeforeAddItem;
			/// <summary>
			/// Occurs before <see cref="T:System.Windows.Forms.ToolStripItem"></see> item
			/// will be removed from collection.
			/// </summary>
			event RibbonItemsEventHandler BeforeRemoveItem;
			#endregion
		}
	}
	#endregion
}
#endif
