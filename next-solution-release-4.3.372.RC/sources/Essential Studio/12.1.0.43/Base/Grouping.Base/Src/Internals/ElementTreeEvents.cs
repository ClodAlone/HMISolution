//-------------------------------------------------------------------------------------------------
// <copyright file="ElementTreeEvents.cs" company="syncfusion">
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
using System.ComponentModel;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;

using Syncfusion.Grouping;

namespace Syncfusion.Grouping.Internals
{
#if TREECHANGED
    internal delegate void ElementsTreeChangeEventHandler(object sender, ElementsTreeChangeEventArgs e);

    internal sealed class ElementsTreeChangeEventArgs : SyncfusionEventArgs 
    {
        CollectionChangeAction action;
        ElementTreeTableEntry entry;
    
        internal ElementsTreeChangeEventArgs(CollectionChangeAction action, ElementTreeTableEntry entry) 
        {
            this.action = action;
            this.entry = entry;
        }
    
        [TraceProperty(true)]
        internal CollectionChangeAction Action
        {
            get
            {
                return action;
            }
            set
            {
                action = value;
            }
        }
    
        [TraceProperty(true)]
        internal ElementTreeTableEntry Entry
        {
            get
            {
                return entry;
            }
            set
            {
                entry = value;
            }
        }
    }
#endif
}

