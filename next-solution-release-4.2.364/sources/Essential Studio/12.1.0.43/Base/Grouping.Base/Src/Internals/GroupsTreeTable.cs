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

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Diagnostics;

using Syncfusion.Grouping;

namespace Syncfusion.Grouping.Internals
{

	internal class GroupsTreeTableEntry : ElementTreeTableEntry
	{
		public new Group Element
		{
			get
			{
				return (Group) base.Value;
			}
			set
			{
				base.Value = value;
			}
		}

		/// <override/>
		public override object GetSortKey()
		{
			//if (Value == null)
			//	return null;
			return Element.CategoryKeys;
		}

	}

	internal class GroupsTreeTable : ElementTreeTable
	{
		Element _owner;

		public GroupsTreeTable(Element owner)
			: base(owner)
		{
			this._owner = owner;
			this.AllowSetItem = true;
		}

		public Element Owner
		{
			get
			{
				return _owner;
			}
		}

		public new GroupsTreeTableEntry GetEntryAtCounterPosition(Counter searchPosition)
		{
			return (GroupsTreeTableEntry) base.GetEntryAtCounterPosition(searchPosition);
		}

		public new GroupsTreeTableEntry this[int index]
		{
			get
			{
				return (GroupsTreeTableEntry) base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}

}