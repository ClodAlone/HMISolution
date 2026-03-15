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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;

using Syncfusion.Collections.BinaryTree;
using Syncfusion.ComponentModel;

namespace Syncfusion.Collections
{
	/// <summary>
	/// Specifies the change in the ListProperty. Used by OnChanging and OnChanged events of strong typed collections.
	/// </summary>
	public enum ListPropertyChangedType
	{
		/// <summary>
		/// An item is appended.
		/// </summary>
		Add,
		/// <summary>
		/// An item is removed.
		/// </summary>
		Remove,
		/// <summary>
		/// An item is inserted.
		/// </summary>
		Insert,
		/// <summary>
		/// An item is moved.
		/// </summary>
		Move,
		/// <summary>
		/// The whole collection is changed.
		/// </summary>
		Refresh,
		/// <summary>
		/// An item is replaced.
		/// </summary>
		ItemChanged,
		/// <summary>
		/// A nested property of an item is changed.
		/// </summary>
		ItemPropertyChanged
	}

	/// <summary>
	/// Used by OnChanging and OnChanged events of strong typed collections.
	/// </summary>
	public sealed class ListPropertyChangedEventArgs : SyncfusionCancelEventArgs 
	{
		ListPropertyChangedType listChangedType;
		int index;
		object item;
		string property;
		object tag;
	
		/// <summary>
		/// Initializes the ListPropertyChangedEventArgs.
		/// </summary>
		/// <param name="listChangedType"></param>
		/// <param name="index"></param>
		/// <param name="item"></param>
		/// <param name="property"></param>
		public ListPropertyChangedEventArgs(ListPropertyChangedType listChangedType, int index, object item, string property) 
		{
			this.listChangedType = listChangedType;
			this.index = index;
			this.property = property;
			this.item = item;
		}
	
		/// <summary>
		/// Initializes the ListPropertyChangedEventArgs.
		/// </summary>
		/// <param name="listChangedType"></param>
		/// <param name="index"></param>
		/// <param name="item"></param>
		/// <param name="property"></param>
		/// <param name="tag"></param>
		public ListPropertyChangedEventArgs(ListPropertyChangedType listChangedType, int index, object item, string property, object tag) 
		{
			this.listChangedType = listChangedType;
			this.index = index;
			this.property = property;
			this.item = item;
			this.tag = tag;
		}

		/// <summary>
		/// Returns the type in which the list changed.
		/// </summary>
		[TraceProperty(true)]
		public ListPropertyChangedType Action
		{
			get
			{
				return listChangedType;
			}
		}
	
		/// <summary>
		/// Returns the index of the item that is changed.
		/// </summary>
		[TraceProperty(true)]
		public int Index
		{
			get
			{
				return index;
			}
		}

		/// <summary>
		/// Returns a reference to the affected item.
		/// </summary>
		[TraceProperty(true)]
		public object Item
		{
			get
			{
				return item;
			}
		}

	
		/// <summary>
		/// Returns the names of the affected property.
		/// </summary>
		[TraceProperty(true)]
		public string Property
		{
			get
			{
				return property;
			}
		}

		/// <summary>
		/// If tag is EventArgs, then it returns the Tag casted to EventArgs.
		/// </summary>
		[TraceProperty(true)]
		public EventArgs Inner
		{
			get
			{
				return tag as EventArgs;
			}
		}

		/// <summary>
		/// Gets / sets a tag.
		/// </summary>
		public object Tag
		{
			get
			{
				return tag;
			}
			set
			{
				tag = value;
			}
		}
	}

	/// <summary>
	/// Used by OnChanging and OnChanged events of strong typed collections.
	/// </summary>
	public delegate void ListPropertyChangedEventHandler(object sender, ListPropertyChangedEventArgs e);
}
	