#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Collections;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Collection of tab groups.
    /// </summary>
    public class TabGroupsCollection
        : ArrayList
    {
        #region Override

        /// <summary>
        /// Hides Add( object ) method.
        /// </summary>
        /// <param name="value">Ribbon tab group object </param>
        /// <returns>Returns Integer</returns>
        public override int Add(object value)
        {
            int result = -1;

            RibbonTabGroup group = value as RibbonTabGroup;
            if (group != null)
            {
                result = base.Add(group);
                OnGroupAdded(group);
            }
            else throw new ArgumentException("RibbonTabGroup is expected");

            return result;
        }
    
        protected virtual void OnGroupAdded(RibbonTabGroup group)
        {
            group.Disposed += new EventHandler(Group_Disposed);

            if (GroupAdded != null)
            {
                GroupAdded(this, new RibbonTabGroupEventArgs(group));
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Raised when new group is added.
        /// </summary>
        public event RibbonTabGroupEventHandler GroupAdded;

        /// <summary>
        /// Raised when disposed group is removed.
        /// </summary>
        public event RibbonTabGroupEventHandler GroupRemoved;
        #endregion

        #region Event Handlers
        /// <summary>
        /// Removes disposed control from collection.
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">EventArgs that contains the event data.</param>
       public void Group_Disposed(object sender, EventArgs e)
        {
            RibbonTabGroup group = (RibbonTabGroup)sender;

            Remove(group);
            group.Disposed -= new EventHandler(Group_Disposed);

            if (GroupRemoved != null)
            {
                GroupRemoved(this, new RibbonTabGroupEventArgs(group));
            }
        }
        #endregion
    }
}
#endif
