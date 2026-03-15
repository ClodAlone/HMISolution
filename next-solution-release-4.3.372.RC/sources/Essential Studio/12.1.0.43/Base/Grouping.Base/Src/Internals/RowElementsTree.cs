//-------------------------------------------------------------------------------------------------
// <copyright file="RowElementsTree.cs" company="syncfusion">
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
    internal class RowElementsTreeTableEntry : ElementTreeTableEntry
    {
        public new RowElement Element
        {
            get
            {
                return (RowElement)base.Value;
            }

            set
            {
                base.Value = value;
            }
        }

        /// <summary>
        /// RowElement doesn't get sorted.
        /// </summary>
        /// <returns>returns null</returns>
        public override object GetSortKey()
        {
            return null;
        }
    }

    internal class RowElementsTreeTable : ElementTreeTable
    {
        IContainerElement _owner;

        public RowElementsTreeTable(IContainerElement owner)
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

        public new RowElementsTreeTableEntry GetEntryAtCounterPosition(ITreeTableCounter searchPosition)
        {
            return (RowElementsTreeTableEntry)base.GetEntryAtCounterPosition(searchPosition);
        }

        public new RowElementsTreeTableEntry this[int index]
        {
            get
            {
                return (RowElementsTreeTableEntry)base[index];
            }

            set
            {
                base[index] = value;
            }
        }
    }
}
