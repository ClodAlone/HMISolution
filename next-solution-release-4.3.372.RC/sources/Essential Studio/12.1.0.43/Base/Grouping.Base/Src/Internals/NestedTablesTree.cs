//-------------------------------------------------------------------------------------------------
// <copyright file="NestedTablesTree.cs" company="syncfusion">
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
    internal class NestedTablesTreeTableEntry : ElementTreeTableEntry
    {
        public new NestedTable Element
        {
            get
            {
                return (NestedTable)base.Value;
            }

            set
            {
                base.Value = value;
            }
        }

        /// <summary>
        /// NestedTable doesn't get sorted.
        /// </summary>
        /// <returns>returns null</returns>
        public override object GetSortKey()
        {
            return null;
        }
    }

    internal class NestedTablesTreeTable : ElementTreeTable
    {
        IContainerElement _owner;

        public NestedTablesTreeTable(IContainerElement owner)
            : base((Element)owner)
        {
            this._owner = owner;
            this.AllowSetItem = true;
        }

        public IContainerElement Owner
        {
            get
            {
                return _owner;
            }
        }

        public new NestedTablesTreeTableEntry GetEntryAtCounterPosition(ITreeTableCounter searchPosition)
        {
            return (NestedTablesTreeTableEntry)base.GetEntryAtCounterPosition(searchPosition);
        }

        public new NestedTablesTreeTableEntry this[int index]
        {
            get
            {
                return (NestedTablesTreeTableEntry)base[index];
            }

            set
            {
                base[index] = value;
            }
        }
    }
}
