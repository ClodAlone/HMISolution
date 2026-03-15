#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    #region EventArgs Descendants
    /// <summary>
    /// Event arguments for RibbonTabItemEventHandler.
    /// </summary>
    public class RibbonTabItemEventArgs
        : EventArgs
    {
        #region Public Fields
        /// <summary>
        /// Underlying tab item.
        /// </summary>
        public RibbonTabItem Item;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the RibbonTabItemEventArgs class.
        /// </summary>
        /// <param name="item">Underlying tab item.</param>
        public RibbonTabItemEventArgs(RibbonTabItem item)
        {
            if (item == null) throw new ArgumentNullException("item");

            this.Item = item;
        }
        #endregion
    }

    /// <summary>
    /// Event arguments for RibbonTabGroupEventHandler.
    /// </summary>
    public class RibbonTabGroupEventArgs
        : EventArgs
    {
        #region Public Fields
        /// <summary>
        /// Underlying group item.
        /// </summary>
        public RibbonTabGroup Group;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the RibbonTabGroupEventArgs class.
        /// </summary>
        /// <param name="group">Underlying group item.</param>
        public RibbonTabGroupEventArgs(RibbonTabGroup group)
        {
            if (group == null) throw new ArgumentNullException("group");

            this.Group = group;
        }
        #endregion
    }

    /// <summary>
    /// Event arguments for RibbonTabItemChangedEventHandler.
    /// </summary>
    public class RibbonTabItemChangedEventArgs
        : EventArgs
    {
        #region Public Fields
        /// <summary>
        /// Old tab item.
        /// </summary>
        public RibbonTabItem OldItem;

        /// <summary>
        /// New tab item.
        /// </summary>
        public RibbonTabItem NewItem;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the RibbonTabItemChangedEventArgs class.
        /// </summary>
        /// <param name="oldItem">Item that was selected.</param>
        /// <param name="newItem">Item that became selected.</param>
        public RibbonTabItemChangedEventArgs(RibbonTabItem oldItem, RibbonTabItem newItem)
        {
            if (newItem == null) throw new ArgumentNullException("newItem");

            this.OldItem = oldItem;
            this.NewItem = newItem;
        }
        #endregion
    }

    /// <summary>
    /// Event arguments for RibbonTabItemChangingEventHandler.
    /// </summary>
    public class RibbonTabItemChangingEventArgs
        : RibbonTabItemChangedEventArgs
    {
        #region Public Fields
        /// <summary>
        /// Indicated whether tab changing should be cancelled.
        /// </summary>
        public bool Cancel;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the RibbonTabItemChangingEventArgs class.
        /// </summary>
        /// <param name="oldItem">Item that was selected.</param>
        /// <param name="newItem">Item that became selected.</param>
        public RibbonTabItemChangingEventArgs(RibbonTabItem oldItem, RibbonTabItem newItem)
            : base(oldItem, newItem)
        {
        }
        #endregion
    }

    /// <summary>
    /// Event arguments for AddNewToolStripItemEventHandler.
    /// </summary>
    public class AddNewToolStripItemEventArgs
        : EventArgs
    {
        #region Public Fields
        /// <summary>
        /// Type of item to add.
        /// </summary>
        public Type Type;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the AddNewToolStripItemEventArgs class.
        /// </summary>
        /// <param name="type">Item that became selected.</param>
        public AddNewToolStripItemEventArgs(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (!type.IsSubclassOf(typeof(ToolStripItem))) throw new ArgumentOutOfRangeException("type");

            this.Type = type;
        }
        #endregion
    }

    /// <summary>
    /// Event arguments for NewItemDroppedAtSingleItemGroupEventHandler.
    /// </summary>
    public class NewItemDroppedAtSingleItemGroupEventArgs
        : ToolStripItemEventArgs
    {
        #region Public Fields
        /// <summary>
        /// Indicates whether new single item has to be inserted before the sender ribbon tab group.
        /// </summary>
        public bool CloserToLeft;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the NewItemDroppedAtSingleItemGroupEventArgs class.
        /// </summary>
        /// <param name="item">ToolStripItem that was dropped.</param>
        /// <param name="bCloserToLeft">Indicates whether item was dropped closer to the left edge of the sender.</param>
        public NewItemDroppedAtSingleItemGroupEventArgs(ToolStripItem item, bool bCloserToLeft)
            : base(item)
        {
            this.CloserToLeft = bCloserToLeft;
        }
        #endregion
    }
    #endregion

    #region Delegates
    /// <summary>
    /// Delegate for events related to RibbonTabItem.
    /// </summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="args"> EventArgs that contains the event data.</param>
    public delegate void RibbonTabItemEventHandler(object sender, RibbonTabItemEventArgs args);

    /// <summary>
    /// Delegate for events related to RibbonTabGroup.
    /// </summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="args"> EventArgs that contains the event data.</param>
    public delegate void RibbonTabGroupEventHandler(object sender, RibbonTabGroupEventArgs args);

    /// <summary>
    /// Delegate for RibbonTabItemChanged event.
    /// </summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="args"> EventArgs that contains the event data.</param>
    public delegate void RibbonTabItemChangedEventHandler(object sender, RibbonTabItemChangedEventArgs args);

    /// <summary>
    /// Delegate for RibbonTabItemChanging event.
    /// </summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="args"> EventArgs that contains the event data.</param>
    public delegate void RibbonTabItemChangingEventHandler(object sender, RibbonTabItemChangingEventArgs args);

    /// <summary>
    /// Delegate for AddNewToolStripItem event.
    /// </summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="args"> EventArgs that contains the event data.</param>
    public delegate void AddNewToolStripItemEventHandler(object sender, AddNewToolStripItemEventArgs args);

    /// <summary>
    /// Delegate for NewItemDroppedAtSingleItemGroup event.
    /// </summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="args"> EventArgs that contains the event data.</param>
    public delegate void NewItemDroppedAtSingleItemGroupEventHandler(object sender, NewItemDroppedAtSingleItemGroupEventArgs args);
    #endregion
}
#endif