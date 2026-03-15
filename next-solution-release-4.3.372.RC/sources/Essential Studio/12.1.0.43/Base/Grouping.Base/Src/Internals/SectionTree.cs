//-------------------------------------------------------------------------------------------------
// <copyright file="SectionTree.cs" company="syncfusion">
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
    internal class SectionsTreeTableEntry : ElementTreeTableEntry
    {
        public Section Section
        {
            get
            {
                return (Section)base.Value;
            }

            set
            {
                base.Value = value;
            }
        }

        /// <summary>
        /// By default, elements do not get sorted.
        /// </summary>
        /// <returns>returns null.</returns>
        /// <override/>
        public override object GetSortKey()
        {
            return null;
        }
    }

    internal class SectionsTreeTable : ElementTreeTable
    {
        Group _owner;

        public SectionsTreeTable(Group owner)
            : base(owner)
        {
            _owner = owner;
            this.AllowSetItem = true;
        }

        public Group Owner
        {
            get
            {
                return _owner;
            }
        }

        public new SectionsTreeTableEntry GetEntryAtCounterPosition(ITreeTableCounter searchPosition)
        {
            return (SectionsTreeTableEntry)base.GetEntryAtCounterPosition(searchPosition);
        }

        public new SectionsTreeTableEntry this[int index]
        {
            get
            {
                return (SectionsTreeTableEntry)base[index];
            }

            set
            {
                base[index] = value;
            }
        }
    }
}