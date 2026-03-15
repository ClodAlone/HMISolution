//-------------------------------------------------------------------------------------------------
// <copyright file="GroupCategoryTreeTable.cs" company="syncfusion">
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
    internal class GroupCategoryTreeTableEntry : ElementTreeTableEntry
    {
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
     
        /// <returns>returns Sort key</returns>
        /// <override/>
        public override object GetSortKey()
        {
            ////if (Value == null)
            ////    return null;
            return Element.CategoryKeys;
        }
    }

    internal class GroupCategoryTreeTable : ElementTreeTable
    {
        Element _owner;

        public GroupCategoryTreeTable(Element owner)
            : base(owner)
        {
            this._owner = owner;
            this.AllowSetItem = true;
        }

        /// <override/>
        public override bool WithoutCounter
        {
            get
            {
                return false;
            }

            set
            {
                if (value)
                {
                    throw new NotSupportedException("Setting WithoutCounter is not allowed for a table with groups or relation child columns.");
                }
            }
        }

        public Element Owner
        {
            get
            {
                return _owner;
            }
        }

        public new GroupCategoryTreeTableEntry this[int index]
        {
            get
            {
                return (GroupCategoryTreeTableEntry)base[index];
            }

            set
            {
                base[index] = value;
            }
        }
    }
}