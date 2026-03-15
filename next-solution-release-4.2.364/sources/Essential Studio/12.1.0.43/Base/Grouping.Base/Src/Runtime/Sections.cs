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
using System.Diagnostics;
using System.Text;

using Syncfusion.Diagnostics;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Grouping.Internals;

namespace Syncfusion.Grouping
{
	public abstract class Section : Element
	{
		protected Section(Group parent)
			: base(parent)
		{
		}

		SectionsTreeTableEntry sectionEntry;

		internal override ElementTreeTableEntry GetElementEntry() { return SectionEntry;  }

		internal SectionsTreeTableEntry SectionEntry
		{
			get
			{
				return sectionEntry;
			}
			set
			{
				sectionEntry = value;
			}
		}

		public override void RefreshSummariesBottomUp()
		{
			if (this.SectionEntry != null)
				this.SectionEntry.RefreshSummariesBottomUp(true);
		}

		public new Group ParentElement
		{
			get
			{
				return base.ParentElement as Group;
			}
			set
			{
				base.ParentElement = value;
			}
		}

		public override ITreeTableCounter GetCounter()
		{
			if (this.GetVisibleInParent())
				return new Counter(GetVisibleCount(), GetYAmountCount(), GetFilteredRecordCount(), GetElementCount(), GetRecordCount());
			else
				return new Counter(0, 0, GetFilteredRecordCount(), GetElementCount(), GetRecordCount());
		}

	}

	public class EmptySection : Section, IDisplayElement
	{
		public EmptySection(Group parent) 
			: base(parent)
		{
		}

		public override DisplayElementKind Kind
		{
			get { return DisplayElementKind.Empty; }
		}

		public override int GetVisibleCount()
		{
			return 1; 
		}

		public override double GetYAmountCount()
		{
			return ParentTable.DefaultEmptySectionHeight; 
		}
		
		public override void RefreshCounterTopDown(bool notifyCounterSource)
		{

		}

		public override void RefreshSummaries()
		{
		}

		public override void RefreshSummary()
		{
		}

		public override int GetFilteredRecordCount()
		{
			return 0;
		}

		public override int GetElementCount()
		{
			return 1;
		}

		public override int GetRecordCount()
		{
			return 0;
		}

	}

}
