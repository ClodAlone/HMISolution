//-------------------------------------------------------------------------------------------------
// <copyright file="RecordPartsTree.cs" company="syncfusion">
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
    internal class RecordPartsTreeTableEntry : ElementTreeTableEntry
    {
        public RecordPart RecordPart
        {
            get
            {
                return (RecordPart)base.Value;
            }

            set
            {
                base.Value = value;
            }
        }

        /// <summary>
        /// By default, elements do not get sorted.
        /// </summary>
        /// <returns>returns null</returns>
        /// <override/>
        public override object GetSortKey()
        {
            return null;
        }
    }

    internal class RecordPartsTreeTable : ElementTreeTable
    {
        Record _owner;
        internal int visibleCount = -1;
        internal double yAmountCount = -1;
        internal RecordPartInRecordCollection _recordParts = null;

        public RecordPartsTreeTable(Record owner)
            : base(owner)
        {
            _owner = owner;
            this.AllowSetItem = true;
        }

        public Record Owner
        {
            get
            {
                return _owner;
            }
        }

        public new RecordPartsTreeTableEntry GetEntryAtCounterPosition(ITreeTableCounter searchPosition)
        {
            return (RecordPartsTreeTableEntry)base.GetEntryAtCounterPosition(searchPosition);
        }

        public new RecordPartsTreeTableEntry this[int index]
        {
            get
            {
                return (RecordPartsTreeTableEntry)base[index];
            }

            set
            {
                base[index] = value;
            }
        }
    }
}