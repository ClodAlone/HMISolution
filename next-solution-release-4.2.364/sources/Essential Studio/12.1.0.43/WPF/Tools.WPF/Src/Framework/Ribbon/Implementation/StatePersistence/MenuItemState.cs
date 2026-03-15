#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Syncfusion.Windows.Tools.Controls;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Save the MenuItem states
    /// </summary>
    public  class MenuItemState
    {

        /// <summary>
        /// Gets or sets the synchronized item.
        /// </summary>
        /// <value>The synchronized item.</value>
        public string SynchronizedItem { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItemState"/> class.
        /// </summary>
        public MenuItemState()
        {

        }
    }

    /// <summary>
    /// Saves the QuickAccessToolBarItem state.
    /// </summary>
    public class QATItemState
    {
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the index of the <see cref="QuickAccessToolBarItem"/> in <see cref="QuickAccessToolBar"/>.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Instantiates a new instance of QATItemState.
        /// </summary>
        /// <param name="label">label of the QuickAccessToolBarItem</param>
        /// <param name="index">index of the QuickAccessToolBarItem</param>
        public QATItemState(string label, int index)
        {
            this.Index = index;
            this.Label = label;
        }

        /// <summary>
        /// Instantiates a new instance of QATItemState.
        /// </summary>
        public QATItemState()
        { }
    }

    public class TabItemState
    {
        /// <summary>
        /// Ribbontab caption
        /// </summary>
        public string Caption { get; set; }
        public string OriginalCaption { get; set; }
        public int OriginalIndex { get; set; }

        public TabItemState()
        {
            
        }

        /// <summary>
        /// Instantiates a new instance of TabItemState
        /// </summary>
        /// <param name="caption"></param>
        public TabItemState(string caption,string originalOption,int originalindex)
        {
            this.Caption = caption;
            this.OriginalCaption = originalOption;
            this.OriginalIndex = originalindex;
        }
    }

    public class BarItemState
    {
        /// <summary>
        /// RibbonBar header
        /// </summary>
        public string Header { get; set; }
        public int TabIndex { get; set; }
        public string OriginalHeader { get; set; }
        public int OriginalIndex { get; set; }

        public BarItemState()
        {

        }
        /// <summary>
        /// Instantiates a new instance of BarItemState
        /// </summary>
        /// <param name="header"></param>
        public BarItemState(string header,int tabIndex,string originalHeader,int originalindex)
        {
            this.Header = header;
            this.TabIndex = tabIndex;
            this.OriginalHeader = originalHeader;
            this.OriginalIndex = originalindex;
        }
    }
}
