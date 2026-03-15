#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents event args with information about target tab item.
    /// </summary>
    public class CloseTabEventArgs : EventArgs
    {
        #region Public properies
        /// <summary>
        /// Gets or sets the target tab item.
        /// </summary>
        /// <value>The target tab item.</value>
        [CLSCompliant(false)]
        public TabItemAdv TargetTabItem
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public bool cancel
        {
            get;
            set;
        }
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="CloseTabEventArgs"/> class.
        /// </summary>
        /// <param name="targetItem">The target item.</param>
        [CLSCompliant(false)]
        public CloseTabEventArgs(TabItemAdv targetItem)
        {
            TargetTabItem = targetItem;
        }
        #endregion
    }

    /// <summary>
    /// Represents the AfterLabelEdit event data.
    /// </summary>
    public class AfterLabelEditEventArgs : EventArgs
    {
        #region Public properies
        /// <summary>
        /// Gets or sets the header after edit.
        /// </summary>
        /// <value>The header after edit.</value>
        public object HeaderAfterEdit
        {
            get;
            set;
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes new instance of the AfterLabelEditEventArgs class.
        /// </summary>
        /// <param name="headerAfterEdit">The header.</param>
        public AfterLabelEditEventArgs(object headerAfterEdit)
        {
            HeaderAfterEdit = headerAfterEdit;
        }
        #endregion
    }

    /// <summary>
    /// Represents the BeforeLabelEdit event data.
    /// </summary>
    public class BeforeLabelEditEventArgs : EventArgs
    {
        #region Public properies
        /// <summary>
        /// Gets or sets the header before edit.
        /// </summary>
        /// <value>The header before edit.</value>
        public object HeaderBeforeEdit
        {
            get;
            set;
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes new instance of the BeforeLabelEditEventArgs class.
        /// </summary>
        /// <param name="headerBeforeEdit">The header.</param>
        public BeforeLabelEditEventArgs(object headerBeforeEdit)
        {
            HeaderBeforeEdit = headerBeforeEdit;
        }
        #endregion
    }

    /// <summary>
    /// Represents the SelectedItemChanged event data.
    /// </summary>
    public class SelectedItemChangedEventArgs : EventArgs
    {
        #region Public properies
        /// <summary>
        /// Gets or sets the old selected item.
        /// </summary>
        /// <value>The old selected item.</value>
        [CLSCompliant(false)]
        public TabItemAdv OldSelectedItem
        {
            get;

            set;
        }

        /// <summary>
        /// Gets or sets the new selected item.
        /// </summary>
        /// <value>The new selected item.</value>
        [CLSCompliant(false)]
        public TabItemAdv NewSelectedItem
        {
            get;

            set;
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes new instance of the SelectedItemChangedEventArgs class.
        /// </summary>
        /// <param name="oldSelectedItem">Old tab item.</param>
        /// <param name="newSelectedItem">New tab item.</param>
        [CLSCompliant(false)]
        public SelectedItemChangedEventArgs(TabItemAdv oldSelectedItem, TabItemAdv newSelectedItem)
        {
            OldSelectedItem = oldSelectedItem;
            NewSelectedItem = newSelectedItem;
        }
        #endregion
    }
}
