//-------------------------------------------------------------------------------------------------
// <copyright file="GroupSortOrderTreeTable.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Diagnostics;

using Syncfusion.Grouping;

namespace Syncfusion.Grouping.Internals
{
    internal class GroupSortOrderTreeTableEntry : ElementTreeTableEntry
    {
        public override void InvalidateCounterBottomUp(bool notifyParentRecordSource)
        {
            base.InvalidateCounterBottomUp(notifyParentRecordSource);

            if (Element.GroupCategoryEntry != null)
            {
                Element.GroupCategoryEntry.InvalidateCounterBottomUp(true);
            }
        }

        public new Group Element
        {
            get
            {
                return (Group)base.Value;
            }

            set
            {
                base.Value = value;
            }
        }

         /// <returns>returns Element</returns>
        /// <override/>
        public override object GetSortKey()
        {
            ////if (Value == null)
            ////    return null;
            return Element;
        }
    }

    internal class GroupSortOrderTreeTable : ElementTreeTable
    {
        Element _owner;

        public GroupSortOrderTreeTable(Element owner)
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

        public new GroupSortOrderTreeTableEntry GetEntryAtCounterPosition(ITreeTableCounter searchPosition)
        {
            return (GroupSortOrderTreeTableEntry)base.GetEntryAtCounterPosition(searchPosition);
        }

        public new GroupSortOrderTreeTableEntry this[int index]
        {
            get
            {
                return (GroupSortOrderTreeTableEntry)base[index];
            }

            set
            {
                base[index] = value;
            }
        }
    }
}